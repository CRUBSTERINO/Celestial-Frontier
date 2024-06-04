using System.Collections.Generic;
using UnityEngine;

public class PersonSettings
{
    public string Name { get; set; }
    public Sprite Portrait { get; set; }
    public string Appearence { get; set; }
    public List<string> PhysicalAttributes { get; set; }
    public List<string> Skills { get; set; }
    public List<string> PersonalityTraits { get; set; }

    public PersonSettings(string name, Sprite portrait, string appearence, List<string> physicalAttributes, List<string> skills, List<string> personalityTraits)
    {
        Name = name;
        Portrait = portrait;
        Appearence = appearence;
        PhysicalAttributes = physicalAttributes;
        Skills = skills;
        PersonalityTraits = personalityTraits;
    }
}
