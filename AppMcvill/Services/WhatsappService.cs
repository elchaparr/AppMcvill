using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using AppMcvill.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace AppMcvill.Services
{
    public class WhatsappService
    {
        private readonly string phoneNumberId = "950121674840010";
        private readonly string accessToken = "EAALy0y4lyKoBQAzW82mb6V4FTk659TKUZCLV3JdNyaZCdC0MdC98hxiEHQEJGWZBizyQZBT6Rr9AFB72p3lacg76G43ZBlZCQ8MkDnXMbUYlQVDhNaHjrMKOHRFTx2dIIa5OxyjnVfM6rTbzmrDJKiqTp4GNbvSrlsQe6RNCZAepx4uixoFlxHGB89RvJABXQZDZD";

        public async Task EnviarMensajeAsync(string telefonoDestino, string idRequisicion, string linea, string material)
        {
            var url = $"https://graph.facebook.com/v18.0/{phoneNumberId}/messages";

            var body = new
            {
                messaging_product = "whatsapp",
                to = telefonoDestino,
                type = "template",
                template = new
                {
                    name = "requisicion_nueva",
                    language = new { code = "es_MX" },
                    components = new[]
                    {
                        new {
                            type = "body",
                            parameters = new object[]
                            {
                                new { type = "text", text = idRequisicion.ToString() },
                                new { type = "text", text = linea },
                                new { type = "text", text = material }
                            }
                        }
                    }
                }
            };


            var json = JsonSerializer.Serialize(body);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await http.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error enviando WhatsApp: {error}");
            }
        }
        public async Task<RespuestaWhatsApp> ObtenerRespuestaAsync()
        {
            using var http = new HttpClient();

            string url = "https://alexis-unerect-graig.ngrok-free.dev/webhook/ultimo";

            var json = await http.GetStringAsync(url);
            //await Application.Current.MainPage.DisplayAlert("Éxito", json, "OK");
            return JsonSerializer.Deserialize<RespuestaWhatsApp>(json);
        }

    }


}
