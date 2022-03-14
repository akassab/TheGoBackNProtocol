using A2Sender.enums;

namespace A2Sender.models.packets
{
    // Packet data class
    abstract public class Packet
    {
        public static readonly uint MAX_SIZE_BYTES = 500;
        public static readonly uint SEQUENCE_NUMBER_DIVISOR = 32;
        
        // 0: SACK, 1: Data, 2: EOT
        public readonly TypeEnum type;
        // Modulo 32 
        public readonly uint sequenceNumber;
        // Length of the String variable ‘data’ 
        public readonly uint length;
        // String with Max Length 500
        public readonly string data;

        // constructor
        protected Packet(TypeEnum type, uint sequenceNumber = 32, uint length = 0, string data = "")
        {
            this.type = type;
            this.sequenceNumber = sequenceNumber;
            this.length = length;
            this.data = data;
        }

        public static Packet? PacketFactory(TypeEnum type, uint sequenceNumber = 32, uint length = 0, string data = "") {
            switch (type) {
                case TypeEnum.Data:
                    return new DataPacket(sequenceNumber, length, data);
                case TypeEnum.Sack:
                    return new SackPacket(sequenceNumber);
                case TypeEnum.Eot:
                    return new EotPacket();
                default:
                    return null;
            }
        }
    
        public override string ToString() {
            return $"Type: {this.type}, Seqnum: {this.sequenceNumber}, Length: {this.length}, Data: ...length={this.data.Length}";
        }
    }
}

