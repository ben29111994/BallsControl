using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class Vibrate : MonoBehaviour
{
    bool isVibrate = false;

    void OnCollisionEnter(Collision other) {
        if (!UnityEngine.iOS.Device.generation.ToString().Contains("5") && !isVibrate)
        {
            isVibrate = true;
            StartCoroutine(delayVibrate());
            MMVibrationManager.Haptic(HapticTypes.LightImpact);
        }
    }

    IEnumerator delayVibrate()
    {
        yield return new WaitForSeconds(0.2f);
        isVibrate = false;
    }
}
