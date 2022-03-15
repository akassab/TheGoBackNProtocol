using A2Sender.models;
namespace A2Sender.services
{
    // WindowService: Is a singleton service class representing the sender's sliding window.
    public static class WindowService
    {
        // The total number of packets expected to send.
        private static int? totalNumPackets = null;
        // The size of the window (N).
        private static int size = 1;
        // The number of packets sent to the emulator-> receiver so far.
        private static int numPacketsSent = 0; // unique
        // The number of acknowledged packets received from the receiver->emulator->this program.
        private static int numPacketsReceived = 0;
        // The base index of this sliding window.
        private static int baseIndex = 0;
        // Was the window reset?
        private static bool wasReset = false;
        // Window lock for accessing the window when multi-threading.
        public static readonly object windowLock = new object();
        // Dictionary that maps a sent sequence number to a PacketStatus that contains information about sent packets
        //  (whether they have been acknowledges yet or not and the time at which the packet was sent).
        private static Dictionary<uint, PacketStatus> sentPackets = new Dictionary<uint, PacketStatus>();

        // SetTotalNumPackets(totalNumPackets): Sets the totalNumPackets field or throws an exception if it was already set.
        public static void SetTotalNumPackets(int totalNumPackets) {
            if (WindowService.totalNumPackets == null) {
                WindowService.totalNumPackets = totalNumPackets;
            }
            else {
                throw new Exception("Total Number of packets should be not be set twice.");
            }
        }

        // GetTotalNumPackets(): Returns the totalNumber of packets (including the eot packet) and throws an exception
        //  if totalNumPackets was not even set.
        public static int GetTotalNumPackets() {
            if (totalNumPackets == null) {
                throw new Exception("Total Number of packets should not be null");
            }
            else {
                return (int) totalNumPackets;
            }
        }

        // GetWindowSize(): Returnst the window size so far.
        public static int GetWindowSize() {
            return size;
        }

        // ResetWindowSize(): Increments the window size by 1.
        public static void IncrementWindowSize() {
                size += 1;
        }

        // GetNumPacketsSent(): Returns the number of packets sent so far.
        public static int GetNumPacketsSent() {
            return numPacketsSent;
        }

        // IncrementNumPacketsSent(): Increments the number of packets sent by one and returns true.
        public static bool IncrementNumPacketsSent() {
                numPacketsSent += 1;
                return true;
        }

        // GetNumPacketsReceived(): Returns the number of (unique) acknowledged packets received so far.
        public static int GetNumPacketsReceived() {
            return numPacketsReceived;
        }

        // SetPacketAcknowledged(sequenceNumber): Marks a packet as received and increments numPacketsReceived by one,
        //  only if the packet has not been received yet.
        public static void SetPacketAcknowledged(uint sequenceNumber) {
            sentPackets[sequenceNumber].acknowledged = true;
            numPacketsReceived += 1;
        }

        //GetBaseIndex(): Gets the base index of this window.
        public static int GetBaseIndex() {
            return baseIndex;
        }

        // IncrementBaseIndex(): Increments base index by 1.
        public static void IncrementBaseIndex() {
            baseIndex += 1;
        }

        // GetWasReset(): Returns true if the window size was reset and false otherwise.
        public static bool GetWasReset() {
            if (wasReset) {
                wasReset = false;
                return true;
            }
            return false;
        }

        // ResetWindowSize(): Resets the window size.
        public static void ResetWindowSize() {
            numPacketsSent = 0;
            size = 1;
            wasReset = true;
        }

        // InitializePacketStatus(sequenceNumber): Adds sequence number to sentPackets dictionary and initializes the packet status.
        public static void InitializePacketStatus(uint sequenceNumber) {
                sentPackets[sequenceNumber] = new PacketStatus();
        }

        // GetPacketStatus(sequenceNumber): Gets the packet status a sequence number, and returns null otherwise.
        public static PacketStatus? GetPacketStatus(uint sequenceNumber) {
            if (sentPackets.TryGetValue(sequenceNumber, out PacketStatus? packetStatus) && packetStatus != null) {
                return packetStatus;
            }
            else {
                StackTraceService.ConsoleLog($"Could not get packet status for packet with sequence number {sequenceNumber}");
                return null;
            }
        }
    }
}
