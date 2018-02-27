using System.Collections.Generic;

namespace App
{
    public class CalendarDay
    {
        public CalendarDay()
        {
            
        }

        public CalendarDay(string id, List<CalendarDayEntry> slices)
        {
            Id = id;
            Slices = slices;
        }

        public string Id { get; set; }

        public List<CalendarDayEntry> Slices { get; set; }
    }
}
