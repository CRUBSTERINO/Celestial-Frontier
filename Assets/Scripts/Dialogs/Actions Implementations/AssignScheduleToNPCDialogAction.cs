using UnityEngine;

namespace DialogSystem.Actions
{
    public class AssignScheduleToNPCDialogAction : DialogAction
    {
        [SerializeField] private DayScheduleScriptableObject _schedule;
        [SerializeField] private string _npcId;

        public override void Trigger(Person person)
        {
            NpcManager npcManager = UniqueIdentifier.FindObjectWithID(_npcId)
                .GetComponent<NpcManager>();

            if (npcManager == null)
            {
                Debug.LogError($"No NPCManager found on GameObject with ID {_npcId}.");
                return;
            }

            WorldTimeService worldTimeService = ServiceLocator.Instance.GetService<WorldTimeService>();

            npcManager.SetupSchedule(worldTimeService, _schedule);
        }

        public override DialogAction CloneAction()
        {
            AssignScheduleToNPCDialogAction action = new AssignScheduleToNPCDialogAction()
            {
                _schedule = _schedule,
                _npcId = _npcId
            };

            return action;
        }
    }
}
