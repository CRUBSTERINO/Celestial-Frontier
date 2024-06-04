using UnityEngine;

namespace DialogSystem
{
    [CreateAssetMenu(fileName = "DialogAIEnhancerConfig", menuName = "Dialog Enhancer")]
    public class DialogAiEnhancerPromptScriptableObject : ScriptableObject
    {
        // All these properties are used to describe particular information in the prompt.
        [SerializeField, TextArea(5, 10)] private string _aiRoleModelDescription; // Description of AI's role model (how it should act).
        [SerializeField, TextArea(5, 10)] private string _personDescription; // Template for description of any person.
        [SerializeField, TextArea(5, 10)] private string _interlocutorLine;
        [SerializeField, TextArea(5, 10)] private string _rewritingPrompt;

        public string AIRoleModelDescription => _aiRoleModelDescription;
        public string PersonDescription => _personDescription;
        public string InterlocutorLine => _interlocutorLine;
        public string RewritingPrompt => _rewritingPrompt;
    } 
}
