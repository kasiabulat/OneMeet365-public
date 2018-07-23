namespace OneMeet365
{
    /// <summary>
    /// Class for storing conversation state. 
    /// </summary>
    public class OneMeetBotState
    {
        public int TurnCount { get; set; } = 0;

        public Microsoft.Bot.Schema.Activity LatestCard = null;
        public OneMeetEvent detectedInformation;
        public LUIS.ConversationState luisConversationState = LUIS.ConversationState.ANALYZE_REQUEST;

    }
}
