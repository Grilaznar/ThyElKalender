using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using Thy_El_Teknik_Kalender_9000;
using System.IO;
using System.Security.Cryptography;
using Thy_El_Teknik_Kalender_9000.Properties;

namespace Thy_El_Teknik_Kalender_9000
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      //string protoResource = "Thy_El_Teknik_Kalender_9000.protobuf-net.dll";
      //EmbeddedAssembly.Load(protoResource, "protobuf-net.dll");

      AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);

      Settings.Default.SettingsKey = 
        Path.Combine(Environment.GetFolderPath(
          Environment.SpecialFolder.LocalApp‌​licationData),
          "Thy_El_Kalender_9000\\user.config");

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new CalendarEditor());
    }
    static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
    {
      return Load();
    }
    static Assembly Load()
    {
      // Get the byte[] of the DLL
      byte[] ba = null;
      string resource = "Thy_El_Teknik_Kalender_9000.protobuf-net.dll";
      Assembly curAsm = Assembly.GetExecutingAssembly();
      using (Stream stm = curAsm.GetManifestResourceStream(resource))
      {
        ba = new byte[(int)stm.Length];
        stm.Read(ba, 0, (int)stm.Length);
      }

      bool fileOk = false;
      string tempFile = "";

      using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
      {
        // Get the hash value of the Embedded DLL
        string fileHash = BitConverter.ToString(sha1.ComputeHash(ba)).Replace("-", string.Empty);

        // The full path of the DLL that will be saved
        tempFile = Path.GetTempPath() + "System.Data.SQLite.dll";

        // Check if the DLL is already existed or not?
        if (File.Exists(tempFile))
        {
          // Get the file hash value of the existed DLL
          byte[] bb = File.ReadAllBytes(tempFile);
          string fileHash2 = BitConverter.ToString(sha1.ComputeHash(bb)).Replace("-", string.Empty);

          // Compare the existed DLL with the Embedded DLL
          if (fileHash == fileHash2)
          {
            // Same file
            fileOk = true;
          }
          else
          {
            // Not same
            fileOk = false;
          }
        }
        else
        {
          // The DLL is not existed yet
          fileOk = false;
        }
      }

      // Create the file on disk
      if (!fileOk)
      {
        System.IO.File.WriteAllBytes(tempFile, ba);
      }

      // Load it into memory    
      return Assembly.LoadFile(tempFile);
    }
  }
}
