using System.Net;
using System.Net.Sockets;
using A2Sender.models;
using A2Sender.models.packets;
using A2Sender.utils;

namespace A2Sender.services
{
    public static class SenderService
    {

        public static readonly object senderLock = new object();

        private static readonly UdpClient udpSender = new UdpClient(AddressFamily.InterNetwork);

        public static bool TrySendPackets() {
            // Verify we are listening
            if (!ListenerService.GetIsListening()) {
                // This should now ever execute
                throw new Exception("Sender: TrySendPackets(): We can't send a packet if we aren't listening for any.");
            }
            // Create array of packets from file
            if (! FileUtils.TryCreatePacketsFromFile(ConsoleParametersService.GetFileName(), out (DataPacket[]? dataPackets, EotPacket? eotPacket) packets)) {
                Console.WriteLine("SenderService: TrySendPackets(): Cannot send packets without any packets to send.");
                return false;
            }

            int nPackest = packets.dataPackets.Length;
            // ready up udp sender client
            IPEndPoint hostEndPoint = new IPEndPoint(
                IPAddress.Parse(ConsoleParametersService.GetHostAddress()), 
                ConsoleParametersService.GetPortEmulator()
                );
            // Establishes a default remote host using the hostEndPoint
            udpSender.Connect(hostEndPoint);
            // send each packet in order
            try {
                // Send Data Packets

                int p_i = WindowService.GetBaseIndex() - 1;
                while (p_i < nPackest) {

                   
                    SpinWait.SpinUntil(() => (WindowService.GetNumPacketsSent() + 1) == WindowService.GetWindowSize() && WindowService.IncrementNumPacketsSent());

                    lock (WindowService.windowLock) {
                        if (WindowService.GetWasReset()) {
                            p_i = WindowService.GetBaseIndex();
                        }
                        else {
                            p_i += 1;
                        }
                    }
                    int p_i_copy = p_i; // copy as p_i for the sake of threading in a for loop
                    SendPacket(packets.dataPackets[p_i_copy]);
                }

                // Send EOT packet
                SendPacket(packets.eotPacket);
                
            }
            catch (Exception e) {
                Console.WriteLine("SenderService: TrySendPackets(): Failed to start thread for a packet.");
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        public static void SendPacket(Packet packet) {
            try {
                    udpSender.Send(PacketUtils.Encode(packet));
                    Console.WriteLine("Sent packet: " + packet.sequenceNumber);
                    WindowService.InitializePacketStatus(packet.sequenceNumber);
                    new Thread(() => StartTimerForPacket(packet)).Start();
              
            }
            catch (Exception e) {
                Console.WriteLine("Failed to send packet with sequence number: " + packet.sequenceNumber);
                Console.WriteLine(e.ToString());
            }
        }
        
        // How do deal with infniite loop
        // Maybe we can stop this thread earlier via some function call
        private static void StartTimerForPacket(Packet packet) {
            // Timeout tester
            Thread.Sleep(ConsoleParametersService.GetTimeOut());
            lock (WindowService.windowLock) {
                if (!WindowService.GetPacketStatus(packet.sequenceNumber).acknowledged) {
                    if (packet.sequenceNumber == WindowService.GetBaseIndex()) {
                        WindowService.ResetWindowSize();
                    }
                    SendPacket(packet);
                }
            }
            
        }
    }
}