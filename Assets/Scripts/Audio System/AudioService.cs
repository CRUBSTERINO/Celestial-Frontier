using System.Collections.Generic;
using UnityEngine;

public class AudioService : IService
{
    private AudioServiceScriptableObject _config;
    private AudioSourceMaster _ambientAudioSourceMaster;
    private AudioSourceMaster _musicAudioSourceMaster;
    private List<AudioSourceMaster> _effectAudioSourcesMasters;
    private float _musicVolume = 1f;
    private float _effectsVolume = 1f;
    private AudioClip _defaultMusicAudioClip;
    private Transform _parent;

    public AudioService(AudioServiceScriptableObject config)
    {
        _config = config;
    }

    public void OnStart()
    {
        _parent = Object.Instantiate(new GameObject("Parent for Audio Sources")).transform;
        Object.DontDestroyOnLoad(_parent.gameObject);

        InstantiateAmbientAudioSource(null, true);
        InstantiateMusicAudioSource(null, true);

        _config.VolumeSettingsChanged += UpdateVolumes;

        SceneManagmentService sceneManagment = ServiceLocator.Instance.GetService<SceneManagmentService>();

        sceneManagment.OnGameplaySceneLoadingStarted += OnGameplaySceneLoadingStartedHandler;
        sceneManagment.OnGameplaySceneLoaded += OnGameplaySceneLoadedHandler;

        SetAmbientAudioClip(sceneManagment.CurrentScene.AmbientAudioClip);

        _effectAudioSourcesMasters = new List<AudioSourceMaster>();
        UpdateVolumes();
    }

    public void OnDestroy()
    {
        SceneManagmentService sceneManagment = ServiceLocator.Instance.GetService<SceneManagmentService>();

        _config.VolumeSettingsChanged -= UpdateVolumes;

        sceneManagment.OnGameplaySceneLoadingStarted -= OnGameplaySceneLoadingStartedHandler;
        sceneManagment.OnGameplaySceneLoaded -= OnGameplaySceneLoadedHandler;
    }

    private void UpdateVolumes()
    {
        _musicVolume = _config.AmbientVolume;
        _effectsVolume = _config.EffectsVolume;

        if (_ambientAudioSourceMaster != null)
        {
            _ambientAudioSourceMaster.AudioSource.volume = _musicVolume;
        }

        if (_musicAudioSourceMaster != null)
        {
            _musicAudioSourceMaster.AudioSource.volume = _musicVolume;
        }

        foreach (AudioSourceMaster master in _effectAudioSourcesMasters)
        {
            if (master != null)
            {
                master.AudioSource.volume = _effectsVolume; 
            }
        }
    }

    private void OnEffectAudioSourceMasterDestroyedHandler(AudioSourceMaster master)
    {
        int index = _effectAudioSourcesMasters.IndexOf(master);
        if (index != -1)
        {
            master.OnDestroyed -= OnEffectAudioSourceMasterDestroyedHandler;
            _effectAudioSourcesMasters[index] = null;
        }
    }

    private void OnGameplaySceneLoadingStartedHandler(SceneEntranceScriptableObject sceneEntrance)
    {
        FadeOutAllAudioSources();
        //KillAllEffectsAudioSources();
    }

    private void OnGameplaySceneLoadedHandler(SceneEntranceScriptableObject sceneEntrance)
    {
        if (_ambientAudioSourceMaster.AudioSource.clip == sceneEntrance.SceneConfig.AmbientAudioClip)
        {
            _ambientAudioSourceMaster.FadeIn(_config.FadeDuration, _musicVolume);
            return;
        }

        SetAmbientAudioClip(sceneEntrance.SceneConfig.AmbientAudioClip);
    }

    private void KillAllEffectsAudioSources()
    {
        foreach (var audioSourceMaster in _effectAudioSourcesMasters)
        {
            if (audioSourceMaster != null && audioSourceMaster.AudioSource.loop)
            {
                Object.Destroy(audioSourceMaster.gameObject); 
            }
        }
    }

    private AudioSourceMaster InstantiateAmbientAudioSource(AudioClip clip, bool isLoop)
    {
        if (_ambientAudioSourceMaster != null)
        {
            Object.Destroy(_ambientAudioSourceMaster.gameObject);
        }

        GameObject instantiatedGameObject = Object.Instantiate(_config.BackgroundAudioSourcePrefab, _parent);
        AudioSourceMaster master = instantiatedGameObject.GetComponent<AudioSourceMaster>();
        master.AudioSource.loop = isLoop;
        master.AudioSource.volume = _musicVolume;

        if (clip != null)
        {
            master.AudioSource.clip = clip;
            master.AudioSource.Play();

            if (!isLoop)
            {
                Object.Destroy(instantiatedGameObject, clip.length);
            }
        }

        _ambientAudioSourceMaster = master;
        _ambientAudioSourceMaster.gameObject.name = "Ambient Audio Source";
        _ambientAudioSourceMaster.AudioSource.ignoreListenerPause = true;

        return master;
    }

