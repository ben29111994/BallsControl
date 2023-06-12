using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrignometricRotation : MonoBehaviour
{
    public Vector3 AngleLimit;
    public Vector3 RotationFrequency;
    Vector3 FinalRotation;
    Vector3 StartRotation;
    void Start()
    {
        StartRotation = transform.localEulerAngles;
    }
    void Update()
    {
        Debug.Log(Mathf.Sin(Time.timeSinceLevelLoad * RotationFrequency.x));
        FinalRotation.x = StartRotation.x + Mathf.Sin(Time.timeSinceLevelLoad * RotationFrequency.x) * AngleLimit.x;
        FinalRotation.y = StartRotation.y + Mathf.Sin(Time.timeSinceLevelLoad * RotationFrequency.y) * AngleLimit.y;
        FinalRotation.z = StartRotation.z + Mathf.Sin(Time.timeSinceLevelLoad * RotationFrequency.z) * AngleLimit.z;
        transform.localEulerAngles = new Vector3(FinalRotation.x, FinalRotation.y, FinalRotation.z);
    }
}
