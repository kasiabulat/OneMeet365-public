using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Microsoft.Bot;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Schema;
using TextAnalytics;
<<<<<<< HEAD
=======
using System.Net.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Teams;
using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Rest;
using Microsoft.Rest.Serialization;
>>>>>>> 1925422... Fixed wrong extentions

namespace Teams
{
    public static class FixedExtensions {
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
            {
                throw new ArgumentNullException("mentionedUser", "Mentioned user and user ID cannot be null");
            }

            if (string.IsNullOrEmpty(mentionedUser.Name) && string.IsNullOrEmpty(mentionText))
            {
                throw new ArgumentException("Either mentioned user name or mentionText must have a value");
            }

            if (!string.IsNullOrWhiteSpace(mentionText))
            {
                mentionedUser.Name = mentionText;
            }

            string mentionEntityText = string.Format("<at>{0}</at>", mentionedUser.Name);

            if (textLocation == MentionTextLocation.AppendText)
            {
                activity.Text = activity.Text + " " + mentionEntityText;
            }
            else
            {
                activity.Text = mentionEntityText + " " + activity.Text;
            }

            if (activity.Entities == null)
            {
                activity.Entities = new List<Entity>();
            }

            activity.Entities.Add(new Mention()
            {
                Text = mentionEntityText,
                Mentioned = mentionedUser
            });

