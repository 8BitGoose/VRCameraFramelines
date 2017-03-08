using UnityEngine;
using System.Collections;

public class VRCameraFrameObject : MonoBehaviour 
{
	[Header("Levels")]
	public GameObject LowerHorizon;
	public GameObject LowerLevel;
	public GameObject UpperHorizon;
	public GameObject UpperLevel;
	public GameObject MiddleHorizon;
	public GameObject MiddleLevel;

	[Header("Other Display Stuff")]
	public GameObject Reticle;
	public GameObject HorizontalGridLines;
	public GameObject VerticleGridLines;
	public GameObject ThinFrameLines;
	public GameObject BackoutFrameLines;
	public GameObject PrinterGridlineMarks;

	public void SetHorizonsActive(bool isMiddle, bool isLower, bool isUpper)
	{
		if( MiddleHorizon != null)
		{
			if(isMiddle == false)
			{	
				MiddleHorizon.SetActive(false);
				MiddleLevel.SetActive(false);
			}
		}

		if(UpperHorizon != null && UpperLevel != null)
		{
			UpperLevel.SetActive(false);
			if(isUpper)
			{
				UpperLevel.SetActive(true);
			}
			else
			{
				FilmingAidVRHorizon aid = UpperHorizon.GetComponent<FilmingAidVRHorizon>();
				if(aid != null)
					aid.enabled = false;
			}
		}

		if(LowerHorizon != null && LowerLevel != null)
		{
			LowerLevel.SetActive(false);
			if(isLower)
			{
				LowerLevel.SetActive(true);
			}
			else
			{
				FilmingAidVRHorizon aid = LowerHorizon.GetComponent<FilmingAidVRHorizon>();
				if(aid != null)
					aid.enabled = false;
			}
		}
	}

	public void SetBlackout(bool showBlackout)
	{
		if(this.BackoutFrameLines != null)
			this.BackoutFrameLines.SetActive(showBlackout);
		
		if(this.ThinFrameLines != null)
			this.ThinFrameLines.SetActive(!showBlackout);

		if(this.PrinterGridlineMarks != null)
			this.PrinterGridlineMarks.SetActive(true);
		
	}

	public void ShowReticle(bool showReticle)
	{
		if(showReticle == false && this.Reticle != null)
		{
			Reticle.SetActive(false);
		}
	}

	public void ShowGridLines(VRCameraFrameLines.GridLines gridActive)
	{
		if(gridActive == VRCameraFrameLines.GridLines.HorizontalOnly || gridActive == VRCameraFrameLines.GridLines.None)
		{
			if(VerticleGridLines != null)
				VerticleGridLines.SetActive(false);
		}
		if(gridActive == VRCameraFrameLines.GridLines.VerticleOnly || gridActive == VRCameraFrameLines.GridLines.None)
		{
			if(HorizontalGridLines != null)
				HorizontalGridLines.SetActive(false);
		}
	}
}
