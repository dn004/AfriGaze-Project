using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;

public class SetupSphereAndVideo : MonoBehaviour
{
    public AssetReference videoToPlay;
    public VideoPlayer videoPlayer;
    public AsyncOperationHandle<VideoClip> operation;


    public void ShowEnvironment()
    {
        LoadAddressable(videoToPlay);
    }


    private void LoadAddressable(AssetReference reference)
    {
        operation = Addressables.LoadAssetAsync<VideoClip>(reference);
        operation.Completed += Operation_Completed;

    }

    private void Operation_Completed(AsyncOperationHandle<VideoClip> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            videoPlayer.clip = obj.Result;
            videoPlayer.Prepare();
        }
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.clip = null;
            operation.Release();
        }
    }


}
