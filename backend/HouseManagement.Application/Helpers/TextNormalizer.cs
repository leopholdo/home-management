using System.Globalization;
using System.Text;

public static class TextNormalizer
{
  public static string Normalize(string input)
  {
    if (string.IsNullOrWhiteSpace(input))
      return string.Empty;

    var nfd = input.Trim().Normalize(NormalizationForm.FormD);
    var sb = new StringBuilder(nfd.Length);

    foreach (var c in nfd)
      if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
        sb.Append(c);

    return sb.ToString().ToLowerInvariant();
  }
}