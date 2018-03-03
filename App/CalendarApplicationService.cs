using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Client.Documents.Session;

namespace App
{
    public class CalendarApplicationService
    {
        private readonly IDocumentSession _session;

        public CalendarApplicationService(IDocumentSession session)
        {
            _session = session;
        }

        public void Update(Calendar calendar)
        {
            var days = CalendarChunkHelper.SplitIntoChunks(calendar).ToList();
            var exisiting = _session.Load<CalendarChunk>(days.Select(x => x.Id));
            foreach (var calendarDay in days)
            {
                if (exisiting.ContainsKey(calendarDay.Id) && exisiting[calendarDay.Id] != null)
                {
                    exisiting[calendarDay.Id].Update(calendarDay.Entries);
                }
                else
                {
                    _session.Store(calendarDay);
                }
            }
        }

        public Calendar Get(string id, DateTime begin, DateTime end)
        {
            var entries = new List<Reservation>();
            var chunks = _session.Load<CalendarChunk>(CalendarChunkHelper.CreateChunkIds(id, begin, end))
                .Values.Where(x => x != null).SelectMany(x => x.Entries).ToList();

            foreach (var groupedChunks in chunks.GroupBy(x => x.CalendarEntryId))
            {
                var firstChunk = groupedChunks.First();
                var lastChunk = groupedChunks.Last();
                var entry = new Reservation(groupedChunks.Key, id, firstChunk.Begin, lastChunk.End);
                entries.Add(entry);
            }

            return new Calendar(id, new CalendarBounds(begin, end), entries);
        }

        private class CalendarChunkHelper
        {
            private readonly Reservation _reservation;

            private CalendarChunkHelper(Reservation reservation)
            {
                _reservation = reservation;
            }

            public static IEnumerable<CalendarChunk> SplitIntoChunks(Calendar calendar)
            {
                return calendar.Reservations.Select(entry => new CalendarChunkHelper(entry))
                    .SelectMany(helper => helper.SplitIntoChunks()).GroupBy(x => x.Id).Select(entry => new CalendarChunk(entry.Key, entry.ToList()));
            }

            public static IEnumerable<string> CreateChunkIds(string id, DateTime begin, DateTime end)
            {
                var ids = new List<string>();
                while (begin <= end)
                {
                    var chunkId = $"{id}-{begin.Year}-{begin.Month}-{begin.Day}";
                    begin = begin.AddDays(1);
                    ids.Add(chunkId);
                }
                return ids;
            }

            private IEnumerable<CalendarChunkEntry> SplitIntoChunks()
            {
                var slices = new List<CalendarChunkEntry>();
                var begin = _reservation.Begin.Date;
                var end = _reservation.End.Date;

                while (begin <= end)
                {
                    var slice = new CalendarChunkEntry
                    {
                        Id = $"{_reservation.CalendarId}-{begin.Year}-{begin.Month}-{begin.Day}",
                        Begin = Max(begin, _reservation.Begin),
                        End = Min(begin.AddHours(23).AddMinutes(59).AddSeconds(59), _reservation.End),
                        CalendarEntryId = _reservation.Id
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
