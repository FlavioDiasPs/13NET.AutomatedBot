using System;
using System.Data;

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
            int perguntaID = 0;            

            sub.Subscribe("perguntas", (channel, msg) =>
            {
                var pergunta = msg.ToString();
                var canal = channel.ToString();

                Console.WriteLine($"Pergunta: {pergunta}");

                string resposta = CalculatedAnswer(pergunta.Replace("?", ""));

                if (string.IsNullOrEmpty(resposta))
                    resposta = AkiFlaviorAnswer(pergunta, perguntaID);

                Console.WriteLine($"Resposta: {resposta}");

                db.HashSet($"p{++perguntaID}", new HashEntry[] { new HashEntry("AkiFlavior", resposta) });
                pub.Publish("Respostas", resposta);
            });

            Console.ReadLine();
        }

        static string CalculatedAnswer(string math)
        {
            object value = null;
            try
            {
                value = new DataTable().Compute(math, null);
            }
            catch (Exception) { }

            if (value is null) return null;
            return value.ToString();
        }

        static string AkiFlaviorAnswer(string pergunta, int perguntaID)
        {
            switch(perguntaID)
            {
                case 1:
                    return "Voce mentiu! a primeira pergunta deveria ser 1+1?";                    
                case 2:
                    return "Pergunta no posto ipiranga"; 
                case 3:
                    return "Pergunta no posto ipiranga";
                case 4:
                    return "Essa pergunta e muito loka tiu, vou saber essa treta n.";
                case 5:
                    return "Se jesus na causa.";
                case 6:
                    return "Cristo e a resposta.";
                case 7:
                    return "Se o professor nao sabe, imagina eu! ;D";                
                case 8:
                    return $"Nao sei a resposta, entao vou cantar uma musica:   " +
                        $"Pau que nasce tooooooorto " +
                        $"Nunca se endireita " +
                        $"Menina que requebra " +
                        $"A mae pega na cabeca " +
                        $"Domingo ela nao vai " +
                        $"Vai, vai " +
                        $"Domingo ela nao vai nao " +
                        $"Vai, vai, vai " +
                        $"Domingo ela nao vai " +
                        $"Vai, vai " +
                        $"Domingo ela nao vai nao " +
                        $"Vai, vai, vai " +
                        $"Segure o tchan " +
                        $"Amarre o tchan " +
                        $"Segure o tchan tchan tchan " +
                        $"Tchan tchan ";
                default:
                    return $"Nao sei a resposta, entao vou ensinar a preparar um miojo:   " +
                         $"Pegue o miojo";



            }
        }
    }
}
