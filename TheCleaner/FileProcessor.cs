

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.IO.Compression;

namespace TheCleaner {

   class FileProcessor {
      public int FilesProcessedCount { get; private set; } = 0;

      private DateTime _dtNow;
      private Output _log = null;
      private CommandLine _params = null;

      public FileProcessor(DateTime dtNow, Output log, CommandLine cl) {
         _dtNow = dtNow;
         _log = log;
         _params = cl;
      }

      public ExitCode Execute() {
         ExitCode exitCode = ExitCode.Success;
         FilesProcessedCount = 0;
         if (!_params.HasValidFolderPath) {
            exitCode = ExitCode.InvalidPath;
         }
         else if (!_params.HasValidArguments) {
            exitCode = ExitCode.InvalidArguments;
         }
         else {
/*
            List<string> lstFiles = Lib.GetLocalFiles(_params.FolderPath, "*.log");
            if (lstFiles.Count > 0) {
               exitCode = ProcessFiles(lstFiles);
            }
*/
         }
         return exitCode;
      }

      private ExitCode ProcessFiles(List<string> lstFiles) {
         ExitCode exitCode = ExitCode.Success;
         foreach (string sPathFilename in lstFiles) {
            DateTime? dt = ExtractDateFromFilename(sPathFilename);
            if (dt.HasValue) {
               if ((_dtNow - dt.Value).Days >= _params.DaysToLeaveFiles) {
                  if (ArchiveFile(sPathFilename)) {
                     FilesProcessedCount++;
                     if (_params.Move) {
                        if (!MoveFile(sPathFilename)) {
                           exitCode = ExitCode.KnownError;
                           break;
                        }
                     }
                     else if (_params.Delete) {
                        if (!DeleteFile(sPathFilename)) {
                           exitCode = ExitCode.KnownError;
                           break;
                        }
                     }
                  }
                  else {
                     exitCode = ExitCode.KnownError;
                     break;
                  }
               }
            }
            else {
               _log.WriteLine("Unable to extract date from \"{0}\".", sPathFilename);
            }
         }
         return exitCode;
      }

      [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
      private bool ArchiveFile(string sPathFilename) {
         bool bSuccess = false;

         string sFilename = Path.GetFileName(sPathFilename);
         string sZipFile = Path.ChangeExtension(sPathFilename, ".zip");

         if (File.Exists(sZipFile)) {
            bSuccess = IsArchiveValid(sPathFilename, sZipFile);
            if (bSuccess) {
               _log.WriteLine("Archive \"{0}\" already exists.", sZipFile);
            }
            else {
               _log.WriteLine("Archive \"{0}\" does not match log file \"{1}\".", sZipFile, sPathFilename);
            }
         }
         else {
            using (FileStream fsZipFile = new FileStream(sZipFile, FileMode.Create)) {
               using (ZipArchive archive = new ZipArchive(fsZipFile, ZipArchiveMode.Create)) {
                  bSuccess = true;
                  try {
                     archive.CreateEntryFromFile(sPathFilename, sFilename);
                  }
                  catch (Exception e) {
                     bSuccess = false;
                     _log.WriteLine("Failed to zip file \"{0}\".", sPathFilename);
                     _log.WriteLine(e.Message);
                  }
               }
            }
         }
         return bSuccess;
      }

      private bool MoveFile(string sPathFilename) {
         bool bSuccess = true;
         if (!Directory.Exists(_params.MoveFolderFull())) {
            try {
               Directory.CreateDirectory(_params.MoveFolderFull());
            }
            catch (Exception e) {
               _log.WriteLine("Unable to create move folder \"{0}\".", _params.MoveFolderFull());
               _log.WriteLine(e.Message);
               bSuccess = false;
            }
         }
         if (bSuccess) {
            try {
               File.Move(sPathFilename, _params.MoveFolderFull() + "\\" + Path.GetFileName(sPathFilename));
            }
            catch (Exception e) {
               _log.WriteLine("Unable to move file \"{0}\".", sPathFilename);
               _log.WriteLine(e.Message);
               bSuccess = false;
            }
         }
         return bSuccess;
      }

      private bool DeleteFile(string sPathFilename) {
         bool bSuccess = true;
         try {
            File.Delete(sPathFilename);
         }
         catch (Exception e) {
            _log.WriteLine("Unable to delete file \"{0}\".", sPathFilename);
            _log.WriteLine(e.Message);
            bSuccess = false;
         }
         return bSuccess;
      }

      private bool IsArchiveValid(string sPathFilename, string sZipFile) {
         bool bIsValid = false;
         try {
            string sFilename = Path.GetFileName(sPathFilename);
            FileInfo fileInfo = new FileInfo(sPathFilename);
            using (ZipArchive archive = ZipFile.OpenRead(sZipFile)) {
               if (archive.GetEntry(sFilename) != null) {
                  if (archive.GetEntry(sFilename).Length == fileInfo.Length) {
                     bIsValid = true;
                  }
               }
            }
         }
         catch (Exception e) {
            _log.WriteLine("Error validating archive \"{0}\".", sZipFile);
            _log.WriteLine(e.Message);
         }
         return bIsValid;
      }

      private static DateTime? ExtractDateFromFilename(string sFilename) {
         DateTime? dt = null;
         string sDate = ExtractDateFromString(Path.GetFileName(sFilename));
         if (!string.IsNullOrWhiteSpace(sDate)) {
            if (DateTime.TryParseExact(sDate, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dtOutput)) {
               dt = dtOutput;
            }
         }
         return dt;
      }

      // ASSUMES STRUCTURE "u_ex[yyMMdd].log" E.G. "u_ex171201.log"
      private static string ExtractDateFromString(string sStringContainingDate) {
         string sDate = null;
         if (sStringContainingDate.StartsWith("u_ex") && sStringContainingDate.EndsWith(".log")) {
            sDate = sStringContainingDate.Substring(4, 6);
         }
         return sDate;
      }
   }
}
