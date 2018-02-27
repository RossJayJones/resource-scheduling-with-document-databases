using System;

namespace App
{
    public class CalendarEntry
    {
        public CalendarEntry(string id,
            string calendarId,
            DateTime begin,
            DateTime end)
        {
            Id = id;
            CalendarId = calendarId;
            Begin = begin;
            End = end;
        }

        public string Id { get; set; }
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public string CalendarId { get; set; }
    }
}
