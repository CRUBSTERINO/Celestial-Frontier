using Cysharp.Threading.Tasks;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine;

namespace DialogSystem
{
    public class DialogAiEnhancer : MonoBehaviour
    {
        [SerializeField] private DialogAiEnhancerPromptScriptableObject _promptConfig;

        private OpenAIService _aiService;
        private List<Person> _persons;
        private Dictionary<Person, List<Message>> _personsMessageHistories;

        private void Start()
        {
            _aiService = ServiceLocator.Instance.GetService<OpenAIService>();
        }

        private string FormulatePrompt(string initialSentenceText) // from and to now have no weight until no "identity feature" is added
        {
            string formulatedQuestion = string.Format(_promptConfig.RewritingPrompt, initialSentenceText); // Substitute initial text in preset for prompt
            formulatedQuestion = formulatedQuestion.Replace(@"\", string.Empty); // Remove backslashes that denote special characters
            return formulatedQuestion;
        }

        // Compiles the description of every person's traits into single string
        private string GetPersonDescriptionAsString(Person person)
        {
            string physicalAttributesString = ListToComaString(person.PhysicalAttributes);
            string skillsString = ListToComaString(person.Skills);
            string personalityTraitsString = ListToComaString(person.PersonalityTraits);

            string description = string.Format(_promptConfig.PersonDescription, person.Name, person.Appearence, physicalAttributesString, skillsString, personalityTraitsString);
            return description;
        }

        // Lists the items in the list separated by commas
        private string ListToComaString(List<string> list)
        {
            StringBuilder sb = new StringBuilder();

            foreach (string item in list)
            {
                sb.Append($"{item}, ");
            }

            string comaString = sb.ToString();
            comaString = comaString.TrimEnd(new char[] { ',', ' ' });

            return comaString;
        }

        // Configures each of dialogs participants when it starts
        public void SetupDialogParticipants(List<Person> persons)
        {
            _persons = persons;
            _personsMessageHistories = new Dictionary<Person, List<Message>>(persons.Count);

            foreach (Person person in persons)
            {
                List<Message> messages = new List<Message>
            {
                new Message(OpenAI.Role.System, string.Format(_promptConfig.AIRoleModelDescription, GetPersonDescriptionAsString(person))), // Setup AI's role model
            };

                _personsMessageHistories.Add(person, messages);
            }

            // Is commented, because the description of interlocutor is removed
            /*foreach (Person person in _persons)
            {
                foreach (Person interlocutor in _persons)
                {
                    if (interlocutor != person)
                    {
                        //_personsMessageHistories[person].Add(new Message(Role.System, string.Format(_promptConfig.InterlocutorDescription, GetPersonDescriptionAsString(interlocutor))));
                        _personsMessageHistories[person].Add(new Message(Role.System, _promptConfig.InterlocutorDescription));
                    }
                }
            }*/
        }

        public void RecordInterlocutorsSentences(Person listener, List<string> lines)
        {
            string interlocutorsText = string.Empty;

            // Form one single string from all lines that interlocutor had said
            foreach (string line in lines)
            {
                interlocutorsText += line + " ";
            }
            interlocutorsText = interlocutorsText.Replace(@"\", string.Empty);
            interlocutorsText = string.Format(_promptConfig.InterlocutorLine, interlocutorsText);

            // Add formed string to listeners messages history
            List<Message> listenerMessageHistory = _personsMessageHistories[listener];
            Message interlocutorsTextMessage = new Message(OpenAI.Role.User, interlocutorsText);
            listenerMessageHistory.Add(interlocutorsTextMessage);
        }

        public async UniTask<string> EnhanceInitialSentence(Person speaker, string initialSentenceText, Action<string> responseHandler, CancellationToken cancellationToken)
        {
            if (!_aiService.ShouldEnhanceQuestions)
            {
                responseHandler.Invoke(initialSentenceText);
                return initialSentenceText;
            }

            List<Message> speakerMessageHistory = _personsMessageHistories[speaker];
            string aiPrompt = FormulatePrompt(initialSentenceText);
            string responseText = string.Empty;

            try
            {
                string response = await _aiService.SendQuestionToChat(aiPrompt, speakerMessageHistory, true, (response) => responseHandler.Invoke(responseText += response.FirstChoice?.Delta.ToString().Trim('"')), cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
                return string.Empty;
            }
        }
    } 
}
