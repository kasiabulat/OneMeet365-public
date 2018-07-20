using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class Commands
    {
        private static string botName = "1meet";

        public static List<string> Help()
        {
            return new List<string>
            {
                $"Create event\n@{botName} at <location> departing to <place> with maximum of <number> people",
                $"Reference event\n@{botName} reference event <ID>"
            };
        }
    }
}
