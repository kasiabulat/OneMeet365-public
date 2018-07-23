# OneMeet 365 - overview

Microsoft Teams (and originally Skype) bot for organizing events (GroupMeet functionality) and dividing channel/group members into randomly chosen pairs (PairMeet functionality).

![alt text](https://github.com/kasiabulat/OneMeet365/blob/master/images/logo.png "Project logo")

The idea of project and its first version was created during Microsoft Hackaton 2018 by the group of Microsoft Prague interns. Our main intention was to create the bot for organizing events to encourage employees to meet one each other and exchange experience. Hackaton challange we choose was "Inclusive Microsoft" - to create something that continues to build an inclusive culture and productive work environment for all.

![alt text](https://github.com/kasiabulat/OneMeet365/blob/master/images/authors.jpg "OneMeet hackers")

OneMeet365 hackers (from left on the photo): Pavel Lučivňák, Tomáš Iser, Katarzyna Bułat, Bartosz Sumper, Liza Babu, Tomasz Kotarski.

# Main functionalities
## GroupMeet

It gives users the possibility to create events others will be easily able to join. By either talking to bot in natural language or using the direct command, it creates the card in channel with important information like: activity, place and time of meeting and maximum number of attendees. Other channel members can click join/leave button to participate (before the event begins). Event creator can cancel the appointment. It is quicker and more effective then having long conversations discussing every detail.

## PairMeet

It divides channel members into randomly chosen pairs and notifies the users who their pair is. It encourages users to get to know each other, talk to new people and exchange experience.

# Used technologies

## Bot framework

We started our project basing on EchoBot sample from Microsoft Bot Framework.

https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0

For local debugging purposes we use bot framework emulator:

https://docs.microsoft.com/en-us/azure/bot-service/bot-service-debug-emulator?view=azure-bot-service-4.0

## Natural Language Processing

Requests for creating events can be send to bot in natural language. We are using LUIS (Language Understanding Intelligent Service) from Microsoft Cognitive Services.

Our app is registered at https://eu.luis.ai/ (we use west Europe endpoint, for non-Europe endpoints app should be registered at https://luis.ai).

LUIS documentation can be found there: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/what-is-luis

## Azure

We use the following resources in Microsoft Cloud (https://ms.portal.azure.com/):
* Cognitive Services for LUIS (NLP)
* Web App Bot synchronized with github repository for continuous development
* App Service exposing the bot
* SQL database to store information about created events

## .NET Core

Our project is using muliti-platform .NET Core. For more information visit the page: https://docs.microsoft.com/en-us/dotnet/core/.

## Entity framework

We use Entity Framework Core (https://docs.microsoft.com/en-us/ef/core/) to conviniently manage the database. Currently we store our data in Azure, but the database connection string can be easily changed in appsettings.json to different one. 

# Guidelines for developers

## Project structure

The core of the project is located in OneMeetBot class divided into two files: OneMeetBot.cs and OneMeetBotHelperFunctions.cs. 

Database, IDatabase and EventsContext are handling entity framework and database connection.

Under the LUIS directory, there are classes responsible for Natural Language Processing part. 

All dependencies are injected in Startup (useful article about dependency injection in ASP .NET can be found there: https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1).

## Configuration

Our configuration files include:

1. General: Configuration.cs
2. NLP: LUIS/Configuration.cs
3. Database: appsettings.json

## Hacks (we are not proud of them, but there are some)
1. There's a class FixedExtensions in OneMeetBotHelperFunctions.cs. It is the wrapper over Microsoft Teams Connector which is compatible with Bot SDK v4 Preview.
2. We have quite ugly workaround for the problem of deleting bot name from the message sent in channel. It would be good to investigate it.
3. To store information about the event we need to know its card resource id. It is returned from the function sending the card, but we want to store it inside this card as well. Our solution is to update the card with resource id immediately after creation. 

## Registering application

Currently, our project works only on one MS Teams channel (it can be improved, but then user would need to specify which channel he/she refers to in the conversation with bot). To use it in a new channel all Azure resources need to be created once again. 

To deploy the bot follow these instuctions (you will need a valid non-trial Azure subscription):

1. Create a Web App Bot (Azure portal -> Create a resource -> Web app bot)

* In "bot template" choose SDK v4 (Preview) and language C#. It's important, despite code being overwritten by continuous deployment.  
* Generate Microsoft App ID and password. Replace the onces from Configuration.cs with them.
* After creating the resource, navigate to your Web App Bot -> Build -> Continuous deployment from source control and set it up. Each time you push code to chosen branch, Azure bot will be redeployed.
* This step will create App Service as well.

2. Create SQL database (Azure portal -> Create a resource -> SQL database)

* You will need to create a new server as well. Remember its name, admin login and password.
* For testing purposes, after creating the resource, go to database Firewall settings (Overview -> Set server firewall) and add your IP.
* Copy connection string (Overview -> Show database connection strings) to appsettings.json. Replace {User ID} with selected admin login and {your_password} with admin password.


Useful materials:

https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started-portal

https://docs.microsoft.com/en-us/azure/sql-database/sql-database-get-started-portal-firewall

3. Add Microsoft Teams integration:
* In Azure Portal -> Web App Bot -> Channels, add Microsoft Teams channel.
* In MS Teams, navigate to More apps (three dots) -> App Studio -> Manifest editor -> Create a new app
    * App ID in Details -> App details -> Identification is Teams bot ID, not Azure app ID, you can click generate button.
    * As a branding image, you can use images/logo.png from the repository.
    * In bots tab, set up the new one, here as Microsoft App ID type the Azure one. Add both personal and team scope.

(4.) If you want to improve NLP, you would probably need to create new LUIS app at luis.ai from scratch and create Language Understanding resource in Azure. 

1. If you need Europe endpoint use eu.luis.ai, otherwise luis.ai. 
2. Register using Microsoft account.
3. You can get through this tutorial: https://docs.microsoft.com/en-us/azure/cognitive-services/luis/luis-get-started-create-app to get an overview of how LUIS works.
4. Create new application you will refer to in code. After it is ready, remember to add key for chosen chosen region and change settings in LUIS.Configuration appropriately.
5. Restore the current state of application:
* Currently three utilities intets are used: Utilities.Cancel, Utilities.Confirm and Utilities.Help.
* Apart from them we created intents:
    * Greet - with utterances for greeting bot and starting conversation
    * CreateGroupMeet - with utterances for organizing events like "I would like to go for a pizza today", "Please, create a lunch event at 2 pm tomorrow in Grosseto", "I plan to go swimming near Vltava river"
    * AddPeople - with utterances for defining attendees limit like "I want to set the limit to six people", "Let it be 3 people", "I want max 4 people to join me"
    * AddPlace - with utterances for defining place like "I want to go to Baifu", "Hub 6th floor, please"
    * AddTime - with utterances for defining time like "Set it to 28th September", "I want to meet at twelve", "Let it be 6 pm"
    * There should be also None intent for messages bot is not intended to handle like "check the wheather"
* We use the following built in entities: datetimeV2 (for matching event date and time) and number (for matching people limit).
* We created the entities: activity and place with values recognized from phrase lists.

# Future work

## Known issues

1. Using current implementation of NLP, bot is able to only get three information from the first message: activity, event place and meeting time. Bot always asks then for the number of people joining and meeting place (and other information if missing in the first message). It would be more convinient for users if bot would be able to distinguish between meeting and event place in the first message, as well as between time and number of people.
2. We failed to implement Skype PairMeet functionality, because it seems that on Skype we cannot get the list of group members' names, only their ids. Maybe there's a workaround for this problem.
3. We needed to use some hacks (see: Guidelines for developers)

## Ideas for improvements

1. Test, fix and deploy OneMeet365 version for Skype.
2. Add the possibility to attach images to event card.
3. Alert users when event is 
about to start (AlarmBot example may be useful: https://github.com/Microsoft/BotBuilder/tree/master/CSharp/Samples/AlarmBot, time zones need to be handled properly)

Other usefull materialas:

https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Samples/SimpleAlarmBot/

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.1

4. Improve NLP, so that it learns from users messages (for example it adds place and activity typed by user to phrase list)









