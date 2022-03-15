using A2Sender.enums;

namespace A2Sender.models.packets
{
    // Packet: Abtract class that reprsents a Packet.
    abstract public class Packet
    {
        // Constant represnting that max number of characters allowed in a packet.
        public static readonly uint MAX_LENGTH = 500;
        // Constant used to number the sequenceNumber for each packet when performing modulo.
        public static readonly uint SEQUENCE_NUMBER_DIVISOR = 32;
        
        // Type of packet (Data, Sack, Eot).
        public readonly TypeEnum type;
        // Sequence number of the packet.
        public readonly uint sequenceNumber;
        // Length of the packet (number of characters).
        public readonly uint length;
        // String data in the packet.
        public readonly string data;

        // Constructor.
        protected Packet(TypeEnum type, uint sequenceNumber, uint length = 0, string data = "")
        {
            this.type = type;
            this.sequenceNumber = sequenceNumber;
            this.length = length;
            this.data = data;
        }

        // Overrides ToString() method.
        public override string ToString() {
            return $"Type: {this.type}, Seqnum: {this.sequenceNumber}, Length: {this.length}, Data: ...length={this.data.Length}";
        }
    }
}
