using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;

namespace OneMeet365.LUIS
{
    public class Connector
    {
        public const string CREATE_GROUP_MEET = "CreateGroupMeet";
        public const string INCORRECT_DATA = "IncorrectData";
        public const string CONFIRM = "Utilities.Confirm";
        public const string CANCEL = "Utilities.Cancel";

        public const string ACTIVITY = "activity";
        public const string PLACE = "place";
        public const string DATE = "builtin.datetimeV2.date";
        public const string DATETIME = "builtin.datetimeV2.datetime";
        public const string TIME = "builtin.datetimeV2.time";
        public const string NUMBER = "builtin.number";


        public async Task<Response> MakeRequest(string message)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Configuration.SubscriptionKey);

            var uri = Configuration.AppEndpointBeginning + Configuration.ApplicationId + "?subscription-key=" + Configuration.SubscriptionKey + "&verbose=true&timezoneOffset=0&q=" + message;
            var response = await client.GetAsync(uri);
            var jsonString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Response>(jsonString);
        }

        public string GetRequestType(Response luisResponse)
        {
            return luisResponse.topScoringIntent.intent;
        }

        public string GetFirstAnswer(ITurnContext context, Response luisResponse, out OneMeetEvent oneMeetEvent)
        {
            oneMeetEvent = new OneMeetEvent();

<<<<<<< HEAD
            string response = "Hi, sure!";
            var activities = luisResponse.entities.FindAll((Entity e) => e.type == ACTIVITY);
            if(activities.Count() >= 1)
            {
                response += " I see you want to go for an activity: " + activities[0].entity;
                oneMeetEvent.Activity = activities[0].entity;
            }
            foreach(var activity in activities.Skip(1))
            {
                response += ", " + activity.entity;
            }
            if (activities.Count() >= 1) response += ".";
            if (activities.Count() > 1) response += " Quite a lot of activities. I am selecting the first one."; 

            var dates = luisResponse.entities.FindAll((Entity e) => e.type == DATE);
            oneMeetEvent.Time = "";

            if (dates.Count() >= 1)
            {
                response += " I see your event date is " + dates[0].resolution.values[0].value + ".";
                oneMeetEvent.Time += dates[0].resolution.values[0].value + " ";
            }
=======
            string response = "Hi, sure! \U0001F60E";
            string activityString = GetActivityString(luisResponse);
>>>>>>> 1d13d2d... NLP improvements, refactoring, introducing time parsing

            if (activityString != "")
            {
                response += " I see you want to go for " + activityString + ".";
                oneMeetEvent.Activity = activityString;
            }

            string timeString = GetTimeSting(context, luisResponse);
            if(timeString != "") {
                response += " I see your event date and time is " + timeString + ".";
                oneMeetEvent.Time = timeString;
            }

            var place = luisResponse.entities.Find((Entity e) => e.type == PLACE);
            if(place != null)
            {
                oneMeetEvent.EventPlace = place.entity;
                response += " I'm adding the event place as "+ place.entity + ".";
            }
            return response;
        }

        public Tuple<ConversationState,string> CheckState(OneMeetEvent state)
        {
            ConversationState conversationState = ConversationState.ANALYZE_REQUEST;
            string message = "";

            if (state.Activity == null)
            {
                message = " I don't know what you want to do, yet.\n";
                conversationState = ConversationState.ASK_ACTIVITY;
            }
            else if (state.EventPlace == null)
            {
                message = " I don't know where you want to go, yet.\n";
                conversationState = ConversationState.ASK_PLACE;
            }
            else if (state.Time == null)
            {
                message = " I don't know when you want to meet, yet.\n";
                conversationState = ConversationState.ASK_TIME;
            }
            else if (state.MaxNumberOfPeople == 0)
            {
                message = " Please, tell me how many people can join your event?\n";
                conversationState = ConversationState.ASK_PEOPLE;
            }
            else if (state.MeetingPlace == null)
            {
                message = " Great! Only one thing left to decide. Where do you want to meet?\n";
                conversationState = ConversationState.ASK_MEETING_PLACE;
            }
            else
            {
                message = "Have fun!\n";
                conversationState = ConversationState.ANALYZE_REQUEST;
            }

            return new Tuple<ConversationState, string>(conversationState, message);
        }

        public string GetPlace(Response luisResponse, ref OneMeetEvent oneMeetEvent, ref ConversationState state)
        {
            var place = luisResponse.entities.Find((Entity e) => e.type == PLACE);
            if (place != null)
            {
                oneMeetEvent.EventPlace = place.entity;
                state = ConversationState.ASK_NEXT;
                return "I got it. I set the event place to " + place.entity + ".";
            }
            state = ConversationState.GET_PLACE_AS_MESSAGE;
            return "Sorry, I didn't understand. I'll get the event place name as your next message.";
        }

        public string GetMeetingPlace(Response luisResponse, ref OneMeetEvent oneMeetEvent, ref ConversationState state)
        {
            var place = luisResponse.entities.Find((Entity e) => e.type == PLACE);
            if (place != null)
            {
                oneMeetEvent.MeetingPlace = place.entity;
                state = ConversationState.CONFIRM_REQUEST;
                return "I got it. I set the meeting place to " + place.entity + ".";
            }
            state = ConversationState.GET_MEETING_PLACE_AS_MESSAGE;
            return "Sorry, I didn't understand. I'll get the meeting place name as your next message.";
        }

        private string GetActivityString(Response luisResponse)
        {
            var activities = luisResponse.entities.FindAll((Entity e) => e.type == ACTIVITY);
            string activitiesString = "";

            if (activities.Count() >= 1)
            {
                activitiesString = activities[0].entity;
            }
            foreach (var activity in activities.Skip(1).SkipLast(1))
            {
                activitiesString += ", " + activity.entity;
            }
            if (activities.Count() > 1)
            {
                activitiesString += " and " + activities.Last().entity;
            }
            return activitiesString;
        }

        public string GetActivity(Response luisResponse, ref OneMeetEvent oneMeetEvent, ref ConversationState luisConversationState)
        {
            var activityString = GetActivityString(luisResponse);
            if (activityString != "")
            {
                oneMeetEvent.Activity = activityString;
                luisConversationState = ConversationState.ASK_NEXT;
                return "Cool. I set the activity to " + activityString + ".";
            }
            luisConversationState = ConversationState.GET_ACTIVITY_AS_MESSAGE;
            return "Sorry, I didn't understand. I'll get the activity name as your next message.";
        }

        public string GetTimeSting(ITurnContext context, Response luisResponse)
        {
            var datetime = luisResponse.entities.Find((Entity e) => e.type == DATE || e.type == DATETIME || e.type == TIME);
            if (datetime != null)
            {
                var dateTimeOffset = TimeHelper.ParseTimex(context, datetime.resolution.values[0].timex);
                return dateTimeOffset.ToString();
            }
            else
            {
                // Try to find number and treat it as hour
                var hour = luisResponse.entities.Find((Entity e) => e.type == NUMBER).resolution.value;
                var dateTimeOffset = TimeHelper.ParseTimex(context, "T"+hour);
                return dateTimeOffset.ToString();
            }
        }

        public async Task<string> GetNLPTime(ITurnContext context, string message)
        {
            Response luisResponse = await MakeRequest(message);
            return GetTimeSting(context, luisResponse);
        }

        public string GetTime(ITurnContext context, Response luisResponse, ref OneMeetEvent oneMeetEvent, ref ConversationState luisConversationState)
        {
            var response = "";
            var datetime = GetTimeSting(context, luisResponse);
            if (datetime != "")
            {
                response += " I set your event time to " + datetime + ".";
                oneMeetEvent.Time = datetime;
                luisConversationState = ConversationState.ASK_NEXT;
                return response;
            }
            luisConversationState = ConversationState.GET_TIME_AS_MESSAGE;
            return "Sorry, I didn't understood. I'll get the time as your next message.";
        }

        public string GetPeople(Response luisResponse, ref OneMeetEvent oneMeetEvent, ref ConversationState luisConversationState)
        {
            string response = "";
            var number = luisResponse.entities.Find((Entity e) => e.type == NUMBER);
            if (number != null)
            {
                int result = 0;
                if(!Int32.TryParse(number.entity, out result))
                {
                    result = number.resolution.value;
                }
                oneMeetEvent.MaxNumberOfPeople = result;
                response = "I set people limit to " + result.ToString() + ".";
            }
            else
            {
                response = "I didn't detect the number. I set it as unlimited.";
                oneMeetEvent.MaxNumberOfPeople = Int32.MaxValue;

            }

            luisConversationState = ConversationState.ASK_NEXT;
            return response;
        }
    }
}