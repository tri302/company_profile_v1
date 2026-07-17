namespace CompanyProfile.Helper
{
    public static class StringHelper
    {
        public static string ExtractIframeSrc(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            // Jika terdeteksi tag iframe
            if (input.Contains("<iframe", StringComparison.OrdinalIgnoreCase))
            {
                // Cari src=" atau src='
                string[] anchors = { "src=\"", "src='" };
                foreach (var anchor in anchors)
                {
                    var start = input.IndexOf(anchor, StringComparison.OrdinalIgnoreCase);
                    if (start != -1)
                    {
                        start += anchor.Length;
                        var quoteType = anchor.EndsWith("\"") ? "\"" : "'";
                        var end = input.IndexOf(quoteType, start);
                        
                        if (end > start)
                        {
                            return input.Substring(start, end - start);
                        }
                    }
                }
            }

            return input.Trim(); 
        }
    }
}