using System;

namespace TheCleaner {
   class Program {
      private const string CONFIG_LOGOUTPUT = "LogOutput";

      static int Main(string[] args) {
         ExitCode exitCode = ExitCode.UnknownError;
         DateTime dtNow = DateTime.Now.Date;
         string sOutputTarget = Config.Get(CONFIG_LOGOUTPUT, Output.TARGET_CONSOLE);
         Output log = new Output(sOutputTarget);
         CommandLine cl = new CommandLine(log, args);

         log.WriteLine("==== TheCleaner Started ====");
         if (cl.HasValidFolderPath && cl.HasValidArguments) {
            log.WriteLine("Processing: \"{0}\"...", cl.FolderPath);
            FileProcessor processFolder = new FileProcessor(dtNow, log, cl);
            exitCode = processFolder.Execute();
            if (processFolder.FilesProcessedCount >= 0) {
               if (processFolder.FilesProcessedCount == 1) {
                  log.WriteLine("1 file zipped.");
               }
               else {
                  log.WriteLine("{0} files zipped.", processFolder.FilesProcessedCount);
               }
            }
         }

         if (exitCode == ExitCode.Success) {
            log.WriteLine("==== TheCleaner finished successfully ====");
         }
         else {
            log.WriteLine("==== TheCleaner finished with errors ====");
         }

         // THIS ONLY MAKES SENSE WHEN OUTPUT IS TO THE CONSOLE WINDOW
         if (sOutputTarget == Output.TARGET_CONSOLE) {
            if (cl.Pause) {
               Console.ReadKey();
            }
         }

         return (int) exitCode;
      }
   }
}
