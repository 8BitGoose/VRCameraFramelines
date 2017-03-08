using UnityEngine;
using System.Collections;

public class VRCameraFrameLines : MonoBehaviour 
{
	public enum AspectRatio
	{
		HD16x9,
		HD16x10,
		SD4x3,
		SD3x2
	}

	public enum FrameType
	{
		Gridlines = 0,
		FrameOnly, // Faster than Gridlines
	}

	public enum GridLines
	{
		All = 0,
		HorizontalOnly,
		VerticleOnly,
		None
	}

	[Header("Settings (Must restart scene to take effect)")]

	[Tooltip("(GRIDLINES) will show in the VR headset, but not on the computer screen. This requires an extra camera, so it is less performant. " +
		"(FRAME ONLY) will show exactly what the player sees without another camera, but you lose the gridlines in the shot." +
		"(NOTE) With 'FrameOnly' selected, IsMiddleActive, ShowGridLines, and Show Centre Reticle do nothing.")]
	public FrameType FrameTypeSettings;

	[Tooltip("The layer on which the framelines appear, your main camera must cull this so it doesn't show on the screen")]
	public int FrameLineLayer = 29;

	[Tooltip("The ratio you would display on the main computer screen for recording.")]
	public AspectRatio Ratio;

	[Tooltip("Shows framlines within the view of the main camera. Disabled in 'FrameOnly' mode.")]
	public GridLines ShowGridLines = GridLines.All;

	[Tooltip("Shows a horizon level at the top of the VR headset.")]
	public bool TopHorizonActive = true;

	[Tooltip("Shows a horizon level in the middle of the VR headset.")]
	public bool CenterHorizonActive = true;

	[Tooltip("Shows a horizon level at the bottom of the VR headset.")]
	public bool BottomHorizonActive = true;

	[Tooltip("Shows an aiming reticle for the middle of the VR headset.")]
	public bool ShowCenterReticle = true;

	[Tooltip("Blacks out the outside of the view in the VR headset for a focused view on what is being recorded.")]
	public bool BlackoutOutsideFrame = true;

	public MonoBehaviour[] ComponentsToDisableOnLoad = new MonoBehaviour[0];

//	[Header("Runtime")]
	private GameObject CurrentFrameLines = null;
	private VRCameraFrameObject ctrl = null;

	[Header("Prefabs")]
	public GameObject FramelinesPrefab;
	[Tooltip("If you need special things attached to your regular camera, copy the prefab in the Resources folder to a new folder and add the prefab here. Not used with Frame Only setting.")]
	public GameObject DisplayCameraPrefab;

	void Awake()
	{
		if(this.isActiveAndEnabled == false)
		{
			VRCameraHideRef[] objs = GameObject.FindObjectsOfType<VRCameraHideRef>();
			for(int i = 0; i < objs.Length; i++)
				objs[i].gameObject.SetActive(false);
		}
	}

	void Start()
	{
		switch (this.FrameTypeSettings)
		{
		case FrameType.Gridlines:
			this.InitializeInViewGridlines();
			break;

		case FrameType.FrameOnly:
			this.InitializeFrameOnly();
			// Hides items that were supposed to be hidden if you were using two cameras
			VRCameraHideRef[] objs = GameObject.FindObjectsOfType<VRCameraHideRef>();
			for(int i = 0; i < objs.Length; i++)
				objs[i].gameObject.SetActive(false);
			break;

		default:
			break;
		}

		if(CurrentFrameLines != null)
		{
			switch(this.Ratio)
			{
			case AspectRatio.HD16x9:
				break;

			case AspectRatio.HD16x10:
				CurrentFrameLines.transform.localScale = new Vector3(1, 1.115f, 1);
				break;

			case AspectRatio.SD4x3:
				CurrentFrameLines.transform.localScale = new Vector3(1, 1.35f, 1);
				break;

			case AspectRatio.SD3x2:
				CurrentFrameLines.transform.localScale = new Vector3(1, 1.19f, 1);
				break;
			}
		}

		// set the layers of the frameline to the desired layer
		SetLayer(CurrentFrameLines.transform, this.FrameLineLayer);

		Camera steamVRCamera = this.GetComponentInChildren<Camera>();
		steamVRCamera.cullingMask &= ~(1 << this.FrameLineLayer);
	}

