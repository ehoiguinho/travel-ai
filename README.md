
API de recomendação de viagens desenvolvida em **C# / .NET 10**, com foco em **RAG (Retrieval-Augmented Generation)**, busca semântica e geração de respostas com LLM.

## Objetivo

O TravelAI foi desenvolvido como um projeto prático para estudar **AI Engineering** e entender como integrar um LLM a dados específicos de uma aplicação.

O principal problema abordado é:

> Como fazer um LLM responder perguntas utilizando informações reais do catálogo de viagens, em vez de depender somente do conhecimento prévio do modelo?

A solução utiliza **RAG**, recuperando informações relevantes do banco de dados e utilizando-as como contexto para a geração da resposta.

```text
Pergunta do usuário
        ↓
Geração do embedding
        ↓
Busca semântica com pgvector
        ↓
Top-K documentos relevantes
        ↓
Score de similaridade
        ↓
Threshold de relevância
        ↓
Contexto
        ↓
LLM
        ↓
Resposta
```

O sistema também possui um mecanismo simples de recomendação baseado no **histórico de compras e categorias de interesse do usuário**.

---

## Exemplo

Pergunta:

```text
Quero conhecer lugares históricos e culturais na Europa.
```

O sistema realiza a busca semântica e recupera viagens relacionadas, como:

```text
Lisboa Cultural
Istambul Entre Dois Mundos
Roma Histórica Premium
Londres Histórica
Barcelona Mediterrânea
```

Essas informações são utilizadas como contexto para o LLM gerar a resposta.

Quando nenhuma informação possui similaridade suficiente com a pergunta, o sistema **não envia contexto irrelevante para o LLM** e retorna uma resposta controlada.

---

## Stack

* C#
* .NET 10
* ASP.NET Core Web API
* Entity Framework Core

### Banco de dados

* PostgreSQL
* pgvector

### IA

* Cohere Embed v4
* Cohere LLM
* Embeddings
* Vector Search
* Cosine Similarity
* RAG

### Ferramentas

* Git
* GitHub
* Postman

---

### Viagens

```http
GET    /api/viagem
POST   /api/viagem
PUT    /api/viagem/{id}
DELETE /api/viagem/{id}
```

### Recomendações

```http
GET /api/usuario/{id}/interesses
GET /api/usuario/{id}/recomendacoes
```

### RAG

```http
POST /api/knowledge/indexar
POST /api/knowledge/buscar
POST /api/chat/perguntar
```

---

Pré-requisitos:

* .NET 10 SDK
* PostgreSQL com pgvector
* API Key da Cohere

Configure:

```bash
export COHERE_API_KEY="sua-chave"
```

Depois:

```bash
dotnet restore
dotnet ef database update
dotnet run
```

---

## O que foi desenvolvido

O projeto foi construído para colocar em prática conceitos de:

* Embeddings
* Semantic Search
* Vector Database
* Cosine Similarity
* Top-K Retrieval
* Similarity Threshold
* Context Injection
* Grounding
* LLM Generation
* RAG

---
