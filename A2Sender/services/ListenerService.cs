using A2Sender.enums;
using A2Sender.models;
using A2Sender.models.packets;
using A2Sender.utils;
using System.Net;
using System.Net.Sockets;
namespace A2Sender.services
{
    // Singleton class that listens for ack packets sent by the emulator (ultimately from the receiver).
    public static class ListenerService
    {
        public static UdpClient? udpClient = null;

        public static void ListenForSackPackets() {
            try
            {   
                // setup
                IPEndPoint groupEndpoint = new IPEndPoint(IPAddress.Any, ConsoleArgumentsService.GetPortSender()); 
                udpClient = new UdpClient(ConsoleArgumentsService.GetPortSender());
                StackTraceService.ConsoleLog("Listening for packets...");
                while (true)
                {
                    // wait for bytes
                    byte[] receivedPacketBytes = udpClient.Receive(ref groupEndpoint);         

                    // lock the WindowService while we process this incoming data
                    lock (WindowService.windowLock) {

                        // decode received packetBytes
                        Packet? receivedPacket;
                        if (!PacketUtils.TryDecode(receivedPacketBytes, out receivedPacket) && receivedPacket != null) {
                            StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Could not decode packet. Ignoring...");
                            continue;
                        }

                        if (receivedPacket.type == TypeEnum.Eot) {
                            StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Obtained last packet (EOT)! Closing listener.");
                            FileUtils.WriteLineToLogFile(LogFileEnum.Ack, "EOT");
                            break;
                        }
                        else if (WindowService.IsBeforeBaseIndex(receivedPacket.sequenceNumber)) {
                            StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Obtained pack from before base index. Ignoring...");
                            FileUtils.WriteLineToLogFile(LogFileEnum.Ack, receivedPacket.sequenceNumber.ToString());
                            continue;
                        }

                        // get the packet status
                        PacketStatus? packetStatus = WindowService.GetPacketStatus(receivedPacket.sequenceNumber);

                        if (packetStatus == null) { 
                            StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Packet curropted. Ignoring...");
                            continue;
                        }
                        else if (packetStatus.acknowledged || packetStatus.IsExpired()) {
                            if (packetStatus.acknowledged && packetStatus.IsExpired()) {
                                StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Packet {receivedPacket.sequenceNumber}  acknowledged already and expired. Ignoring...");
                            }
                            else if (packetStatus.acknowledged) {
                                StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Packet{receivedPacket.sequenceNumber}  acknowledged already. Ignoring...");
                            }
                            else if (packetStatus.IsExpired()) {
                                StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Packet {receivedPacket.sequenceNumber} expired. Ignoring...");
                            }
                            // write to log file
                            FileUtils.WriteLineToLogFile(LogFileEnum.Ack, receivedPacket.sequenceNumber.ToString());
                            continue;
                        }

                        StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Packet acknowledged {receivedPacket.sequenceNumber}!");
                        WindowService.SetPacketAcknowledged(receivedPacket.sequenceNumber);

                        // packet is at base of window
                        if (receivedPacket.sequenceNumber == WindowService.GetBaseIndex()) {
                             if (receivedPacket.sequenceNumber != WindowService.GetNumPacketsTotal() - 1) {
                                 WindowService.IncrementBaseIndex();
                             }
                             
                             // remove from window
                             WindowService.RemovePacketFromWindow(receivedPacket.sequenceNumber);

                             PacketStatus? nextPacketStatus = WindowService.GetPacketStatus(((uint)WindowService.GetBaseIndex()));
                             // remove conseuctive from base as well
                             while (nextPacketStatus != null && nextPacketStatus.acknowledged) {
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
