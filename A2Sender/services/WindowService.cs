using A2Sender.enums;
using A2Sender.models;
using A2Sender.utils;
namespace A2Sender.services
{
    // WindowService: Is a singleton service class representing the sender's sliding window.
    public static class WindowService
    {
        // For race conditions
        public static readonly object windowLock = new object();

        // Window
        private static int windowSize = 1;
        private static int windowCapacity = 0;
        private static bool wasWindowReset = false;
        private static int baseIndex = 0;
        private static int timestamp = 0;
        private static int lap = 0;

        // History of packets
        private static int numPacketsTotal = 0;
        private static int numPacketsAcknowledged = 0;

        // Dictionary that maps a sent sequence number to a PacketStatus that contains information about sent packets
        //  (whether they have been acknowledges yet or not and the time at which the packet was sent).
        private static Dictionary<uint, PacketStatus> sentPackets = new Dictionary<uint, PacketStatus>();

        // windowSize and wasWindowReset methods
        public static int GetWindowSize() {
            return windowSize;
        }
        public static void IncrementWindowSize() {
            if (windowSize != 10) {
                windowSize += 1;
                FileUtils.WriteLineToLogFile(LogFileEnum.N, windowSize.ToString());
            }
        }
        public static bool GetWasWindowReset() {
            if (wasWindowReset) {
                wasWindowReset = false;
                return true;
            }
            return false;
        }
        public static void ResetWindowSize() {
            windowCapacity = 0;
            windowSize = 1;
            FileUtils.WriteLineToLogFile(LogFileEnum.N, windowSize.ToString());
            wasWindowReset = true;
        }

        // windowCapacity methods
        public static int GetWindowCapacity() {
            return windowCapacity;
        }
        public static bool IncrementWindowCapacity() 
        {
            if (windowSize == windowCapacity) {
                return false;
            }
            else {
                windowCapacity += 1;
                return true;
            }
        }

        // baseIndex methods
        public static int GetBaseIndex() {
            return baseIndex;
        }
        public static void IncrementBaseIndex() {
            if (baseIndex == 31) {
                baseIndex = 0;
                lap += 1;
            }
            else {
                baseIndex += 1;
            }
        }

        // timestamp methods
        public static int GetTimestamp() {
            return timestamp;
        }
        public static void IncrementTimestamp() {
            timestamp += 1;
        }

        // Get which lap we are on since we only use seq numbers 0-31
        public static int GetLap() {
            return lap;
        }
        
        // numPacketsTotal methods
        public static void SetNumPacketsTotal(int numPacketsTotal) {
            WindowService.numPacketsTotal = numPacketsTotal;
        }
        public static int GetNumPacketsTotal() {
            return numPacketsTotal;
        }

        public static void InitializePacketStatus(uint sequenceNumber) {
            sentPackets[sequenceNumber] = new PacketStatus();
        }
        public static PacketStatus? GetPacketStatus(uint sequenceNumber) {
            if (sentPackets.TryGetValue(sequenceNumber, out PacketStatus? packetStatus) && packetStatus != null) {
                return packetStatus;
            }
            else {
                return null;
            }
        }
        public static void SetPacketAcknowledged(uint sequenceNumber) {
            sentPackets[sequenceNumber].acknowledged = true;
        }

        public static void RemovePacketFromWindow(uint sequenceNumber) {
            sentPackets.Remove(sequenceNumber);
            windowCapacity -= 1;
        }
        public static bool IsBeforeBaseIndex(uint sequenceNumber) {
            int difference = baseIndex - 10;
            if (difference < 0) {
                return (sequenceNumber >= (32 + difference)) || sequenceNumber < baseIndex;
            }
            return (sequenceNumber >= difference && sequenceNumber < baseIndex);
        }
    }
}
