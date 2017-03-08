using UnityEngine;
using System.Collections;

public class FilmingAidVRHorizon : MonoBehaviour 
{
	// Keeps the horizon upright

	private Transform trans;
	private Transform parentTrans;

	void Update()
	{
		if(trans == null || parentTrans == null)
		{
			trans = this.transform;
			parentTrans = trans.parent;
		}

		trans.localRotation = Quaternion.Euler(trans.localRotation.eulerAngles.x, trans.localRotation.eulerAngles.y, parentTrans.localRotation.eulerAngles.z);
	}
}
