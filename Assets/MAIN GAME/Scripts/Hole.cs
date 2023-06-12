using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FluffyUnderware.Curvy;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.Curvy.Controllers;
using UnityEngine.UI;

public class Hole : MonoBehaviour
{
    public GameObject effectHole;
    public GameObject effectBall;
    public Text standardText;
    public int standard;
    public int current;
    public static Hole instance;
    public GameObject coin;
    bool isDetect = true;

    void OnEnable() {
        instance = this;
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Ball") && isDetect)
        {
            other.GetComponent<SplineController>().Spline = null;
            other.transform.DOMove(new Vector3(transform.position.x, 0, transform.position.z), 0.1f);
            transform.DOKill();
            transform.localScale = Vector3.one * 10;
            transform.DOScale(new Vector3(15, 10, 15), 0.2f).SetLoops(2, LoopType.Yoyo);
            var eHole = Instantiate(effectHole, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
            eHole.GetComponent<ParticleSystem>().startColor = other.GetComponent<Renderer>().material.color;
            other.transform.DOScale(Vector3.zero, 0.25f);
            Destroy(other.gameObject, 0.25f);
            // standard--;
            current++;
            if(current > standard)
            {
                // standardText.text = "+" + Mathf.Abs(standard).ToString();
                GameController.instance.score++;
                GameController.instance.status.text = GameController.instance.score.ToString();
                GameController.instance.status.transform.DOKill();
                GameController.instance.status.transform.localScale = Vector3.one;
                GameController.instance.status.transform.DOPunchScale(Vector3.one * 0.5f, 0.2f);
                var temp = Instantiate(coin, transform.position, Quaternion.identity);
                temp.transform.DOMoveY(Random.Range(5, 15), 0.5f).SetLoops(2, LoopType.Yoyo);
                Destroy(temp, 0.75f);
            }
            standardText.text = current.ToString() + "/" + standard.ToString();

            GameController.instance.RemoveBall(other.gameObject);
            isDetect = false;
            StartCoroutine(delayDrop());
        }
    }

    IEnumerator delayDrop()
    {
        yield return new WaitForSeconds(0.03f);
        isDetect = true;
    }
}
