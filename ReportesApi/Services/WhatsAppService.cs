//servicio de conexion e interaccion con el API whatsApp
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.Services
{
    public class WhatsAppService
    {
        private readonly string _token;
        private readonly string _phoneNumberId;
        private readonly HttpClient _client;

        public WhatsAppService(string token, string phoneNumberId)
        {
            _token = token ?? throw new Exception("AccessToken no configurado");
            _phoneNumberId = phoneNumberId ?? throw new Exception("PhoneNumberId no configurado");

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        }

        // Enviar mensaje de texto simple
        public async Task SendTextAsync(string to, string message)
        {

            Console.WriteLine($"[WhatsAppService] Intentando enviar mensaje:");
            Console.WriteLine($"Número destinatario: {to}");
            Console.WriteLine($"Mensaje: {message}");


            var payload = new
            {
                messaging_product = "whatsapp",
                to = to,
                type = "text",
                text = new { body = message }
            };

            await SendPayloadAsync(payload);
        }

        // Enviar imagen
        public async Task SendImageAsync(string to, string imageUrl, string caption = "")
        {

            Console.WriteLine($"[WhatsAppService] Intentando enviar imagen a {to} con caption: {caption}");

            var payload = new
            {
                messaging_product = "whatsapp",
                to = to,
                type = "image",
                image = new { link = imageUrl, caption = caption }
            };
              //informacion que se manda desde una peticion  
            await SendPayloadAsync(payload);
        }

        // Enviar plantilla
        public async Task SendTemplateAsync(string to, object template)
        {

            Console.WriteLine($"[WhatsAppService] Intentando enviar plantilla a {to}");

            var payload = new
            {
                messaging_product = "whatsapp",
                to = to,
                type = "template",
                template = template
            };

            await SendPayloadAsync(payload);
        }


        // Envío genérico a la API
        private async Task SendPayloadAsync(object payload)
        {
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var url = $"https://graph.facebook.com/v17.0/{_phoneNumberId}/messages";

            var response = await _client.PostAsync(url, content);
            var respText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new WhatsAppApiException($"Error enviando mensaje: {response.StatusCode} - {respText}");
            }

            Console.WriteLine($"[WhatsAppService] Mensaje enviado: {respText}");
        }

    }

    public class WhatsAppApiException : Exception
    {
        public WhatsAppApiException(string message) : base(message) { }
    }
}
