using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NellieBot.Extensions
{
    static class StringEx
    {
        public static string TrimForEmbed(this string value)
        {
            if (value == null) return "";
            if (value.Length > 1024) return value.Substring(0, 1021) + "...";
            return value;
        }

        public static string DefaultIfNullOrEmpty(string? value, string defaultValue)
        {
            if (value == null || value.Length == 0) return defaultValue;
            return value;
        }
    }
}
