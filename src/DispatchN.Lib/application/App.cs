using Sorling.DispatchN.Lib.commands;
using Sorling.DispatchN.Lib.console;

namespace Sorling.DispatchN.Lib.application;

public static class App
{
   private static readonly CancellationTokenSource _cancellationTokenSource = new();

   private static Lifetime? _lifetime;

   private static readonly CommandHandlerRegistration _root = new(null, "root", null);

   public sealed class SubCommandIndexer
   {
      public CommandHandlerInfo this[string path] {
         get {
            ArgumentNullException.ThrowIfNull(path, nameof(path));
            string[] parts = path.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            CommandHandlerInfo currcmd = new("", "", _root, 0);
            string currpath = "";

            for (int i = 0; i < parts.Length; i++) {
               if (currcmd.CommandHandlerRegistration.SubCommands.TryGetValue(parts[i], out CommandHandlerRegistration? cmd)) {
                  currcmd = new(currpath, parts[i], cmd, i + 1);
                  currpath += " " + parts[i];
               }
               else {
                  break;
               }
            }

            return currcmd;
         }
      }
   }

   public static SubCommandIndexer CommandTree => _commandTree;

   public static void AddCommand(string path, string helpText, Func<ExecuteContext, Task<int>>? handler) {
      ArgumentNullException.ThrowIfNull(path, nameof(path));
      CommandHandlerRegistration currcmd = _root;
      string[] parts = path.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

      for (int i = 0; i < parts.Length; i++) {
         currcmd = currcmd.SubCommands.TryGetValue(parts[i], out CommandHandlerRegistration? cmd)
            ? i + 1 == parts.Length
               ? currcmd.SetSubCommand(parts[i], helpText, handler)
               : cmd
            : i + 1 == parts.Length
               ? currcmd.SetSubCommand(parts[i], helpText, handler)
               : currcmd.SetSubCommand(parts[i], $"Command directory {parts[i]}", null);
      }
   }

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
   private static readonly SubCommandIndexer _commandTree = new();

   private static async Task<int> MainLoopAsync(string[] args) {
      do {
         string cmd = await ConsoleLineReaderWTABComp.ReadLineAsync(_tabcallback, _cancellationTokenSource.Token);
         Console.WriteLine(cmd);
      } while (!_cancellationTokenSource.IsCancellationRequested);

      return 28;
   }
}
