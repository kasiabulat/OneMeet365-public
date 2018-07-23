using System.Collections.Generic;

namespace OneMeet365
{
    public interface IDatabase
    {
        void Put(EventCardData data);
        IList<EventCardData> GetAll();
        EventCardData UpdateAtendees(string key, Atendee changedUser);
    }
}
