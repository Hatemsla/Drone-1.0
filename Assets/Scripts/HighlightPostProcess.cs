using UnityEngine;

namespace DroneFootball
{
    public class HighlightPostProcess : MonoBehaviour
    {
        private void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            Graphics.Blit(source, destination);
        }
    }
}