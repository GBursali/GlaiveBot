using Discord;
using Discord.Commands;
using discordapp.Properties;
using HtmlAgilityPack;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace discordapp.Classes
{
    class ChatCommands : ModuleBase<SocketCommandContext>
    {
        const string TODOPATH = "todo.txt";

        /// <summary>
        /// AND HIS NAME IS JOHN CENAAAA
        /// </summary>
        /// <returns></returns>
        [Command("sosaal", RunMode = RunMode.Async)]
        [Summary("Ear Hurting")]
        public async Task EarRape()
        {
            playSound("sosaal.mp3");
        }

        /// <summary>
        /// EXPLOOOOOSION
        /// </summary>
        /// <returns></returns>
        [Command("explosion",RunMode=RunMode.Async)]
        [Summary("BOOOOOM")]
        public async Task Boom()
        {
            playSound("explosion.mp3");
        }

        /// <summary>
        /// Delete messages.
        /// </summary>
        /// <returns></returns>
        [Command("wipe", RunMode = RunMode.Async)]
        [Summary("Delete messages.")]
        public async Task wipe()
        {
            var messages = await Context.Channel.GetMessagesAsync().Flatten();
            TimeSpan WIPELIMIT = new TimeSpan(14, 0, 0, 0);//There is for deleting messages. Messages cant be older than that.
            var messageList = messages.Where(x => x.Timestamp > (DateTimeOffset.Now - WIPELIMIT));
            await Context.Channel.DeleteMessagesAsync(messageList);
            //Suggestion: Loop it through and delete all?
            //Result: Too much usage, not worth it. Not on this machine though.
        }
        [Command("spam", RunMode = RunMode.Async)]
        [Summary("Spamming the channel")]
        public async Task spam([Summary("Spam Count")] int count)
        {
            for (var i = 1; i <= count; i++)
                await ReplyAsync("Spam ->" + i);
        }

        [Command("wakeup", RunMode = RunMode.Async)]
        [Summary("Spam someone")]
        public async Task pmSpam([Summary("Spam Count")]int count,[Remainder] [Summary("Who")] string uName)
        {
            var users = await Context.Channel.GetUsersAsync().Flatten();
            var user = users.First(x => x.Username.Contains(uName));
            //if (user.Id == Settings1.Default.ADMINID) return; //Cheating for myself, lol.
            for (int i = 1; i <= count; i++) await user.SendMessageAsync("Wakie Wakie");
        }

        [Command("dice", RunMode = RunMode.Async)]
        [Summary("Roll a dice")]
        public async Task rolltheDice()
        {
            await ReplyAsync($"{Context.User.Mention} rolls a dice: {new Random().Next(1, 7)}");
        }

        [Command("pick", RunMode = RunMode.Async)]
        [Summary("Randomly pick one item.")]
        public async Task pickOne([Summary("Choices")]params string[] opt)
        {
            await ReplyAsync($"Selected -> {opt[new Random().Next(0, opt.Length + 1)]}");
        }

        [Command("exit", RunMode = RunMode.Async)]
        [Summary("Power off(for bot, ofc)")]
        public async Task go()
        {
            await ReplyAsync("bye");
            Environment.Exit(0);
        }

        [Command("todo", RunMode = RunMode.Async)]
        [Summary("Opens or adds some item in the TODO list")]
        public async Task toDo([Remainder][Summary("Yapılacak eylem")]string even= "")
        {
            if (string.IsNullOrEmpty(even))
            {
                var index = 0;
                await Context.User.SendMessageAsync(string.Join("\n",File.ReadAllLines(TODOPATH).Select(x=>(index++) + " " + x)));
                await Context.Message.DeleteAsync();
            }

            else
            {
                File.AppendAllText(TODOPATH, $"{Context.User.Username}-> {even}\n");
                await ReplyAsync("Eklendi");
            }
        }

        [Command("çevir",RunMode=RunMode.Async)]
        [Summary("English to turkish translator")]
        public async Task ceviri([Remainder][Summary("Word")] string word)
        {
            var hD = new HtmlDocument();
            hD.LoadHtml(Encoding.UTF8.GetString(getWebClient().DownloadData($"http://tureng.com/tr/turkce-ingilizce/{WebUtility.HtmlEncode(word)}")));
            var meanings = hD.DocumentNode.SelectNodes("//td[contains(@class, 'tr') and contains(@class, 'ts')]").Select(x => WebUtility.HtmlDecode(x.InnerText)).Take(5);
            await ReplyAsync($"'{word}' çevirisi:");
            await ReplyAsync(string.Join("\n", meanings));
        }

        [Command("hug",RunMode=RunMode.Async)]
        [Summary("Hugs the sender")]
        public async Task hug([Remainder][Summary("Who")]IUser user)
        {
            await ReplyAsync($":hugging: {user.Mention}");
        }
    
        [Command("cry", RunMode = RunMode.Async)]
        [Summary("Makes the bot cry")]
        public async Task cry()
        {
            await ReplyAsync($":sob:");
        }
        
        [Command("gg",RunMode =RunMode.Async)]
        [Summary("wp")]
        public async Task gG()
        {
            await ReplyAsync("GGWP");
        }

        [Command("fixed",RunMode =RunMode.Async)]
        [Summary("Remove from the TODO list.")]
        public async Task fix(int index)
        {
            var admin = Context.Guild.GetUser(Settings1.Default.ADMINID);
            if (Context.User.Id == admin.Id)
            {
                var lines = File.ReadAllLines(TODOPATH).ToList();
                lines.RemoveAt(index);
                File.WriteAllLines(TODOPATH, lines);
                await ReplyAsync("GG");
            }
            else await ReplyAsync($"Bunu sadece {admin.Mention} kullanabilir.");
        }

        [Command("dolar",RunMode = RunMode.Async)]
        [Summary("Dollar to Turkish Lira")]
        public async Task Dolar()
        {
            const string LINK = "https://www.google.com.tr/search?&q=dolar";
            const string ID = "knowledge-currency__tgt-amount";

            var hD = new HtmlDocument();
            using(var wc = getWebClient())
            {
                hD.LoadHtml(wc.DownloadString(LINK));
            }
            string kur = hD.GetElementbyId(ID).InnerText;
            await ReplyAsync($"1 Dollar = {kur} TL");
            
        }

        [Command("ceyda", RunMode = RunMode.Async)]
        [Summary("Ceydaya soru sorar")]
        public async Task askher([Remainder][Summary("Soru")] string soru) => Program.AskCeyd_A(soru, Context,""); 

        private WebClient getWebClient()
        {
            var wc = new WebClient
            {
                Encoding = Encoding.UTF8,
                UseDefaultCredentials = true
            };
            wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36");
            wc.Proxy.Credentials = CredentialCache.DefaultCredentials;
            return wc;
        }
        private async void playSound(string path)
        {
            var ch = Context.Guild.VoiceChannels.First(x => x.Users.Contains(Context.User));
            var pro = Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg.exe",
                Arguments = $" -hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
            using (var iac = await ch.ConnectAsync())
            using (var pcm = iac.CreatePCMStream(Discord.Audio.AudioApplication.Mixed))
                if (pro != null)
                    await pro.StandardOutput.BaseStream.CopyToAsync(pcm);
        }

    }
}
