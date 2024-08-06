using System.Reflection;
using System.Text;

namespace Sorling.DispatchN.Lib.console;

public static class ConsoleLineReaderWTABComp
{
   private static string GetBackspaceXTimes(int times) {
      StringBuilder sb = new("");
      for (int i = 0; i < times; i++) {
         _ = sb.Append("\b \b");
      }

      return sb.ToString();
   }

   public static async Task<string> ReadLineAsync(Func<string, CancellationToken, Task<TABSuggestions?>> suggestionsCallback, CancellationToken cancellationToken) {
      ArgumentNullException.ThrowIfNull(nameof(suggestionsCallback));
      ArgumentNullException.ThrowIfNull(nameof(cancellationToken));

      StringBuilder sb = new();
      bool isintabcomp = false;
      char[] confirmchars = [];

      bool cancelkey = false;
      void oncancelkeypress(object? sender, ConsoleCancelEventArgs e) {
         e.Cancel = true;
         cancelkey = true;
      }

      Console.CancelKeyPress += oncancelkeypress;

      TABSuggestions? tabsuggestions = null;
      int suggestionindex = 0;
      int suggestionlength = 0;

      while (!cancellationToken.IsCancellationRequested) {
         while (!Console.KeyAvailable) {
            await Task.Delay(100, cancellationToken);
            if (cancellationToken.IsCancellationRequested || cancelkey) {
               Console.CancelKeyPress -= oncancelkeypress;
               return "";
            }
         }

         ConsoleKeyInfo key = Console.ReadKey(true);

         if (key.Key == ConsoleKey.Enter) {
            if (isintabcomp) {
               isintabcomp = false;
            }
            else {
               Console.WriteLine();
               Console.CancelKeyPress -= oncancelkeypress;
               return sb.ToString();
            }
         }
         else if (key.Key == ConsoleKey.Backspace) {
            if (sb.Length > 0) {
               if (isintabcomp && tabsuggestions != null) {
                  isintabcomp = false;
                  int diff = sb.Length - tabsuggestions.SuggestionStartIndex;
                  sb.Length -= diff;
                  Console.Write(GetBackspaceXTimes(diff));
               }
               else {
                  --sb.Length;
                  Console.CursorVisible = false;
                  Console.Write("\b \b");
                  Console.CursorVisible = true;
               }
            }
         }
         else if (key.Key == ConsoleKey.Tab) {
            if (isintabcomp) {
               sb.Length -= suggestionlength;
               Console.CursorVisible = false;
               Console.Write(GetBackspaceXTimes(suggestionlength));
               if (suggestionindex > -1 && tabsuggestions is not null && suggestionindex + 1 < tabsuggestions.Suggestions.Length) {
                  suggestionlength = tabsuggestions.Suggestions[++suggestionindex].Length;
                  Console.Write(tabsuggestions.Suggestions[suggestionindex]);
                  _ = sb.Append(tabsuggestions.Suggestions[suggestionindex]);
               }
               else {
                  isintabcomp = false;
               }

               Console.CursorVisible = true;
            }
            else {
               tabsuggestions = await suggestionsCallback(sb.ToString(), cancellationToken);
               if (tabsuggestions != null) {
                  isintabcomp = true;
                  suggestionindex = tabsuggestions.Suggestions.Length > 1 ? 0 : -1;
                  Console.Write(tabsuggestions.Suggestions[0]);
                  suggestionlength = tabsuggestions.Suggestions[0].Length;
                  _ = sb.Append(tabsuggestions.Suggestions[0]);
               }
            }
         }
         else if (isintabcomp && confirmchars.Contains(key.KeyChar)) {
            isintabcomp = false;
            Console.Write(key.KeyChar);
            _ = sb.Append(key.KeyChar);
            tabsuggestions = await suggestionsCallback(sb.ToString(), cancellationToken);
            if (tabsuggestions != null) {
               isintabcomp = true;
               suggestionindex = tabsuggestions.Suggestions.Length > 1 ? 0 : -1;
               Console.Write(tabsuggestions.Suggestions[0]);
               suggestionlength = tabsuggestions.Suggestions[0].Length;
               _ = sb.Append(tabsuggestions.Suggestions[0]);
            }
         }
         else {
            _ = sb.Append(key.KeyChar);
            Console.Write(key.KeyChar);
         }
      }

      Console.CancelKeyPress -= oncancelkeypress;
      return "";
   }

   private static void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e) => throw new NotImplementedException();
}
