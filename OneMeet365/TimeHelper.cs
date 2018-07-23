using System;
using System.Globalization;
using Microsoft.Bot.Builder;

namespace OneMeet365
{
    public static class TimeHelper
    {
       public static DateTimeOffset ParseTimex(ITurnContext context, string timex)
        {
            
            var dateTime = DateTime.ParseExact(timex, new string[] {
                "yyyy-MM-ddTHH:mm:ss",
                "yyyy-MM-ddTHH:mm",
                "yyyy-MM-ddTHH",
                "yyyy-MM-dd",
                "THH:mm:ss",
                "THH:mm",
                "THH"
            }, CultureInfo.CurrentCulture, DateTimeStyles.None);

            return new DateTimeOffset(dateTime, context.Activity.LocalTimestamp.Value.Offset);
        }
    }
}
