using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;



internal static class EventLogger
{

    private const string sourceName = "DVLD Data Logs";
    private const string registryPath = @"SOFTWARE\DVLD";


    public static void EnsureEventSource()
    {

        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(registryPath, true) ?? Registry.CurrentUser.CreateSubKey(registryPath))
        {
            
            if (key?.GetValue("EventSourceRegistered")?.ToString() == "true")
                return; // Already registered, no need for admin rights

            try
            {
                EventLog.CreateEventSource(sourceName, "Application");

                key.SetValue("EventSourceRegistered", "true"); // Mark as registered
            }
            catch //(Exception ex)
            {
                //Console.WriteLine($"Failed to create event source: {ex.Message}");
            }

        }

    }


    public static void WriteExceptionToEventViewer(string exceptionMessage, [CallerFilePath] string filePath = "",
                       [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    { 

        EnsureEventSource();

        StringBuilder logMessage = new StringBuilder();
        logMessage.AppendLine("An exception was caught!");
        logMessage.AppendLine($"Caller Info -> File: {filePath}");
        logMessage.AppendLine($"               Method: {memberName}");
        logMessage.AppendLine($"               Line: {lineNumber}");
        logMessage.AppendLine("Exception Details:");
        logMessage.AppendLine(exceptionMessage);


        try
        {
            EventLog.WriteEntry(sourceName, logMessage.ToString(), EventLogEntryType.Error);
        }
        catch //(Exception ex)
        {
            //Console.WriteLine($"Failed to log error: {ex.Message}");
        }

    }


}

