using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace OneMeet365
{
    public class Database : IDatabase
    {
        EventsContext eventsContext;
        public Database(EventsContext eventsContext)
        {       
            this.eventsContext = eventsContext;
        }
        public void Put(EventCardData data)
        {
            lock (eventsContext)
            {
                eventsContext.Events.Add(data);
                eventsContext.SaveChanges();
            }
        }
        public IList<EventCardData> GetAll()
        {
            lock (eventsContext)
            {
                return eventsContext.Events.Select(e => e).Include(e => e.Attendees).Include(e => e.EventData).ToList();
            }
        }
        public EventCardData UpdateAtendees(string key, Atendee changedUser)
        {
            lock (eventsContext)
            {
                var events = eventsContext.Events.Select(e => e).Where(e => e.ResourceResponseId == key).Include(e => e.Attendees).Include(e => e.EventData).ToList();
                var updatedEvent = events.First();

                if (updatedEvent.Attendees.Any(user => user.Name == changedUser.Name))
                {
                    int index = updatedEvent.Attendees.FindIndex(user => user.Name == changedUser.Name);
                    updatedEvent.Attendees.RemoveAt(index);
                }
                else if (updatedEvent.EventData.MaxNumberOfPeople <= 0 || updatedEvent.EventData.MaxNumberOfPeople >= Int32.MaxValue || updatedEvent.Attendees.Count < updatedEvent.EventData.MaxNumberOfPeople)
                {
                    updatedEvent.Attendees.Add(changedUser);
                }

                eventsContext.SaveChanges();
                return updatedEvent;
            }
        }
    }
}
