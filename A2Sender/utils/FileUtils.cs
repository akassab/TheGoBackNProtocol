using A2Sender.models.packets;
using A2Sender.services;

namespace A2Sender.utils
    // FileUtils: Singleton class that provides utilities for reading file into bytes and 
    //  creating packets from a file.
{    public static class FileUtils
    {
        // ReadFile(fileName): Tries create an array of bytes representing bytes containing data in fileName and
        //  returns null when it cannot.
        public static byte[]? ReadFile(string fileName)
        {
            try
            {
                return File.ReadAllBytes(fileName);
            }
            catch
            {
                StackTraceService.ConsoleLog($"Could not read bytes from file name {fileName}.");
                return null;
            }
        }

         // TryCreatePacketsFromFile(...): Returns a tuple containing 1. an array of DataPackets created from fileName, and
         //     2. an eot packet. Returns true on success and false otherwise.
        public static bool TryCreatePacketsFromFile(string fileName, out (DataPacket[]? dataPackets, EotPacket? eotPacket) packets)
        {
            // Get all text from the file
            string text;
            try {
                text = File.ReadAllText(fileName);
            }
            catch (Exception e) {
                StackTraceService.ConsoleLog($"Failed to read text from file: {fileName}");
                packets = (null,null);
                return false;
            }

            // Calculate the number of packets needed and determine if that number is even an unsigned integer
            int numCharacters = text.Length;
            uint nPackets= ((((uint)numCharacters) + Packet.MAX_LENGTH - ((uint)1)) / Packet.MAX_LENGTH);
            // Zero packets to send
            if (nPackets < 1) {
                StackTraceService.ConsoleLog("Zero packets to send.");
                packets = (null,null);
                return false;
            }

            // Create array of data packets
            DataPacket[] dataPackets = new DataPacket[nPackets+1];
            int startIndex = 0;
            for (uint p_i = 0; p_i < nPackets; ++p_i) {
                // full packet
                if (numCharacters - Packet.MAX_LENGTH >= 0) {
                    dataPackets[p_i] = new DataPacket(p_i % Packet.SEQUENCE_NUMBER_DIVISOR, Packet.MAX_LENGTH, text.Substring(startIndex, (int) Packet.MAX_LENGTH));
                    startIndex += (int) Packet.MAX_LENGTH;
                    numCharacters -= (int) Packet.MAX_LENGTH;
                }
                // less than full packet
                else {
                    dataPackets[p_i] = new DataPacket(p_i % Packet.SEQUENCE_NUMBER_DIVISOR, (uint) numCharacters, text.Substring(startIndex,numCharacters));
                }
            }
            packets = (dataPackets, new EotPacket(nPackets+1 % Packet.SEQUENCE_NUMBER_DIVISOR));
            return true;
        }
    }
}
