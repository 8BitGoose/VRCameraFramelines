using UnityEngine;
using System.Collections;

public class VRCameraHideRef : MonoBehaviour 
{
	// ATTACH THIS TO ANYTHING YOU WANT TO BE HIDDEN IF YOU ARE NOT USING A TWO CAMERA SET UP

	public void EnableMeshes()
	{
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		renderer.enabled = true;
	}

	public void DisableMeshes()
	{
		MeshRenderer renderer = GetComponent<MeshRenderer>();
		renderer.enabled = false;
	}
}
