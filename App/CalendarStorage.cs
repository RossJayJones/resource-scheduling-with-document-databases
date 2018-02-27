using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace App
{
    public class CalendarStorage
    {
        private readonly LiteCollection<CalendarDay> _collection;

        public CalendarStorage(LiteCollection<CalendarDay> collection)
        {
            _collection = collection;
        }

        public void Store(Calendar calendar)
        {
            foreach (var calendarDay in CalendarDayHelper.Slice(calendar))
            {
                _collection.Upsert(calendarDay.Id, calendarDay);
            }
        }

        public Calendar Get(string id, DateTime begin, DateTime end)
        {
            var entries = new List<CalendarEntry>();
            var slices = CalendarDayHelper.ResolveSliceIds(id, begin, end).Where(x => x != null)
                .Select(x => _collection.FindById(x)).Where(x => x != null).SelectMany(x => x.Slices).ToList();

            foreach (var slice in slices.GroupBy(x => x.CalendarEntryId))
            {
                var entry = new CalendarEntry(slice.Key, id, slice.First().Begin, slice.Last().End);
                entries.Add(entry);
            }

            return new Calendar(id, new CalendarBounds(begin, end), entries);
        }

        private class CalendarDayHelper
        {
            private readonly CalendarEntry _entry;

            private CalendarDayHelper(CalendarEntry entry)
            {
                _entry = entry;
            }

            public static IEnumerable<CalendarDay> Slice(Calendar calendar)
            {
                return calendar.Entries.Select(x => new CalendarDayHelper(x)).SelectMany(x => x.Slice()).GroupBy(x => x.Id).Select(x => new CalendarDay(x.Key, x.ToList()));
            }

            public static IEnumerable<string> ResolveSliceIds(string id, DateTime begin, DateTime end)
            {
                var ids = new List<string>();
                while (begin <= end)
                {
                    var sliceId = $"{id}-{begin.Year}-{begin.Month}-{begin.Day}";
                    begin = begin.AddDays(1);
                    ids.Add(sliceId);
                }
                return ids;
            }

            private IEnumerable<CalendarDayEntry> Slice()
            {
                var slices = new List<CalendarDayEntry>();
                var begin = _entry.Begin.Date;
                var end = _entry.End.Date;

                while (begin <= end)
                {
                    var slice = new CalendarDayEntry
                    {
                        Id = $"{_entry.CalendarId}-{begin.Year}-{begin.Month}-{begin.Day}",
                        Begin = Max(begin, _entry.Begin),
                        End = Min(begin.AddHours(23).AddMinutes(59).AddSeconds(59), _entry.End),
                        CalendarEntryId = _entry.Id
                    };
                    slices.Add(slice);
                    begin = begin.AddDays(1);
                }

                return slices;
            }

            private DateTime Max(DateTime a, DateTime b)
            {
                if (b > a)
                {
                    return b;
                }

                return a;
            }

            private DateTime Min(DateTime a, DateTime b)
            {
                if (b > a)
                {
                    return a;
                }

                return b;
            }
        }
    }
}
