using UnityEngine;

namespace DialogSystem
{
    public class DialogParticipantScriptableObject : ScriptableObject
    {
        [field: SerializeField] public string ParticipantId { get; set; }

        public void Initialize(string participantId)
        {
            ParticipantId = participantId;
        }
    } 
}
