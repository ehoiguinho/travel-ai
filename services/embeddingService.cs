using System.Net.Http.Json;
using Pgvector;

namespace TravelAI.Services;

public class EmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public EmbeddingService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _apiKey = Environment.GetEnvironmentVariable("COHERE_API_KEY")
            ?? throw new InvalidOperationException(
                "A variável COHERE_API_KEY não foi configurada."
            );
    }

    public async Task<Vector> GerarEmbeddingDocumentoAsync(string texto)
    {
        return await GerarEmbeddingAsync(
            texto,
            "search_document"
        );
    }

    public async Task<Vector> GerarEmbeddingConsultaAsync(string texto)
    {
        return await GerarEmbeddingAsync(
            texto,
            "search_query"
        );
    }

    private async Task<Vector> GerarEmbeddingAsync(
        string texto,
        string inputType)
    {
        var url = "https://api.cohere.com/v2/embed";

        var request = new
        {
            model = "embed-v4.0",
            texts = new[]
            {
                texto
            },
            input_type = inputType,
            output_dimension = 1024,
            embedding_types = new[]
            {
                "float"
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
            System.Text.Json.JsonSerializer.Deserialize<EmbeddingResponse>(
                responseBody
            );

        if (resultado?.embeddings?.@float == null ||
            resultado.embeddings.@float.Length == 0)
        {
            throw new InvalidOperationException(
                "A Cohere não retornou um embedding válido."
            );
        }

        var valores = resultado.embeddings.@float[0];

        return new Vector(valores);
    }

    private class EmbeddingResponse
    {
        public EmbeddingsData? embeddings { get; set; }
    }

    private class EmbeddingsData
    {
        public float[][]? @float { get; set; }
    }
}