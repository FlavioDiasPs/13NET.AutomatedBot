using System;
using System.Data;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace _13NET_Bot
{
    class Program
    {                
        static void Main(string[] args)
        {
            var _RedisClient = ConnectionMultiplexer.Connect("localhost");
            var db = _RedisClient.GetDatabase();
            var pub = _RedisClient.GetSubscriber();
            var sub = _RedisClient.GetSubscriber();
            int perguntaID = 0;            

            sub.Subscribe("perguntas", (channel, msg) =>
            {
                var pergunta = msg.ToString();
                var canal = channel.ToString();

                Console.WriteLine($"Pergunta: {pergunta}");

                string resposta = CalculatedAnswer(pergunta.Replace("?", ""));

                if (string.IsNullOrEmpty(resposta))
                    resposta = AkiFlaviorAnswer(pergunta, perguntaID);

                if (string.IsNullOrEmpty(resposta))
                    resposta = BotAnswer(pergunta);
                
                Console.WriteLine($"Resposta: {resposta}");

                db.HashSet($"p{++perguntaID}", new HashEntry[] { new HashEntry("AkiFlavior", resposta) });
                pub.Publish("respostas", resposta);
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

        static string BotAnswer(string pergunta)
        {           
            string baseUrl = @"http://sandbox.api.simsimi.com/request.p";
            string key = "8ec6296f-323a-4af9-b2e4-72ed6f8831c7";
            string lc = "pt";
            double filter = 1;
            string requestUri = $"?key={key}&lc={lc}&ft={filter}&text={pergunta}";
            string resposta = string.Empty;
            
            var _botClient = new HttpClient();
            _botClient.BaseAddress = new Uri(baseUrl);
            _botClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Console.WriteLine($"Enviando o request: {baseUrl}{requestUri}");
            using (var response = _botClient.GetAsync(requestUri).GetAwaiter().GetResult())
            {
                using (HttpContent content = response.Content)
                {
                    var jsonString = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    var token = JToken.Parse(jsonString);
                    resposta = token["response"].ToObject<string>();
                }
            }

            if (string.IsNullOrEmpty(resposta))
                return null;
            else
                return resposta;
        }
    }
}
