using System;
using System.Threading;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OpenAI;
using OpenAI.Chat;
using UnityEngine;
using OpenAI.Models;

public class OpenAIService : IService
{
    private OpenAIServiceScriptableObject _config;
    private OpenAIClient _client;
    private bool _isChatPending;

    public bool ShouldEnhanceQuestions => _config.ShouldEnhanceQuestions;

    public OpenAIService(OpenAIServiceScriptableObject config)
    {
        _config = config;
        _client = new OpenAIClient(config.ApiKey);
    }

    public void OnDestroy()
    {
        
    }

    public void OnStart()
    {
        
    }

    public async UniTask<string> SendQuestionToChat(string question, List<Message> messages, bool shouldKeepMessagesInHistory, Action<ChatResponse> responseHandler = default, CancellationToken cancellationToken = default)
    {
        if (!_config.ShouldEnhanceQuestions)
        {
            return question;
        }

        if (_isChatPending || string.IsNullOrWhiteSpace(question)) return string.Empty;
        _isChatPending = true;

        Message questionMessage = new Message(Role.User, question + "\n");
        messages.Add(questionMessage);

        try
        {
            ChatRequest chatRequest = new ChatRequest(messages, Model.GPT3_5_Turbo);
            var response = await _client.ChatEndpoint.StreamCompletionAsync(chatRequest, responseHandler, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                messages.Remove(questionMessage);
            }
            else
            {
                messages.Add(new Message(Role.Assistant, response.FirstChoice?.ToString()));
            }

            if (!shouldKeepMessagesInHistory)
            {
                messages.RemoveRange(messages.Count - 2, 2);
            }

            return response.ToString();
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            messages.Remove(questionMessage);
            return string.Empty;
        }
        finally
        {
            _isChatPending = false;
        }
    }
}
