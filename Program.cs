using System;
using System.Linq;
using Discord;
using Discord.Commands;
using discordapp.Classes;
using Discord.WebSocket;
using discordapp.Properties;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace discordapp
{
    internal sealed class Program
    {
        private static void Main()=>new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient _dc;
        public CommandService Comms = new CommandService();
        private ModuleInfo _info;
        private readonly char _prefix = '<';
        private IServiceProvider _services;

        private async Task MainAsync()
        {
            _dc = new DiscordSocketClient();
            await _dc.LoginAsync(TokenType.Bot, Settings1.Default.TOKEN);
            await _dc.StartAsync();

            _services = new ServiceCollection().AddSingleton(Comms).AddSingleton(_dc).BuildServiceProvider();
            _info = await Comms.AddModuleAsync<ChatCommands>();
            _dc.MessageReceived += HandleCommandAsync;
            await Task.Delay(-1);

        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            int argPos = 0;
            // Don't process the command if it was a System Message
            if (!(messageParam is SocketUserMessage message) || message.Author.Id == _dc.CurrentUser.Id)  return;

            //Don't process the command if it was not prefixed or mentioned me.
            if (!(message.HasCharPrefix(_prefix, ref argPos)) || message.Content[1] == '@') return;

            var context = new SocketCommandContext(_dc, message);
            
            if (message.Content == $"{_prefix}help")
            {
                //Basicly, it gets all the parameters seperated by commas, but cute, isnt it?
                var parameters = _info.Commands.Select(x => string.Join(",", x.Parameters.Select(y => y.Summary.Replace(" ", "_")))); 

                await context.Channel.SendMessageAsync(string.Join("\n\n",
                    _info.Commands.Select(x => $"{x.Name} {parameters} = {x.Summary}")));
                return;
            }
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await Comms.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
            {
                if (result.Error == CommandError.UnknownCommand)
                {
                    var seed = "IDK what that is.";
                    AskCeyd_A(message.Content.Substring(1),context,seed);
                }
                else await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        public static void AskCeyd_A(string content,SocketCommandContext context,string seed)
        {
            #region Sensitive Information
            const string USERNAME = "<USERNAME HERE>";
            const string TOKEN = "<TOKEN HERE>";
            const string ADDRESS = "<CEYD-A API ADDRESS HERE>";
            #endregion

            using (var wc = new WebClient())
            {
                var values = new NameValueCollection()
                {
                    {"username",USERNAME },
                    {"token",TOKEN },
                    {"code",content },
                    {"type","text" }
                };
                var answer = Encoding.UTF8.GetString(wc.UploadValues(ADDRESS, values));
                var res = (JObject)JsonConvert.DeserializeObject(answer.Substring(1, answer.Length-4));

                context.Channel.SendMessageAsync($"i asked {seed} .Her answer is \n\n{res["answer"]}");
            }
        }
    }
}