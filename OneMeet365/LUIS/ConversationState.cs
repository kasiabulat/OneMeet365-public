namespace OneMeet365.LUIS
{
    public enum ConversationState
    {
        ANALYZE_REQUEST,
        CONFIRM_REQUEST,
        ASK_NEXT,
        ASK_ACTIVITY,
        GET_ACTIVITY_AS_MESSAGE,
        ASK_PLACE,
        GET_PLACE_AS_MESSAGE,
        ASK_TIME,
        GET_TIME_AS_MESSAGE,
        ASK_PEOPLE,
        ASK_MEETING_PLACE,
        GET_MEETING_PLACE_AS_MESSAGE
    }
}
