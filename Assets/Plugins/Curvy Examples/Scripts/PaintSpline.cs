// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.Curvy.Controllers;
using UnityEngine.UI;
using DG.Tweening;

/* 
 * In this example we let the user draw a spline on screen!
 * 
 */
namespace FluffyUnderware.Curvy.Examples
{
    public class PaintSpline : MonoBehaviour
    {
        public float StepDistance = 30;
        public SplineController Controller;
        public List<SplineController> listController = new List<SplineController>();

        CurvySpline mSpline;
        Vector2 mLastControlPointPos;
        public bool mResetSpline = true;

        public LayerMask dragMask;
        public Vector3 firstPoint;
        public Text countText;
        public LineRenderer mRenderer;
        List<Vector3> listPos = new List<Vector3>();
        public Vector3[] newPos;
        GameObject targetCanon;

        void Awake()
        {
            // for this example we assume the component is attached to a GameObject holding a spline
            mSpline = GetComponent<CurvySpline>();
            mRenderer = GetComponent<LineRenderer>();
        }

        void OnGUI()
        {
            // before using the spline, ensure it's initialized and the Controller is available
            if (mSpline == null || !mSpline.IsInitialized || !Controller)
                return;

            var e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDrag:
                    // Start a new line?
                    if (mResetSpline)
                    {
                        mSpline.Clear(); // delete all Control Points
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;

                        Vector3 p = Vector3.zero;
                        if (Physics.Raycast(ray, out hit, 1000, dragMask))
                        {
                            p = hit.point;
                        }
                        addCP(p); // add the first Control Point
                        firstPoint = p;
                        mLastControlPointPos = e.mousePosition; // Store current mouse position
                        mResetSpline = false;
                    }
                    else
                    {
                        // only create a new Control Point if the minimum distance is reached
                        float dist = (e.mousePosition - mLastControlPointPos).magnitude;
                        if (dist >= StepDistance)
                        {
                            mLastControlPointPos = e.mousePosition;
                            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                            RaycastHit hit;

                            Vector3 p = Vector3.zero;
                            if (Physics.Raycast(ray, out hit, 1000, dragMask))
                            {
                                p = hit.point;
                            }
                            addCP(p);
                            StartCoroutine(GetInLine());
                        }
                    }

                    break;
                case EventType.MouseUp:
                    mResetSpline = true;

                    break;
            }
        }

        public void Run(GameObject canon)
        {
            targetCanon = canon;
            firstPoint = transform.GetChild(0).position;  
            StartCoroutine(DelayStart());
        }

        IEnumerator DelayStart()
        {
            var total = listController.Count;
            newPos = new Vector3[mRenderer.positionCount];
            mRenderer.GetPositions(newPos);
            targetCanon.transform.DOLookAt(newPos[2], 0.15f);
            targetCanon.transform.DOScaleZ(0.75f, 0.05f).SetLoops(-1, LoopType.Yoyo);
            foreach(var item in listController)
            {
                if (item != null)
                {
                    item.gameObject.SetActive(true);
                    if (newPos.Length < 2)
                    {
                        // Debug.LogError("Jump: " + name);
                        var find = GameObject.FindGameObjectWithTag("Hole");
                        var des = find.transform.position;
                        var dis = Vector3.Distance(item.transform.position, des);
                        var time = dis/18;
                        item.transform.DOMove(new Vector3(des.x, item.transform.position.y, des.z), time).SetEase(Ease.Flash);
                    }
                    else
                    StartCoroutine(delayMoveTo(item));
                    total--;
                    countText.text = total.ToString();
                }
                yield return new WaitForSeconds(0.075f);
            }
            targetCanon.transform.DOKill();
            targetCanon.transform.localScale = Vector3.one * targetCanon.transform.localScale.x;
            Destroy(countText.gameObject);
            StartCoroutine(delayRemoveLine());
            targetCanon.transform.DOMoveY(-5, 2);
        }

        IEnumerator delayMoveTo(SplineController item)
        {
            var des = firstPoint;
            var dis = Vector3.Distance(item.transform.position, des);
            var time = dis / 18;
            item.transform.DOMove(firstPoint, time);
            yield return new WaitForSeconds(time);
            // try
            // {
            if (item != null)
            {
                item.Spline = mSpline;
                item.AbsolutePosition = 0;
                item.GetComponent<SphereCollider>().isTrigger = true;
            }
            // }
            // catch { }
        }

        IEnumerator delayRemoveLine()
        {
            yield return new WaitForSeconds(0.044f);
            newPos = new Vector3[mRenderer.positionCount];
            listPos.Clear();
            mRenderer.GetPositions(newPos);
            foreach(var item in newPos)
            {
                listPos.Add(item);
            }
            listPos.RemoveAt(0);
            newPos = listPos.ToArray();
            mRenderer.positionCount = newPos.Length;           
            // for (int i = 0; i < newPos.Length; i++)
            // {
            //     mRenderer.SetPosition(i, newPos[i]);
            // }
            mRenderer.SetPositions(newPos);
            if(newPos.Length >= 1)
            {
                StartCoroutine(delayRemoveLine());
            }
        }

        IEnumerator GetInLine()
        {
            foreach(var item in listController)
            {
                if (item.PlayState != CurvyController.CurvyControllerState.Playing)
                    item.Play();
                yield return new WaitForSeconds(0.1f);
            }
        }

        // Add a Control Point and set it's position
        CurvySplineSegment addCP(Vector3 p)
        {

            // Vector3 p = Camera.main.ScreenToWorldPoint(mousePos);
            // p.y *= -1; // flip Y to get the correct world position
            // p.z += 100; //To move further than camera's plane. The value 100 comes from the Canvas' plane distance
            var cp = mSpline.InsertAfter(null, p, false);

            return cp;
        }
    }
}