    private AudioSourceMaster InstantiateMusicAudioSource(AudioClip clip, bool isLoop)
    {
        if (_musicAudioSourceMaster != null)
        {
            Object.Destroy(_musicAudioSourceMaster.gameObject);
        }

        GameObject instantiatedGameObject = Object.Instantiate(_config.BackgroundAudioSourcePrefab, _parent);
        AudioSourceMaster master = instantiatedGameObject.GetComponent<AudioSourceMaster>();
        master.AudioSource.loop = isLoop;
        master.AudioSource.volume = _musicVolume;

        if (clip != null)
        {
            master.AudioSource.clip = clip;
            master.AudioSource.Play();

            if (!isLoop)
            {
                Object.Destroy(instantiatedGameObject, clip.length);
            }
        }

        _musicAudioSourceMaster = master;
        _musicAudioSourceMaster.gameObject.name = "Music Audio Source";
        _musicAudioSourceMaster.AudioSource.ignoreListenerPause = true;

        return master;
    }

    public AudioSourceMaster Instantiate3dEffectAudioSource(AudioClip clip, Vector3 position, bool isLoop = false, bool ignoreAudioListenerPause = false)
    {
        if (clip != null)
        {
            GameObject instantiatedGameObject = Object.Instantiate(_config.EffectAudioSourcePrefab, position, Quaternion.identity, _parent);
            AudioSourceMaster master = instantiatedGameObject.GetComponent<AudioSourceMaster>();
            master.AudioSource.loop = isLoop;
            master.AudioSource.volume = _effectsVolume;
            master.AudioSource.ignoreListenerPause = ignoreAudioListenerPause;

            master.AudioSource.clip = clip;
            master.AudioSource.Play();

            if (!isLoop)
            {
                master.OnDestroyed += OnEffectAudioSourceMasterDestroyedHandler;
                Object.Destroy(instantiatedGameObject, clip.length);
            }

            int nullIndex = -1;
            for (int i = 0; i < _effectAudioSourcesMasters.Count; i++)
            {
                if (_effectAudioSourcesMasters[i] == null) nullIndex = i;
            }

            if (nullIndex != -1)
            {
                _effectAudioSourcesMasters[nullIndex] = master;
            }
            else
            {
                _effectAudioSourcesMasters.Add(master);
            }

            return master;
        }

        return null;
    }

    public AudioSourceMaster Instantiate2dAudioSource(AudioClip clip, bool isLoop = false, bool ignoreAudioListenerPause = false)
    {
        if (clip != null)
        {
            GameObject instantiatedGameObject = Object.Instantiate(_config.BackgroundAudioSourcePrefab, _parent);
            AudioSourceMaster master = instantiatedGameObject.GetComponent<AudioSourceMaster>();
            master.AudioSource.loop = isLoop;
            master.AudioSource.ignoreListenerPause = ignoreAudioListenerPause;
            master.AudioSource.volume = _effectsVolume;

            master.AudioSource.clip = clip;
            master.AudioSource.Play();
            if (!isLoop)
            {
                master.OnDestroyed += OnEffectAudioSourceMasterDestroyedHandler;
                Object.Destroy(instantiatedGameObject, clip.length);
            }

            int nullIndex = -1;
            for (int i = 0; i < _effectAudioSourcesMasters.Count; i++)
            {
                if (_effectAudioSourcesMasters[i] == null) nullIndex = i;
            }

            if (nullIndex != -1)
            {
                _effectAudioSourcesMasters[nullIndex] = master;
            }
            else
            {
                _effectAudioSourcesMasters.Add(master);
            }

            return master;
        }

        return null;
    }

    public void SetAmbientAudioClip(AudioClip clip)
    {
        _ambientAudioSourceMaster.BlendAudioClips(clip, _config.FadeDuration, _config.BlendPause, _musicVolume);
    }

    public void SetMusicAudioClip(AudioClip clip)
    {
        _musicAudioSourceMaster.BlendAudioClips(clip, _config.FadeDuration, _config.BlendPause, _musicVolume);
    }

    public void SetMusicAudioClipAsDefault(AudioClip clip)
    {
        _musicAudioSourceMaster.BlendAudioClips(clip, _config.FadeDuration, _config.BlendPause, _musicVolume);
        _defaultMusicAudioClip = clip;
    }

    public void SetDefaultMusicAudioClip()
    {
        _musicAudioSourceMaster.BlendAudioClips(_defaultMusicAudioClip, _config.FadeDuration, _config.BlendPause, _musicVolume);
    }

    public void FadeOutAllAudioSources()
    {
        _ambientAudioSourceMaster?.FadeOut(_config.FadeDuration);
        _musicAudioSourceMaster?.FadeOut(_config.FadeDuration);
        foreach (AudioSourceMaster master in _effectAudioSourcesMasters)
        {
            if (master != null)
            {
                master.FadeOut(_config.FadeDuration); 
            }
        }
    }
}
