using UnityEngine;

public class PopUpUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    private void Start()
    {
        DisablePopUp();
    }

    public void EnablePopUp()
    {
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
    }

    public void DisablePopUp()
    {
        _canvasGroup.alpha = 0f;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
    }
}
