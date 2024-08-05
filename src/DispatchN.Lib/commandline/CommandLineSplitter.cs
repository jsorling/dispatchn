using System.Text;

namespace Sorling.DispatchN.Lib.commandline;

public static class CommandLineSplitter
{
   public static string[] SplitCmdLine(this string cmd) {
      StringBuilder sb = new();
      List<string> output = [];

      bool isinsinglequote = false;
      bool isindoublequote = false;

      for (int i = 0; i < cmd.Length; i++) {
         if (isinsinglequote && cmd[i] == '\'') {
            if (i + 1 < cmd.Length && cmd[i + 1] == '\'') {
               _ = sb.Append('\'');
               i++;
            }
            else {
               isinsinglequote = false;
            }
         }
         else if (isindoublequote && cmd[i] == '"') {
            if (i + 1 < cmd.Length && cmd[i + 1] == '"') {
               _ = sb.Append('"');
               i++;
            }
            else {
               isindoublequote = false;
            }
         }
         else if (isindoublequote || isinsinglequote) {
            _ = sb.Append(cmd[i]);
         }
         else if (cmd[i] == '\'') {
            isinsinglequote = true;
         }
         else if (cmd[i] == '"') {
            isindoublequote = true;
         }
         else if (cmd[i] == ' ') {
            if (sb.Length > 0) {
               output.Add(sb.ToString());
               sb = new();
            }
         }
         else {
            _ = sb.Append(cmd[i]);
         }
      }

      if (sb.Length > 0) {
         output.Add(sb.ToString());
      }

      return [.. output];
   }
}
