using System.Diagnostics;
using System.Reflection;

namespace A2Sender.services
{
    // StackTraceService: A singleton service for printing to the console.
    static public class StackTraceService
    {
        // ConsoleLog(message): Prints to the console a message concatanated to the class and method of the caller.
        public static void ConsoleLog(string message)
        {
            StackFrame? stackFrame = new StackTrace().GetFrame(1);
            if (stackFrame != null) {
                MethodBase? methodInformation = stackFrame.GetMethod();
                if (methodInformation != null) {
                    string methodName = methodInformation.Name;
                    if (methodInformation.ReflectedType != null) {
                        string className = methodInformation.ReflectedType.Name;
                        Console.WriteLine($"{className}: {methodName}(): {message}");
                        return;
                    }
                }
            }
            throw new Exception("Something wen't wrong with the stack frame.");
        }
    }
}
