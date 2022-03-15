using A2Sender.enums;
using A2Sender.models.packets;
using A2Sender.services;

namespace A2Sender.utils
    // FileUtils: Singleton class that provides utilities for reading file into bytes and 
    //  creating packets from a file.
{    public static class FileUtils
    {

        // Maps LogFileEnums to the string file name.
        static readonly Dictionary<LogFileEnum, string> logFileName = new Dictionary<LogFileEnum, string> 
        {
            { LogFileEnum.SeqNum , "seqnum.log" },
            { LogFileEnum.Ack,  "ack.log" },
            { LogFileEnum.N,  "N.log" }
        };


        // TryWriteToFile(fileName, logFile): Tries to write a line to the fileName assoicated with LogFileEnum.
        public static bool TryCreateEmptyLogFiles() {
            try {
                foreach (KeyValuePair<LogFileEnum, string> logFile in logFileName) {
                    File.Create(logFile.Value).Dispose();
                }
                return true;

            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        // TryWriteLineToFile(logFileEnum, line): Tries to write a line to the fileName assoicated with LogFileEnum.
        public static void WriteLineToLogFile(LogFileEnum logFileEnum, string sequenceNumberString) {
            File.AppendAllText(logFileName[logFileEnum], $"t={WindowService.GetTimestamp()} {sequenceNumberString}{Environment.NewLine}");
            WindowService.IncrementTimestamp();
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
