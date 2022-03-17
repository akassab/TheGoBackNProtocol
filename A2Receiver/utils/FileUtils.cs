using A2Receiver.services;

namespace A2Receiver.utils
    // FileUtils: Singleton class that provides utilities for reading file into bytes and 
    //  creating packets from a file.
{    public static class FileUtils
    {

        public static string arrivalLogFileName = "arrival.log";

        // Tries to create a file to store the received data.
        public static bool TryCreateEmptyFile(string fileName) {

            if (File.Exists(fileName)) {
                File.Delete(fileName);
            }

            try {
                File.Create(fileName).Dispose();
                return true;
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        
        // Tries to write a line to the logFile.
        public static void WriteLineToLogFile(string sequenceNumberString) {
            File.AppendAllText(arrivalLogFileName, $"{sequenceNumberString}{Environment.NewLine}");
        }
        
        // Writes text to the data file.
        public static void WriteLineToDataFile(string stringData) {
            File.AppendAllText(ConsoleArgumentsService.GetFileName(), stringData);
        }
    }
}
