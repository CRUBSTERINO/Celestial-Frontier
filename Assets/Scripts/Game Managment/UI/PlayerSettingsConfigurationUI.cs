using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSettingsConfigurationUI : MonoBehaviour
{
    [SerializeField] private Button _enterButton;
    // For now should be set in Editor.
    // If I will add portrait selector, than will remake
    [SerializeField] private Sprite _playerPortrait;
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private TMP_InputField _appearenceInputField;
    [SerializeField] private TMP_InputField _physicalAttributeInputField;
    [SerializeField] private TMP_InputField _skillsInputField;
    [SerializeField] private TMP_InputField _personalityTraitsInputField;

    private void Start()
    {
        _enterButton.onClick.AddListener(SetupPlayerSettings);
    }

    private void SetupPlayerSettings()
    {
        PersonSettings settings = GeneratePersonSettings();

        ServiceLocator.Instance.GetService<GameManager>().SetPlayerSettings(settings);

        _enterButton.onClick.RemoveAllListeners();
        Destroy(gameObject);
    }

    private PersonSettings GeneratePersonSettings()
    {
        List<string> physicalAttributes = _physicalAttributeInputField.text.Split(',').ToList();
        List<string> skills = _skillsInputField.text.Split(',').ToList();
        List<string> personalityTraits = _personalityTraitsInputField.text.Split(',').ToList();

        PersonSettings personSettings = new PersonSettings(_nameInputField.text, _playerPortrait, _appearenceInputField.text, physicalAttributes, skills, personalityTraits);
        return personSettings;
    }
}
