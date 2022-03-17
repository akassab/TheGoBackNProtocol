using System.Diagnostics;
using System.Reflection;

namespace A2Receiver.services
{
    // StackTraceService: A singleton service for printing to the console.
    static public class StackTraceService
    {
        // Prints to the console a message concatanated to the class and method of the caller.
        public static void ConsoleLog(string message)
        {
            StackFrame? stackFrame = new StackTrace().GetFrame(1);
            if (stackFrame != null) {
                MethodBase? methodInformation = stackFrame.GetMethod();
                if (methodInformation != null) {
                    if (methodInformation.ReflectedType != null) {
                        string className = methodInformation.ReflectedType.Name;
                        Console.WriteLine($"{className}: {message}");
                        return;
                    }
                }
            }
            throw new Exception("Something went wrong with the stack frame.");
        }
    }
}
