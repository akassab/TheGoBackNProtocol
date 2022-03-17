using A2Receiver.models.packets;
using A2Receiver.utils;

namespace A2Receiver.services
{
     // WindowService: Is a singleton service class representing the receiver's sliding window.
    public static class WindowService
    {
        // The size of the window (N).
        private static int size = 10;
        private static int baseIndex = 0;

        // HashSet that contains the sequence number of packets that have arrived and been
        //  successfully acknowledged at the receiver program (this program).
        private static HashSet<uint> receivedPackets = new HashSet<uint>();
        private static Dictionary<uint, Packet> packets = new Dictionary<uint, Packet>();

        public static int GetBaseIndex() {
            return baseIndex;
        }
        // Increments base index by 1. Must be between 0-31.
        public static void IncrementBaseIndex() {
             if (baseIndex == 31) {
                baseIndex = 0;
            }
            else {
                baseIndex += 1;
            }
        }

        public static bool GetPacketAcknowledged(uint sequenceNumber) {
            return receivedPackets.Contains(sequenceNumber);
        }
        public static void SetPacketAcknowledged(Packet packet) {
            receivedPackets.Add(packet.sequenceNumber);
            packets[packet.sequenceNumber] = packet;
        }

        // Buffers the packets in the window starting from the base index and writes to the data file.
        public static void Buffer() {
            uint[] sortedBuffer = receivedPackets.OrderBy(s => s).ToArray();
            foreach (uint sequenceNumber in sortedBuffer) {
                if (sequenceNumber == baseIndex) {
                    FileUtils.WriteLineToDataFile(packets[sequenceNumber].data);
                    receivedPackets.Remove(sequenceNumber);
                    packets.Remove(sequenceNumber);
                    IncrementBaseIndex();
                }
                else {
                    break;
                }
            }
        }

        // Checks if the sequence number provided lies in the packet window
        public static bool IsPacketInWindow(uint sequenceNumber) {
            int lastindex = baseIndex + 9;
            if (lastindex > 31) {
                int overlap = lastindex - 32;
                return (sequenceNumber >= baseIndex && sequenceNumber <= 31) 
                || (sequenceNumber >= 0 && sequenceNumber <= overlap);

            }
            else {
                return sequenceNumber >= baseIndex && sequenceNumber <= lastindex;
            }
        }

        // Checks if the sequence number is within the last 10
        //  consecutive sequence numbers of the base and false otherwise.
        public static bool IsBeforeBaseIndex(uint sequenceNumber) {
            int tenAwayFromBase = baseIndex - 10;
            if (tenAwayFromBase < 0) {
                return (sequenceNumber >= (32 + tenAwayFromBase)) || sequenceNumber < baseIndex;
            }
            return (sequenceNumber >= tenAwayFromBase && sequenceNumber < baseIndex);
        }
    }
}
