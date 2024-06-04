using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UniqueIdentifier))]
public class Person : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _portrait;
    [SerializeField, ResizableTextArea] private string _appearence;
    [SerializeField] private List<string> _physicalAttributes;
    [SerializeField] private List<string> _skills;
    [SerializeField] private List<string> _personalityTraits;

    public string Name => _name;
    public Sprite Portrait => _portrait;
    public string Appearence => _appearence;
    public List<string> PhysicalAttributes => _physicalAttributes;
    public List<string> Skills => _skills;
    public List<string> PersonalityTraits => _personalityTraits;

    // Character settings are not stored immediately in the appropriate data type, as this requires repeated serialization of settings for each NPC
    public void SetSettings(PersonSettings settings)
    {
        _name = settings.Name;
        _portrait = settings.Portrait;
        _appearence = settings.Appearence;
        _physicalAttributes = settings.PhysicalAttributes;
        _skills = settings.Skills;
        _personalityTraits = settings.PersonalityTraits;
    }
}