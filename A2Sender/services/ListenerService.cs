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
        // Is the listner service listening for packets?
        private static bool listening = false;
        // The udp client.
        public static UdpClient? udpClient = null;

        // IsListening(): returns true if the ListenerService is listening for packets and false otherwise.
        public static bool IsListening() {
            return ListenerService.listening;
        }

        // ListenForSackPackets(): Start listening for Sack Packets.
        public static void ListenForSackPackets() {
            // Verify this has only been called once.
            if (IsListening()) {
                throw new Exception("Listener is already listening.");
            }
            ListenerService.listening = true;
            
            // Setup
            int listenerPort  = ConsoleArgumentsService.GetPortSender();
            IPEndPoint groupEndpoint = new IPEndPoint(IPAddress.Any, listenerPort);
            udpClient = new UdpClient(listenerPort);

            // Start new thread
            new Thread(async () => {
                try
                {   
                    while (true)
                    {
                        // Wait for bytes
                        StackTraceService.ConsoleLog("Listening for packets...");
                        byte[] receivedPacketBytes = udpClient.Receive(ref groupEndpoint);                  
                        StackTraceService.ConsoleLog($"Received broadcast from {groupEndpoint}.");

                        // Lock the WindowService while we process this incoming data
                        lock (WindowService.windowLock) {

                            // Decode received packetBytes
                            Packet? receivedPacket;
                            if (!PacketUtils.TryDecode(receivedPacketBytes, out receivedPacket) && receivedPacket != null) {
                                StackTraceService.ConsoleLog("Received byte array cannot be decoded into a Packet. Ignoring packet.");
                                continue;
                            }

                            // Get the packet status
                            PacketStatus? packetStatus = WindowService.GetPacketStatus(receivedPacket.sequenceNumber);
                            // See if the packetStatus is null (it was never sent in the first palce) or it has been acknowledged already or is expired
                            //  then skip
                            if (packetStatus == null) {
                                continue;
                            }
                            if (packetStatus.acknowledged || packetStatus.IsExpired()) {
                                FileUtils.WriteLineToLogFile(LogFileEnum.Ack, receivedPacket.sequenceNumber.ToString());
                                continue;
                            }

                            WindowService.SetPacketAcknowledged(receivedPacket.sequenceNumber);
                
                            // If the acknowledged packet was the base index, then increment the base index by 1.
                            if (receivedPacket.sequenceNumber == WindowService.GetBaseIndex()) {
                                Console.WriteLine("here too");
                                WindowService.IncrementBaseIndex();
                            }
                                // we are done and we got the total number of packets
                                if (WindowService.GetNumPacketsReceived() == WindowService.GetTotalNumPackets()) {
                                    FileUtils.WriteLineToLogFile(LogFileEnum.Ack, "EOT");
                                    StackTraceService.ConsoleLog("Done!");
                                    break;
                                }
                                else {
                                    FileUtils.WriteLineToLogFile(LogFileEnum.Ack, receivedPacket.sequenceNumber.ToString());
                                }
                        }
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    Console.WriteLine("closed catched");
                    udpClient.Close();
                }
            }).Start();
        }
    }
}
