
using System;
using System.Diagnostics;
using System.IO;

namespace TheCleaner {

   public sealed class Output {
      public const string TARGET_CONSOLE = "console";
      public const string TARGET_DEBUG = "debug";
      public const string TARGET_FILE = "file";

      private const string DEFAULT_FILENAME = "logfile.txt";

      private string _sOutputTarget = null;
      private string _sLogFilename = null;

      public Output(string sOutputTarget) {
         _sOutputTarget = sOutputTarget;
         _sLogFilename = Environment.CurrentDirectory + "\\" + DEFAULT_FILENAME;
      }

      public Output(string sOutputTarget, string sLogFilename) {
         _sOutputTarget = sOutputTarget;
         if (string.IsNullOrWhiteSpace(sLogFilename)) {
            sLogFilename = DEFAULT_FILENAME;
         }
         _sLogFilename = Environment.CurrentDirectory + "\\" + sLogFilename;
      }

      public void WriteLine(string sMessage) {
         switch (_sOutputTarget) {
            case TARGET_CONSOLE:
               Console.WriteLine("[" + DateTime.Now.ToString("dd-MM-YYYY HH:mm:ss") + "] " + sMessage);
               break;
            case TARGET_DEBUG:
               Debug.WriteLine("[" + DateTime.Now.ToString("dd-MM-YYYY HH:mm:ss") + "] " + sMessage);
               break;
            case TARGET_FILE:
               WriteLineToFile("[" + DateTime.Now.ToString("dd-MM-YYYY HH:mm:ss") + "] " + sMessage);
               break;
         }
      }

      public void WriteLine(string sMessage, params object[] args) {
         WriteLine(string.Format(sMessage, args));
      }

      private Object objLock = new Object();
      private void WriteLineToFile(string sMessage) {
         RecycleLogFile();
         try {
            using (StreamWriter sw = new StreamWriter(_sLogFilename, true)) {
               lock (objLock) {
                  sw.WriteLine(sMessage);
               }
            }
         }
         catch (Exception) {
         }
      }

      //--------------------------------------------------------------------
      // CHECK THE LOG FILE AND IF OVER MAXIMUM SIZE, RECYCLE
      //--------------------------------------------------------------------
      private void RecycleLogFile() {
         const long lMaxSize = 5000;
         if (File.Exists(_sLogFilename)) {
            // MAKE SURE IT ISN'T TOO BIG
            FileInfo fileInfo = new FileInfo(_sLogFilename);
            if (fileInfo.Length > lMaxSize) {
               try {
                  fileInfo.Delete();
               }
               catch (Exception) {

               }
            }
         }
      }
   }
}
