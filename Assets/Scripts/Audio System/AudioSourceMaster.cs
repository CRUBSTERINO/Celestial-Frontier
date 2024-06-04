using DG.Tweening;
using System;
using UnityEngine;

public class AudioSourceMaster : MonoBehaviour
{
    private AudioSource _audioSource;
    private Sequence _sequence;

    public AudioSource AudioSource { get { return _audioSource; } }

    public event Action<AudioSourceMaster> OnDestroyed;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }

    public void BlendAudioClips(AudioClip audioClip, float blendDuration, float blendPause, float volume)
    {
        float fadeDuration = blendDuration - blendPause;
        Tween fadeOutTween = FadeOut(fadeDuration);
        fadeOutTween.OnComplete(() => 
        {
            _audioSource.clip = audioClip;
            _audioSource.Play();
            FadeIn(fadeDuration, volume).SetDelay(blendPause);
        });
    }

    public Tween FadeIn(float fadeDuration, float volume)
    {
        _sequence.Kill();
        Tween tween = DOTween.To(() => _audioSource.volume, x => _audioSource.volume = x, volume, fadeDuration).SetEase(Ease.InOutSine);
        InitializeSequence(tween);

        return tween;
    }

    public Tween FadeOut(float fadeDuration)
    {
        _sequence.Kill();
        Tween tween = DOTween.To(() => _audioSource.volume, x => _audioSource.volume = x, 0, fadeDuration).SetEase(Ease.InOutSine);
        InitializeSequence(tween);

        return tween;
    }

    private void InitializeSequence(Tween tween)
    {
        _sequence = DOTween.Sequence(tween);
    }
}
