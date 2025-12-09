//Controlador de Webhooks de WhatsApp para recibir,
// interpretar y responder a los mensajes de los usuarios de WhatsApp
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using Backend.Services;
using Backend.Utils;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;

[ApiController]
[Route("api/whatsapp/webhook")]
public class WhatsAppWebhookController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly WhatsAppService _whatsappService;

    // Diccionario para estados de usuario (en memoria)
    private static readonly Dictionary<string, string> userStates = new();

    public WhatsAppWebhookController(IConfiguration config, WhatsAppService whatsappService)
    {
        _config = config;
        _whatsappService = whatsappService;
    }

    // El [HttpGet] (Verify) se mantiene igual

    //Recepción de mensajes
    [HttpPost]
    public async Task<IActionResult> Receive([FromBody] JsonElement payload)
    {
        try
        {
            // ... (Parser JSON se mantiene igual)
            var entry = payload.GetProperty("entry")[0];
            var changes = entry.GetProperty("changes")[0];
            var value = changes.GetProperty("value");

            if (!value.TryGetProperty("messages", out JsonElement messages))
                return Ok();

            var message = messages[0];
            string fromRaw = message.GetProperty("from").GetString();



            // Manejo de mensajes que no son texto (evita fallar si envían una imagen, etc.)
            if (!message.TryGetProperty("text", out JsonElement textElement))
            {
                await _whatsappService.SendTextAsync(fromRaw, "Por favor, envía mensajes de texto para interactuar con el menú.");
                return Ok();
            }
            string text = textElement.GetProperty("body").GetString()?.Trim();


            // Reinicia si el usuario escribe "Hola"
            if (text?.Equals("Hola", StringComparison.OrdinalIgnoreCase) == true)
            {
                userStates.Remove(fromRaw);
            }

            //  NUEVO FLUJO DE ESTADOS 

            //  Estado inicial: enviar menú principal (Consultar/Generar)
            if (!userStates.ContainsKey(fromRaw))
            {
                userStates[fromRaw] = "WAITING_MAIN_SELECTION";
                await _whatsappService.SendTextAsync(fromRaw, CategoryData.MainWelcomeMessage);
                return Ok();
            }

            string currentState = userStates.GetValueOrDefault(fromRaw, "WAITING_MAIN_SELECTION");

            switch (currentState)
            {
                //  NIVEL 1: Esperando la selección de Consultar (1) o Generar (2)
                case "WAITING_MAIN_SELECTION":
                    if (text == "1") // 1. Consultar reporte
                    {
                        userStates[fromRaw] = "WAITING_REPORT_FOLIO";
                        await _whatsappService.SendTextAsync(fromRaw, "Por favor, ingresa el **folio o número de reporte** que deseas consultar.");
                    }
                    else if (text == "2") // 2. Generar reporte
                    {
                        userStates[fromRaw] = "WAITING_CATEGORY_SELECTION"; // Avanza al menú de categorías
                        await _whatsappService.SendTextAsync(fromRaw, CategoryData.CategorySelectMessage);
                    }
                    else
                    {
                        await _whatsappService.SendTextAsync(fromRaw, "Opción no válida. Por favor, responde con **1** (Consultar) o **2** (Generar).");
                    }
                    break;

                //  NIVEL 2: Esperando la selección de categoría (1-20)
                case "WAITING_CATEGORY_SELECTION":
                    if (int.TryParse(text, out int categoryNum) && categoryNum >= 1 && categoryNum <= 20)
                    {
                        string categoryKey = categoryNum.ToString();

                        // Lógica de Redirección (Caso 1: Agua) o Submenú
                        string subcategoryMessage = CategoryData.GetSubcategoryMessage(categoryKey);

                        if (categoryKey == "1") // 1. Agua (Redirige)
                        {
                            await _whatsappService.SendTextAsync(fromRaw, subcategoryMessage);
                            userStates.Remove(fromRaw); // Finaliza el flujo
                        }
                        else // 2-20. Flujo de Reporte (Avanza a Subcategoría)
                        {
                            // Aquí necesitarás iniciar el objeto de reporte (UserReportData)
                            // Por ahora, solo avanzamos el estado:
                            userStates[fromRaw] = $"WAITING_SUBCATEGORY_{categoryKey}";
                            await _whatsappService.SendTextAsync(fromRaw, subcategoryMessage);
                        }
                    }
                    else
                    {
                        // Manejar texto libre o número inválido
                        // (Aquí iría tu futura lógica de canalización de texto libre)
                        await _whatsappService.SendTextAsync(fromRaw, "Opción de categoría no válida. Por favor, ingresa el número (1-20) o escribe **Hola** para reiniciar.");
                    }
                    break;

                // NIVEL 3: Flujo de Consulta
                case "WAITING_REPORT_FOLIO":
                    // Lógica para buscar el folio en la base de datos
                    await _whatsappService.SendTextAsync(fromRaw, $"Buscando reporte con folio: {text}...");
                    // Implementar lógica de búsqueda
                    userStates.Remove(fromRaw); // Reinicia el flujo después de la consulta
                    break;

                // NIVEL 4: Flujo de Subcategorías (Ejemplo para categoría 3 - Alumbrado)
                case string s when s.StartsWith("WAITING_SUBCATEGORY_"):
                    // Aquí procesas la selección de subcategoría y pasas a pedir la dirección
                    string parentCategoryKey = currentState.Replace("WAITING_SUBCATEGORY_", "");
                    // Lógica de validación de subcategoría aquí

                    // Suponiendo que la subcategoría es válida
                    userStates[fromRaw] = "WAITING_ADDRESS";
                    await _whatsappService.SendTextAsync(fromRaw, $"Recibido. Ahora dime la **dirección exacta** del reporte.");
                    break;

                default:
                    // Si el estado no está mapeado o es un error, reiniciar el menú principal
                    userStates[fromRaw] = "WAITING_MAIN_SELECTION";
                    await _whatsappService.SendTextAsync(fromRaw, "Ha ocurrido un error en el flujo. Reiniciando el menú principal.\n" + CategoryData.MainWelcomeMessage);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error procesando mensaje");
            Console.WriteLine(ex.Message);
        }

        return Ok();
    }
}