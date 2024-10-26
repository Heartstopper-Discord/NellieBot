using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NellieBot.Extensions
{
  static class StringEx
  {
    public static string TrimForEmbed(this string value, bool name = false)
    {
      if (string.IsNullOrEmpty(value)) value = "-";
      if (value.Length > 256 && name) return value.Substring(0, 253) + "...";
      if (value.Length > 1024) return value.Substring(0, 1021) + "...";
      return value;
    }

    public static string WrapForEmbed(this string value)
    {
      if (string.IsNullOrEmpty(value)) value = "-";
      if (value.Length > 1024) return $"```\n`{value.Substring(0, 1015)}...\n```";
      return $"```\n{value}\n```";
    }

    public static string DefaultIfNullOrEmpty(string? value, string defaultValue)
    {
      if (value == null || value.Length == 0) return defaultValue;
      return value;
    }
  }
}
