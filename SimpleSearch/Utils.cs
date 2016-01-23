using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSearch
{
	internal static class Utils
	{
		public static string LinesToSeparatedString(char seperator, string[] lines)
		{
			StringBuilder sb = new StringBuilder();
			foreach (string line in lines)
			{
				if (sb.Length > 0)
					sb.Append(seperator);
				sb.Append(line);
			}
			return sb.ToString();
		}


	}
}
