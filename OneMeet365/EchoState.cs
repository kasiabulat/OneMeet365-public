<<<<<<< HEAD:Teams/EchoState.cs
﻿using Common;
using TextAnalytics;

namespace Teams
=======
﻿namespace OneMeet365
>>>>>>> 3336c32... all projects merged into one, code refactor:Teams/TeamsBotState.cs
{
    /// <summary>
    /// Class for storing conversation state. 
    /// </summary>
    public class EchoState
    {
        public int TurnCount { get; set; } = 0;
<<<<<<< HEAD
        public OneMeetEvent detectedInformation;
<<<<<<< HEAD:Teams/EchoState.cs
        public LuisConversationState luisConversationState = LuisConversationState.ANALYZE_REQUEST;
=======
=======
        public LUIS.ConversationState luisConversationState = LUIS.ConversationState.ANALYZE_REQUEST;
>>>>>>> 3336c32... all projects merged into one, code refactor:Teams/TeamsBotState.cs

        public Microsoft.Bot.Schema.Activity LatestCard = null;
>>>>>>> 29816a3... createcard updatecard
    }
}
