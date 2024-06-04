using UnityEngine;

public class PlayerSoundManager : MonoBehaviour
{
    [SerializeField] private PlayerController _controller;
    [SerializeField] private PlayerManager _playerManager;
    [Space]
    [SerializeField] private AudioClip _leftFootstepAudioClip;
    [SerializeField] private AudioClip _rightFootstepAudioClip;
    [SerializeField] private float _stepsDeltaTime;
    [Space]
    [SerializeField] private AudioClip _pickUpItemAudioClip;
    [SerializeField] private AudioClip _removeItemAudioClip;

    private AudioService _audioService;
    private bool _isRightStep;
    private float _elapsedDelta;
    private Inventory _inventory;

    private void Start()
    {
        _audioService = ServiceLocator.Instance.GetService<AudioService>();

        _inventory = _playerManager.Inventory;

        _inventory.OnItemAdded += PlayPickUpItemSound;
        _inventory.OnItemRemoved += PlayerRemoveItemSound;
    }

    private void OnDestroy()
    {
        _inventory.OnItemAdded -= PlayPickUpItemSound;
        _inventory.OnItemRemoved -= PlayerRemoveItemSound;
    }

    private void Update()
    {
        if (!_controller.IsMoving)
        {
            _elapsedDelta = _stepsDeltaTime;
            return;
        }

        _elapsedDelta += Time.deltaTime;

        if (_elapsedDelta >= _stepsDeltaTime)
        {
            AudioClip footstepClip = _isRightStep ? _rightFootstepAudioClip : _leftFootstepAudioClip;

            _audioService.Instantiate3dEffectAudioSource(footstepClip, transform.position, false);

            _isRightStep = !_isRightStep;
            _elapsedDelta = 0f;
        }
    }

    private void PlayPickUpItemSound()
    {
        _audioService.Instantiate3dEffectAudioSource(_pickUpItemAudioClip, transform.position, false);
    }

    private void PlayerRemoveItemSound()
    {
        _audioService.Instantiate3dEffectAudioSource(_removeItemAudioClip, transform.position, false);
    }
}
