using Sorling.DispatchN.Lib.console;

Func<string, CancellationToken, Task<TABSuggestions?>> tabcallback = async (string s, CancellationToken ct) => {
   string lastpart = s.Split(Path.DirectorySeparatorChar).Last();
   string[] items = lastpart.Contains(Path.VolumeSeparatorChar)
      ? Directory.EnumerateFileSystemEntries(s).Select(e => Path.GetFileName(e)).ToArray()
      : Directory.GetLogicalDrives();

   return new TABSuggestions(s.Length, items, [Path.DirectorySeparatorChar], true);
};

string s = await ConsoleLineReaderWTABComp.ReadLineAsync(tabcallback);
Console.WriteLine(s);
