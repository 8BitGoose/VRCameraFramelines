using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
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

//	[Header("Hack")]
//	GameObject TextToDisplay;

	[Header("Settings (Must restart scene to take effect)")]

	[Tooltip("(GRIDLINES) will show in the VR headset, but not on the computer screen. This requires an extra camera, so it is less performant. " +
		"(FRAME ONLY) will show exactly what the player sees without another camera, but you lose the gridlines in the shot." +
		"(NOTE) With 'FrameOnly' selected, IsMiddleActive, ShowGridLines, and Show Centre Reticle do nothing.")]
	public FrameType FrameTypeSettings;

	[Tooltip("The layer on which the framelines appear, your main camera must cull this so it doesn't show on the screen")]
	public int FrameLineLayer = 29;
	[Tooltip("The layer that will ONLY appear in the headset. Useful if you want reminder points or other graphics that aren't shown on the main screen.")]
	public int OnlyHeadsetLayer = 28;



	[Tooltip("The ratio you would display on the main computer screen for recording.")]
	public AspectRatio Ratio;

	[Tooltip("Blacks out the outside of the view in the VR headset for a focused view on what is being recorded.")]
	public bool BlackoutOutsideFrame = true;

	[Tooltip("Shows a horizon level at the top of the VR player view.")]
	public bool TopHorizonActive = true;

	[Tooltip("Shows a horizon level at the bottom of the VR player view.")]
	public bool BottomHorizonActive = true;

	[Header("Gridlines (Does NOT work in Frame Only)")]
	public bool SmoothCamera = true;
	public float SmoothMoveRate = 8f;
	public float SmoothRotateRate = 7f;

	[Tooltip("Shows framlines within the view of the main camera. Disabled in 'FrameOnly' mode.")]
	public GridLines ShowGridLines = GridLines.All;

	[Tooltip("Shows a horizon level in the middle of the VR player view (not shown externally).")]
	public bool CenterHorizonActive = true;

	[Tooltip("Shows an aiming reticle for the middle of the VR player view (not shown externally).")]
	public bool ShowCenterReticle = true;

	[Space]

	[Header("In Game Notes (Gridlines or FrameOnly)")]


	[Tooltip("Shows a note outside of the view at the TOP for the player to read during the recording")]
	[TextArea(2, 2)]
	public string TopNote;
	[Tooltip("Shows a note outside of the view at the BOTTOM for the player to read during the recording")]
	[TextArea(2, 2)]
	public string BottomNote;

//	[Header("Overlay Notes (Gridlines Only)")]
	[Tooltip("Shows a big list of notes on the main screen for using notes during the recording")]
	[TextArea(5, 2)]
	public string OnScreenNote;
	public Color TextColor = Color.white;
//	public float TextSize;

	[Tooltip("If you need to disable any components on load for the player's VR view. For example, disabling a special renderer for performance reasons.")]
	public MonoBehaviour[] ComponentsToDisableOnLoad = new MonoBehaviour[0];

	[Tooltip("Objects to load on load in case you need placeholders that will only show in the player's headset.")]
	public GameObject[] ObjectsToEnableOnLoad;

