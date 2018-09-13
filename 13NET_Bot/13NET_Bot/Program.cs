using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StackExchange.Redis;

namespace _13NET_Bot
{
    class Program
    {
        static void Main(string[] args)
        {
            var _client = ConnectionMultiplexer.Connect("localhost");
            var db = _client.GetDatabase();
            var pub = _client.GetSubscriber();
            var sub = _client.GetSubscriber();
            int qtdPergunta = 0;

            sub.Subscribe("perguntas", (channel, msg) =>
            {
                var pergunta = msg.ToString();
                var canal = channel.ToString();

                Console.WriteLine($"Pergunta: {pergunta}");                

                var resposta = "Vai no posto ipiranga.";// Resposta(pergunta);
                Console.WriteLine($"Resposta: {resposta}");

                db.HashSet($"p{++qtdPergunta}", new HashEntry[] { new HashEntry("AkiFlavior", resposta) });
                pub.Publish("Respostas", resposta);
            });

            Console.ReadLine();
        }
    }
}
