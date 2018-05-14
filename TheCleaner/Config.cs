
using System;
using System.Configuration;

namespace TheCleaner {

   public static class Config {

      //--------------------------------------------------------------------
      // GET A STRING VALUE
      //--------------------------------------------------------------------
      public static string Get(string sKey, string sDefault) {
         object oValue = ReadValue(sKey);
         return (oValue != null ? oValue.ToString() : sDefault);
      }

      //--------------------------------------------------------------------
      // GET AN ARRAY OF STRING VALUES
      //--------------------------------------------------------------------
      public static string[] Get(string sKey, string sDefault, char cSeparator) {
         string[] asReturn = null;
         object oValue = ReadValue(sKey);
         if (oValue != null) {
            asReturn = oValue.ToString().Split(cSeparator);
         }
         return asReturn;
      }

      //--------------------------------------------------------------------
      // GET A INT VALUE
      //--------------------------------------------------------------------
      public static int Get(string sKey, int iDefault) {
         object oValue = ReadValue(sKey);
         return (oValue != null ? Convert.ToInt32(oValue) : iDefault);
      }

      //--------------------------------------------------------------------
      // GET A FLOAT VALUE
      //--------------------------------------------------------------------
      public static float Get(string sKey, float fDefault) {
         object oValue = ReadValue(sKey);
         return (oValue != null ? Convert.ToSingle(oValue) : fDefault);
      }

      //--------------------------------------------------------------------
      // GET A DOUBLE VALUE
      //--------------------------------------------------------------------
      public static double Get(string sKey, double dDefault) {
         object oValue = ReadValue(sKey);
         return (oValue != null ? Convert.ToDouble(oValue) : dDefault);
      }

      //--------------------------------------------------------------------
      // GET A BOOLEAN VALUE
      //--------------------------------------------------------------------
      public static bool Get(string sKey, bool bDefault) {
         object oValue = ReadValue(sKey);
         return (oValue != null ? Convert.ToBoolean(oValue) : bDefault);
      }

      //--------------------------------------------------------------------
      // GET A VALUE. RETURNS "NULL" IF UNDEFINED OR EMPTY.
      //--------------------------------------------------------------------
      private static object ReadValue(string sKey) {
         if (ConfigurationManager.AppSettings[sKey] != null) {
            return ConfigurationManager.AppSettings[sKey];
         }
         else {
            return null;
         }
      }
   }
}
