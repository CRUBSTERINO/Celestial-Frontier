using UnityEngine;

public class AudioSpawner : MonoBehaviour
{
    [SerializeField] private AudioClip _audioClip;
    [SerializeField] private bool _isLooping;

    private void Start()
    {
        AudioService audioService = ServiceLocator.Instance.GetService<AudioService>();

        audioService.Instantiate3dEffectAudioSource(_audioClip, transform.position, _isLooping);
    }
}
