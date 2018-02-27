using System;
using System.IO;
using App;
using LiteDB;
using Xunit;

namespace Tests
{
    public class CalendarTests
    {
        [Fact]
        public void ItShouldPreventOverllapingReservationsWithinCalendar()
        {
            using (var stream = new MemoryStream())
            using (var db = new LiteDatabase(stream))
            {
                var bounds = new CalendarBounds(DateTime.Parse("1 Jan 2018 10:00"), DateTime.Parse("2 Jan 2018 10:30"));
                var calendar = new Calendar("desk1", bounds);
                calendar.Reserve("john", DateTime.Parse("1 Jan 2018 10:00"), DateTime.Parse("1 Jan 2018 10:30"));
                calendar.Reserve("peter", DateTime.Parse("1 Jan 2018 10:30"), DateTime.Parse("2 Jan 2018 10:45"));
                var storage = new CalendarStorage(db.GetCollection<CalendarDay>("days"));
                storage.Store(calendar);
                calendar = storage.Get("desk1", DateTime.Parse("1 Jan 2018"), DateTime.Parse("2 Feb 2018"));
                Assert.Throws<CalendarException>(() =>  calendar.Reserve("john", DateTime.Parse("1 Jan 2018 10:30"), DateTime.Parse("2 Jan 2018 10:45")));
            }
        }

        [Fact]
        public void ItShouldPreventReservationsOutsideBoundsOfCalendar()
        {
            using (var stream = new MemoryStream())
            using (var db = new LiteDatabase(stream))
            {
                var bounds = new CalendarBounds(DateTime.Parse("1 Jan 2018 10:00"), DateTime.Parse("2 Jan 2018 10:30"));
                var calendar = new Calendar("desk1", bounds);
                calendar.Reserve("john", DateTime.Parse("1 Jan 2018 10:00"), DateTime.Parse("1 Jan 2018 10:30"));
                calendar.Reserve("peter", DateTime.Parse("1 Jan 2018 10:30"), DateTime.Parse("2 Jan 2018 10:45"));
                var storage = new CalendarStorage(db.GetCollection<CalendarDay>("days"));
                storage.Store(calendar);
                calendar = storage.Get("desk1", DateTime.Parse("1 Jan 2018"), DateTime.Parse("2 Jan 2018"));
                Assert.Throws<CalendarException>(() => calendar.Reserve("john", DateTime.Parse("3 Jan 2018 10:30"), DateTime.Parse("4 Jan 2018 10:45")));
            }
        }
    }
}