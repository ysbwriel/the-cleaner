

using System;
using System.IO;

namespace TheCleaner {

   public sealed class CommandLine {
      private const string PARAM_MOVE = "/M";
      private const string PARAM_DELETE = "/D";
      private const string PARAM_PAUSE = "/P";
      private const string PARAM_NDAYS = "/NDAYS";
      private const int DEFAULT_NDAYS = 1;
      private const string DEFAULT_MOVEFOLDER = "moved";

      public string FolderPath { get; private set; } = null;
      public bool Move { get; private set; } = false;
      public bool Delete { get; private set; } = false;
      public bool Pause { get; private set; } = false;
      public int DaysToLeaveFiles { get; private set; } = DEFAULT_NDAYS;
      public string MoveFolder { get; private set; } = DEFAULT_MOVEFOLDER;

      public bool HasValidFolderPath { get; private set; } = false;
      public bool HasValidArguments { get; private set; } = false;

      private string _sMoveFolder = null;
      public string MoveFolderFull() {
         if (_sMoveFolder == null) {
            if (Path.IsPathRooted(MoveFolder)) {
               _sMoveFolder = MoveFolder;
            }
            else {
               if (HasValidFolderPath) {
                  _sMoveFolder = FolderPath + "\\" + MoveFolder;
               }
               else {
                  _sMoveFolder = Environment.CurrentDirectory + "\\" + MoveFolder;
               }
            }
         }
         return _sMoveFolder;
      }

      public CommandLine(Output log, string[] args) {
         if (args.Length == 0) {
            log.WriteLine("Please add command line arguments.");
            return;
         }

         if (args[0] == "/?") {
            DisplayHelp();
         }
         else {
            ParseArguments(log, args);
         }
      }

      private void ParseArguments(Output log, string[] args) {
         // FIRST ARGUMENT IS ALWAYS THE FOLDER PATH
         FolderPath = args[0];

         if (!Lib.IsPathValid(FolderPath)) {
            log.WriteLine("The path \"{0}\" is invalid.", FolderPath);
         }
         else if (!Directory.Exists(FolderPath)) {
            log.WriteLine("The path \"{0}\" does not exist.", FolderPath);
         }
         else {
            FolderPath = FolderPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            HasValidFolderPath = true;
         }

         // PARSE ARGUMENTS
         HasValidArguments = true;      // NO ARGUMENTS IS A VALID ARGUMENT
         for (int i = 1; i < args.Length; i++) {
            switch (args[i].ToUpper().Split(':')[0]) {
               case PARAM_MOVE:
                  Move = true;
                  break;
               case PARAM_DELETE:
                  Delete = true;
                  break;
               case PARAM_PAUSE:
                  Pause = true;
                  break;
               case PARAM_NDAYS:
                  DaysToLeaveFiles = ExtractDaysToLeaveFiles(args[i]);
                  if (DaysToLeaveFiles < 1) {
                     log.WriteLine("Argument \"{0}\" is invalid.", PARAM_NDAYS);
                     HasValidArguments = true;
                  }
                  break;
               default:
                  HasValidArguments = false;
                  log.WriteLine("Unknown argument: \"{0}\".", args[i]);
                  break;
            }
         }
      }

      private int ExtractDaysToLeaveFiles(string sArg) {
         int iDaysToLeaveFiles = -1;
         string[] asArg = sArg.Split(':');
         if (asArg.Length == 2) {
            if (!int.TryParse(asArg[1], out iDaysToLeaveFiles)) {
               iDaysToLeaveFiles = -1;
            }
         }
         return iDaysToLeaveFiles;
      }

      private void DisplayHelp() {
         Console.WriteLine("Zips standard IIS log files.");
         Console.WriteLine();
         Console.WriteLine("WCLogZipper [drive:][path] [{0}] [{1}] [{2}] [{3}:[days]]"
               , PARAM_MOVE
               , PARAM_DELETE
               , PARAM_PAUSE
               , PARAM_NDAYS
         );
         Console.WriteLine();
         Console.WriteLine(" {0}        Move file", PARAM_MOVE);
         Console.WriteLine(" {0}        Delete file", PARAM_DELETE);
         Console.WriteLine("            {0} and {1} are mutually exclusive (obv.) so {0} will override {1}", PARAM_MOVE, PARAM_DELETE);
         Console.WriteLine(" {0}        Pause before exiting", PARAM_PAUSE);
         Console.WriteLine("            Only applies when output is to the console window");
         Console.WriteLine(" {0}        Number of days to leave files before zipping; default: {1}.", PARAM_NDAYS, DEFAULT_NDAYS);
         Console.WriteLine();
      }
   }
}
