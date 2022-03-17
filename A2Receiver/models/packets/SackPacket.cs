using A2Receiver.enums;

namespace A2Receiver.models.packets
{
    // Is a Packet (inheritence).
    public class SackPacket : Packet
    {
        public SackPacket(uint sequenceNumber) : base(TypeEnum.Sack, sequenceNumber) {}
    }
}
