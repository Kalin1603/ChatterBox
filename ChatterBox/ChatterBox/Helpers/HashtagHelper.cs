using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ChatterBox.Helpers
{
    public static class HashtagHelper
    {
        public static List<string> GetHashtags(string text)
        {
            var hashtags = new List<string>();
            if (!string.IsNullOrEmpty(text))
            {
                var matches = Regex.Matches(text, @"#\w+");
                foreach (Match match in matches)
                {
                    hashtags.Add(match.Value);
                }
            }
            return hashtags;
        }
    }
}