using A2Sender.enums;
using A2Sender.models.packets;
using A2Sender.utils;
using System.Net;
using System.Net.Sockets;
namespace A2Sender.services
{
        // SenderService: Singleton class that sends data and eot packet(s) to the emulator and eventually the receiver.
        //  - Uses multithreading and takes advantage of the WindowService lock and its own lock.
        public static class SenderService
    {
         // Window lock for accessing the window when multi-threading.
        public static readonly object senderLock = new object();

        // UdpClient used to send data packets.
        private static readonly UdpClient udpSender = new UdpClient(AddressFamily.InterNetwork);

        // TrySendDataAndEotPackets(): Tries to send data and eot packets of the file to the emulator and ultimately the sender.
        //  Returns false when it could not and true otherwise.
        public static bool TrySendDataAndEotPackets() {

            if (!ListenerService.IsListening()) {
                throw new Exception("Can't send a packet if we aren't listening for any.");
            }

            // Create array of packets from file
            if (!FileUtils.TryCreatePacketsFromFile(ConsoleArgumentsService.GetFileName(), out (DataPacket[]? dataPackets, EotPacket? eotPacket) packets)) {
                StackTraceService.ConsoleLog("Cannot send packets without any packets to send.");
                return false;
            }

            // Get number of packets we have to send and let the window service know
            int nPackest = packets.dataPackets.Length;
            WindowService.SetTotalNumPackets(nPackest + 1);

            // Setup UDP client
            IPEndPoint hostEndPoint = new IPEndPoint(
                IPAddress.Parse(ConsoleArgumentsService.GetHostAddress()), 
                ConsoleArgumentsService.GetPortEmulator()
                );
            udpSender.Connect(hostEndPoint);

            try {

                // Send Data Packets
                int p_i = WindowService.GetBaseIndex();
                while (p_i < nPackest) {
                    
                    // If the total number of packets == window size wait here.
                    SpinWait.SpinUntil(() => (WindowService.GetNumPacketsSent() < WindowService.GetWindowSize() && WindowService.IncrementNumPacketsSent()));

                    lock (WindowService.windowLock) {
                        if (WindowService.GetWasReset()) {
                            p_i = WindowService.GetBaseIndex();
                        }
                    }

                    int p_i_copy = p_i;
                    // Send the data packet
                    SendPacket(packets.dataPackets[p_i_copy]);
                }

                // Send the final EOT packet
                StackTraceService.ConsoleLog("Sending eot");
                SendPacket(packets.eotPacket);
            }
            catch (Exception e) {
                StackTraceService.ConsoleLog("Failed to start thread for a packet.");
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        // SendPacket(packet): Sends a data packet and starts StartTimerForPacket() on a new thread.
        public static void SendPacket(Packet packet) {
            try {

                    lock (WindowService.windowLock) {
                            udpSender.Send(PacketUtils.Encode(packet));
                            StackTraceService.ConsoleLog($"Sent packet: {packet.sequenceNumber}");
                        // Eot Packet
                        if (WindowService.GetNumPacketsSent() == WindowService.GetTotalNumPackets() - 1) {
                            FileUtils.WriteLineToLogFile(LogFileEnum.SeqNum, "EOT");
                        }
                        else {
                            FileUtils.WriteLineToLogFile(LogFileEnum.SeqNum, packet.sequenceNumber.ToString());
                        }
                    }

                    WindowService.InitializePacketStatus(packet.sequenceNumber);
                    new Thread(() => StartTimerForPacket(packet)).Start();
            }
            catch (Exception e) {
                StackTraceService.ConsoleLog($"Failed to send packet with sequence number: {packet.sequenceNumber}");
                Console.WriteLine(e.ToString());
            }
        }
        
        // How do deal with infniite loop
        // Maybe we can stop this thread earlier via some function call
        // StartTimerForPacket(packet): Starts a timer for packet and tries to send it again if it was never received in time.
        private static void StartTimerForPacket(Packet packet) {
            Thread.Sleep(ConsoleArgumentsService.GetTimeout());
            lock (WindowService.windowLock) {
                // If it was acknowledged then exit
                if (WindowService.GetPacketStatus(packet.sequenceNumber).acknowledged) {
                    return;
                }
                if (packet.sequenceNumber == WindowService.GetBaseIndex()) {
                    WindowService.ResetWindowSize();
                }
                SendPacket(packet);
            }   
        }
    }
}
