using System.Text;
using System.Net;
using A2Sender.enums;
using A2Sender.models.packets;

namespace A2Sender.utils
{
    // Packet data class
    public static class PacketUtils
    {
        static public byte[] Encode(Packet packet) {
            // Adjust byte orders for integer fields
            byte[] typeLittenEndian = BitConverter.GetBytes(IPAddress.NetworkToHostOrder((long)packet.type));
            byte[] typeBigEndian = new byte[4];
            Array.Copy(typeLittenEndian, 4, typeBigEndian, 0, 4);

            byte[] sequenceNumberLittleEndian =  BitConverter.GetBytes(IPAddress.NetworkToHostOrder((long)packet.sequenceNumber));
            byte[] sequenceNumberBigEndian = new byte[4];
            Array.Copy(sequenceNumberLittleEndian, 4, sequenceNumberBigEndian, 0, 4);

            byte[] lengthLittleEndian = BitConverter.GetBytes(IPAddress.NetworkToHostOrder((long)packet.length));
            byte[] lengthBigEndian = new byte[4];
            Array.Copy(lengthLittleEndian, 4, lengthBigEndian, 0, 4);

            var encodedData = Encoding.ASCII.GetBytes(packet.data);

            var x =  new byte[][] {
               typeBigEndian,
               sequenceNumberBigEndian,
               lengthBigEndian,
               encodedData
            }.SelectMany(b => b).ToArray();
            return x;
        }

        static public bool TryDecode(byte[] packetBytes, out Packet? packet) {
            int nBytes = packetBytes.Length;

            if (!(nBytes > 12)) {
                Console.WriteLine("Could not decode a packet with ".Concat(nBytes.ToString()).Concat("number of bytes."));
                packet = null;
                return false;
            }

            try {
                // decode type
                byte[] typeBytes = new Byte[4];
                Array.Copy(packetBytes, 0, typeBytes, 0, 4);
                TypeEnum? type = TypeEnumUtils.UIntToTypeEnum(BitConverter.ToUInt32(typeBytes));
                if (type == null) {
                    Console.WriteLine("Could not determine type of the packet.");
                    packet = null;
                    return false;
                }
                // decode sequenceNumber
                byte[] sequenceBytes = new Byte[4];
                Array.Copy(packetBytes, 4, sequenceBytes, 0, 4);
                uint sequenceNumber = BitConverter.ToUInt32(sequenceBytes);
                // decode length
                byte[] lengthBytes = new Byte[4];
                Array.Copy(packetBytes, 8, lengthBytes, 0, 4);
                uint length = BitConverter.ToUInt32(lengthBytes);
                // decode string data
                string data = Encoding.ASCII.GetString(packetBytes, 12, nBytes - 12);

                // Create the packet
                packet = Packet.PacketFactory((TypeEnum) type, sequenceNumber, length, data);
            
                return true;
            }
            catch (Exception e) {
                 Console.WriteLine("Could not decode a packet for some reason");
                 packet = null;
                 return false;
            }
        }
    }
}
