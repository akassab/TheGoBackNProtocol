namespace A2Sender.services
{
    public static class WindowService
    {
        private static int size = 1;
        private static int numPacketsSent = 0;
        private static int baseIndex = 0;

        public static int GetWindowSize() {
            return size;
        }

        public static int GetNumPacketsSent() {
            return numPacketsSent;
        }

        public static int GetBaseIndex() {
            return baseIndex;
        }

        public static void ResetWindowSize() {
            size = 1;
        }

        public static void IncrementWindowSize() {
            size += 1;
        }

        public static void IncrementNumPacketsSent() {
            numPacketsSent += 1;
        }
    }
}