            return activity;
        }
    }

    public class EchoBot : IBot
    {
        private Microsoft.Bot.Connector.Teams.Models.TeamsChannelData channelData = null;
        LuisConnector connector = new LuisConnector();


        Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials credentials = new Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials(Configuration.AppId, Configuration.AppPassword);

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
                var state = context.GetConversationState<EchoState>();

                // Bump the turn count. 
                state.TurnCount++;

                var messageText = context.Activity.Text;
                messageText = Utils.GetMessageWithoutMention(messageText);
                messageText = messageText.Replace("<at>", "");
                messageText = messageText.Replace("</at>", "");
                messageText = messageText.Trim();
<<<<<<< HEAD
                // if (messageText.Length > 0 && !char.IsLetterOrDigit(messageText[0])) {
                //     messageText = messageText.Remove(0, 1);
                // }

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
                context.OnSendActivities(async (handlerContext, activities, handlerNext) =>
                {
                    Console.WriteLine("OnSendActivities - Activity.Text: " + handlerContext.Activity.Text);
                    activities.ForEach(a => { if (a != null) { Console.WriteLine("OnSendActivities - a.Text: " + a.Text); } });

                    var activity = activities[0];
                    if (activity != null && activity.Text == "Old activity!") {
                        //Console.WriteLine("activity id " + activity.Id);
                        //activity.Text = "Updated!!!!!!";
                        //await context.UpdateActivity(activity);
                    }

                    return await handlerNext();
                });
                context.OnUpdateActivity(async (handlerContext, activities, handlerNext) =>
                {
                    Console.WriteLine("OnUpdateActivity - Activity.Text: " + handlerContext.Activity.Text);
                    Console.WriteLine("OnUpdateActivity - activities.Text: " + activities.Text);

                    return await handlerNext();
                });

>>>>>>> 47fc24c... Updating activities
=======
>>>>>>> 492053f... fix
=======
                await context.SendActivity(messageText);
=======
                await context.SendActivity($"<pre>{messageText}</pre>");
<<<<<<< HEAD
>>>>>>> c1333e9... pre echo
=======
                await context.SendActivity($"<pre>help</pre>");
>>>>>>> 4337416... echo help
=======
>>>>>>> a21a3aa... attendees

>>>>>>> 5622b97... echo
                if (messageText.ToLower().StartsWith("help"))
                {
                    var helpMessage = new System.Text.StringBuilder();
                    helpMessage.AppendLine("<strong>1. Pair Meet</strong>");
                    helpMessage.AppendLine("In a channel / group, write <em>pairmeet @OneMeet 365</em>. The bot will create random pairs of users and notify them in private messages.\n");
                    helpMessage.AppendLine("<strong>2. Group Meet</strong>");
                    helpMessage.AppendLine("First setup your bot in a channel / group by typing <em>here @OneMeet 365</em>, this is only needed once. Then when you want to create a new event, write a private message to the bot. Either talk to the bot in a natural language or use a direct command:\n");
                    helpMessage.AppendLine("<em>groupmeet at {where to meet} at {time to meet} departing to {place / activity} with maximum of {number} people</em>");
                    await context.SendActivity(helpMessage.ToString());
                }
                else if (messageText.ToLower().StartsWith("here"))
                {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
                    var parametersString = messageText.Substring("groupme".Length + 1);
                    var parameters = parametersString.Split(' ');

                    var heroCard = new HeroCard
                    {
                        Title = $"Let's go to {parameters[0]}!",
                        Subtitle = $"Party starts at {parameters[1]}",
                        Text = "Empower others to have fun!",
                        Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Join", value: "join") }
=======
                    string channelId = context.Activity.Conversation.Id;
                    channelId = channelId.Split(';')[0];
                    channelData = new Microsoft.Bot.Connector.Teams.Models.TeamsChannelData {
                        Channel = new Microsoft.Bot.Connector.Teams.Models.ChannelInfo(channelId)
>>>>>>> e31e5c5... Teams groupme to channel
                    };
=======
                    Configuration.channelId = context.Activity.Conversation.Id;
                    Configuration.channelId = Configuration.channelId.Split(';')[0];
>>>>>>> b1e138b... Fixed channel id
=======
                    Configuration.channelId = context.Activity.ChannelId;
                    Configuration.GroupConversationId = Configuration.channelId.Split(';')[0];
                    Configuration.GroupConversationId = context.Activity.Conversation.Id;
>>>>>>> 0a715ab... implement groupme on Skype
=======
                    if (context.Activity.ChannelId == "msteams") {
                        Configuration.TeamsConversationId = context.Activity.Conversation.Id;
                        Configuration.TeamsConversationId = Configuration.TeamsConversationId.Split(';')[0];
                    } else if (context.Activity.ChannelId == "skype") {
                        Configuration.SkypeConversationId = context.Activity.Conversation.Id;
                    }
<<<<<<< HEAD
>>>>>>> 3f03a74... Teams&Skype both at the same time
=======
                    await context.SendActivity("Success! You have registered the bot in this channel!");
>>>>>>> b2d44ee... logging
                }
                else if (messageText.ToLower().StartsWith("joinleave")) {
                    try
=======
                    if (messageText.ToLower().StartsWith("help"))
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
=======
                    if (messageText.ToLower().StartsWith("help"))
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
                    {
                        var helpMessage = new System.Text.StringBuilder();
                        helpMessage.AppendLine("<strong>1. Pair Meet</strong>");
                        helpMessage.AppendLine("In a channel / group, write <em>pairmeet @OneMeet 365</em>. The bot will create random pairs of users and notify them in private messages.\n");
                        helpMessage.AppendLine("<strong>2. Group Meet</strong>");
                        helpMessage.AppendLine("First setup your bot in a channel / group by typing <em>here @OneMeet 365</em>, this is only needed once. Then when you want to create a new event, write a private message to the bot. Either talk to the bot in a natural language or use a direct command:\n");
                        helpMessage.AppendLine("<em>groupmeet at {where to meet} at {time to meet} departing to {place / activity} with maximum of {number} people</em>");
                        await context.SendActivity(helpMessage.ToString());
                    }
                    else if (messageText.ToLower().StartsWith("here"))
                    {
                        if (context.Activity.ChannelId == "msteams") {
                            Configuration.TeamsConversationId = context.Activity.Conversation.Id;
                            Configuration.TeamsConversationId = Configuration.TeamsConversationId.Split(';')[0];
                        } else if (context.Activity.ChannelId == "skype") {
                            Configuration.SkypeConversationId = context.Activity.Conversation.Id;
                        }
                        await context.SendActivity("Success! You have registered the bot in this channel!");
                    }
<<<<<<< HEAD
<<<<<<< HEAD
                }
                else if (messageText.ToLower().StartsWith("pairmeet")) {
                    try {
                        var serviceUri = new System.Uri(context.Activity.ServiceUrl, System.UriKind.Absolute);
                        using (var client = new Microsoft.Bot.Connector.ConnectorClient(serviceUri, credentials))
=======
                    if (messageText.ToLower().StartsWith("help"))
                    {
                        Commands.Help().ForEach(async l => await context.SendActivity(l));
                    }
                    else if (messageText.ToLower().StartsWith("here"))
                    {
                        Configuration.channelId = context.Activity.Conversation.Id;
                        Configuration.channelId = Configuration.channelId.Split(';')[0];
                    }
                    else if (messageText.ToLower().StartsWith("pairmeet"))
                    {
                        try
>>>>>>> 552f612... work on natural conversation with bot - meeting place detected correctly
                        {
                            var serviceUri = new System.Uri(context.Activity.ServiceUrl, System.UriKind.Absolute);
                            using (var client = new Microsoft.Bot.Connector.ConnectorClient(serviceUri, credentials))
                            {
                                var membersResponse = await client.Conversations.GetConversationMembersWithHttpMessagesAsync(context.Activity.Conversation.Id);
                                var members = membersResponse.Body.ToList();
                                var pairs = new Dictionary<ChannelAccount, ChannelAccount>();

                                Random rnd = new Random();
                                while (members.Count > 1)
                                {
=======
                    else if (messageText.ToLower().StartsWith("joinleave")) {
                        try
                        {
=======
                    else if (messageText.ToLower().StartsWith("joinleave")) {
                        try
                        {
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
                            var cardData = JsonConvert.DeserializeObject<EventCardData>(context.Activity.Value.ToString());
                            if (cardData.Attendees.Any(user => user.Id == context.Activity.From.Id)) {
                                // remove:
                                int index = cardData.Attendees.FindIndex(user => user.Id == context.Activity.From.Id);
                                cardData.Attendees.RemoveAt(index);
                            } else {
                                // add:
                                if (cardData.EventData.MaxNumberOfPeople > 0 && cardData.EventData.MaxNumberOfPeople < Int32.MaxValue && cardData.Attendees.Count >= cardData.EventData.MaxNumberOfPeople) {
                                    return;
                                } else {
                                    cardData.Attendees.Add(context.Activity.From);
                                }
                            }
                            await context.UpdateActivity(CreateActivityForEvent(context, cardData));
                        } catch (ErrorResponseException ex) {
                            await context.SendActivity(ex.Response.Content.ToString());
                        } catch (Exception ex) {
                            await context.SendActivity(ex.ToString());
                        }
                    }
                    else if (messageText.ToLower().StartsWith("groupmeet")) {
                        try
                        {
                            var newEvent = Utils.ParseNewEvent(messageText);
                            if (newEvent == null) {
                                await context.SendActivity("Sorry, I did not understand correctly. \U0001F613");
                            } else {
                                SendEventToTeams(context, newEvent);
                            }

                        } catch (ErrorResponseException ex) {
                            await context.SendActivity(ex.StackTrace);
                            await context.SendActivity(ex.Response.Content.ToString());
                        } catch (Exception ex) {
                            await context.SendActivity(ex.ToString());
                        }
                    }
                    else if (messageText.ToLower().StartsWith("pairmeet")) {
                        try {
                            var serviceUri = new System.Uri(context.Activity.ServiceUrl, System.UriKind.Absolute);
                            using (var client = new Microsoft.Bot.Connector.ConnectorClient(serviceUri, credentials))
                            {
                                var membersResponse = await client.Conversations.GetConversationMembersWithHttpMessagesAsync(context.Activity.Conversation.Id);
                                var members = membersResponse.Body.ToList();
                                var pairs = new Dictionary<ChannelAccount, ChannelAccount>();

                                Random rnd = new Random();
                                while (members.Count > 1) {
<<<<<<< HEAD
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
=======
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
                                    int otherIndex = rnd.Next(1, members.Count);
                                    pairs.Add(members[0], members[otherIndex]);
                                    members.RemoveAt(otherIndex);
                                    members.RemoveAt(0);
                                }
<<<<<<< HEAD
<<<<<<< HEAD
                                if (members.Count == 1)
                                {
=======
                                if (members.Count == 1) {
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
=======
                                if (members.Count == 1) {
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
                                    pairs.Add(members[0], members[0]);
                                    members.Clear();
                                }

<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
=======
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
                                Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials.TrustServiceUrl(context.Activity.ServiceUrl, System.DateTime.MaxValue);
                                var teamsClient = client.GetTeamsConnectorClient();
                                var teamsChannelData = context.Activity.GetChannelData<Microsoft.Bot.Connector.Teams.Models.TeamsChannelData>();

<<<<<<< HEAD
<<<<<<< HEAD
                                foreach (var pair in pairs)
                                {
                                    var conversationResponse = await client.Conversations.CreateOrGetDirectConversation(
=======
                            foreach (var pair in pairs) {
                                var conversationResponse = await client.Conversations.CreateOrGetDirectConversation(
                                    context.Activity.Recipient,
                                    pair.Key,
                                    teamsChannelData.Tenant.Id);
                                var activity1 = MessageFactory.Text("Hello ");
                                activity1.AddMentionToText(pair.Key, MentionTextLocation.AppendText, pair.Key.Name);
                                activity1.Text += "! Your buddy for this session is ";
                                activity1.AddMentionToText(pair.Value, MentionTextLocation.AppendText, pair.Value.Name);
                                activity1.Text += "! Have fun!";
                                await client.Conversations.SendToConversationWithHttpMessagesAsync(conversationResponse.Id, activity1);
                                if (pair.Key != pair.Value) {
                                    conversationResponse = await client.Conversations.CreateOrGetDirectConversation(
>>>>>>> 333ff6b... at mentions
                                        context.Activity.Recipient,
                                        pair.Key,
                                        teamsChannelData.Tenant.Id);
<<<<<<< HEAD
                                    await client.Conversations.SendToConversationWithHttpMessagesAsync(conversationResponse.Id, MessageFactory.Text(
                                        $"Hello {pair.Key.Name}! Your buddy for this session is {pair.Value.Name}! Have fun!"
                                    ));
                                    if (pair.Key != pair.Value)
                                    {
=======
=======
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
                                foreach (var pair in pairs) {
                                    var conversationResponse = await client.Conversations.CreateOrGetDirectConversation(
                                        context.Activity.Recipient,
                                        pair.Key,
                                        teamsChannelData.Tenant.Id);
                                    var activity1 = MessageFactory.Text("Hello ");
                                    activity1.AddMentionToText(pair.Key, MentionTextLocation.AppendText, pair.Key.Name);
                                    activity1.Text += "! Your buddy for this session is ";
                                    activity1.AddMentionToText(pair.Value, MentionTextLocation.AppendText, pair.Value.Name);
                                    activity1.Text += "! Have fun!";
                                    await client.Conversations.SendToConversationWithHttpMessagesAsync(conversationResponse.Id, activity1);
                                    if (pair.Key != pair.Value) {
<<<<<<< HEAD
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
=======
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
                                        conversationResponse = await client.Conversations.CreateOrGetDirectConversation(
                                            context.Activity.Recipient,
                                            pair.Value,
                                            teamsChannelData.Tenant.Id);
<<<<<<< HEAD
<<<<<<< HEAD
                                        await client.Conversations.SendToConversationWithHttpMessagesAsync(conversationResponse.Id, MessageFactory.Text(
                                            $"Hello {pair.Value.Name}! Your buddy for this session is {pair.Key.Name}! Have fun!"
                                        ));
                                    }
=======
                                    var activity2 = MessageFactory.Text("Hello ");
                                    activity2.AddMentionToText(pair.Value, MentionTextLocation.AppendText, pair.Value.Name);
                                    activity2.Text += "! Your buddy for this session is ";
                                    activity2.AddMentionToText(pair.Key, MentionTextLocation.AppendText, pair.Key.Name);
                                    activity2.Text += "! Have fun!";
                                    await client.Conversations.SendToConversationWithHttpMessagesAsync(conversationResponse.Id, activity2);
>>>>>>> 333ff6b... at mentions
                                }

                                await context.SendActivity("I have notified the users in private messages!");
                            }
<<<<<<< HEAD
=======

                            await context.SendActivity("I have notified the users in private messages!");
>>>>>>> 47fc24c... Updating activities
                        }
                        catch (ErrorResponseException ex)
                        {
                            await context.SendActivity(ex.Response.Content.ToString());
                        }
                        catch (Exception ex)
                        {
                            await context.SendActivity(ex.ToString());
                        }
                    }
<<<<<<< HEAD
                    else if (messageText.ToLower().StartsWith("connector"))
                    {
                        try
                        {
                            var serviceUri = new System.Uri(context.Activity.ServiceUrl, System.UriKind.Absolute);
                            using (var client = new Microsoft.Bot.Connector.ConnectorClient(serviceUri, credentials))
                            {
                                var response = await client.Conversations.GetConversationMembersWithHttpMessagesAsync(context.Activity.Conversation.Id);
                                var members = response.Body;
                                System.Text.StringBuilder membersString = new System.Text.StringBuilder();
                                foreach (var member in members)
                                {
                                    membersString.AppendLine($"Name: {member.Name}, ID: {member.Id}");
                                }
                                await context.SendActivity(membersString.ToString());
                                var cpars = new ConversationParameters(
                                    bot: context.Activity.Recipient,
                                    members: new ChannelAccount[] { new ChannelAccount("29:1zjEQuhXaQTIhcoMff01ISAQdVITclCoC7gWwZIzVnQBHMw5NjBrnpS71Q40TBNtr2yjQ2lN-lM1_pYzD_PGZRA") }
                                );
                                Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials.TrustServiceUrl(context.Activity.ServiceUrl, System.DateTime.MaxValue);
                                var teamsClient = client.GetTeamsConnectorClient();
                                var teamsChannelData = context.Activity.GetChannelData<Microsoft.Bot.Connector.Teams.Models.TeamsChannelData>();
                                var response2 = await client.Conversations.CreateOrGetDirectConversation(context.Activity.Recipient, new ChannelAccount("29:1k2w9qvQK64KH9eX8ptyOv5pPdkD5WjWYu5b9ST8g26K62jkcgduRXt7LbQ1y-GaOUsccQvf_MLzvgaTdEE74YQ"),
                                    teamsChannelData.Tenant.Id);
                                await client.Conversations.SendToConversationWithHttpMessagesAsync(response2.Id, MessageFactory.Text("Hi all"));
                                await context.SendActivity(response2.Id);
                            }
                        }
                        catch (ErrorResponseException ex)
                        {
                            await context.SendActivity(ex.Response.Content.ToString());
                        }
                        catch (Exception ex)
                        {
                            await context.SendActivity(ex.ToString());
                        }
                    }
                    else if (messageText.ToLower().StartsWith("groupme"))
                    {
                        if (Configuration.channelId == null)
                        {
                            await context.SendActivity("Please initialize me first! Use `here`!");
                            return;
                        }
                        try
                        {
                            channelData = new Microsoft.Bot.Connector.Teams.Models.TeamsChannelData
                            {
                                Channel = new Microsoft.Bot.Connector.Teams.Models.ChannelInfo(Configuration.channelId)
                            };

                            var parametersString = messageText.Substring("groupme".Length + 1);
                            var parameters = parametersString.Split(' ');
                            var heroCard = new HeroCard
                            {
                                Title = $"Let's go to {parameters[0]}!",
                                Subtitle = $"Party starts at {parameters[1]}",
                                Text = "Empower others to have fun!",
                                Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Join", value: "join") }
                            };

                            var activity = MessageFactory.Attachment(heroCard.ToAttachment());
                            // activity.Id = context.Activity.Id;
                            // await context.UpdateActivity(activity);

                            var cpars = new ConversationParameters(
                                isGroup: true,
                                bot: null,
                                members: null,
                                topicName: "GroupMe",
                                activity: (Activity)activity,
                                channelData: channelData
                            );
                            await (context.Adapter as Microsoft.Bot.Builder.Adapters.BotFrameworkAdapter).CreateConversation(
                                Configuration.channelId, context.Activity.ServiceUrl, credentials, cpars, (newContext) =>
                                {
                                    return Task.CompletedTask;
                                });
                            await context.SendActivity("Notification sent to the channel!");
                        }
                        catch (System.Exception ex)
                        {
                            await context.SendActivity(ex.ToString());
                        }
<<<<<<< HEAD
=======
                    } catch (ErrorResponseException ex) {
                        await context.SendActivity(ex.Response.Content.ToString());
                    } catch (Exception ex) {
                        await context.SendActivity(ex.ToString());
                    }
                }
<<<<<<< HEAD
                else if (messageText.ToLower().StartsWith("groupme"))
                {
                    if (Configuration.SkypeConversationId == null && Configuration.TeamsConversationId == null) {
                        await context.SendActivity("Please initialize me first! Use `here`!");
                        return;
>>>>>>> 3f03a74... Teams&Skype both at the same time
                    }
<<<<<<< HEAD
                    else if (messageText.ToLower().StartsWith("stats"))
                    {
                        await context.SendActivity(context.Adapter.GetType().ToString());
                        var creds = new Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials("36a939bb-7743-4ca8-96c3-cf4384de26b3", "jbfhvVFH0%}tpSRUG9563|~");
                        var channelData = new Microsoft.Bot.Connector.Teams.Models.TeamsChannelData
                        {
                            Channel = new Microsoft.Bot.Connector.Teams.Models.ChannelInfo("19:1a74c4717a5a4e559a2964f6199e5fdd@thread.skype")
=======
                    try {
                        var newEvent = Utils.ParseNewEvent(messageText);

                        var heroCard = new HeroCard
                        {
                            Title = $"Let's go to {newEvent.WhereToGo}!",
                            Subtitle = $"Activity starts at {newEvent.WhenToMeet}",
                            Text = $"Meet at {newEvent.WhereToMeet}",
                            Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Join", value: "join") }
>>>>>>> 0a715ab... implement groupme on Skype
                        };
<<<<<<< HEAD
                        IMessageActivity newMessage = Activity.CreateMessageActivity();
                        newMessage.Type = ActivityTypes.Message;
                        newMessage.Text = "Hello, on a new thread";
                        var cpars = new ConversationParameters(
                            isGroup: true,
                            bot: null,
                            members: null,
                            topicName: "Test Conversation",
                            activity: (Activity)newMessage,
                            channelData: channelData
                        );
<<<<<<< HEAD
<<<<<<< HEAD
                        await (context.Adapter as Microsoft.Bot.Builder.Adapters.BotFrameworkAdapter).CreateConversation(
                            Configuration.channelId, context.Activity.ServiceUrl, credentials, cpars, (newContext) => {
                                return Task.CompletedTask;
                            });
                        await context.SendActivity("Notification sent to the channel!");
=======
=======

                        var activity = MessageFactory.Attachment(heroCard.ToAttachment());
                        // activity.Id = context.Activity.Id;
                        // await context.UpdateActivity(activity);

>>>>>>> 3f03a74... Teams&Skype both at the same time

                        if (Configuration.TeamsConversationId != null)
                        {
                            channelData = new Microsoft.Bot.Connector.Teams.Models.TeamsChannelData {
                                Channel = new Microsoft.Bot.Connector.Teams.Models.ChannelInfo(Configuration.TeamsConversationId)
                            };
                            var cpars = new ConversationParameters(
                                isGroup: true,
                                bot: null,
                                members: null,
                                topicName: "GroupMe",
                                activity: (Activity)activity,
                                channelData: channelData
                            );
                            await (context.Adapter as Microsoft.Bot.Builder.Adapters.BotFrameworkAdapter).CreateConversation(
                                Configuration.TeamsConversationId, context.Activity.ServiceUrl, credentials, cpars, (newContext) =>
                                {
                                    return Task.CompletedTask;
                                });
                        }
                        if (Configuration.SkypeConversationId != null)
                        {
                            var serviceUri = new System.Uri(context.Activity.ServiceUrl, System.UriKind.Absolute);
                            var cpars = new ConversationParameters(
                                isGroup: true,
                                bot: null,
                                members: null,
                                topicName: "GroupMe",
                                activity: (Activity)activity
                            );
                            using (var client = new Microsoft.Bot.Connector.ConnectorClient(serviceUri, credentials))
                            {
                                cpars.Members = await client.Conversations.GetActivityMembersAsync(Configuration.SkypeConversationId, context.Activity.Id);
                                await client.Conversations.SendToConversationAsync(Configuration.SkypeConversationId, (Activity)activity);
                            }
<<<<<<< HEAD
                        await context.SendActivity($"Notification sent to the channel {Configuration.channelId}");
>>>>>>> 0a715ab... implement groupme on Skype
=======
                        }
                        await context.SendActivity($"Notifications sent!");
>>>>>>> 3f03a74... Teams&Skype both at the same time
                    } catch (System.Exception ex) {
                        await context.SendActivity(ex.ToString());
                    }
                }
                else if (messageText.ToLower().StartsWith("luis"))
                {
<<<<<<< HEAD
                        LUISConnector connector = new LUISConnector();
                        string response = await connector.MakeRequest(messageText);
                        await context.SendActivity($"Turn {state.TurnCount}: LUIS responce: '{response}'");
=======
                    await context.SendActivity(context.Adapter.GetType().ToString());
<<<<<<< HEAD
                    var creds = new Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials("36a939bb-7743-4ca8-96c3-cf4384de26b3", "jbfhvVFH0%}tpSRUG9563|~");
<<<<<<< HEAD
                    var cpars = new ConversationParameters();
=======
=======
                    var creds = new Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials(Configuration.AppId, Configuration.AppPassword);
>>>>>>> 0a715ab... implement groupme on Skype
                    var channelData = new Microsoft.Bot.Connector.Teams.Models.TeamsChannelData {
                        Channel = new Microsoft.Bot.Connector.Teams.Models.ChannelInfo("19:1a74c4717a5a4e559a2964f6199e5fdd@thread.skype")
                    };
                    IMessageActivity newMessage = Activity.CreateMessageActivity();
                    newMessage.Type = ActivityTypes.Message;
                    newMessage.Text = "Hello, on a new thread";
                    var cpars = new ConversationParameters(
                        isGroup: true,
                        bot: null,
                        members: null,
                        topicName: "Test Conversation",
                        activity: (Activity)newMessage,
                        channelData: channelData
                    );
>>>>>>> 813541a... Connector for Teams
                    try {
                        await (context.Adapter as Microsoft.Bot.Builder.Adapters.BotFrameworkAdapter).CreateConversation(
                            context.Activity.Conversation.Id, context.Activity.ServiceUrl, creds, cpars, (newContext) => {
                                return newContext.SendActivity("New activity!");
                            });
                    } catch(System.Exception e) {
                        await context.SendActivity(e.ToString());
                    }
                    string baseUrl = $"{context.Activity.ServiceUrl}v3/conversations/19:1a74c4717a5a4e559a2964f6199e5fdd@thread.skype/members";
                    //The 'using' will help to prevent memory leaks.
                    //Create a new instance of HttpClient
                    // TODO we should fetch it
                    string myToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjdfWnVmMXR2a3dMeFlhSFMzcTZsVWpVWUlHdyIsImtpZCI6IjdfWnVmMXR2a3dMeFlhSFMzcTZsVWpVWUlHdyJ9.eyJhdWQiOiJodHRwczovL2FwaS5ib3RmcmFtZXdvcmsuY29tIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvZDZkNDk0MjAtZjM5Yi00ZGY3LWExZGMtZDU5YTkzNTg3MWRiLyIsImlhdCI6MTUzMjQzNDM3NCwibmJmIjoxNTMyNDM0Mzc0LCJleHAiOjE1MzI0MzgyNzQsImFpbyI6IjQyQmdZQWhOV01WNVlKZkkzZFhuZHE4UG1GSWtBUUE9IiwiYXBwaWQiOiIzNmE5MzliYi03NzQzLTRjYTgtOTZjMy1jZjQzODRkZTI2YjMiLCJhcHBpZGFjciI6IjEiLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9kNmQ0OTQyMC1mMzliLTRkZjctYTFkYy1kNTlhOTM1ODcxZGIvIiwidGlkIjoiZDZkNDk0MjAtZjM5Yi00ZGY3LWExZGMtZDU5YTkzNTg3MWRiIiwidXRpIjoiMUl2cG1xV2lza3Fpc0hFakdDTUtBQSIsInZlciI6IjEuMCJ9.Eq1VYici_xt8ReiCnyCjx83t7o6VKij7eLpIwYLdFFsQ9lmAdOw5qNqb8w5JvNGYLM1wZsM8QZ9HVshr0PLfXC3wKlTnI9myiAK8JhurGg3wj2iaRXOxFgtIZu-g617GWEt3faB8q4nQfbJm9T1p3kc0raT0J-Clif1ETcCeGsGFVPVEdv4Qghge107BlRF83dLffFD07gtGG2uCMlivaxHuVAf4pTAlOcDf3XvrH0AHbzyj1IB7n7MgFvjCkigaovg82QNxXhWJAl8TBYSsaYIZeQ6_ersXk9uctIjSmt_TkYOgc1XdD-5e-jn9ZzT_VEI1H--mAwc3JxTxNfwWvw";
                    using (HttpClient client = new HttpClient()) {
                    //Setting up the response...         
                        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", myToken);
                        using (HttpResponseMessage res = await client.GetAsync(baseUrl))
=======
                        try
>>>>>>> 552f612... work on natural conversation with bot - meeting place detected correctly
                        {
                            await (context.Adapter as Microsoft.Bot.Builder.Adapters.BotFrameworkAdapter).CreateConversation(
                                context.Activity.Conversation.Id, context.Activity.ServiceUrl, creds, cpars, (newContext) =>
                                {
                                    return newContext.SendActivity("New activity!");
                                });
                        }
                        catch (System.Exception e)
                        {
                            await context.SendActivity(e.ToString());
                        }
                        string baseUrl = $"{context.Activity.ServiceUrl}v3/conversations/19:1a74c4717a5a4e559a2964f6199e5fdd@thread.skype/members";
                        //The 'using' will help to prevent memory leaks.
                        //Create a new instance of HttpClient
                        // TODO we should fetch it
                        string myToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjdfWnVmMXR2a3dMeFlhSFMzcTZsVWpVWUlHdyIsImtpZCI6IjdfWnVmMXR2a3dMeFlhSFMzcTZsVWpVWUlHdyJ9.eyJhdWQiOiJodHRwczovL2FwaS5ib3RmcmFtZXdvcmsuY29tIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvZDZkNDk0MjAtZjM5Yi00ZGY3LWExZGMtZDU5YTkzNTg3MWRiLyIsImlhdCI6MTUzMjQzNDM3NCwibmJmIjoxNTMyNDM0Mzc0LCJleHAiOjE1MzI0MzgyNzQsImFpbyI6IjQyQmdZQWhOV01WNVlKZkkzZFhuZHE4UG1GSWtBUUE9IiwiYXBwaWQiOiIzNmE5MzliYi03NzQzLTRjYTgtOTZjMy1jZjQzODRkZTI2YjMiLCJhcHBpZGFjciI6IjEiLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9kNmQ0OTQyMC1mMzliLTRkZjctYTFkYy1kNTlhOTM1ODcxZGIvIiwidGlkIjoiZDZkNDk0MjAtZjM5Yi00ZGY3LWExZGMtZDU5YTkzNTg3MWRiIiwidXRpIjoiMUl2cG1xV2lza3Fpc0hFakdDTUtBQSIsInZlciI6IjEuMCJ9.Eq1VYici_xt8ReiCnyCjx83t7o6VKij7eLpIwYLdFFsQ9lmAdOw5qNqb8w5JvNGYLM1wZsM8QZ9HVshr0PLfXC3wKlTnI9myiAK8JhurGg3wj2iaRXOxFgtIZu-g617GWEt3faB8q4nQfbJm9T1p3kc0raT0J-Clif1ETcCeGsGFVPVEdv4Qghge107BlRF83dLffFD07gtGG2uCMlivaxHuVAf4pTAlOcDf3XvrH0AHbzyj1IB7n7MgFvjCkigaovg82QNxXhWJAl8TBYSsaYIZeQ6_ersXk9uctIjSmt_TkYOgc1XdD-5e-jn9ZzT_VEI1H--mAwc3JxTxNfwWvw";
                        using (HttpClient client = new HttpClient())
                        {
                            //Setting up the response...         
                            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", myToken);
                            using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                            {
                                using (HttpContent content = res.Content)
                                {
                                    string data = await content.ReadAsStringAsync();
                                    if (data != null)
                                    {
                                        await context.SendActivity(data);
                                    }
                                }
                            }
                        }
<<<<<<< HEAD
>>>>>>> b54f776... Try using REST API
                    }
<<<<<<< HEAD
=======

                    await context.SendActivity($"Service URL: {context.Activity.ServiceUrl}\nConversation ID: {context.Activity.Conversation.Id}");
                }
<<<<<<< HEAD
                else if (messageText.ToLower().StartsWith("lang"))
=======
                }
                // else if (messageText.ToLower().StartsWith("connector")) {
                //     try {
                //         var serviceUri = new System.Uri(context.Activity.ServiceUrl, System.UriKind.Absolute);
                //         using (var client = new Microsoft.Bot.Connector.ConnectorClient(serviceUri, credentials))
                //         {
                //             var response = await client.Conversations.GetConversationMembersWithHttpMessagesAsync(context.Activity.Conversation.Id);
                //             var members = response.Body;
                //             System.Text.StringBuilder membersString = new System.Text.StringBuilder();
                //             foreach (var member in members) {
                //                 membersString.AppendLine($"Name: {member.Name}, ID: {member.Id}");
                //             }
                //             await context.SendActivity(membersString.ToString());
                //             var cpars = new ConversationParameters(
                //                 bot: context.Activity.Recipient,
                //                 members: new ChannelAccount[] { new ChannelAccount("29:1zjEQuhXaQTIhcoMff01ISAQdVITclCoC7gWwZIzVnQBHMw5NjBrnpS71Q40TBNtr2yjQ2lN-lM1_pYzD_PGZRA") }
                //             );
                //             Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials.TrustServiceUrl(context.Activity.ServiceUrl, System.DateTime.MaxValue);
                //             var teamsClient = client.GetTeamsConnectorClient();
                //             var teamsChannelData = context.Activity.GetChannelData<Microsoft.Bot.Connector.Teams.Models.TeamsChannelData>();
                //             var response2 = await client.Conversations.CreateOrGetDirectConversation(context.Activity.Recipient, new ChannelAccount("29:1k2w9qvQK64KH9eX8ptyOv5pPdkD5WjWYu5b9ST8g26K62jkcgduRXt7LbQ1y-GaOUsccQvf_MLzvgaTdEE74YQ"),
                //                 teamsChannelData.Tenant.Id);
                //             await client.Conversations.SendToConversationWithHttpMessagesAsync(response2.Id, MessageFactory.Text("Hi all"));
                //             await context.SendActivity(response2.Id);
                //         }
                //     } catch (ErrorResponseException ex) {
                //         await context.SendActivity(ex.Response.Content.ToString());
                //     } catch (Exception ex) {
                //         await context.SendActivity(ex.ToString());
                //     }
                // }
                // else if (messageText.ToLower().StartsWith("groupme"))
                // {
                //     if (Configuration.SkypeConversationId == null && Configuration.TeamsConversationId == null) {
                //         await context.SendActivity("Please initialize me first! Use `here`!");
                //         return;
                //     }
                //     try {
                //         var newEvent = Utils.ParseNewEvent(messageText);

                //         var heroCard = new HeroCard
                //         {
                //             Title = $"Let's go to {newEvent.WhereToGo}!",
                //             Subtitle = $"Activity starts at {newEvent.WhenToMeet}",
                //             Text = $"Meet at {newEvent.WhereToMeet}",
                //             Buttons = new List<CardAction> { new CardAction(ActionTypes.PostBack, "Join", value: "join") }
                //         };

                //         var activity = MessageFactory.Attachment(heroCard.ToAttachment());
                //         // activity.Id = context.Activity.Id;
                //         // await context.UpdateActivity(activity);


                //         if (Configuration.TeamsConversationId != null)
                //         {
                //             channelData = new Microsoft.Bot.Connector.Teams.Models.TeamsChannelData {
                //                 Channel = new Microsoft.Bot.Connector.Teams.Models.ChannelInfo(Configuration.TeamsConversationId)
                //             };
                //             var cpars = new ConversationParameters(
                //                 isGroup: true,
                //                 bot: null,
                //                 members: null,
                //                 topicName: "GroupMe",
                //                 activity: (Activity)activity,
                //                 channelData: channelData
                //             );
                //             await (context.Adapter as Microsoft.Bot.Builder.Adapters.BotFrameworkAdapter).CreateConversation(
                //                 Configuration.TeamsConversationId, context.Activity.ServiceUrl, credentials, cpars, (newContext) =>
                //                 {
                //                     return Task.CompletedTask;
                //                 });
                //         }
                //         if (Configuration.SkypeConversationId != null)
                //         {
                //             var serviceUri = new System.Uri(context.Activity.ServiceUrl, System.UriKind.Absolute);
                //             var cpars = new ConversationParameters(
                //                 isGroup: true,
                //                 bot: null,
                //                 members: null,
                //                 topicName: "GroupMe",
                //                 activity: (Activity)activity
                //             );
                //             using (var client = new Microsoft.Bot.Connector.ConnectorClient(serviceUri, credentials))
                //             {
                //                 cpars.Members = await client.Conversations.GetActivityMembersAsync(Configuration.SkypeConversationId, context.Activity.Id);
                //                 await client.Conversations.SendToConversationAsync(Configuration.SkypeConversationId, (Activity)activity);
                //             }
                //         }
                //         await context.SendActivity($"Notifications sent!");
                //     } catch (System.Exception ex) {
                //         await context.SendActivity(ex.ToString());
                //     }
                // }
                // else if (messageText.ToLower().StartsWith("stats"))
                // {
                //     await context.SendActivity(context.Adapter.GetType().ToString());
                //     var creds = new Microsoft.Bot.Connector.Authentication.MicrosoftAppCredentials(Configuration.AppId, Configuration.AppPassword);
                //     var channelData = new Microsoft.Bot.Connector.Teams.Models.TeamsChannelData {
                //         Channel = new Microsoft.Bot.Connector.Teams.Models.ChannelInfo("19:1a74c4717a5a4e559a2964f6199e5fdd@thread.skype")
                //     };
                //     IMessageActivity newMessage = Activity.CreateMessageActivity();
                //     newMessage.Type = ActivityTypes.Message;
                //     newMessage.Text = "Hello, on a new thread";
                //     var cpars = new ConversationParameters(
                //         isGroup: true,
                //         bot: null,
                //         members: null,
                //         topicName: "Test Conversation",
                //         activity: (Activity)newMessage,
                //         channelData: channelData
                //     );
                //     try {
                //         await (context.Adapter as Microsoft.Bot.Builder.Adapters.BotFrameworkAdapter).CreateConversation(
                //             context.Activity.Conversation.Id, context.Activity.ServiceUrl, creds, cpars, (newContext) => {
                //                 return newContext.SendActivity("New activity!");
                //             });
                //     } catch(System.Exception e) {
                //         await context.SendActivity(e.ToString());
                //     }
                //     string baseUrl = $"{context.Activity.ServiceUrl}v3/conversations/19:1a74c4717a5a4e559a2964f6199e5fdd@thread.skype/members";
                //     //The 'using' will help to prevent memory leaks.
                //     //Create a new instance of HttpClient
                //     // TODO we should fetch it
                //     string myToken = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjdfWnVmMXR2a3dMeFlhSFMzcTZsVWpVWUlHdyIsImtpZCI6IjdfWnVmMXR2a3dMeFlhSFMzcTZsVWpVWUlHdyJ9.eyJhdWQiOiJodHRwczovL2FwaS5ib3RmcmFtZXdvcmsuY29tIiwiaXNzIjoiaHR0cHM6Ly9zdHMud2luZG93cy5uZXQvZDZkNDk0MjAtZjM5Yi00ZGY3LWExZGMtZDU5YTkzNTg3MWRiLyIsImlhdCI6MTUzMjQzNDM3NCwibmJmIjoxNTMyNDM0Mzc0LCJleHAiOjE1MzI0MzgyNzQsImFpbyI6IjQyQmdZQWhOV01WNVlKZkkzZFhuZHE4UG1GSWtBUUE9IiwiYXBwaWQiOiIzNmE5MzliYi03NzQzLTRjYTgtOTZjMy1jZjQzODRkZTI2YjMiLCJhcHBpZGFjciI6IjEiLCJpZHAiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9kNmQ0OTQyMC1mMzliLTRkZjctYTFkYy1kNTlhOTM1ODcxZGIvIiwidGlkIjoiZDZkNDk0MjAtZjM5Yi00ZGY3LWExZGMtZDU5YTkzNTg3MWRiIiwidXRpIjoiMUl2cG1xV2lza3Fpc0hFakdDTUtBQSIsInZlciI6IjEuMCJ9.Eq1VYici_xt8ReiCnyCjx83t7o6VKij7eLpIwYLdFFsQ9lmAdOw5qNqb8w5JvNGYLM1wZsM8QZ9HVshr0PLfXC3wKlTnI9myiAK8JhurGg3wj2iaRXOxFgtIZu-g617GWEt3faB8q4nQfbJm9T1p3kc0raT0J-Clif1ETcCeGsGFVPVEdv4Qghge107BlRF83dLffFD07gtGG2uCMlivaxHuVAf4pTAlOcDf3XvrH0AHbzyj1IB7n7MgFvjCkigaovg82QNxXhWJAl8TBYSsaYIZeQ6_ersXk9uctIjSmt_TkYOgc1XdD-5e-jn9ZzT_VEI1H--mAwc3JxTxNfwWvw";
                //     using (HttpClient client = new HttpClient()) {
                //     //Setting up the response...         
                //         client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", myToken);
                //         using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                //         {
                //             using (HttpContent content = res.Content)
                //             {
                //                 string data = await content.ReadAsStringAsync();
                //                 if (data != null)
                //                 {
                //                     await context.SendActivity(data);
                //                 }
                //             }
                //         }
                //     }

                //     await context.SendActivity($"Service URL: {context.Activity.ServiceUrl}\nConversation ID: {context.Activity.Conversation.Id}");
                // }
=======
                else if (messageText.ToLower().StartsWith("cancel")) {
                    state.luisConversationState = LuisConversationState.ANALYZE_REQUEST;
                    await context.SendActivity($"Processing cancelled. If you want to start a group event, please send me a message with activity, place and time.");
                }
>>>>>>> 492053f... fix
                else if (state.luisConversationState == LuisConversationState.ANALYZE_REQUEST)
>>>>>>> 3365f6c... Help message, groupmeet
                {
<<<<<<< HEAD
                    NLPUtils nlpUtils = new NLPUtils();
                    var toDetect = messageText.Substring("lang".Length + 1);
                    await context.SendActivity($"Turn {state.TurnCount}: {nlpUtils.DetectLanguage(toDetect)}");
=======
                    LuisResponse response = await connector.MakeRequest(messageText);
                    if (response.topScoringIntent.intent == LuisConnector.CREATE_GROUP_MEET)
                    {
                        string first_reply = connector.GetFirstAnswer(response, out state.detectedInformation);
                        state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                        await context.SendActivity($"{first_reply}");
=======
                                        var activity2 = MessageFactory.Text("Hello ");
                                        activity2.AddMentionToText(pair.Value, MentionTextLocation.AppendText, pair.Value.Name);
                                        activity2.Text += "! Your buddy for this session is ";
                                        activity2.AddMentionToText(pair.Key, MentionTextLocation.AppendText, pair.Key.Name);
                                        activity2.Text += "! Have fun!";
                                        await client.Conversations.SendToConversationWithHttpMessagesAsync(conversationResponse.Id, activity2);
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
                    else if (messageText.ToLower().StartsWith("cancel")) {
                        state.luisConversationState = LuisConversationState.ANALYZE_REQUEST;
                        await context.SendActivity($"Processing cancelled. If you want to start a group event, please send me a message with activity, place and time.");
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
=======
                                        var activity2 = MessageFactory.Text("Hello ");
                                        activity2.AddMentionToText(pair.Value, MentionTextLocation.AppendText, pair.Value.Name);
                                        activity2.Text += "! Your buddy for this session is ";
                                        activity2.AddMentionToText(pair.Key, MentionTextLocation.AppendText, pair.Key.Name);
                                        activity2.Text += "! Have fun!";
                                        await client.Conversations.SendToConversationWithHttpMessagesAsync(conversationResponse.Id, activity2);
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
                    else if (messageText.ToLower().StartsWith("cancel")) {
                        state.luisConversationState = LuisConversationState.ANALYZE_REQUEST;
                        await context.SendActivity($"Processing cancelled. If you want to start a group event, please send me a message with activity, place and time.");
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
                    }
                    else {
                        // Try to analyse user message with Natural Language Processing  
                        LuisResponse response = await connector.MakeRequest(messageText);

<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> cf8364d... minor fixes
                }
>>>>>>> 4db4523... print conversation id
=======
                else if (state.luisConversationState == LuisConversationState.ANALYZE_FIRST_REQUEST)   {
                    LuisResponse response = await connector.MakeRequest(messageText);
                    if (response.topScoringIntent.intent == LuisConnector.CREATE_GROUP_MEET)
=======

                        await context.SendActivity($"Service URL: {context.Activity.ServiceUrl}\nConversation ID: {context.Activity.Conversation.Id}");
                    }
<<<<<<< HEAD
                    else if (state.luisConversationState == LuisConversationState.ANALYZE_FIRST_REQUEST)
>>>>>>> 552f612... work on natural conversation with bot - meeting place detected correctly
=======
                    else if (state.luisConversationState == LuisConversationState.ANALYZE_REQUEST)
>>>>>>> 46aa8db... work on nlp, simple conversation works
                    {
                        LuisResponse response = await connector.MakeRequest(messageText);
                        if (response.topScoringIntent.intent == LuisConnector.CREATE_GROUP_MEET)
                        {
                            string first_reply = connector.GetFirstAnswer(response, out state.detectedInformation);
                            state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                            await context.SendActivity($"{first_reply}");
                        }
                        else await context.SendActivity($"I did not understood... If you want to create group meet, please tell me.");

                    }
                    else if (state.luisConversationState == LuisConversationState.CONFIRM_REQUEST)
                    {
                        LuisResponse response = await connector.MakeRequest(messageText);

                        if (response.topScoringIntent.intent == LuisConnector.CONFIRM)
                        {
                            string message = "OK. ";
                            if (state.detectedInformation.Activity == null)
                            {
                                message += " I don't know what you want to do, yet.\n";
                                state.luisConversationState = LuisConversationState.ASK_ACTIVITY;
                            }
                            else if (state.detectedInformation.WhereToGo == null)
                            {
                                message += " I don't know where you want to go, yet.\n";
                                state.luisConversationState = LuisConversationState.ASK_PLACE;
                            }
                            else if(state.detectedInformation.WhenToMeet == null)
                            {
                                message += " I don't know when you want to meet, yet.\n";
                                state.luisConversationState = LuisConversationState.ASK_TIME;
                            }
                            else if (state.detectedInformation.MaxNumberOfPeople == 0)
                            {
                                message += " Please, tell me how many people can join your event?\n";
                                state.luisConversationState = LuisConversationState.ASK_PEOPLE;
                            }
                            else if (state.detectedInformation.WhereToMeet == null)
                            {
                                message += " Great! Only one thing left to decide. Where do you want to meet?\n";
                                state.luisConversationState = LuisConversationState.ASK_MEETING_PLACE;
                            }
                            else {
                                message += " I have all the needed data. I am creating the event.\n";
                                state.luisConversationState = LuisConversationState.ANALYZE_REQUEST;
                            }

                            await context.SendActivity($"{message}");
                        }
                        else
                        {
<<<<<<< HEAD
=======
                            message += " Great! Only one thing left to decide. Where do you want to meet?\n";
                            state.luisConversationState = LuisConversationState.ASK_MEETING_PLACE;
                        }
                        else {
<<<<<<< HEAD
                            message += " I have all the needed data. (party) I am creating the event.\n";
>>>>>>> f2f8697... typo and smileys
=======
                            message += " I have all the needed data. \U0001F389 I am creating the event.\n";
>>>>>>> 694919d... text
                            state.luisConversationState = LuisConversationState.ANALYZE_REQUEST;
<<<<<<< HEAD
                            await context.SendActivity($"I am really sorry. Can you give me your data again?");
=======
                            SendEventToTeams(context, state.detectedInformation);
                        }
>>>>>>> a585e37... teams luis

                        }
                    }
<<<<<<< HEAD
                    else if (state.luisConversationState == LuisConversationState.ASK_ACTIVITY)
=======
                    else if (response.topScoringIntent.intent == LuisConnector.CANCEL)
                    {
                        state.luisConversationState = LuisConversationState.ANALYZE_REQUEST;
                        await context.SendActivity($"OK. I cancel the process of creating event. If you want to start it again, please send me a message with activity, place and time.");
                    }
                    else
>>>>>>> 58f34fa... canceling added
                    {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
                        LuisResponse response = await connector.MakeRequest(messageText);
<<<<<<< HEAD
<<<<<<< HEAD

<<<<<<< HEAD
<<<<<<< HEAD
>>>>>>> 4311936... work on natural conversation with bot
=======
=======
                        await context.SendActivity($"I am sorry, I didn't get it. Can you give me your data again?");
=======
                        await context.SendActivity($"I am sorry, I didn't get it. (sad) Can you give me your data again?");
>>>>>>> f2f8697... typo and smileys
=======
                        await context.SendActivity($"I am sorry, I didn't get it. \U0001F613 Can you give me your data again?");
>>>>>>> 694919d... text

>>>>>>> cf8364d... minor fixes
                    }
                }
<<<<<<< HEAD
>>>>>>> d09bc85... work on natural conversation with bot
                else // Echo back to the user whatever they typed.
                    await context.SendActivity($"Turn {state.TurnCount}: You sent '{context.Activity.Text}'");
=======

=======
                        string message = connector.getActivity(response, ref state.detectedInformation, ref state.luisConversationState);
=======
                        string message = connector.GetActivity(response, ref state.detectedInformation, ref state.luisConversationState);
>>>>>>> cc2327c... all required data are extracted
                        await context.SendActivity($"{message}");
>>>>>>> 46aa8db... work on nlp, simple conversation works
                    }
                    else if (state.luisConversationState == LuisConversationState.ASK_PLACE)
                    {
                        LuisResponse response = await connector.MakeRequest(messageText);
                        string message = connector.GetPlace(response, ref state.detectedInformation, ref state.luisConversationState);
                        await context.SendActivity($"{message}");
                    }
                    else if (state.luisConversationState == LuisConversationState.GET_PLACE_AS_MESSAGE)
                    {
                        state.detectedInformation.WhereToGo = messageText;
                        state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                        await context.SendActivity($"I set place to {messageText}");
                    }
                    else if (state.luisConversationState == LuisConversationState.GET_ACTIVITY_AS_MESSAGE)
                    {
                        state.detectedInformation.Activity = messageText;
                        state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                        await context.SendActivity($"I set activity to {messageText}");
                    }
                    else if (state.luisConversationState == LuisConversationState.ASK_TIME)
                    {
                        LuisResponse response = await connector.MakeRequest(messageText);
                        string message = connector.GetTime(response, ref state.detectedInformation, ref state.luisConversationState);
                        await context.SendActivity($"{message}");
                    }
                    else if (state.luisConversationState == LuisConversationState.ASK_PEOPLE)
                    {
                        LuisResponse response = await connector.MakeRequest(messageText);
                        string message = connector.GetPeople(response, ref state.detectedInformation, ref state.luisConversationState);
                        await context.SendActivity($"{message}");
                    }
                    else if (state.luisConversationState == LuisConversationState.ASK_MEETING_PLACE)
                    {
                        LuisResponse response = await connector.MakeRequest(messageText);
                        string message = connector.GetMeetingPlace(response, ref state.detectedInformation, ref state.luisConversationState);
                        await context.SendActivity($"{message}");
                    }
                    else if (state.luisConversationState == LuisConversationState.GET_MEETING_PLACE_AS_MESSAGE)
                    {
                        state.detectedInformation.WhereToMeet = messageText;
                        state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                        await context.SendActivity($"I set place to {messageText}");
                    }
                    else // Echo back to the user whatever they typed.
                        await context.SendActivity($"Turn {state.TurnCount}: You sent '{context.Activity.Text}'");
=======
                else if (state.luisConversationState == LuisConversationState.ASK_ACTIVITY)
                {
                    LuisResponse response = await connector.MakeRequest(messageText);
                    string message = connector.GetActivity(response, ref state.detectedInformation, ref state.luisConversationState);
                    await context.SendActivity($"{message}");
                }
                else if (state.luisConversationState == LuisConversationState.ASK_PLACE)
                {
                    LuisResponse response = await connector.MakeRequest(messageText);
                    string message = connector.GetPlace(response, ref state.detectedInformation, ref state.luisConversationState);
                    await context.SendActivity($"{message}");
                }
                else if (state.luisConversationState == LuisConversationState.GET_PLACE_AS_MESSAGE)
                {
                    state.detectedInformation.WhereToGo = messageText;
                    state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                    await context.SendActivity($"I set place to {messageText}");
                }
                else if (state.luisConversationState == LuisConversationState.GET_ACTIVITY_AS_MESSAGE)
                {
                    state.detectedInformation.Activity = messageText;
                    state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                    await context.SendActivity($"I set activity to {messageText}");
                }
                else if (state.luisConversationState == LuisConversationState.ASK_TIME)
                {
                    LuisResponse response = await connector.MakeRequest(messageText);
                    string message = connector.GetTime(response, ref state.detectedInformation, ref state.luisConversationState);
                    await context.SendActivity($"{message}");
                }
                else if (state.luisConversationState == LuisConversationState.ASK_PEOPLE)
                {
                    LuisResponse response = await connector.MakeRequest(messageText);
                    string message = connector.GetPeople(response, ref state.detectedInformation, ref state.luisConversationState);
                    await context.SendActivity($"{message}");
                }
                else if (state.luisConversationState == LuisConversationState.ASK_MEETING_PLACE)
                {
                    LuisResponse response = await connector.MakeRequest(messageText);
                    string message = connector.GetMeetingPlace(response, ref state.detectedInformation, ref state.luisConversationState);
                    await context.SendActivity($"{message}");
                }
                else if (state.luisConversationState == LuisConversationState.GET_MEETING_PLACE_AS_MESSAGE)
                {
                    state.detectedInformation.WhereToMeet = messageText;
                    state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                    await context.SendActivity($"I set place to {messageText}");
                }
            else // Echo back to the user whatever they typed.
                await context.SendActivity($"Sorry, no idea how to help you. You sent '{context.Activity.Text}'");
>>>>>>> 694919d... text
=======
                        // Check if with high probability user want to cancel the request 
                        if (response.topScoringIntent.intent == LuisConnector.CANCEL && response.topScoringIntent.score > 0.40)
                        {
                            state.luisConversationState = LuisConversationState.ANALYZE_REQUEST;
                            await context.SendActivity($"OK. I cancel the process of creating event. If you want to start it again, please send me a message with activity, place and time.");
                        }

                        string message;

                        // Check the conversation state
                        switch (state.luisConversationState) {
                            case LuisConversationState.ANALYZE_REQUEST:
                                if (response.topScoringIntent.intent == LuisConnector.CREATE_GROUP_MEET)
                                {
                                    string first_reply = connector.GetFirstAnswer(response, out state.detectedInformation);
                                    state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                                    await context.SendActivity($"{first_reply}");
                                }
                                else await context.SendActivity($"I did not understand... \U0001F613 If you want to create group meet, please send me a message with activity, place and time.");
                                break;
                            case LuisConversationState.ASK_ACTIVITY:
                                message = connector.GetActivity(response, ref state.detectedInformation, ref state.luisConversationState);
                                await context.SendActivity($"{message}");
                                break;
                            case LuisConversationState.ASK_PLACE:
                                message = connector.GetPlace(response, ref state.detectedInformation, ref state.luisConversationState);
                                await context.SendActivity($"{message}");
                                break;
                            case LuisConversationState.GET_PLACE_AS_MESSAGE:
                                state.detectedInformation.WhereToGo = messageText;
                                state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                                await context.SendActivity($"I set place to {messageText}");
                                break;
                            case LuisConversationState.ASK_TIME:
                                message = connector.GetTime(response, ref state.detectedInformation, ref state.luisConversationState);
                                await context.SendActivity($"{message}");
                                break;
                            case LuisConversationState.ASK_PEOPLE:
                                message = connector.GetPeople(response, ref state.detectedInformation, ref state.luisConversationState);
                                await context.SendActivity($"{message}");
                                break;
                            case LuisConversationState.ASK_MEETING_PLACE:
                                message = connector.GetMeetingPlace(response, ref state.detectedInformation, ref state.luisConversationState);
                                await context.SendActivity($"{message}");
                                break;
                            case LuisConversationState.GET_MEETING_PLACE_AS_MESSAGE:
                                state.detectedInformation.WhereToMeet = messageText;
                                state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                                await context.SendActivity($"I set place to {messageText}");
                                break;
                            default:
                                // Check cancel again
                                state.luisConversationState = LuisConversationState.ANALYZE_REQUEST;
                                if (response.topScoringIntent.intent == LuisConnector.CANCEL)                             
                                   await context.SendActivity($"OK. I cancel the process of creating event. If you want to start it again, please send me a message with activity, place and time.");
                                else
                                   await context.SendActivity($"Sorry, I did not understand... Please try to create the event again");

                                break;
                        }

                        if (state.luisConversationState == LuisConversationState.CONFIRM_REQUEST)
                        {
                            var checkStateResp = connector.CheckState(state.detectedInformation);
                            state.luisConversationState = checkStateResp.Item1;

                            if (state.luisConversationState.Equals(LuisConversationState.ANALYZE_REQUEST))
                            {
                                SendEventToTeams(context, state.detectedInformation);
                            }

                            await context.SendActivity($"{checkStateResp.Item2}");
                        }
                    }
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
=======
                        // Check if with high probability user want to cancel the request 
                        if (response.topScoringIntent.intent == LuisConnector.CANCEL && response.topScoringIntent.score > 0.40)
                        {
                            state.luisConversationState = LuisConversationState.ANALYZE_REQUEST;
                            await context.SendActivity($"OK. I cancel the process of creating event. If you want to start it again, please send me a message with activity, place and time.");
                        }

                        string message;

                        // Check the conversation state
                        switch (state.luisConversationState) {
                            case LuisConversationState.ANALYZE_REQUEST:
                                if (response.topScoringIntent.intent == LuisConnector.CREATE_GROUP_MEET)
                                {
                                    string first_reply = connector.GetFirstAnswer(response, out state.detectedInformation);
                                    state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                                    await context.SendActivity($"{first_reply}");
                                }
                                else await context.SendActivity($"I did not understand... \U0001F613 If you want to create group meet, please send me a message with activity, place and time.");
                                break;
                            case LuisConversationState.ASK_ACTIVITY:
                                message = connector.GetActivity(response, ref state.detectedInformation, ref state.luisConversationState);
                                await context.SendActivity($"{message}");
                                break;
                            case LuisConversationState.ASK_PLACE:
                                message = connector.GetPlace(response, ref state.detectedInformation, ref state.luisConversationState);
                                await context.SendActivity($"{message}");
                                break;
                            case LuisConversationState.GET_PLACE_AS_MESSAGE:
                                state.detectedInformation.WhereToGo = messageText;
                                state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                                await context.SendActivity($"I set place to {messageText}");
                                break;
                            case LuisConversationState.ASK_TIME:
                                message = connector.GetTime(response, ref state.detectedInformation, ref state.luisConversationState);
                                await context.SendActivity($"{message}");
                                break;
                            case LuisConversationState.ASK_PEOPLE:
                                message = connector.GetPeople(response, ref state.detectedInformation, ref state.luisConversationState);
                                await context.SendActivity($"{message}");
                                break;
                            case LuisConversationState.ASK_MEETING_PLACE:
                                message = connector.GetMeetingPlace(response, ref state.detectedInformation, ref state.luisConversationState);
                                await context.SendActivity($"{message}");
                                break;
                            case LuisConversationState.GET_MEETING_PLACE_AS_MESSAGE:
                                state.detectedInformation.WhereToMeet = messageText;
                                state.luisConversationState = LuisConversationState.CONFIRM_REQUEST;
                                await context.SendActivity($"I set place to {messageText}");
                                break;
                            default:
                                // Check cancel again
                                state.luisConversationState = LuisConversationState.ANALYZE_REQUEST;
                                if (response.topScoringIntent.intent == LuisConnector.CANCEL)                             
                                   await context.SendActivity($"OK. I cancel the process of creating event. If you want to start it again, please send me a message with activity, place and time.");
                                else
                                   await context.SendActivity($"Sorry, I did not understand... Please try to create the event again");

                                break;
                        }

                        if (state.luisConversationState == LuisConversationState.CONFIRM_REQUEST)
                        {
                            var checkStateResp = connector.CheckState(state.detectedInformation);
                            state.luisConversationState = checkStateResp.Item1;

                            if (state.luisConversationState.Equals(LuisConversationState.ANALYZE_REQUEST))
                            {
                                SendEventToTeams(context, state.detectedInformation);
                            }

                            await context.SendActivity($"{checkStateResp.Item2}");
                        }
                    }
>>>>>>> c6f2d3c... making NLP conversations quicker, some NLP refactor
            } else if (context.Activity.Type == ActivityTypes.ConversationUpdate) {
                
>>>>>>> 552f612... work on natural conversation with bot - meeting place detected correctly
            }
            } catch (System.Exception e) {
                System.Console.WriteLine(e.ToString());
            }
        }

        private HeroCard CreateCard(EventCardData cardData) {
            var attendees = new System.Text.StringBuilder();
            if (cardData.Attendees != null && cardData.Attendees.Count > 0) {
                attendees.Append("<ul>");
                foreach (var user in cardData.Attendees) {
                    attendees.Append($"<li>{user.Name}</li>");
                }
                attendees.Append("</ul>");
            } else {
                attendees.Append("<em>There are no attendees.</em>");
            }
            var attendeesText = "Attendees";
            if (cardData.EventData.MaxNumberOfPeople > 0 && cardData.EventData.MaxNumberOfPeople < Int32.MaxValue) {
                attendeesText += $" ({cardData.Attendees.Count} / {cardData.EventData.MaxNumberOfPeople})";
            } else if (cardData.Attendees.Count > 0) {
                attendeesText += $" ({cardData.Attendees.Count})";
            }
            attendeesText += ":";
            var card = new HeroCard
            {
                Title = $"{cardData.EventData.Activity} {cardData.EventData.WhereToGo}",
                Subtitle = $"We meet at {cardData.EventData.WhereToMeet} at {cardData.EventData.WhenToMeet}.",
                Text = $"<strong>{attendeesText}</strong>\n{attendees.ToString()}",
                Buttons = new List<CardAction> { new CardAction(ActionTypes.MessageBack, "Join / Leave", value: JsonConvert.SerializeObject(cardData), text: "joinleave", displayText: "") }
            };
            return card;
        }

        private Activity CreateActivityForEvent(ITurnContext context, EventCardData cardData) {
            var activity = context.Activity.CreateReply();
            activity.Attachments.Add(CreateCard(cardData).ToAttachment());
            if (!string.IsNullOrEmpty(cardData.ResourceResponseId)) {
                activity.Id = cardData.ResourceResponseId;
            }
            return activity;
        }

        private async void SendEventToTeams(ITurnContext context, OneMeetEvent newEvent) {
            var cardData = new EventCardData {
                ResourceResponseId = null,
                Attendees = new List<ChannelAccount>{ context.Activity.From },
                EventData = newEvent
            };
            var activity = CreateActivityForEvent(context, cardData);

            if (Configuration.TeamsConversationId != null)
            {
                var serviceUri = new System.Uri(context.Activity.ServiceUrl, System.UriKind.Absolute);
                using (var client = new Microsoft.Bot.Connector.ConnectorClient(serviceUri, credentials))
                {
                    activity.Conversation.Id = Configuration.TeamsConversationId;
                    var resource = await client.Conversations.SendToConversationAsync(activity);
                    cardData.ResourceResponseId = resource.Id;
                    activity.Id = resource.Id;
                    activity.Attachments[0] = CreateCard(cardData).ToAttachment();
                    await client.Conversations.UpdateActivityAsync(activity);
                }
                await context.SendActivity("I have notified the channel! \U0001F389");
            } else {
                await context.SendActivity("Channel / group is not initialized. Type <em>here @OneMeet 365</em> in a channel!");
            }
        }
    }    
}
