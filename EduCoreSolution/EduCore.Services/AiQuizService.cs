using AutoMapper;
using EduCore.Domain.Contracts;
using EduCore.Domain.Entities.QuizModel;
using EduCore.Services.Helpers;
using EduCore.Services_Abstraction;
using EduCore.Shared.DTOs.Quiz.Teacher;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EduCore.Services
{
    public class AiQuizService : IAiQuizService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly HttpClient _httpClient;

        public AiQuizService(IUnitOfWork unitOfWork, IConfiguration configuration, IHttpClientFactory httpClientFactory,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
           _mapper = mapper;
            _httpClient = httpClientFactory.CreateClient("Gemini");
        }



        public async Task<AiGeneratedQuizDto> GenerateQuizAsync(int quizId, string teacherId, GenerateQuizAiRequest request)
        {
            await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, quizId, teacherId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, quizId);
            var prompt = BuildPrompt(request);
            var response = await CallGeminiAsync(prompt);
            return ParseResponse(response);

        }

        private async Task<string> CallGeminiAsync(string prompt)
        {
            var apiKey = _configuration["Gemini:GeminiKey"];
            var model = _configuration["Gemini:Model"] ?? "gemini-2.0-flash";
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                },
                generationConfig = new
                {
                    temperature = 0.7,
                    maxOutputTokens = 16384
                }
            };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API error {(int)response.StatusCode}: {errorBody}");
            }
            var responseJson = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(responseJson);
            var parts = parsed.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts");
            
            // For thinking models (gemini-2.5-*), parts[0] is the thinking/reasoning text.
            // The actual response is the last part.
            var lastPart = parts[parts.GetArrayLength() - 1];
            return lastPart.GetProperty("text").GetString()!;
        }

        private string BuildPrompt(GenerateQuizAiRequest request)
        {
            return $$"""
        Generate a quiz with exactly {{request.QuestionCount}} {{request.QuestionType}} questions about: "{{request.Topic}}"
        Difficulty level: {{request.Difficulty}}
        Points per question: {{request.PointsPerQuestion}}

        Return ONLY a valid JSON object in this exact format, no markdown, no extra text:
        {
          "questions": [
            {

             "text": "Question text here",
              "type": "{{request.QuestionType}}",
              "points": {{request.PointsPerQuestion}},
              "options": [
                {"text": "Option text", "isCorrect": true},
                {"text": "Option text", "isCorrect": false},
                {"text": "Option text", "isCorrect": false},
                {"text": "Option text", "isCorrect": false}
              ]
            }
          ]
        }

        Rules:
        - For Mcq: exactly 4 options, exactly 1 correct
        - For TrueFalse: exactly 2 options (True/False), exactly 1 correct
        - Questions must be clear and educational
        - Return ONLY the JSON object
        """;
        }
        private AiGeneratedQuizDto ParseResponse(string response)
        {
            var cleaned = response.Trim();
            if (cleaned.StartsWith("```"))
            {
                var lines = cleaned.Split('\n');
                cleaned = string.Join('\n', lines
                    .Skip(1)
                    .TakeWhile(l => !l.TrimStart().StartsWith("```")));
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<AiGeneratedQuizDto>(cleaned, options)!;
        }

        public async Task<AiGeneratedQuizDto> SaveGeneratedQuizAsync(int quizId, string teacherId, AiGeneratedQuizDto generated)
        {
            await ValidationHelpers.GetQuizOrThrowAsync(_unitOfWork, quizId, teacherId);
            await ValidationHelpers.EnsureNoAttemptsAsync(_unitOfWork, quizId);
            foreach (var q in generated.Questions)
            {
                var question = _mapper.Map<Question>(q);
                question.QuizId = quizId;
                await _unitOfWork.GetRepository<Question, int>().AddAsync(question);
                await _unitOfWork.SaveChangesAsync();

                var options = _mapper.Map<List<AnswerOption>>(q.Options);
                options.ForEach(opt => opt.QuestionId = question.Id);

                foreach (var option in options)
                    await _unitOfWork.GetRepository<AnswerOption, int>().AddAsync(option);
            }

            await _unitOfWork.SaveChangesAsync();
            return generated;
        }
    }
}
