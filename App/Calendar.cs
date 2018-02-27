using System;
using System.Collections.Generic;
using System.Linq;

namespace App
{
    public class Calendar
    {
        public Calendar(string id, CalendarBounds bounds, IEnumerable<CalendarEntry> entries) : this(id, bounds)
        {
            Entries = entries.ToList();
        }

        public Calendar(string id, CalendarBounds bounds)
        {
            Id = id;
            Bounds = bounds;
            Entries = new List<CalendarEntry>();
        }

        public string Id { get; set; }

        public CalendarBounds Bounds { get; }

        public List<CalendarEntry> Entries { get; set; }

        public bool IsAvailable(DateTime begin, DateTime end)
        {
            var entries = Entries.Where(x => x.Begin < end && begin < x.End);
            return !entries.Any();
        }

        public void Reserve(string id, DateTime begin, DateTime end)
        {
            if (!Bounds.IsWithinBounds(begin, end))
            {
                throw new CalendarException($"Entry {begin} - {end} is outside the calendar bounds");
            }

            if (!IsAvailable(begin, end))
            {
                throw new CalendarException($"Entry {begin} - {end} is not available");
            }

            var entry = new CalendarEntry(id, Id, begin, end);
            Entries.Add(entry);
        }

        public void Cancel(string id)
        {
            foreach (var entry in Entries.Where(x => x.Id == id).ToList())
            {
                Entries.Remove(entry);
            }
        }
    }
}
