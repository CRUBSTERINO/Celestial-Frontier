using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private Image _loadingCircleImage;

    public async void VisualizeSceneLoadingOperation(AsyncOperation loadingTask)
    {
        while (!loadingTask.isDone)
        {
            _loadingCircleImage.fillAmount = loadingTask.progress;
            await UniTask.Yield(); 
        }
    }
}
