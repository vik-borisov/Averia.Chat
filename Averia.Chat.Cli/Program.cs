using System;
using System.Threading.Tasks;
using CommandLine;
using Grpc.Core;
using Grpc.Net.Client;

namespace Averia.Chat.Cli
{
    class Program
    {
        public class Options
        {
            [Value(0, Default = "ls")]
            public string Command { get; set; }
            
            [Option('a', "address", Default = "http://localhost:5001", Required = false, HelpText = "Адрес сервера")]
            public string Address { get; set; }
        }
        
        static async Task Main(string[] args)
        {
            await Parser.Default.ParseArguments<Options>(args)
                .WithParsedAsync(async o =>
                {
                    using var channel = GrpcChannel.ForAddress(o.Address);
                    var client = new Management.ManagementClient(channel);

                    switch (o.Command)
                    {
                        case "ls":
                            var usersReply = await client.UsersAsync(new EmptyRequest());
                            foreach (var userId in usersReply.UserIds)
                            {
                                Console.WriteLine(userId);
                            }
                            break;
                        case "stop":
                            await client.StopAsync(new EmptyRequest());
                            Console.WriteLine("TERMINATED");
                            break;
                        case "watch":
                            await foreach (var message in client.Messages(new EmptyRequest()).ResponseStream.ReadAllAsync())
                            {
                                Console.WriteLine($"{message.Message.UserId} : {message.Message.Text}");
                            }
                            break;
                    }
                }).ConfigureAwait(false);
        }
    }
}