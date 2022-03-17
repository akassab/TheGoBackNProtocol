using A2Receiver.models.packets;
using A2Receiver.utils;
using System.Net;
using System.Net.Sockets;

namespace A2Receiver.services
{
    // Singleton class that sends data and eot packet(s) to the emulator and eventually the sender.
    public static class SenderService
    {
        // UdpClient used to send data packets.
        private static readonly UdpClient udpSender = new UdpClient(AddressFamily.InterNetwork);
        
        // Setup UDP client
        public static void ConnectUdpClient() {
            IPEndPoint hostEndPoint = new IPEndPoint(
                IPAddress.Parse(ConsoleArgumentsService.GetHostName()), 
                ConsoleArgumentsService.GetPortEmulator()
                );
            udpSender.Connect(hostEndPoint);
        }

        // Sends a sack packet to the sender
        public static void SendSackPacket(Packet packet) {
            try {
                // send the packets
                udpSender.Send(PacketUtils.Encode(packet));
               
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
