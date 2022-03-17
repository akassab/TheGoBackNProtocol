using A2Receiver.enums;

namespace A2Receiver.models.packets
{
    public class EotPacket : Packet
    {
        // Is a Packet (inheritence).
        public EotPacket(uint sequenceNumber) : base(TypeEnum.Eot, sequenceNumber) {}
    }
}
