using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VRCameraFrameLines))]
public class VRCameraEditor : Editor 
{
	SerializedProperty ComponentsToDisableOnLoad, ObjectsToEnableOnLoad, DisplayCameraPrefab;//, OnScreenNote;
	private int defaultLayer = 29;

	void OnEnable()
	{
		// Fetch the objects from the GameObject script to display in the inspector
		ComponentsToDisableOnLoad = serializedObject.FindProperty("ComponentsToDisableOnLoad");
		ObjectsToEnableOnLoad = serializedObject.FindProperty("ObjectsToEnableOnLoad");
		DisplayCameraPrefab = serializedObject.FindProperty("DisplayCameraPrefab");
//		OnScreenNote = serializedObject.FindProperty("OnScreenNote");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		VRCameraFrameLines script = (VRCameraFrameLines)target;

		GUIStyle boldStyle = new GUIStyle();
		boldStyle.fontStyle = FontStyle.Bold;

		EditorGUILayout.Space();

		// Sets what mode we are in!

		int selection = 0;
		if(script.FrameTypeSettings == VRCameraFrameLines.FrameType.FrameOnly)
			selection = 1;
			

		selection = GUILayout.SelectionGrid(selection, new string[] { "All Options", "Performance" }, 2);

		if(selection == 1)
			script.FrameTypeSettings = VRCameraFrameLines.FrameType.FrameOnly;
		else
			script.FrameTypeSettings = VRCameraFrameLines.FrameType.Gridlines;

		if(script.FrameTypeSettings == VRCameraFrameLines.FrameType.Gridlines)
		{
			EditorGUILayout.LabelField("All Options enables everything for the VR Camera Lines system. However, it creates a separate camera which is a hit on performance. " +
				"The camera will display at the native screen display. If you have special conditions for your rendering camera (such as attaching new shaders), " +
				"you should create a prefab and add it to 'Display Camera Prefab'.", EditorStyles.helpBox);
		}
		else
		{
			EditorGUILayout.LabelField("Performance records with the same camera as the player is using and so some options are disabled (or they would show up in the view)" +
				"The camera will display at the native render of your VR headset, usually lower quality.", EditorStyles.helpBox);
		}

		// Show the stuff that works for both modes
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PrefixLabel(new GUIContent("Frame Line Layer", "The layer on which the framelines appear, your main camera must cull this so it doesn't show on the screen. Default is layer 29. If left blank, will default to layer 29."));
		script.FrameLineLayer = EditorGUILayout.LayerField(script.FrameLineLayer);
		if(script.FrameLineLayer < 8)
			script.FrameLineLayer = defaultLayer;
		EditorGUILayout.EndHorizontal();

		script.Ratio = (VRCameraFrameLines.AspectRatio)EditorGUILayout.EnumPopup(
			new GUIContent("Recording Ratio", "The ratio displayed on the main computer screen for recording."),
			script.Ratio);
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Headset Assistant", boldStyle);

		if(script.FrameTypeSettings == VRCameraFrameLines.FrameType.Gridlines)
		{
			script.ShowGridLines = (VRCameraFrameLines.GridLines)EditorGUILayout.EnumPopup(
				new GUIContent("Gridlines", "Shows framlines within the view of the main camera. Disabled in 'FrameOnly' mode."),
				script.ShowGridLines);			
		}

		script.BlackoutOutsideFrame = EditorGUILayout.Toggle(
			new GUIContent("Blackout Frame", "Adds a black mask to the outside of the view in the VR headset for a focused view on what is being recorded."),
			script.BlackoutOutsideFrame);

		script.TopHorizonActive = EditorGUILayout.Toggle(
			new GUIContent("Top Horizon", "Shows a horizon level at the top of the VR player view."),
			script.TopHorizonActive);

		if(script.FrameTypeSettings == VRCameraFrameLines.FrameType.Gridlines)
		{
			script.CenterHorizonActive = EditorGUILayout.Toggle(
				new GUIContent("Center Horizon", "Shows a horizon level in the middle of the view."),
				script.CenterHorizonActive);
		}
		
		script.BottomHorizonActive = EditorGUILayout.Toggle(
			new GUIContent("Bottom Horizon", "Shows a horizon level at the bottom of the VR player view."),
			script.BottomHorizonActive);
		
		if(script.FrameTypeSettings == VRCameraFrameLines.FrameType.Gridlines)
		{
			script.ShowCenterReticle = EditorGUILayout.Toggle(
				new GUIContent("Center Reticle", "Shows an aiming reticle for the middle of the VR player view (not shown externally)."),
				script.ShowCenterReticle);
		}

		EditorGUILayout.Space();

		if(script.FrameTypeSettings == VRCameraFrameLines.FrameType.Gridlines)
		{
			EditorGUILayout.LabelField("Smooth Camera", boldStyle);

			script.SmoothCamera = EditorGUILayout.Toggle(
				new GUIContent("Enabled", "Adds camera smoothing for the external desktop view"),
				script.SmoothCamera);

			if(script.SmoothCamera)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(new GUIContent("Smooth Move Rate", "Determines how quickly the camera will 'catch up' to the headset's position. Lower number is smoother."));
				script.SmoothMoveRate = GUILayout.HorizontalSlider(script.SmoothMoveRate, 3, 12);
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PrefixLabel(new GUIContent("Smooth Rotate Rate", "Determines how quickly the camera will 'catch up' to the headset's rotation. Lower number is smoother."));
				script.SmoothRotateRate = GUILayout.HorizontalSlider(script.SmoothRotateRate, 3, 12);
				EditorGUILayout.EndHorizontal();
			}
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField(new GUIContent ("Top Note", "Shows a note outside of the view at the top for the player to read during the recording."), boldStyle);
		script.TopNote = EditorGUILayout.TextArea(script.TopNote);
		EditorGUILayout.Space();

		EditorGUILayout.LabelField(new GUIContent ("Bottom Note", "Shows a note outside of the view at the BOTTOM for the player to read during the recording."), boldStyle);
		script.BottomNote = EditorGUILayout.TextArea(script.BottomNote);
		EditorGUILayout.Space();

		if(script.FrameTypeSettings == VRCameraFrameLines.FrameType.Gridlines)
		{
			GUIStyle newAreaThing = EditorStyles.textArea;
			EditorGUILayout.LabelField(new GUIContent ("Headset View Notes", "Shows a list of notes on the VR headset while in use."), boldStyle);
			script.OnScreenNote = EditorGUILayout.TextArea(script.OnScreenNote, newAreaThing);
			script.TextColor = EditorGUILayout.ColorField(new GUIContent("Text Color", "Changes the color of the text in the headset view list. Does not affect the top or bottom note color."), 
				script.TextColor);
			EditorGUILayout.Space();
		}

		if(script.FrameTypeSettings == VRCameraFrameLines.FrameType.Gridlines)
		{
			EditorGUILayout.PropertyField(ComponentsToDisableOnLoad, true);
			EditorGUILayout.PropertyField(ObjectsToEnableOnLoad, true);
			EditorGUILayout.Space();
			EditorGUILayout.PropertyField(DisplayCameraPrefab, true);
		}

		EditorGUILayout.Space();

		if(GUI.changed)
			EditorUtility.SetDirty(target);
		serializedObject.ApplyModifiedProperties();

//		DrawDefaultInspector();
	}
}