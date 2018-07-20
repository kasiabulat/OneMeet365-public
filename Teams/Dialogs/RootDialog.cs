using System;
using System.Threading.Tasks;
using Common;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace Teams.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text == "help")
            {
                Commands.Help().ForEach(async l => await context.PostAsync(l));
            }
            else
            {
                // Calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // Return our reply to the user
                await context.PostAsync($"You sent {activity.Text} which was {length} characters");
            }

            context.Wait(MessageReceivedAsync);
        }
    }
}