//	[Header("Prefabs")]
	[Tooltip("If you need special things attached to your desktop camera, copy the prefab in the 'Resources' folder to a new folder and add the prefab here.")]
	public GameObject DisplayCameraPrefab;

	// Runtime
	private GameObject framelinesPrefab;
	private GameObject CurrentFrameLines = null;
	private VRCameraFrameObject ctrl = null;
	private VRCameraHideRef[] hiddenFromDisplayView  = null;
	private VRCameraSmooth camSmooth;
	private TextMesh headsetViewTextMesh;
	private Color lastColor;

	void Awake()
	{
		if(FrameLineLayer > 31 || FrameLineLayer < 1)
		{
			Debug.LogError("Frameline layer on VR Camera Frameline set to " + FrameLineLayer +". This value must be between 1 and 32. Can not be 0");
			FrameLineLayer = 31;
		}
	}

	void Start()
	{
		switch (this.FrameTypeSettings)
		{
		case FrameType.Gridlines:
			this.InitializeInViewGridlines();
			// Moves some items to the line only
			hiddenFromDisplayView = GameObject.FindObjectsOfType<VRCameraHideRef>();
			for(int i = 0; i < hiddenFromDisplayView.Length; i++)
			{
				hiddenFromDisplayView[i].EnableMeshes();
				hiddenFromDisplayView[i].gameObject.layer = this.OnlyHeadsetLayer;
			}
			break;

		case FrameType.FrameOnly:
			this.InitializeFrameOnly();
			// Hides items that were supposed to be hidden if you were using two cameras
			hiddenFromDisplayView = GameObject.FindObjectsOfType<VRCameraHideRef>();
			for(int i = 0; i < hiddenFromDisplayView.Length; i++)
				hiddenFromDisplayView[i].DisableMeshes();
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

		CreateTextToDisplay(this.TopNote, true);
		CreateTextToDisplay(this.BottomNote, false);

		CreateOnScreenText(this.OnScreenNote);
	}

	void Update()
	{
		if(camSmooth != null)
		{
			camSmooth.LerpPositionRate = this.SmoothMoveRate;
			camSmooth.LerpRotationRate = this.SmoothRotateRate;
		}
		if(this.headsetViewTextMesh != null)
		{
			if(this.TextColor != this.lastColor)
			{
				headsetViewTextMesh.color = this.TextColor;
				this.lastColor = this.TextColor;
			}
		}
	}

	private void CreateTextToDisplay(string text, bool IsTop)
	{
		if(string.IsNullOrEmpty(text))
			return;
		
		GameObject newDisplayItem = new GameObject();
		MeshRenderer renderer = newDisplayItem.AddComponent<MeshRenderer>();
		TextMesh textMesh = newDisplayItem.AddComponent<TextMesh>();
		textMesh.text = text;
		newDisplayItem.name = "Bottom Note Display";
		if(IsTop)
			newDisplayItem.name = "Top Note Display";

		textMesh.fontSize = 500;
		textMesh.characterSize = 4;

		textMesh.alignment = TextAlignment.Center;
		textMesh.anchor = TextAnchor.LowerCenter;
		if(IsTop == false)
			textMesh.anchor = TextAnchor.UpperCenter;

		newDisplayItem.transform.SetParent(CurrentFrameLines.transform);
		newDisplayItem.transform.localPosition = new Vector3(-.7f, 4.68f, 8.62f);
		if(IsTop == false)
			newDisplayItem.transform.localPosition = new Vector3(-.7f, -4.68f, 8.62f);
		newDisplayItem.transform.localRotation = Quaternion.identity;
		newDisplayItem.transform.localScale = Vector3.one * 0.002f;
		SetLayer(newDisplayItem.transform, this.FrameLineLayer);
	}

	private void CreateOnScreenText(string text)
	{
		if(string.IsNullOrEmpty(text))
			return;

		if(this.FrameTypeSettings == FrameType.FrameOnly)
			return;

		GameObject newDisplayItem = new GameObject();
		MeshRenderer renderer = newDisplayItem.AddComponent<MeshRenderer>();
		headsetViewTextMesh = newDisplayItem.AddComponent<TextMesh>();
		headsetViewTextMesh.text = text;
		newDisplayItem.name = "On Screen Display";

		headsetViewTextMesh.fontSize = 375;
		headsetViewTextMesh.characterSize = 4;

		headsetViewTextMesh.alignment = TextAlignment.Left;
		headsetViewTextMesh.anchor = TextAnchor.MiddleLeft;
		headsetViewTextMesh.color = this.TextColor;

		newDisplayItem.transform.SetParent(CurrentFrameLines.transform);
		newDisplayItem.transform.localPosition = new Vector3(-5.25f, 0.22f, 8.62f);
		newDisplayItem.transform.localRotation = Quaternion.identity;
		newDisplayItem.transform.localScale = Vector3.one * 0.002f;
		SetLayer(newDisplayItem.transform, this.FrameLineLayer);
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
		if(this.framelinesPrefab == null)
			this.framelinesPrefab = Resources.Load<GameObject>("VRCameraFrameLinesFast");

		if(CurrentFrameLines != null)
			GameObject.Destroy(CurrentFrameLines);

		GameObject frameLines = GameObject.Instantiate(this.framelinesPrefab);

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
		displayCam.transform.localPosition = Vector3.zero;
		displayCam.transform.localRotation = Quaternion.identity;
		displayCam.transform.SetParent(this.transform.parent);

		// Option to disable components on the object
		if(this.ComponentsToDisableOnLoad != null)
		{
			for(int i = 0; i < ComponentsToDisableOnLoad.Length; i++)
			{
				if(ComponentsToDisableOnLoad[i] != null)
					ComponentsToDisableOnLoad[i].enabled = false;
			}
		}

		if(this.ObjectsToEnableOnLoad != null)
		{
			for(int i = 0; i < ObjectsToEnableOnLoad.Length; i++)
				ObjectsToEnableOnLoad[i].SetActive(true);
		}

		// this MAY have broken something. If this doesn't work, change this back
//		Camera noDisplayFramesCam = DisplayCameraPrefab.GetComponent<Camera>();
		Camera noDisplayFramesCam = displayCam.GetComponent<Camera>();

		// Make the main display that has been doubled for the player not display the framelines
		noDisplayFramesCam.cullingMask = ~( 1 << (this.FrameLineLayer));
		noDisplayFramesCam.cullingMask &= ~( 1 << (this.OnlyHeadsetLayer));

		camSmooth = displayCam.AddComponent<VRCameraSmooth>();
		camSmooth.cameraSelf = noDisplayFramesCam;
		camSmooth.cameraTarget = this.GetComponent<Camera>();
		camSmooth.enableSmooth = this.SmoothCamera;
		camSmooth.LerpPositionRate = this.SmoothMoveRate;
		camSmooth.LerpRotationRate = this.SmoothRotateRate;
	}

	private VRCameraFrameObject CreateVRFrameLines()
	{
		if(this.framelinesPrefab == null)
			this.framelinesPrefab = Resources.Load<GameObject>("VRCameraFrameLines");

		if(CurrentFrameLines != null)
		{
			GameObject.Destroy(CurrentFrameLines);
		}

		GameObject frameLines = GameObject.Instantiate(this.framelinesPrefab);

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
