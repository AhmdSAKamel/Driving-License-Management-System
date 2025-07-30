using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;




internal class LogUIExceptions
{
    private const string sourceName = "DVLD UI Logs";
    private const string registryPath = @"SOFTWARE\DVLD";


    public static void EnsureEventSource()
    {

        if (!EventLog.SourceExists(sourceName))
        {

            try
            {
                EventLog.CreateEventSource(sourceName, "Application");
            }
            catch
            {
                //
            }

        }
       
    }


    public static void WriteExceptionToEventViewer(Exception ex, [CallerFilePath] string filePath = "",
                       [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    {
        EnsureEventSource();

        // Build a detailed log message
        StringBuilder logMessage = new StringBuilder();
        logMessage.AppendLine("An exception was caught!");
        logMessage.AppendLine($"Caller Info -> File: {filePath}");
        logMessage.AppendLine($"               Method: {memberName}");
        logMessage.AppendLine($"               Line: {lineNumber}");
        logMessage.AppendLine("Exception Details:");
        logMessage.AppendLine(ex.ToString());

        try
        {
            // Log the complete details to the event log
            EventLog.WriteEntry(sourceName, logMessage.ToString(), EventLogEntryType.Error);
        }
        catch //(Exception logEx)
        {
            // In case logging to Event Viewer fails, consider other fallback logging methods.
            // For instance, writing directly to a file or using another logging framework.
        }


        /*
          Caller Information Attributes: By adding [CallerFilePath], [CallerMemberName], and [CallerLineNumber] to the parameters, you automatically capture the file path, method name, and line number from where WriteExceptionToEventViewer is called. This produces additional context that can help pinpoint exactly where the exception was logged.

          Using a StringBuilder: We use a StringBuilder to compose a detailed log message that includes both the caller information and the full exception details as provided by ex.ToString(). This gives you a comprehensive view of both the context and the exception itself.

          Fallback Code: The try-catch block around the event log write is important to prevent any failures during logging from propagating further. In a production-level system, if logging fails, you might wish to fallback to another logging mechanism.

          Ex.ToString() & StackTrace: Note that ex.ToString() typically includes the stack trace. However, if you need more granular access to individual stack frames (for instance, if you wish to iterate over them or format them differently), you could use the System.Diagnostics.StackTrace class to analyze the stack frames more explicitly.

          Additional Considerations

          Event Log Message Length: Event Viewer has a maximum message size. If your exception details (combined with the contextual information) exceed this limit, consider truncating the output or splitting the message across multiple entries.

          Security: Make sure that the output you log does not expose sensitive information, especially if the logs might be reviewed by personnel not authorized to see full internal details.

          This enhanced logging approach should fulfill your need to see not only the exception’s details but also the exact location in your program where the logging method was invoked. If you’re curious about alternative approaches, you might also consider third-party logging frameworks such as NLog, log4net, or Serilog that offer more versatile features out of the box.
         */

    }



}
