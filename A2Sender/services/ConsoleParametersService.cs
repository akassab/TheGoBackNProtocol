namespace A2Sender.services
{
    // ConsoleArgumentsService: Singleton Service for parsing, storing, and retreiving console arguments provided 
    //  when running ./sender.sh ... on the command line.
    public static class ConsoleArgumentsService
    {
        // hostname of emulator.
        private static string? hostAddress;
        // port of emulator.
        private static int? portEmulator;
        // port of sender (this program).
        private static int? portSender;
        // timeout for each sent packet.
        private static int? timeout;
        // file name that contains the data to send.
        private static string? fileName;

        // SetConsoleArguments(...): Sets the console parameters. Throws an exception if console parameters were already set.
        public static void SetConsoleArguments(string hostAddress, int portEmulator, int portSender, int timeout, string fileName) {
            if (ConsoleArgumentsService.hostAddress != null
            && ConsoleArgumentsService.portEmulator != null 
            && ConsoleArgumentsService.portSender != null 
            && ConsoleArgumentsService.timeout != null
            && ConsoleArgumentsService.fileName != null) {
                throw new Exception("Console Parameters already set.");
            }

            ConsoleArgumentsService.hostAddress = hostAddress;
            ConsoleArgumentsService.portEmulator = portEmulator;
            ConsoleArgumentsService.portSender = portSender;
            ConsoleArgumentsService.timeout = timeout;
            ConsoleArgumentsService.fileName = fileName;
        }

        // GetHostAddress(): Gets the host address and throws an exception if it was not set.
        public static string GetHostAddress() {
            if (hostAddress == null) {
                throw new Exception("hostAddress not set");
            }
            return hostAddress;
        }

        // GetPortEmulator(): Gets the port emulator and throws an exception if it was not set.
        public static int GetPortEmulator() {
            if (portEmulator == null) {
                throw new Exception("portEmulator not set");
            }
            return (int) portEmulator;
        }

        // GetPortSender(): Gets the port sender and throws an exception if it was not set.
        public static int GetPortSender() {
            if (portSender == null) {
                throw new Exception("portSender not set");
            }
            return (int) portSender;
        }

        // GetTimeout(): Gets the the timeout and throws an exception if it was not set.
        public static int GetTimeout() {
            if (timeout == null) {
                throw new Exception("timeout not set");
            }
            return (int) timeout;
        }

        // GetFileName(): Gets the the file name and throws an exception if it was not set.
        public static string GetFileName() {
            if (fileName == null) {
                throw new Exception("fileName not set");
            }
            return fileName;
        }

        // TryParseAndSetArgs(...): Tries to parse the args provided by console script to set the private fields defined at the top
        //  of this clas. Returns False and outputs a message when something goes wrong and true when success.
        public static bool TryParseAndSetArgs(string[] args)
        {
            StackTraceService.ConsoleLog("Reading Input...");
            
            // validate correct number of arguments
            int numArguments = args.Length;
            if (numArguments != 5) {
                StackTraceService.ConsoleLog("Invalid number of arguments provided.");
                return false;
            }
            // validate no empty or whitespace
            for (int i = 0; i < numArguments; ++i) {
                if (string.IsNullOrEmpty(args[i]) || string.IsNullOrWhiteSpace(args[i]) ) {
                    StackTraceService.ConsoleLog($"Argument {(i+1)} + is invalid.");
                    return false;
                }
            }
            // get host name
            string hostAddress = args[0];
            // get portEmulator
            int portEmulator;
            if (!int.TryParse(args[1], out portEmulator) || portEmulator <= 0) {
                StackTraceService.ConsoleLog("<port_emulator> must an unsigned integer.");
                return false;
            }
            // get port sender
            int portSender;
            if (!int.TryParse(args[2], out portSender) || portSender <= 0) {
                StackTraceService.ConsoleLog("<port_sender> must an unsigned integer.");
                return false;
            }
            // get timeout
            int timeout;
            if (!int.TryParse(args[3], out timeout) || timeout <= 0) {
                StackTraceService.ConsoleLog("<timeout> must an unsigned integer.");
                return false;
            }
            // get fileName
            string fileName = args[4];
        
            // Initialize data class
            SetConsoleArguments(hostAddress, portEmulator, portSender, timeout, fileName);
            Console.WriteLine("Reading input successful!");
            return true;
        }
    }
}
