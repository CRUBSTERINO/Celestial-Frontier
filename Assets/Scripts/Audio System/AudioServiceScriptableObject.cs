using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptables/Audio/Audio Service Config", fileName = "Audio Service Config")]
public class AudioServiceScriptableObject : ScriptableObject
{
    [SerializeField] private float _effectsVolume;
    [SerializeField] private float _ambientVolume;

    [field: SerializeField] public GameObject EffectAudioSourcePrefab {  get; set; }
    [field: SerializeField] public GameObject BackgroundAudioSourcePrefab { get; set; }
    [field: SerializeField] public float BlendPause { get; set; }
    [field: SerializeField] public float FadeDuration { get; set; }

    public float EffectsVolume { get => _effectsVolume;
    set
        {
            _effectsVolume = value;
            VolumeSettingsChanged?.Invoke();
        }
    }
    public float AmbientVolume { get => _ambientVolume; 
    set
        {
            _ambientVolume = value;
            VolumeSettingsChanged?.Invoke();
        }
    }

    public event Action VolumeSettingsChanged;
}
