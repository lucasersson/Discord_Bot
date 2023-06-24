# Discord_Bot
A hobby project of mine that integrates with OpenAi endpoints through the chat-box on discord. Written in .Net 7

If you want to try out the bot for yourself there are 3 prerequisites, having a Discord account, administrative access to a Discord server, and a valid token on OpenAI.

1. Create your Discord application on the [Discord Developer Portal](https://discord.com/developers/applications).  
Create an application and name it to your liking. Click on the 'Bot' tab and then 'Reset Token' button to get your application token.Save this token to your environment variables with the name 'DISCORD_TOKEN'.
Then, press the 'OAuth2' tab and select 'URL Generator'. For scopes, select 'bot'. In the permissions selection menu, you can select Administrator if you want, but the bot needs at the very minimum permissions for 'Send Messages', and 'Use Slash Commands'.
Copy the generated URL, and paste it in your browser. Select the server in which you want the bot to join.

2. Install .Net SDK  
To use the bot, you need to install the [.Net 7 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/7.0) for windows, linux, or macOS.

3. Add OpenAI token to environment variables.  
Create a new account or use an existing for OpenAI's API. Copy the token and add in in your environment variables under the name 'OPENAIAPI_TOKEN'.

4. Start the bot  
Start the bot by cloning the repository and then launching the console application using your ide or running the command 'dotnet run --project DiscordBot' from the repositorys root directory.

5. Try commands
The commands should be registered to your discord server pretty quickly, but in some cases it can take up to 1-2 hours before you can see the commands. When registered, the commands should show up under the group name 'openai'. Enjoy!

