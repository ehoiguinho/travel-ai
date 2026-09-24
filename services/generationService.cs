using System.Net.Http.Json;

namespace TravelAI.Services;

public class GenerationService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public GenerationService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _apiKey = Environment.GetEnvironmentVariable("COHERE_API_KEY")
            ?? throw new InvalidOperationException(
                "A variável COHERE_API_KEY não foi configurada."
            );
    }

    public async Task<string> GerarRespostaAsync(string pergunta, string contexto)
    {
        var url = "https://api.cohere.com/v2/chat";

        var request = new
        {
            model = "command-a-plus-05-2026",

            messages = new[]
            {
                new
                {
                    role = "system",
                    content = """
                    Você é o assistente de viagens do TravelAI.

                    Responda sempre em português do Brasil.

                    Utilize exclusivamente as informações fornecidas
                    no contexto para responder à pergunta.

                    Se a informação necessária não estiver no contexto,
                    informe que não possui informações suficientes.

                    Não invente destinos, preços, duração ou características
                    que não estejam presentes no contexto.

                    Formate as respostas de forma clara, organizada e fácil de ler.

                    Quando apresentar viagens, utilize este formato:

                    **Nome da viagem**

                    País: ...
                    Cidade: ...
                    Duração: ...
                    Preço: ...
                    Destaques: ...

                    Separe cada viagem com uma linha em branco
                    e uma linha horizontal.

                    Não utilize emojis ou ícones.
                    Não utilize tabelas Markdown.
                    """
                },

                new
                {
                    role = "user",
                    content = $"""
                    Contexto disponível:

                    {contexto}

                    Pergunta do usuário:

                    {pergunta}
                    """
                }
            }
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            url
        );

        httpRequest.Headers.Add(
            "Authorization",
            $"Bearer {_apiKey}"
        );

        httpRequest.Content = JsonContent.Create(request);

        var response = await _httpClient.SendAsync(httpRequest);

        var responseBody =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException(
                $"Erro Cohere ({(int)response.StatusCode}): {responseBody}"
            );
        }

        var resultado =
            System.Text.Json.JsonSerializer.Deserialize<ChatResponse>(
                responseBody
            );

        var texto =
            resultado?.message?.content?
                .FirstOrDefault(c => c.type == "text")
                ?.text;

        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new InvalidOperationException(
                "A Cohere não retornou uma resposta válida."
            );
        }

        return texto;
    }

    private class ChatResponse
    {
        public Message? message { get; set; }
    }

    private class Message
    {
        public List<Content>? content { get; set; }
    }

    private class Content
    {
        public string? type { get; set; }
        public string? text { get; set; }
    }
}