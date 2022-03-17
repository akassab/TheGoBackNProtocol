using A2Receiver.enums;
using A2Receiver.models.packets;
using A2Receiver.utils;
using System.Net;
using System.Net.Sockets;

namespace A2Receiver.services
{
    // Singleton class that listens for data and eot packet(s) sent by the emulator (ultimately from the sender).
    public static class ListenerService
    {
        public static void ListenForDataAndEotPackets() {
            try
            {
                // setup
                int receiverPort  = ConsoleArgumentsService.GetPortReceiver();
                IPEndPoint groupEndpoint = new IPEndPoint(IPAddress.Any, receiverPort);
                UdpClient udpClient = new UdpClient(receiverPort);
                StackTraceService.ConsoleLog("Listening for packets...");

                while (true)
                {
                    // wait for bytes
                    byte[] receivedPacketBytes = udpClient.Receive(ref groupEndpoint);            
                    
                    // decode received packetBytes
                    Packet? receivedPacket;
                    if (!PacketUtils.TryDecode(receivedPacketBytes, out receivedPacket) && receivedPacket != null) {
                            StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Could not decode packet. Ignoring.");
                        continue;
                    }

                    // we received an EOT packet
                    if (receivedPacket.type == TypeEnum.Eot) {
                        StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Obtained last packet (EOT)! Responding and Closing listener.");
                        SenderService.SendSackPacket(PacketUtils.Factory(TypeEnum.Eot, receivedPacket.sequenceNumber));
                        FileUtils.WriteLineToLogFile("EOT");
                        break;
                    }
                    // a data packet
                    else if (receivedPacket.type == TypeEnum.Data) {
                        FileUtils.WriteLineToLogFile(receivedPacket.sequenceNumber.ToString());
                        // is before base index
                        if (WindowService.IsBeforeBaseIndex(receivedPacket.sequenceNumber)) {
                            StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Obtained pack from before base index! Respoding with SACK {receivedPacket.sequenceNumber}.");
                            SenderService.SendSackPacket(PacketUtils.Factory(TypeEnum.Sack, receivedPacket.sequenceNumber));
                            continue;
                        }
                        // is in window
                        else if (WindowService.IsPacketInWindow(receivedPacket.sequenceNumber)) {
                            // not acknowledged yet
                            if (!WindowService.GetPacketAcknowledged(receivedPacket.sequenceNumber)) {
                                // set is as acknowldged
                                WindowService.SetPacketAcknowledged(receivedPacket);
                                // is at base
                                if (WindowService.GetBaseIndex() == receivedPacket.sequenceNumber) {
                                    StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Packet is in beginning of window. Not acknowledged yet. Buffering consecutive packets(s) and responding with Sack {receivedPacket.sequenceNumber}.");
                                    // buffer consecutive from base
                                    WindowService.Buffer();
                                }
                                // not at base
                                else {
                                    StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Packet is in window. Not acknowledged yet. Responding with Sack {receivedPacket.sequenceNumber}.");
                                }
                            }
                            // already acknowledged
                            else {
                                 StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Packet is in window. Already acknowledged. Responding with Sack {receivedPacket.sequenceNumber}.");
                            }
                            // send sack
                            SenderService.SendSackPacket(PacketUtils.Factory(TypeEnum.Sack, receivedPacket.sequenceNumber));
                        }
                    }
                    else {
                        // curripted
                        StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}: Packet curropted. Ignoring.");
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
        }
    }
}