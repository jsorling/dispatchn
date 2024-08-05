namespace Sorling.DispatchN.Lib.console;

public class TABSuggestions(int suggestionStartIndex, string[] suggestions, char[] confirmchars, bool quoteSpace)
{
   public int SuggestionStartIndex { get; } = suggestionStartIndex;

   public string[] Suggestions { get; } = suggestions ?? throw new ArgumentNullException(nameof(suggestions));

   public char[] ConfirmChars { get; } = confirmchars;

    public bool QuoteSpace {  get; } = quoteSpace;
}
