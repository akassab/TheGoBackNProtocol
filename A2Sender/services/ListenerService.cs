using System.Net;
using System.Net.Sockets;
using A2Sender.models;
using A2Sender.models.packets;
using A2Sender.utils;

namespace A2Sender.services
{
    public static class ListenerService
    {
        private static bool listening = false;

        public static bool GetIsListening() {
            return ListenerService.listening;
        }

        // Listen for acknowledgement (this should run under a new thread)
        public static void ListenForAck() {

            if (GetIsListening()) {
                throw new Exception("ListenerService: ListenForAck(): Listener is already listening.");
            }

            // start listening for ack
            ListenerService.listening = true;

            // start new thread
            new Thread(async () => {

                // sender's listener port
                int listenerPort = listenerPort = ConsoleParametersService.GetPortSender();
                IPEndPoint groupEndpoint = new IPEndPoint(IPAddress.Any, listenerPort);
                // setup udp client listener for acks
                UdpClient listener = new UdpClient(listenerPort);
                
                try
                {
                    while (true)
                    {
                        Console.WriteLine("ListenerService: ListenForAck(): Waiting for broadcast");
                        byte[] receivedPacketBytes = listener.Receive(ref groupEndpoint);
                        Console.WriteLine($"ListenerService: ListenForAck(): Received broadcast from {groupEndpoint} :");

                        // Decode received packetBytes
                        Packet? receivedPacket;
                        if (!PacketUtils.TryDecode(receivedPacketBytes, out receivedPacket) && receivedPacket != null) {
                            Console.WriteLine("ListenerService: ListenForAck(): Received byte array cannot be decoded into a Packet.");
                            continue;
                        }

                        // See if the decocded packet was sent in the first palce
                        SentPacketInfo? sentPacketInfo;
                        if (!SenderService.sentPackets.TryGetValue((receivedPacket.sequenceNumber), out sentPacketInfo) && sentPacketInfo != null) {
                            Console.WriteLine("ListenerService: ListenForAck(): We never sent a packet with sequence number".Concat(receivedPacket.sequenceNumber.ToString()));
                            continue;
                        }

                        // See if the packet has been acknowledged already
                        if (sentPacketInfo.acknowledged) {
                            // packet already acknowledges and we are receiving it again
                        }
                        // If it has not been acknowledged yet, see if it has been expired or not
                        else if (!sentPacketInfo.IsExpired()) {
                            // not acknowledged yet but we have met the timeout requirements
                            SenderService.sentPackets[receivedPacket.sequenceNumber].acknowledged = true;
                        }
                        else {
                            // failed the timeout requirements
                        }
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    listener.Close();
                }
            }).Start();
        }
    }
}