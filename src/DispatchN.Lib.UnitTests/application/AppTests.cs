using Sorling.DispatchN.Lib.application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatchN.Lib.UnitTests.application;

[TestClass]
public class AppTests
{
   [TestMethod]
   public void FirstLevel() => App.AddCommand("first", "Firstcommand", (exec) => Task.FromResult(3));
}
