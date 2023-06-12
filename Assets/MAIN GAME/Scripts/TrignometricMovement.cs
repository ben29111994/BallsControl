using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrignometricMovement : MonoBehaviour
{
    public Vector3 Distance;
    public Vector3 MovementFrequency;
    Vector3 FinalPosition;
    Vector3 StartPosition;
    void Start()
    {
        StartPosition = transform.position;
    }
    void Update()
    {
        FinalPosition.x = StartPosition.x + Mathf.Sin(Time.timeSinceLevelLoad * MovementFrequency.x) * Distance.x;
        FinalPosition.y = StartPosition.y + Mathf.Sin(Time.timeSinceLevelLoad * MovementFrequency.y) * Distance.y;
        FinalPosition.z = StartPosition.z + Mathf.Sin(Time.timeSinceLevelLoad * MovementFrequency.z) * Distance.z;
        transform.position = new Vector3(FinalPosition.x, FinalPosition.y, FinalPosition.z);
    }
}
