using UnityEngine;

[CreateAssetMenu(fileName = "Open AI Serivce Config", menuName = "Scriptables/Open AI/Open AI Service Config")]
public class OpenAIServiceScriptableObject : ScriptableObject
{
    // true = questions will be sent to chat GPT and returned will be an answer; false = just return the same question (made for dialog testing)
    [field: SerializeField] public bool ShouldEnhanceQuestions { get; set; }
    [field: SerializeField] public string ApiKey { get; set; }
}