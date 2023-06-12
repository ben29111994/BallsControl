using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrignometricScaling : MonoBehaviour {
	
	public Vector3 scaleLimit;
	public Vector3 scaleFrequency;
	Vector3 startScale;
	Vector3 finalScale;

	void Start () {
		startScale = transform.localScale;
	}
	
	void Update () {
		finalScale.x = startScale.x + Mathf.Sin(Time.timeSinceLevelLoad * scaleFrequency.x) * scaleLimit.x;
		finalScale.y = startScale.y + Mathf.Sin(Time.timeSinceLevelLoad * scaleFrequency.y) * scaleLimit.y;
		finalScale.z = startScale.z + Mathf.Sin(Time.timeSinceLevelLoad * scaleFrequency.z) * scaleLimit.z;
		transform.localScale = new Vector3 (finalScale.x,finalScale.y,finalScale.z);
	}
}
