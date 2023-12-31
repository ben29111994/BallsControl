// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;

namespace FluffyUnderware.Curvy.Examples
{
    public class CameraLook : MonoBehaviour
    {

        [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 1.5f; 

        protected void Update()
        {
            if (Time.timeScale < float.Epsilon)
                return;

            // Read the user input
            var x = Input.GetAxis("Mouse X");
            var y = -Input.GetAxis("Mouse Y");

            transform.Rotate(y * m_TurnSpeed,0,0, Space.Self);
            transform.Rotate(0, x * m_TurnSpeed, 0,Space.World);

           
        }
        
    }
}
