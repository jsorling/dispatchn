namespace Sorling.DispatchN.Lib.commands;

public class CommandHandlerRegistration(Func<ExecuteContext, Task<int>>? handler
   , string helpText, CommandHandlerRegistration? parent)
{
   public Func<ExecuteContext, Task<int>>? Handler { get; set; } = handler;

   public string HelpText { get; set; } = helpText;

   public Commands SubCommands { get; } = new(parent);

   public CommandHandlerRegistration SetSubCommand(string commandName, string helpText, Func<ExecuteContext, Task<int>>? handler) {
      if (SubCommands.TryGetValue(commandName, out CommandHandlerRegistration? command)) {
         command.HelpText = helpText;
         command.Handler = handler;
         return command;
      }
      else {
         CommandHandlerRegistration chr = new(handler, helpText, this);
         SubCommands.Add(commandName, chr);
         return chr;
      }
   }
}
