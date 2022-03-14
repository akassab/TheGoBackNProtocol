using System.Net;
using System.Net.Sockets;
using A2Sender.models;
using A2Sender.models.packets;
using A2Sender.utils;

namespace A2Sender.services
{
    public static class SenderService
    {

        public static readonly UdpClient udpSender = new UdpClient(AddressFamily.InterNetwork);

        public static Dictionary<uint, SentPacketInfo> sentPackets = new Dictionary<uint, SentPacketInfo>();

        public static bool TrySendPackets() {
            // Verify we are listening
            if (!ListenerService.GetIsListening()) {
                // This should now ever execute
                throw new Exception("Sender: TrySendPackets(): We can't send a packet if we aren't listening for any.");
            }
            // Create array of packets from file
            Packet[]? packets;
            if (! FileUtils.TryCreatePacketsFromFile(ConsoleParametersService.GetFileName(), out packets)) {
                Console.WriteLine("SenderService: TrySendPackets(): Cannot send packets without any packets to send.");
                return false;
            }
            // check we have at least one packet
            if (packets == null || packets.Length == 0) {
                Console.WriteLine("SenderService: TrySendPackets(): No packets to send.");
                return false;
            }

            int nPackest = packets.Length;
            // ready up udp sender client
            IPEndPoint hostEndPoint = new IPEndPoint(
                IPAddress.Parse(ConsoleParametersService.GetHostAddress()), 
                ConsoleParametersService.GetPortEmulator()
                );
            // Establishes a default remote host using the hostEndPoint
            udpSender.Connect(hostEndPoint);
            // send each packet in order
            try {
                for (int p_i = 0; p_i < nPackest; ++p_i) {
                    // Do we need to lock this
                    SpinWait.SpinUntil(() => WindowService.nSent < WindowService.size);
                    int p_i_copy = p_i; // copy as p_i for the sake of threading in a for loop
                    SenderService.SendPacket(packets[p_i_copy]);
                }
            }
            catch (Exception e) {
                Console.WriteLine("SenderService: TrySendPackets(): Failed to start thread for a packet.");
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        private static void SendPacket(Packet packet) {
            try {
                udpSender.Send(PacketUtils.Encode(packet));
                SenderService.sentPackets[packet.sequenceNumber] = new SentPacketInfo();
                new Thread(() => SenderService.StartTimerForPacket(packet)).Start();
                Console.WriteLine("Sent packet: " + packet.sequenceNumber);
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
            // Send packet again
            if (!SenderService.sentPackets[packet.sequenceNumber].acknowledged) {
                
                if (packet.sequenceNumber == WindowService.baseIndex) {
                    WindowService.size = 1;
                }
                SenderService.SendPacket(packet);
            }
        }
    }
}