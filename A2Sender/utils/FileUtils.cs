using A2Sender.models.packets;

namespace A2Sender.utils
{
    public static class FileUtils
    {
        public static byte[]? ReadFile(string fileName)
        {
            try
            {
                return File.ReadAllBytes(fileName);
            }
            catch
            {
                Console.WriteLine("FileReader: ReadFile(): Could not read bytes from file name " + fileName);
                return null;
            }
        }

         // createPacketsFromFile(fileName) consumes a fileName string expected to be in the directory of the 
        //  calling program, and returns an array of packets containing the file data string (as fragments).
        public static bool TryCreatePacketsFromFile(string fileName, out Packet[]? packets)
        {
            string text;
            try {
                // Convert file contents to string
                text = File.ReadAllText(fileName);
            }
            catch (Exception e) {
                Console.WriteLine("FileUtils: TryCreatePacketsFromFile(): Failed to read text from file: ".Concat(fileName));
                packets = null;
                return false;
            }
            // number of characters in the file
            int numCharacters = text.Length;
            
            // number of packets we will need considering each packet can only hold 500 characters
            uint nPackets;

            // see if the total number of packets is unsigned 32 bit
            string nPacketString = (((((uint)numCharacters) + Packet.MAX_SIZE_BYTES - ((uint)1)) / Packet.MAX_SIZE_BYTES)-((uint)1)).ToString();
            if (!UInt32.TryParse(nPacketString, out nPackets)) {
                Console.WriteLine("FileUtils: TryCreatePacketsFromFile(): Max sequence number is not unsigned 32-bit: ".Concat(nPacketString));
                packets = null;
                return false;
            }
            
            // Zero packets to send
            if (nPackets < 0) {
                Console.WriteLine("FileUtils: TryCreatePacketsFromFile(): Zero packets to send.: ".Concat(nPacketString));
                packets = null;
                return false;
            }

            // initialize packets array
            packets = new Packet[nPackets+1];

            // startIndex offset
            int startIndex = 0;

            // initialize packets in packet array
            for (uint p_i = 0; p_i < nPackets+1; ++p_i) {
                // full packet
                if (numCharacters - Packet.MAX_SIZE_BYTES >= 0) {
                    packets[p_i] = new DataPacket(p_i % Packet.MAX_SIZE_BYTES, Packet.MAX_SIZE_BYTES, text.Substring(startIndex, (int) Packet.MAX_SIZE_BYTES));
                    startIndex += (int) Packet.MAX_SIZE_BYTES;
                    numCharacters -= (int) Packet.MAX_SIZE_BYTES;
                }
                // Final packet/ non-full packet with string data < 500
                else {
                    packets[p_i] = new DataPacket(p_i % Packet.MAX_SIZE_BYTES, (uint) numCharacters, text.Substring(startIndex,numCharacters));
                }
            }
            return true;
        }
    }
}
