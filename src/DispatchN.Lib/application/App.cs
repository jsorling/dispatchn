using Sorling.DispatchN.Lib.console;

namespace Sorling.DispatchN.Lib.application;

public static class App
{
   private static readonly CancellationTokenSource _cancellationTokenSource = new();

   private static Lifetime? _lifetime;

   public static void Run(string[] args) {
      Task<int> mainloop = Task.Run(() => MainLoopAsync(args));
      _lifetime = new(_cancellationTokenSource, mainloop, new TimeSpan(0, 0, 30));
      try {
         mainloop.Wait();
      }
      catch (AggregateException ae) {
         if (ae.InnerException is not TaskCanceledException) {
            throw;
         }
      }
   }

   private static readonly Func<string, CancellationToken, Task<TABSuggestions?>> _tabcallback = async (string s, CancellationToken ct) => {
      string lastpart = s.Split(Path.DirectorySeparatorChar).Last();
      string[] items = lastpart.Contains(Path.VolumeSeparatorChar)
         ? Directory.EnumerateFileSystemEntries(s).Select(e => Path.GetFileName(e)).ToArray()
         : Directory.GetLogicalDrives();

      await Task.Delay(2);

      return new TABSuggestions(s.Length, items, [Path.DirectorySeparatorChar], true);
   };

   private static async Task<int> MainLoopAsync(string[] args) {
      do {
         string cmd = await ConsoleLineReaderWTABComp.ReadLineAsync(_tabcallback, _cancellationTokenSource.Token);
         Console.WriteLine(cmd);
      } while (!_cancellationTokenSource.IsCancellationRequested);

      return 28;
   }
}
