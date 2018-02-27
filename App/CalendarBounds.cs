using System;

namespace App
{
    public class CalendarBounds
    {
        public CalendarBounds(DateTime begin, DateTime end)
        {
            Begin = begin.Date;
            End = end.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        }

        public DateTime Begin { get; }

        public DateTime End { get; }

        public bool IsWithinBounds(DateTime begin, DateTime end)
        {
            return Begin < end && begin < End;
        }
    }
}
