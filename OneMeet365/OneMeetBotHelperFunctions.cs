using System.Collections.Generic;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Connector;
using System;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Connector.Teams;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace OneMeet365
{
    public static class FixedExtensions
    {
        public static async Task<ConversationResourceResponse> CreateOrGetDirectConversation(
            this IConversations conversationClient,
            ChannelAccount bot,
            ChannelAccount user,
            string tenantId)
        {
            var result = await conversationClient.CreateConversationWithHttpMessagesAsync(new ConversationParameters()
            {
                Bot = bot,
                ChannelData = JObject.FromObject(
                new Microsoft.Bot.Connector.Teams.Models.TeamsChannelData
                {
                    Tenant = new Microsoft.Bot.Connector.Teams.Models.TenantInfo
                    {
                        Id = tenantId
                    }
                },
                JsonSerializer.Create(new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                })),
                Members = new List<ChannelAccount>() { user }
            });
            return result.Body;
        }

        public static T AddMentionToText<T>(
            this T activity,
            ChannelAccount mentionedUser,
            MentionTextLocation textLocation = MentionTextLocation.PrependText,
            string mentionText = null)
            where T : IMessageActivity
        {
            if (mentionedUser == null || string.IsNullOrEmpty(mentionedUser.Id))
                throw new ArgumentNullException("mentionedUser", "Mentioned user and user ID cannot be null");

            if (string.IsNullOrEmpty(mentionedUser.Name) && string.IsNullOrEmpty(mentionText))
                throw new ArgumentException("Either mentioned user name or mentionText must have a value");

            if (!string.IsNullOrWhiteSpace(mentionText))
                mentionedUser.Name = mentionText;

            string mentionEntityText = string.Format("<at>{0}</at>", mentionedUser.Name);

            if (textLocation == MentionTextLocation.AppendText)
                activity.Text = activity.Text + " " + mentionEntityText;
            else activity.Text = mentionEntityText + " " + activity.Text;

            if (activity.Entities == null)
                activity.Entities = new List<Entity>();

            activity.Entities.Add(new Mention()
            {
                Text = mentionEntityText,
                Mentioned = mentionedUser
            });

            return activity;
        }
    }
    public partial class OneMeetBot : IBot
    {
        private HeroCard CreateCard(EventCardData cardData)
        {
            var attendees = new System.Text.StringBuilder();
            if (cardData.Attendees != null && cardData.Attendees.Count > 0)
            {
                attendees.Append("<ul>");
                foreach (var user in cardData.Attendees)
                {
                    attendees.Append($"<li>{user.Name}</li>");
                }
                attendees.Append("</ul>");
            }
            else
            {
                attendees.Append("<em>There are no attendees.</em>");
            }
            var attendeesText = "Attendees";
            if (cardData.EventData.MaxNumberOfPeople > 0 && cardData.EventData.MaxNumberOfPeople < Int32.MaxValue)
            {
                attendeesText += $" ({cardData.Attendees.Count} / {cardData.EventData.MaxNumberOfPeople})";
            }
            else if (cardData.Attendees.Count > 0)
            {
                attendeesText += $" ({cardData.Attendees.Count})";
            }
            attendeesText += ":";

            var card = new HeroCard
            {
                Title = $"{cardData.EventData.Activity} {cardData.EventData.EventPlace}",
                Subtitle = $"We meet at {cardData.EventData.Time} in place: {cardData.EventData.MeetingPlace}.",
                Text = $"<strong>{attendeesText}</strong>\n{attendees.ToString()}",
                Buttons = new List<CardAction> {
                    new CardAction(ActionTypes.MessageBack, "Join / Leave", value: JsonConvert.SerializeObject(cardData), text: "joinleave", displayText: ""),
                    new CardAction(ActionTypes.MessageBack, "Cancel", value: JsonConvert.SerializeObject(cardData), text: "cancelevent", displayText: ""),
                },
            };
            return card;
        }

        private Activity CreateActivityForEvent(ITurnContext context, EventCardData cardData)
        {
            var activity = context.Activity.CreateReply();
            activity.Id = cardData.ResourceResponseId;
            activity.Attachments.Add(CreateCard(cardData).ToAttachment());
            return activity;
        }
        private IActivity CancelEvent(ITurnContext context, EventCardData cardData)
        {
            var activity = context.Activity.CreateReply();
            activity.Id = cardData.ResourceResponseId;
            var card = CreateCard(cardData);
            card.Title = "**CANCELED** " + card.Title;
            card.Buttons = null;
            activity.Attachments.Add(card.ToAttachment());
            return activity;
        }

        private async Task SendEventToTeams(ITurnContext context, OneMeetEvent newEvent)
        {
            await SendPrivateMessage(context, "I am creating the event! \U0001F389");
            var cardData = new EventCardData
            {
                CreatorName = context.Activity.From.Name,
                Attendees = new List<Atendee> { new Atendee(context.Activity.From.Name) },
                EventData = newEvent
            };
            var activity = CreateActivityForEvent(context, cardData);

            if (Configuration.TeamsConversationId != null)
            {
                var serviceUri = new Uri(context.Activity.ServiceUrl, UriKind.Absolute);
                using (var client = new ConnectorClient(serviceUri, credentials))
                {
                    activity.Conversation.Id = Configuration.TeamsConversationId;
                    var resource = await client.Conversations.SendToConversationAsync(activity);
                    cardData.ResourceResponseId = resource.Id;
                    activity.Id = resource.Id;
                    activity.Attachments[0] = CreateCard(cardData).ToAttachment();
                    await client.Conversations.UpdateActivityAsync(activity);

                    database.Put(cardData);
                  }
                await SendPrivateMessage(context, "I have notified the channel! \U0001F91F");         
            }
            else
            {
                await SendPrivateMessage(context, "Channel / group is not initialized. Type <em>here @OneMeet 365</em> in a channel!");
            }
        }

        private string GetHelpMessage(ITurnContext context)
        {
            var helpMessage = new System.Text.StringBuilder();
            helpMessage.AppendLine("Hello! \U0001F60A \n Here are my two main functionalities:\n");
            helpMessage.AppendLine("<strong>Pair Meet</strong> \U0001F91D");
            helpMessage.AppendLine("In a channel / group, if you write <em>pairmeet @"+GetBotName(context)+"</em> I will create random pairs of users and notify them in private messages.\n");
            helpMessage.AppendLine("<strong>Group Meet</strong> \U0001F389");
            helpMessage.AppendLine("First set me up in a channel / group by typing <em>here @"+GetBotName(context)+"</em>, this is only needed once. Then when you want to create a new event, write me a private message. You can either talk to me in a natural language (try for example sending \"I want to for for a lunch to Baifu today at noon\") or use a direct command:\n");
            helpMessage.AppendLine("<em>groupmeet go {place} for {activity} at {hour} meet {meetPlace} max {numberOfPeople}</em>\n");
            helpMessage.AppendLine("or its even shorter version:\n<em>groupmeet go {place} for {activity} at {hour}</em> with meeting place the same as event location and possibly unlimited number of people joining</em>\n");
            return helpMessage.ToString();
        }

        private string GetFinalInformation(OneMeetEvent detectedInformation)
        {
            var finalMessage = "I have all the needed data! ";
            finalMessage += "Your activity is  " + detectedInformation.Activity + ". ";
            finalMessage += "Your event place is " + detectedInformation.EventPlace + ". ";
            finalMessage += "You want to meet at " + detectedInformation.Time + " in the place: " + detectedInformation.MeetingPlace + ". ";
            if (detectedInformation.MaxNumberOfPeople > 0 && detectedInformation.MaxNumberOfPeople < Int32.MaxValue)
            {
                finalMessage += "Attendees limit is " + detectedInformation.MaxNumberOfPeople.ToString() + ". ";
            }
            else
            {
                finalMessage += "Number of attendees is unlimited. ";
            }
            finalMessage += "Please confirm if I understood correctly, so I can create the event \U0001F60A";
            return finalMessage;
        }

        private string GetCancelMessage()
        {
            return "OK. I cancel the process of creating event. If you want to start it again, please send me a message with activity, place and time.";
        }

        private Activity GetPairMeetActivity(ChannelAccount directlyTextedPerson, ChannelAccount selectedPartner)
        {
            var activity = MessageFactory.Text("Hello ");
            activity.AddMentionToText(directlyTextedPerson, MentionTextLocation.AppendText, directlyTextedPerson.Name);
            activity.Text += "! Your buddy for this session is ";
            activity.AddMentionToText(selectedPartner, MentionTextLocation.AppendText, selectedPartner.Name);
            activity.Text += "! Have fun!";
            return activity;
        }

        private string GetBotName(ITurnContext context)
        {
            return context.Activity.Recipient.Name;
        }

        private string GetMessageWithoutMention(ITurnContext context, string message)
        {
            return message.Replace(GetBotName(context), "");
        }

        private async static Task<OneMeetEvent> ParseNewEvent(ITurnContext context, string message, LUIS.Connector connector)
        {
            var pattern = "groupmeet go (.*) for (.*) at (.*) meet (.*) max (.*)";
            var match = Regex.Match(message, pattern);
            if (!match.Success)
            {
                match = Regex.Match(message, "groupmeet go (.*) for (.*) at (.*)");
            }
            if (!match.Success)
            {
                return null;
            }

            var timeString = await connector.GetNLPTime(context, match.Groups[3].Value);

            var oneMeetEvent = new OneMeetEvent()
            {
                EventPlace = match.Groups[1].Value,
                Activity = match.Groups[2].Value,
                Time = timeString,
                MeetingPlace = match.Groups[1].Value + " (at location)"
            };

            if (match.Groups.Count == 4)
            {
                return oneMeetEvent;
            }

            int maximum = 0;
            int.TryParse(match.Groups[5].Value, out maximum);
            oneMeetEvent.MeetingPlace = match.Groups[4].Value;
            oneMeetEvent.MaxNumberOfPeople = maximum;

            return oneMeetEvent;
        }

        private async Task SendPrivateMessage(ITurnContext context, string message)
        {
            if (Configuration.TeamsConversationId != null)
            {
                var serviceUri = new Uri(context.Activity.ServiceUrl, UriKind.Absolute);
                var teamsChannelData = context.Activity.GetChannelData<Microsoft.Bot.Connector.Teams.Models.TeamsChannelData>();

                using (var client = new ConnectorClient(serviceUri, credentials))
                {
                    var conversationResponse = await client.Conversations.CreateOrGetDirectConversation(
                        context.Activity.Recipient,
                        context.Activity.From,
                        teamsChannelData.Tenant.Id);
                    var activity = MessageFactory.Text(message);
                    await client.Conversations.SendToConversationWithHttpMessagesAsync(conversationResponse.Id, activity);
                }
            }
            else
            {
                await context.SendActivity(message);
            }
        }
    }
}    
