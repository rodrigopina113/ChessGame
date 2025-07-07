using UnityEngine;

public class WebGLCameraFix : MonoBehaviour
{
    void Start()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        // Reset camera properties that might have been set by WebGLBackgroundVideo
        if (Camera.main != null)
        {
            Camera.main.clearFlags = CameraClearFlags.Skybox; // or CameraClearFlags.SolidColor with proper background
            Camera.main.backgroundColor = Color.black; // or your desired background color
            Debug.Log("[WebGLCameraFix] Reset camera properties for WebGL");
        }
        #endif
    }
}