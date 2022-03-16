using A2Sender.enums;
using A2Sender.models;
using A2Sender.models.packets;
using A2Sender.utils;
using System.Net;
using System.Net.Sockets;
namespace A2Sender.services
{
    // ListenerService: Singleton class that listens for ack packets sent by the emulator (ultimately from the receiver).
    public static class ListenerService
    {

        // Start listening for Sack Packets.
        public static UdpClient? udpClient = null;

        public static void ListenForSackPackets() {
            try
            {   
                // Setup
                IPEndPoint groupEndpoint = new IPEndPoint(IPAddress.Any, ConsoleArgumentsService.GetPortSender()); 
                udpClient = new UdpClient(ConsoleArgumentsService.GetPortSender());
                while (true)
                {
                    // Wait for bytes
                    StackTraceService.ConsoleLog("Listening for packets...");
                    byte[] receivedPacketBytes = udpClient.Receive(ref groupEndpoint);                  
                    StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}!");

                    // Lock the WindowService while we process this incoming data
                    lock (WindowService.windowLock) {

                        // Decode received packetBytes
                        Packet? receivedPacket;
                        if (!PacketUtils.TryDecode(receivedPacketBytes, out receivedPacket) && receivedPacket != null) {
                            StackTraceService.ConsoleLog("Could not decode packet.");
                            StackTraceService.ConsoleLog("Ignoring...");
                            continue;
                        }

                        if (receivedPacket.type == TypeEnum.Eot) {
                            StackTraceService.ConsoleLog("Obtained last packet (EOT)!");
                            StackTraceService.ConsoleLog("Closing Listener...");
                            FileUtils.WriteLineToLogFile(LogFileEnum.Ack, "EOT");
                            break;
                        }
                        else if (WindowService.IsBeforeBaseIndex(receivedPacket.sequenceNumber)) {
                            StackTraceService.ConsoleLog("Obtained pack from before base index!");
                            FileUtils.WriteLineToLogFile(LogFileEnum.Ack, receivedPacket.sequenceNumber.ToString());
                            continue;
                        }

                        // Get the packet status
                        PacketStatus? packetStatus = WindowService.GetPacketStatus(receivedPacket.sequenceNumber);

                        if (packetStatus == null) {
                            StackTraceService.ConsoleLog("Packet Curropted!");
                            StackTraceService.ConsoleLog("Ignoring...");
                            continue;
                        }
                        else if (packetStatus.acknowledged || packetStatus.IsExpired()) {
                            if (packetStatus.acknowledged) {
                                StackTraceService.ConsoleLog("Packet already acknowledged!");
                            }
                            if (packetStatus.IsExpired()) {
                                StackTraceService.ConsoleLog("Packet already expired!");
                            }
                            // Write to log file
                            FileUtils.WriteLineToLogFile(LogFileEnum.Ack, receivedPacket.sequenceNumber.ToString());
                            continue;
                        }

                        StackTraceService.ConsoleLog($"Acknowledging packet with sequence number {receivedPacket.sequenceNumber}!");
                        WindowService.SetPacketAcknowledged(receivedPacket.sequenceNumber);

                        if (receivedPacket.sequenceNumber == WindowService.GetBaseIndex()) {

                             StackTraceService.ConsoleLog($"That was at the baseIndex! Incrementing new base index...");
                             if (receivedPacket.sequenceNumber != WindowService.GetNumPacketsTotal() - 1) {
                                 WindowService.IncrementBaseIndex();
                             }
                             
                             WindowService.RemovePacketFromWindow(receivedPacket.sequenceNumber);

                             PacketStatus? nextPacketStatus = WindowService.GetPacketStatus(((uint)WindowService.GetBaseIndex()));
                             while (nextPacketStatus != null && nextPacketStatus.acknowledged) {

                                 StackTraceService.ConsoleLog($"Next up packet number {WindowService.GetBaseIndex()} was acknowledged before too!.. Incrementing new base index...");
                                 WindowService.RemovePacketFromWindow((uint)WindowService.GetBaseIndex());
                                 WindowService.IncrementBaseIndex();

                                 nextPacketStatus = WindowService.GetPacketStatus(((uint)WindowService.GetBaseIndex()));
                             }
                        }
                        FileUtils.WriteLineToLogFile(LogFileEnum.Ack, receivedPacket.sequenceNumber.ToString());
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
