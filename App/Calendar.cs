using System;
using System.Collections.Generic;
using System.Linq;

namespace App
{
    public class Calendar
    {
        public Calendar(string id, CalendarBounds bounds, IEnumerable<Reservation> entries) : this(id, bounds)
        {
            Reservations = entries.ToList();
        }

        public Calendar(string id, CalendarBounds bounds)
        {
            Id = id;
            Bounds = bounds;
            Reservations = new List<Reservation>();
        }

        public string Id { get; set; }

        public CalendarBounds Bounds { get; }

        public List<Reservation> Reservations { get; set; }

        public bool IsAvailable(DateTime begin, DateTime end)
        {
            var entries = Reservations.Where(x => x.Begin < end && begin < x.End);
            return !entries.Any();
        }

        public void Reserve(string reservationId, DateTime begin, DateTime end)
        {
            if (!Bounds.IsWithinBounds(begin, end))
            {
                throw new CalendarException($"Entry {begin} - {end} is outside the calendar bounds");
            }

            if (!IsAvailable(begin, end))
            {
                throw new CalendarException($"Entry {begin} - {end} is not available");
            }

            var entry = new Reservation(reservationId, Id, begin, end);
            Reservations.Add(entry);
        }

        public void Cancel(string reservationId)
        {
            foreach (var entry in Reservations.Where(x => x.Id == reservationId).ToList())
            {
                Reservations.Remove(entry);
            }
        }
    }
}
