using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Web_Extractor.Utilities
{
    public static class HtmlTools
    {
        public static string StripTags(string htmlInput)
        {
            string pattern = @"<(.|\n)*?>";
            return Regex.Replace(htmlInput, pattern, string.Empty);
        }
    }
}