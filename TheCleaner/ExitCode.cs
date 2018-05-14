

namespace TheCleaner {

   public enum ExitCode : int {
      Success = 0
      , InvalidPath = 1
      , InvalidArguments = 2
      , KnownError = 42
      , UnknownError = 666
   }
}
