/// USED WITH PERMISION FROM https://gist.github.com/kalineh/1eccb28c600ae98516ea577c0afb9339

using UnityEngine;
using System.Collections;

public class VRCameraSmooth : MonoBehaviour
{
	[Header("Settings")]
	public float CameraFOV = 54; // 111.8125 is the default FOV for the player
	[Range(0.0f, 12.0f)]
	public float LerpPositionRate = 8f;
	[Range(1.0f, 12.0f)]
	public float LerpRotationRate = 7f;

	[Header("References")]
    public Camera cameraTarget;
    public Camera cameraSelf;
    public bool enableSmooth = true;

    public void Start()
    {
        if (cameraSelf == null)
            cameraSelf = GetComponent<Camera>();

		if(cameraTarget == null)
		{
			Debug.LogError("Camera target is not set, please set. Disabling script");
			this.enabled = false;
			return;
		}

        // just make sure smooth camera set to None (Main Display)
        // vive will render the both eyes camera, and main game window will show smooth
        cameraSelf.stereoTargetEye = StereoTargetEyeMask.None;
        cameraSelf.targetDisplay = 0;
		cameraSelf.fieldOfView = this.CameraFOV; // GameSettings.Instance.OptionEnableCameraSmoothingFOV;

        cameraSelf.nearClipPlane = cameraTarget.nearClipPlane;
        cameraSelf.farClipPlane = cameraTarget.farClipPlane;
        cameraSelf.transform.position = cameraTarget.transform.position;
        cameraSelf.transform.rotation = cameraTarget.transform.rotation;

        cameraTarget.targetDisplay = 0;
    }

    public void FixedUpdate()
    {
		if (cameraTarget == null)
            return;

        float posRate = LerpPositionRate;
		float rotRate = LerpRotationRate;

        if (enableSmooth)
        {
            transform.position = Vector3.Lerp(transform.position, cameraTarget.transform.position, Mathf.Clamp01(posRate * Time.fixedDeltaTime));
            transform.rotation = Quaternion.Slerp(transform.rotation, cameraTarget.transform.rotation, Mathf.Clamp01(rotRate * Time.fixedDeltaTime));
        }
        else
        {
            transform.position = cameraTarget.transform.position;
            transform.rotation = cameraTarget.transform.rotation;
        }
    }
}