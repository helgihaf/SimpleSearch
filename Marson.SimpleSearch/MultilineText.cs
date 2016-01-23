using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Marson.SimpleSearch
{
    public class MultilineText
    {
        private readonly string[] lines;
        private readonly string separator;

        public MultilineText(string text)
        {
            separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            if (text != null)
            {
                lines = text.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public MultilineText(string text, string separator)
        {
            this.separator = separator;
            if (text != null)
            {
                lines = text.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public MultilineText(string[] lines)
        {
            this.lines = (string[])lines.Clone();
            this.separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
        }

        public MultilineText(string[] lines, string separator)
        {
            this.lines = (string[])lines.Clone();
            this.separator = separator;
        }

        public string Text
        {
            get
            {
                return string.Join(separator, lines);
            }
        }

        public string Separator
        {
            get
            {
                return separator;
            }
        }

        public string[] Lines
        {
            get
            {
                if (lines != null)
                {
                    return (string[])lines.Clone();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
