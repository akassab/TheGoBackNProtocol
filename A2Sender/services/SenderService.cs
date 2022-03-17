using A2Sender.enums;
using A2Sender.models;
using A2Sender.models.packets;
using A2Sender.utils;
using System.Net;
using System.Net.Sockets;

namespace A2Sender.services
{
        // Singleton class that sends data and eot packet(s) to 
        //  the emulator and eventually the receiver and uses multithreading and 
        //  uses the WindowService lock.
        public static class SenderService
    {
        // UdpClient used to send data packets.
        private static readonly UdpClient udpSender = new UdpClient(AddressFamily.InterNetwork);

        // Tries to send data and eot packets of the file to the receiver through the emulator.
        public static void SendDataAndEotPackets() {
            try {
                // setup udp client
                IPEndPoint hostEndPoint = new IPEndPoint(
                    IPAddress.Parse(ConsoleArgumentsService.GetHostAddress()), 
                    ConsoleArgumentsService.GetPortEmulator()
                );
                udpSender.Connect(hostEndPoint);

                // create array of packets from file
                (DataPacket[] dataPackets, EotPacket eotPacket) packets =  FileUtils.CreatePacketsFromFile(ConsoleArgumentsService.GetFileName());

                // get num packets
                int numDataPackets = packets.dataPackets.Length;
                int numEotPacket = 1;
                WindowService.SetNumPacketsTotal(numDataPackets + numEotPacket);
                StackTraceService.ConsoleLog($"Total number of data packets to send: {numDataPackets}");

                // send all data packets
                SendDataPackets(numDataPackets, ref packets.dataPackets);

                // send the final EOT packet
                StackTraceService.ConsoleLog("Sending EotPacket!");
                SendPacket(packets.eotPacket);
            }
            catch (Exception e) {
                StackTraceService.ConsoleLog("ERROR: Closing Sender.");
            }
        }

        // Sends all data packets.
        private static void SendDataPackets(int numDataPackets, ref DataPacket[] dataPackets) {

            int p_i = WindowService.GetBaseIndex();
            while (p_i < numDataPackets) {

                // is the window full? Wait here if it is.
                SpinWait.SpinUntil(() => (WindowService.GetWindowCapacity() < WindowService.GetWindowSize() && WindowService.IncrementWindowCapacity()));

                // incase it was updated
                lock (WindowService.windowLock) {
                    p_i = WindowService.GetBaseIndex() + (WindowService.GetLap()*32);
                }

                int p_i_copy = p_i;
                // Send the data packet
                if (p_i_copy >= numDataPackets) {
                    break;
                }
                // send the packet
                SendPacket(dataPackets[p_i_copy]);
                FileUtils.WriteLineToLogFile(LogFileEnum.SeqNum, dataPackets[p_i_copy].sequenceNumber.ToString());
            }
        }

        // Sends a data packet and starts StartTimerForPacket() on a new thread.
        public static void SendPacket(Packet packet) {
            try {
                lock (WindowService.windowLock) {
                    // incase it was updated
                     if (WindowService.IsBeforeBaseIndex(packet.sequenceNumber)) {
                        return;
                    }
                    // send it
                    udpSender.Send(PacketUtils.Encode(packet));
                    StackTraceService.ConsoleLog($"Sent packet: {packet.sequenceNumber}!");
                    WindowService.InitializePacketStatus(packet.sequenceNumber);
                }
                // start timer thread
                new Thread(() => StartTimerForPacket(packet)).Start();
            }
            catch (Exception e) {
                throw new Exception($"Failed to send packet with sequence number: {packet.sequenceNumber}", e);
            }
        }
        
        // Starts a timer for packet and tries to send it again if it was never received in time.
        private static void StartTimerForPacket(Packet packet) {
            // no timer needed for eot as we assume it always comes back
            if (packet.type == TypeEnum.Eot) {
                return;
            }
            // wait for timeout
            Thread.Sleep(ConsoleArgumentsService.GetTimeout());

            lock (WindowService.windowLock) {
                // incase it was updated
                if (WindowService.IsBeforeBaseIndex(packet.sequenceNumber )) {
                    return;
                }
                
                // check if it was acknowledged while we were sleeping
                PacketStatus? packetStatus = WindowService.GetPacketStatus(packet.sequenceNumber);
                if (packetStatus == null || packetStatus.acknowledged) {
                    StackTraceService.ConsoleLog($"No timeout issue for packet number: {packet.sequenceNumber}!");
                    return;
                }
                // timeout occured
                StackTraceService.ConsoleLog($"Timeout for packet number: {packet.sequenceNumber}! Resetting window size and capacity!");
                // resetting window size since timeout occured
                WindowService.ResetWindowSize();
            }
            SendPacket(packet);
        }
    }
}
