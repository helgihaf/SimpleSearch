using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleSearch
{
    class TextComparer<T> : IComparer<T>
    {
        private Func<T, string> getValue;

        public TextComparer(Func<T, string> getValue)
        {
            this.getValue = getValue;
        }

        public int Compare(T x, T y)
        {
            return string.Compare(getValue(x), getValue(y), true);
        }
    }

    class DateTimeComparer<T> : IComparer<T>
    {
        private Func<T, DateTime> getValue;

        public DateTimeComparer(Func<T, DateTime> getValue)
        {
            this.getValue = getValue;
        }

        public int Compare(T x, T y)
        {
            return DateTime.Compare(getValue(x), getValue(y));
        }
    }

    class Int64Comparer<T> : IComparer<T>
    {
        private Func<T, Int64> getValue;

        public Int64Comparer(Func<T, Int64> getValue)
        {
            this.getValue = getValue;
        }

        public int Compare(T x, T y)
        {
            var valueX = getValue(x);
            var valueY = getValue(y);
            if (valueX < valueY)
                return -1;
            else if (valueX > valueY)
                return 1;
            else
                return 0;
        }
    }
}
