using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sorling.DispatchN.Lib.commands;

public class Commands(CommandHandlerRegistration? parent) : Dictionary<string, CommandHandlerRegistration>
{
   public CommandHandlerRegistration? Parent { get; } = parent;
}
