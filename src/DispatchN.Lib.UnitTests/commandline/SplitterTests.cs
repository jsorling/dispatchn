using Sorling.DispatchN.Lib.commandline;

namespace DispatchN.Lib.UnitTests.commandline;

[TestClass]
public class SplitterTests
{
   [TestMethod]
   public void Split1() {
      string[] t = "".SplitCmdLine();
      Assert.IsTrue(t.Length == 0);
   }

   [TestMethod]
   public void Split2() {
      string[] t = "a b c".SplitCmdLine();
      Assert.IsTrue(t.Length == 3 && t[0] == "a" && t[1] == "b" && t[2] == "c");
   }

   [TestMethod]
   public void Split3() {
      string[] t = " a b c".SplitCmdLine();
      Assert.IsTrue(t.Length == 3 && t[0] == "a" && t[1] == "b" && t[2] == "c");
   }

   [TestMethod]
   public void Split4() {
      string[] t = " a b  c  ".SplitCmdLine();
      Assert.IsTrue(t.Length == 3 && t[0] == "a" && t[1] == "b" && t[2] == "c");
   }

   [TestMethod]
   public void Split5() {
      string[] t = " \"a 1\" b  c  ".SplitCmdLine();
      Assert.IsTrue(t.Length == 3 && t[0] == "a 1" && t[1] == "b" && t[2] == "c");
   }

   [TestMethod]
   public void Split6() {
      string[] t = " \"a\"\" 1\" b  c  ".SplitCmdLine();
      Assert.IsTrue(t.Length == 3 && t[0] == "a\" 1" && t[1] == "b" && t[2] == "c");
   }

   [TestMethod]
   public void Split7() {
      string[] t = " 'a'' 1' b  c  ".SplitCmdLine();
      Assert.IsTrue(t.Length == 3 && t[0] == "a' 1" && t[1] == "b" && t[2] == "c");
   }

   [TestMethod]
   public void Split8() {
      string[] t = " 'a'' \"1' b  c  ".SplitCmdLine();
      Assert.IsTrue(t.Length == 3 && t[0] == "a' \"1" && t[1] == "b" && t[2] == "c");
   }

   [TestMethod]
   public void Split9() {
      string[] t = " '''Bosse''' b  c  ".SplitCmdLine();
      Assert.IsTrue(t.Length == 3 && t[0] == "'Bosse'" && t[1] == "b" && t[2] == "c");
   }

   [TestMethod]
   public void Split10() {
      string[] t = " ''' Bosse ''' b  c  ".SplitCmdLine();
      Assert.IsTrue(t.Length == 3 && t[0] == "' Bosse '" && t[1] == "b" && t[2] == "c");
   }
}