	public static void SetLayer(Transform trans, int layer) 
	{
		trans.gameObject.layer = layer;
		foreach(Transform child in trans)
			SetLayer(child, layer);
	}

	#region Frame Only

	void InitializeFrameOnly()
	{
		if(this.FramelinesPrefab == null)
			this.FramelinesPrefab = Resources.Load<GameObject>("VRCameraFrameLinesFast");

		if(CurrentFrameLines != null)
			GameObject.Destroy(CurrentFrameLines);

		GameObject frameLines = GameObject.Instantiate(this.FramelinesPrefab);

		frameLines.name = "VR Camera Frame Lines";

		frameLines.transform.SetParent(this.transform.parent);
		frameLines.transform.localPosition = this.transform.localPosition;
		frameLines.transform.localRotation = this.transform.localRotation;

		Camera cam = frameLines.GetComponent<Camera>();
		if(cam != null)
			cam.targetDisplay = 0;
		cam.cullingMask = (1 << (this.FrameLineLayer));
		cam.gameObject.layer = this.FrameLineLayer;
		
		ctrl = frameLines.GetComponent<VRCameraFrameObject>();

		ctrl.SetHorizonsActive(this.CenterHorizonActive, this.BottomHorizonActive, this.TopHorizonActive);
		ctrl.ShowGridLines(this.ShowGridLines);
		ctrl.ShowReticle(this.ShowCenterReticle);
		ctrl.SetBlackout(this.BlackoutOutsideFrame);

		CurrentFrameLines = frameLines;
	}

	#endregion

	#region VR On Screen Framelines

	private void InitializeInViewGridlines()
	{
		ctrl = CreateVRFrameLines();

		ctrl.transform.parent = this.gameObject.transform;

		if(DisplayCameraPrefab == null)
			DisplayCameraPrefab = Resources.Load<GameObject>("MainDisplayCamera");

		GameObject displayCam = GameObject.Instantiate(DisplayCameraPrefab) as GameObject;
		displayCam.transform.SetParent(this.transform);
		displayCam.transform.localPosition = Vector3.zero;// + new Vector3(-0.044f, 0, 0);
		displayCam.transform.localRotation = Quaternion.identity;

		// Option to disable components on the object
		if(this.ComponentsToDisableOnLoad != null)
		{
			for(int i = 0; i < ComponentsToDisableOnLoad.Length; i++)
				ComponentsToDisableOnLoad[i].enabled = false;
		}

		Camera noDisplayFramesCam = DisplayCameraPrefab.GetComponent<Camera>();

		// Make the main display that has been doubled for the player not display the framelines
		noDisplayFramesCam.cullingMask = ~( 1 << (this.FrameLineLayer));
	}

	private VRCameraFrameObject CreateVRFrameLines()
	{
		if(this.FramelinesPrefab == null)
			this.FramelinesPrefab = Resources.Load<GameObject>("VRCameraFrameLines");

		if(CurrentFrameLines != null)
		{
			GameObject.Destroy(CurrentFrameLines);
		}

		GameObject frameLines = GameObject.Instantiate(this.FramelinesPrefab);

		frameLines.name = "VR Camera Frame Lines";

		frameLines.transform.SetParent(this.transform.parent);
		frameLines.transform.localPosition = this.transform.localPosition;
		frameLines.transform.localRotation = this.transform.localRotation;

		Camera cam = frameLines.GetComponent<Camera>();
		if(cam != null)
			cam.targetDisplay = 0;
		cam.cullingMask = (1 << this.FrameLineLayer);

		VRCameraFrameObject vrFrameLines = frameLines.GetComponent<VRCameraFrameObject>();

		vrFrameLines.SetHorizonsActive(this.CenterHorizonActive, this.BottomHorizonActive, this.TopHorizonActive);
		vrFrameLines.ShowGridLines(this.ShowGridLines);
		vrFrameLines.ShowReticle(this.ShowCenterReticle);
		vrFrameLines.SetBlackout(this.BlackoutOutsideFrame);

		CurrentFrameLines = frameLines;

		return vrFrameLines;
	}

	#endregion
}
