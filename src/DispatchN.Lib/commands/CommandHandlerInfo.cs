namespace Sorling.DispatchN.Lib.commands;

public record CommandHandlerInfo(string Path, string Name, CommandHandlerRegistration CommandHandlerRegistration, int Depth)
{
   public string FullPath => (Path ?? "" + " " + Name).Trim();

   public bool IsFolder => CommandHandlerRegistration.Handler is null;
}
