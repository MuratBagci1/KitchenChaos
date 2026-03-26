using UnityEngine;

public class LoaderCallback : MonoBehaviour
{
    private bool oneFramePassed;

    private void Update()
    {
        if(oneFramePassed)
        {
            Loader.LoaderCallback(Loader.Scene.GameScene);
        }
        else
        {
            oneFramePassed = true;
        }
    }
}