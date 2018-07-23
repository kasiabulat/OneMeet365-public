using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OneMeet365
{
    public class EventCardData {
        [Key]
        public string ResourceResponseId { get; set; }
        public string CreatorName { get; set; }
        public virtual List<Atendee> Attendees { get; set; }
        public OneMeetEvent EventData { get; set; }
    }
    public class Atendee
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Atendee(string name)
        {
            Name = name;
        }
        public Atendee() { }
    }
}