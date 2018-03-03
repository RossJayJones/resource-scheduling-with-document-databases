using System.Collections.Generic;
using System.Linq;

namespace App
{
    public class CalendarChunk
    {
        public CalendarChunk()
        {
            
        }

        public CalendarChunk(string id, List<CalendarChunkEntry> entries)
        {
            Id = id;
            Entries = entries;
        }

        public string Id { get; set; }

        public List<CalendarChunkEntry> Entries { get; set; }

        public void Update(IEnumerable<CalendarChunkEntry> entries)
        {
            Entries = entries.ToList();
        }
    }
}
