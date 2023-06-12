using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraRatio : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Application.targetFrameRate = 60;
        float targetaspect = 9f / 16f;
        float windowaspect = (float)Screen.width / (float)Screen.height;
        float scaleheight = windowaspect / targetaspect;
        Camera camera = GetComponent<Camera>();
        //10:16
        if (windowaspect > 0.62f && windowaspect < 0.63f)
        {
            camera.fieldOfView = 60;
        }
        //3:4
        else if (windowaspect > 0.74f && windowaspect < 0.76f)
        {
            camera.fieldOfView = 58;
        }
        //18:9
        else if (windowaspect > 0.49f && windowaspect < 0.51f)
        {
            camera.fieldOfView = 60;
        }
        //2:3
        else if (windowaspect > 0.65f && windowaspect < 0.67f)
        {
            camera.fieldOfView = 58;
        }
        //9:19
        else if (windowaspect > 0.46f && windowaspect < 0.48f)
        {
            camera.fieldOfView = 68;
        }
        //9:20
        else if (windowaspect > 0.449f && windowaspect < 0.451f)
        {
            camera.fieldOfView = 71;
        }
    }
}
