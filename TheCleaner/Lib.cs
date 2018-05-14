
using System;
using System.Collections.Generic;
using System.IO;

namespace TheCleaner {

   public static class Lib {

      //--------------------------------------------------------------------
      // IS THE FOLDER PATH VALID
      //--------------------------------------------------------------------
      public static bool IsPathValid(string sFolderPath) {
         if (string.IsNullOrWhiteSpace(sFolderPath) || !Path.IsPathRooted(sFolderPath)) {
            return false;
         }
         bool bIsValidPath = true;
         try {
            Path.GetFullPath(sFolderPath);
         }
         catch (Exception) {
            bIsValidPath = false;
         }
         return bIsValidPath;
      }

      //--------------------------------------------------------------------
      // GET THE LOCAL FILES
      //--------------------------------------------------------------------
      public static List<string> GetLocalFiles(string sFolder, string sPattern) {
         return new List<string>(Directory.EnumerateFiles(sFolder, sPattern));
      }

   }
}
