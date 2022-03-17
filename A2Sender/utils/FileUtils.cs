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

        // Tries to write a line to the fileName assoicated with LogFileEnum.
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

        // Tries to write a line to the fileName assoicated with LogFileEnum.
        public static void WriteLineToLogFile(LogFileEnum logFileEnum, string sequenceNumberString) {
            lock (WindowService.windowLock) {
                File.AppendAllText(logFileName[logFileEnum], $"t={WindowService.GetTimestamp()} {sequenceNumberString}{Environment.NewLine}");
                WindowService.IncrementTimestamp();
            }
        }

        // Returns a tuple containing 1. an array of DataPackets created from fileName, and
        //     2. an eot packet. Returns true on success and false otherwise.
        public static (DataPacket[] dataPackets, EotPacket eotPacket) CreatePacketsFromFile(string fileName)
        {
            // get all text from the file
            string text = File.ReadAllText(fileName);

            // calculate the number of packets needed
            int numCharacters = text.Length;
            uint nPackets= ((((uint)numCharacters) + Packet.MAX_LENGTH - ((uint)1)) / Packet.MAX_LENGTH);

            if (nPackets < 1) {
                StackTraceService.ConsoleLog($"ERROR: Zero packets to send from file {fileName}.");
                throw new Exception();
            }

            // create array of data packets
            DataPacket[] dataPackets = new DataPacket[nPackets];
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
            return (dataPackets, new EotPacket(nPackets % Packet.SEQUENCE_NUMBER_DIVISOR));
        }
    }
}
