using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace OneMeet365
{
    public partial class OneMeetBot : IBot
    {
        LUIS.Connector luisConnector = new LUIS.Connector();

        Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials credentials = new Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials(Configuration.AppId, Configuration.AppPassword);

        IDatabase database;

        public OneMeetBot(IDatabase database)
        {
            this.database = database;
        }

        /// <summary>
        /// Every Conversation turn for our EchoBot will call this method. In here
        /// the bot checks the Activty type to verify it's a message, bumps the 
        /// turn conversation 'Turn' count, and then echoes the users typing
        /// back to them. 
        /// </summary>
        /// <param name="context">Turn scoped context containing all the data needed
        /// for processing this conversation turn. </param>        
        public async Task OnTurn(ITurnContext context)
        {
            try {
            // This bot is only handling Messages
            if (context.Activity.Type == ActivityTypes.Message)
            {
                // Get the conversation state from the turn context
                var state = context.GetConversationState<OneMeetBotState>();

                // Bump the turn count. 
                state.TurnCount++;

                var messageText = context.Activity.Text;

                // Remove bot name mention
                messageText = GetMessageWithoutMention(context, messageText);
                messageText = messageText.Replace("<at>", "");
                messageText = messageText.Replace("</at>", "");
                messageText = messageText.Trim();

                int idx = 0;
                while (!char.IsLetterOrDigit(messageText[idx])) ++idx;
                messageText = messageText.Substring(idx);

                // Check user intention
                var lowerCaseMessage = messageText.ToLower();

                // Initialize bot in channel case
                if (lowerCaseMessage.StartsWith("here"))
                {
                    if (context.Activity.ChannelId == "msteams") {
                        Configuration.TeamsConversationId = context.Activity.Conversation.Id;
                        Configuration.TeamsConversationId = Configuration.TeamsConversationId.Split(';')[0];
                    } else if (context.Activity.ChannelId == "skype") {
                        Configuration.SkypeConversationId = context.Activity.Conversation.Id;
                    }
                    await context.SendActivity("Success! You have registered the bot in this channel!");
                }
                // Join/leave the event button clicked case
                else if (lowerCaseMessage.StartsWith("joinleave")) {
                    try
                    {
                        var cardData = JsonConvert.DeserializeObject<EventCardData>(context.Activity.Value.ToString());
                        EventCardData newCardData = null;

                        var eventDateTime = DateTimeOffset.Parse(cardData.EventData.Time);
                        var userDateTime = context.Activity.LocalTimestamp;

                        if (DateTimeOffset.Compare(eventDateTime,userDateTime.Value)<0)
                        {
                            await SendPrivateMessage(context, $"You cannot join the evnet, because it has already ended. \U0001F613");
                            return;
                        }

                        try
                        {
                            newCardData = database.UpdateAtendees(cardData.ResourceResponseId, new Atendee(context.Activity.From.Name));
                        } catch (Exception)  {
                            await SendPrivateMessage(context, "An error occured when updating Attendees. \U0001F613");
                        }

                        if (newCardData != null)
                            await context.UpdateActivity(CreateActivityForEvent(context, newCardData));
                        else
                            await SendPrivateMessage(context, "An error occured when updating Attendees. \U0001F613");

                    } catch (Exception) {
                        await SendPrivateMessage(context, "An error occured when reading event data. \U0001F613");
                    }
                }
                // Cancel the event button clicked case
                else if (lowerCaseMessage.StartsWith("cancelevent"))
                {
                    try
                    {
                        var cardData = JsonConvert.DeserializeObject<EventCardData>(context.Activity.Value.ToString());

                        // Check if user is eligible to cancel the event
                        if (context.Activity.From.Name == cardData.CreatorName)
                        {
                            await context.UpdateActivity(CancelEvent(context, cardData));
                        }
                        else
                        {
                            await SendPrivateMessage(context, "You cannot cancel the event, because you are not its creator.");
                        }
                    }
                    catch (Exception)
                    {
                        await SendPrivateMessage(context, "An error occured when reading event data. \U0001F613");
                    }
                }
                // GroupMeet direct command call case
                else if (lowerCaseMessage.StartsWith("groupmeet")) {
                    try
                    {
                        var newEvent = await ParseNewEvent(context, messageText, luisConnector);
                        if (newEvent == null) {
                            await SendPrivateMessage(context, "Sorry, I did not understand correctly. \U0001F613");
                        } else {
                            await SendEventToTeams(context, newEvent);
                        }

                    } catch (Exception) {
                        await SendPrivateMessage(context, "Sorry, error occured during parsing command. \U0001F613");
                    }
                }
                // PairMeet command call case
                else if (lowerCaseMessage.StartsWith("pairmeet")) {
                    try { 
                        var serviceUri = new Uri(context.Activity.ServiceUrl, UriKind.Absolute);
                        using (var client = new ConnectorClient(serviceUri, credentials))
                        {
                            var membersResponse = await client.Conversations.GetConversationMembersWithHttpMessagesAsync(context.Activity.Conversation.Id);
                            var members = membersResponse.Body.ToList();
                            var pairs = new Dictionary<ChannelAccount, ChannelAccount>();

                            Random rnd = new Random();
                            while (members.Count > 1) {
                                int otherIndex = rnd.Next(1, members.Count);
                                pairs.Add(members[0], members[otherIndex]);
                                members.RemoveAt(otherIndex);
                                members.RemoveAt(0);
                            }
                            if (members.Count == 1) {
                                pairs.Add(members[0], members[0]);
                                members.Clear();
                            }

                            Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials.TrustServiceUrl(context.Activity.ServiceUrl, System.DateTime.MaxValue);
                            var teamsClient = client.GetTeamsConnectorClient();
                            var teamsChannelData = context.Activity.GetChannelData<Microsoft.Bot.Connector.Teams.Models.TeamsChannelData>();

                            foreach (var pair in pairs) {
                                var conversationResponse = await client.Conversations.CreateOrGetDirectConversation(
                                    context.Activity.Recipient,
                                    pair.Key,
                                    teamsChannelData.Tenant.Id);
                                await client.Conversations.SendToConversationWithHttpMessagesAsync(conversationResponse.Id, GetPairMeetActivity(pair.Key, pair.Value));
                                if (pair.Key != pair.Value) {
                                    conversationResponse = await client.Conversations.CreateOrGetDirectConversation(
                                        context.Activity.Recipient,
                                        pair.Value,
                                        teamsChannelData.Tenant.Id);
                                    await client.Conversations.SendToConversationWithHttpMessagesAsync(conversationResponse.Id, GetPairMeetActivity(pair.Value, pair.Key));
                                }
                            }

                            await context.SendActivity("I have notified the users in private messages!");
                        }
                    } catch (ErrorResponseException ex) {
                        await context.SendActivity(ex.Response.Content.ToString());
                    } catch (Exception ex) {
                        await context.SendActivity(ex.ToString());
                    }
                }
                else if(lowerCaseMessage.StartsWith("time"))
                {
                    await context.SendActivity("Your time is: " + context.Activity.LocalTimestamp.ToString());
                }
                // Default case
                else {
                    // Try to analyse user message with Natural Language Processing  
                    LUIS.Response response = await luisConnector.MakeRequest(messageText);

                    // Check if with high probability user want to cancel the request 
                    if (response.topScoringIntent.intent == LUIS.Connector.CANCEL && response.topScoringIntent.score > 0.40)
                    {
                        state.luisConversationState = LUIS.ConversationState.ANALYZE_REQUEST;
                        await context.SendActivity(GetCancelMessage());
                        return;
                    }

                    string message;

                    // Check the conversation state
                    switch (state.luisConversationState) {
                        case LUIS.ConversationState.ANALYZE_REQUEST:
                            if (response.topScoringIntent.intent == LUIS.Connector.CREATE_GROUP_MEET || 
                                response.intents.Find(i => i.intent == LUIS.Connector.CREATE_GROUP_MEET).score > response.intents.Find(i => i.intent == LUIS.Connector.HELP).score)
                            {
                                string first_reply = luisConnector.GetFirstAnswer(context, response, out state.detectedInformation);
                                state.luisConversationState = LUIS.ConversationState.ASK_NEXT;
                                await SendPrivateMessage(context, $"{first_reply}");
                            } else if (response.topScoringIntent.intent == LUIS.Connector.HELP || response.topScoringIntent.intent == LUIS.Connector.GREET)
                            {
                                await SendPrivateMessage(context, GetHelpMessage(context));
                            }
                            else await SendPrivateMessage(context, $"I did not understand... \U0001F613 If you want to create group meet, please send me a message with activity, place and time.");
                            break;
                        case LUIS.ConversationState.ASK_ACTIVITY:
                            message = luisConnector.GetActivity(response, ref state.detectedInformation, ref state.luisConversationState);
                            await context.SendActivity($"{message}");
                            break;
                        case LUIS.ConversationState.GET_ACTIVITY_AS_MESSAGE:
                            state.detectedInformation.Activity = messageText;
                            state.luisConversationState = LUIS.ConversationState.ASK_NEXT;
                            await context.SendActivity($"I set activity to {messageText}.");
                            break;
                        case LUIS.ConversationState.ASK_PLACE:
                            message = luisConnector.GetPlace(response, ref state.detectedInformation, ref state.luisConversationState);
                            await context.SendActivity($"{message}");
                            break;
                        case LUIS.ConversationState.GET_PLACE_AS_MESSAGE:
                            state.detectedInformation.EventPlace = messageText;
                            state.luisConversationState = LUIS.ConversationState.ASK_NEXT;
                            await context.SendActivity($"I set place to {messageText}.");
                            break;
                        case LUIS.ConversationState.ASK_TIME:
                            message = luisConnector.GetTime(context, response, ref state.detectedInformation, ref state.luisConversationState);
                            await context.SendActivity($"{message}");
                            break;
                        case LUIS.ConversationState.GET_TIME_AS_MESSAGE:
                            state.detectedInformation.Time = messageText;
                            state.luisConversationState = LUIS.ConversationState.ASK_NEXT;
                            await context.SendActivity($"I set time to {messageText}.");
                            break;
                        case LUIS.ConversationState.ASK_PEOPLE:
                            message = luisConnector.GetPeople(response, ref state.detectedInformation, ref state.luisConversationState);
                            await context.SendActivity($"{message}");
                            break;
                        case LUIS.ConversationState.ASK_MEETING_PLACE:
                            message = luisConnector.GetMeetingPlace(response, ref state.detectedInformation, ref state.luisConversationState);
                            await context.SendActivity($"{message}");
                            break;
                        case LUIS.ConversationState.GET_MEETING_PLACE_AS_MESSAGE:
                            state.detectedInformation.MeetingPlace = messageText;
                            state.luisConversationState = LUIS.ConversationState.CONFIRM_REQUEST;
                            await context.SendActivity($"I set place to {messageText}.");
                            break;
                        case LUIS.ConversationState.CONFIRM_REQUEST:
                            if (response.intents.Find(i => i.intent == LUIS.Connector.CONFIRM).score > response.intents.Find(i => i.intent == LUIS.Connector.CANCEL).score)
                            {
                                state.luisConversationState = LUIS.ConversationState.ANALYZE_REQUEST;
                                await SendEventToTeams(context, state.detectedInformation);
                            }
                            else
                            {
                                state.luisConversationState = LUIS.ConversationState.ANALYZE_REQUEST;
                                await context.SendActivity(GetCancelMessage());
                                    
                            }
                            break;
                        default:
                            // Check cancel again
                            state.luisConversationState = LUIS.ConversationState.ANALYZE_REQUEST;
                            if (response.topScoringIntent.intent == LUIS.Connector.CANCEL)
                                await SendPrivateMessage(context, GetCancelMessage());
                            else
                                await SendPrivateMessage(context, $"Sorry, I did not understand... Please try to create the event again");

                            break;
                    }
                    if (state.luisConversationState == LUIS.ConversationState.ASK_NEXT)
                    {
                        var checkStateResp = luisConnector.CheckState(state.detectedInformation);
                        state.luisConversationState = checkStateResp.Item1;
                        await context.SendActivity($"{checkStateResp.Item2}");
                    }
                    if ( state.luisConversationState == LUIS.ConversationState.CONFIRM_REQUEST)
                    {
                        await context.SendActivity(GetFinalInformation(state.detectedInformation));
                    }
                }
            } else if (context.Activity.Type == ActivityTypes.ConversationUpdate) {
                
            }
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }
        }
    }
}    
