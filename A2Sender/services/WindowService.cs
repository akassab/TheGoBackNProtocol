using A2Sender.models;
namespace A2Sender.services
{
    public static class WindowService
    {

        private static int size = 1;
        private static int numPacketsSent = 0;
        private static int baseIndex = 0;
        private static bool wasReset = false;

        public static readonly object windowLock = new object();
        private static Dictionary<uint, PacketStatus> sentPackets = new Dictionary<uint, PacketStatus>();

        public static int GetWindowSize() {
            return size;
        }

        public static int GetNumPacketsSent() {
            return numPacketsSent;
        }

        public static int GetBaseIndex() {
            return baseIndex;
         
        }

        public static bool GetWasReset() {
            if (wasReset) {
                wasReset = false;
                return true;
            }
            return false;
        }

        public static void ResetWindowSize() {
                numPacketsSent = 0;
                size = 1;
                wasReset = true;
        }

        public static void IncrementWindowSize() {
                size += 1;
        }

        public static bool IncrementNumPacketsSent() {
                numPacketsSent += 1;
                return true;
            
        }

        // maybe check if packet is already here
        public static void InitializePacketStatus(uint sequenceNumber) {
                sentPackets[sequenceNumber] = new PacketStatus();
        }

        public static PacketStatus? GetPacketStatus(uint sequenceNumber) {
            if (sentPackets.TryGetValue(sequenceNumber, out PacketStatus? packetStatus) && packetStatus != null) {
                return packetStatus;
            }
            else {
                Console.WriteLine("WindowService: GetPacketStatus(): Could not get packet status for packet with sequence number ".Concat(sequenceNumber.ToString()));
                return null;
            }
        }

        public static void SetPacketAcknowledgeStatus(uint sequenceNumber, bool status) {
            if (sentPackets.TryGetValue(sequenceNumber, out PacketStatus? packetStatus) && packetStatus != null) {
                sentPackets[sequenceNumber].acknowledged = status;
            }
            else {
                Console.WriteLine("WindowService: SetPacketAcknowledgeStatus(): Could not set packet acknowledge status for packet with sequence number ".Concat(sequenceNumber.ToString()));
            }
        }
    }
}