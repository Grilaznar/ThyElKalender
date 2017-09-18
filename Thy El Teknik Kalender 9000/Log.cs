using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace Thy_El_Teknik_Kalender_9000.DataLayer
{
  public class Log
  {
    private static readonly string logFile = AppDomain.CurrentDomain.BaseDirectory + "Log.txt";
    private static object LOCK = new object();
    public static bool Debug = Properties.Settings.Default.Debug;

    public static void Add(string logContent)
    {
      if (!Debug) return;
      CheckLogFile();

      Console.WriteLine(logContent);

      lock (LOCK)
      {
        try
        {
          File.AppendAllText(logFile, MakeEntry(logContent));
        }
        catch(IOException e)
        {
          Console.WriteLine(e.Message);
          Console.WriteLine("@: " + logContent);
        }
      }
    }

    public static void Add(string[] logContent)
    {
      if (!Debug) return;
      CheckLogFile();

      Console.WriteLine(logContent);

      lock (LOCK)
      {
        foreach (string s in logContent)
          File.AppendAllText(logFile, MakeEntry(s));
      }
    }

    public static void ResetLogFile()
    {
      if (!Debug) return;
      CheckLogFile(true);
    }

    private static string MakeEntry(string entryContent)
    {
      return DateTime.Now + ": " + entryContent + Environment.NewLine;
    }

    private static void CheckLogFile(bool createNew = false)
    {
      if (!File.Exists(logFile))
      {
        lock (LOCK)
        {
          File.Create(logFile).Close();
        }
      }
    }

    private static void Lock()
    {
      Monitor.Enter(LOCK);
    }

    private static void Unlock()
    {
      Monitor.Exit(LOCK);
    }
  }
}
