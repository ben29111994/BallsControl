// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using System.Collections;
using FluffyUnderware.DevTools;
using FluffyUnderware.DevTools.Extensions;
using System.Collections.Generic;
using Object = UnityEngine.Object;


namespace FluffyUnderware.Curvy
{
    /// <summary>
    /// Curvy Global Scene Manager component
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(PoolManager))]
    public class CurvyGlobalManager : DTSingleton<CurvyGlobalManager>
    {
        #region ### Public Static Fields (Editor->Runtime Bridge) ###
        public static bool HideManager = false;
        /// <summary>
        /// Resolution of SceneView spline rendering
        /// </summary>
        public static float SceneViewResolution = 0.5f;
        /// <summary>
        /// Default spline color
        /// </summary>
        public static Color DefaultGizmoColor = new Color(0.71f, 0.71f, 0.71f);
        /// <summary>
        /// Default selected spline color
        /// </summary>
        public static Color DefaultGizmoSelectionColor = new Color(0.15f, 0.35f, 0.68f);
        /// <summary>
        /// Default interpolation used by new splines
        /// </summary>
        public static CurvyInterpolation DefaultInterpolation = CurvyInterpolation.CatmullRom;
        /// <summary>
        /// Size of control point gizmos
        /// </summary>
        public static float GizmoControlPointSize = 0.15f;
        /// <summary>
        /// Size of orientation gizmo
        /// </summary>
        public static float GizmoOrientationLength = 1f;
        /// <summary>
        /// Orientation gizmo color
        /// </summary>
        public static Color GizmoOrientationColor = new Color(0.75f, 0.75f, 0.4f);
        public static int SplineLayer = 0;
        /// <summary>
        /// Default view settings
        /// </summary>
        public static CurvySplineGizmos Gizmos = CurvySplineGizmos.Curve | CurvySplineGizmos.Orientation;

        public static bool ShowCurveGizmo
        {
            get { return (Gizmos & CurvySplineGizmos.Curve) == CurvySplineGizmos.Curve; }
            set
            {
                if (value)
                    Gizmos |= CurvySplineGizmos.Curve;
                else
                    Gizmos &= ~CurvySplineGizmos.Curve;
            }
        }

        public static bool ShowApproximationGizmo
        {
            get { return (Gizmos & CurvySplineGizmos.Approximation) == CurvySplineGizmos.Approximation; }
            set
            {
                if (value)
                    Gizmos |= CurvySplineGizmos.Approximation;
                else
                    Gizmos &= ~CurvySplineGizmos.Approximation;
            }
        }

        public static bool ShowTangentsGizmo
        {
            get { return (Gizmos & CurvySplineGizmos.Tangents) == CurvySplineGizmos.Tangents; }
            set
            {
                if (value)
                    Gizmos |= CurvySplineGizmos.Tangents;
                else
                    Gizmos &= ~CurvySplineGizmos.Tangents;
            }
        }

        public static bool ShowOrientationGizmo
        {
            get { return (Gizmos & CurvySplineGizmos.Orientation) == CurvySplineGizmos.Orientation; }
            set
            {
                if (value)
                    Gizmos |= CurvySplineGizmos.Orientation;
                else
                    Gizmos &= ~CurvySplineGizmos.Orientation;
            }
        }

        public static bool ShowLabelsGizmo
        {
            get { return (Gizmos & CurvySplineGizmos.Labels) == CurvySplineGizmos.Labels; }
            set
            {
                if (value)
                    Gizmos |= CurvySplineGizmos.Labels;
                else
                    Gizmos &= ~CurvySplineGizmos.Labels;
            }
        }

        public static bool ShowMetadataGizmo
        {
            get { return (Gizmos & CurvySplineGizmos.Metadata) == CurvySplineGizmos.Metadata; }
            set
            {
                if (value)
                    Gizmos |= CurvySplineGizmos.Metadata;
                else
                    Gizmos &= ~CurvySplineGizmos.Metadata;
            }
        }

        public static bool ShowBoundsGizmo
        {
            get { return (Gizmos & CurvySplineGizmos.Bounds) == CurvySplineGizmos.Bounds; }
            set
            {
                if (value)
                    Gizmos |= CurvySplineGizmos.Bounds;
                else
                    Gizmos &= ~CurvySplineGizmos.Bounds;
            }
        }

        #endregion

        #region ### Private Fields ###

        PoolManager mPoolManager;
        ComponentPool mControlPointPool;

        #endregion

        #region ### Public Methods & Properties ###

        /// <summary>
        /// Gets the PoolManager
        /// </summary>
        public PoolManager PoolManager
        {
            get
            {
                if (mPoolManager == null)
                    mPoolManager = GetComponent<PoolManager>();
                return mPoolManager;
            }
        }

        public ComponentPool ControlPointPool
        {
            get
            {
                return mControlPointPool;
            }
        }

        /// <summary>
        /// Gets all connections in the scene
        /// </summary>
        public CurvyConnection[] Connections
        {
            get
            {
                return GetComponentsInChildren<CurvyConnection>();
            }
        }

        /// <summary>
        /// Returns all the connections that are exclusively connecting cps within the splines parameter
        /// </summary>
        /// <param name="splines"></param>
        /// <returns></returns>
        public CurvyConnection[] GetContainingConnections(params CurvySpline[] splines)
        {
            List<CurvyConnection> connectionsResult = new List<CurvyConnection>();
            List<CurvySpline> splinesList = new List<CurvySpline>(splines);
            foreach (CurvySpline spline in splinesList)
            {
                foreach (CurvySplineSegment controlPoint in spline.ControlPointsList)
                    if (controlPoint.Connection != null && !connectionsResult.Contains(controlPoint.Connection))
                    {
                        bool add = true;
                        // only process connections if all involved splines are part of the prefab
                        foreach (CurvySplineSegment connectedControlPoint in controlPoint.Connection.ControlPointsList)
                        {
                            if (connectedControlPoint.Spline != null && !splinesList.Contains(connectedControlPoint.Spline))
                            {
                                add = false;
                                break;
                            }
                        }
                        if (add)
                            connectionsResult.Add(controlPoint.Connection);
                    }
            }

            return connectionsResult.ToArray();
        }

        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */
        public override void Awake()
        {
            base.Awake();
            name = "_CurvyGlobal_";
            transform.SetAsLastSibling();
            // Unity 5.3 introduces buug that hides GameObject when calling this outside playmode!
            if (Application.isPlaying)
                Object.DontDestroyOnLoad(this);
            mPoolManager = GetComponent<PoolManager>();
            PoolSettings s = new PoolSettings()
            {
                MinItems = 0,
                Threshold = 50,
                Prewarm = true,
                AutoCreate = true,
                AutoEnableDisable = true
            };
            mControlPointPool = mPoolManager.CreateComponentPool<CurvySplineSegment>(s);

        }

        void Start()
        {
            if (HideManager)
                gameObject.hideFlags = HideFlags.HideInHierarchy;
            else
                gameObject.hideFlags = HideFlags.None;
        }

        /*! \endcond */
        #endregion

        #region ### Privates & Internals ###
        /*! \cond PRIVATE */

        [RuntimeInitializeOnLoadMethod]
        static void LoadRuntimeSettings()
        {
            if (!PlayerPrefs.HasKey("Curvy_MaxCachePPU"))
                SaveRuntimeSettings();
            SceneViewResolution = DTUtility.GetPlayerPrefs<float>("Curvy_SceneViewResolution", SceneViewResolution);
            HideManager = DTUtility.GetPlayerPrefs<bool>("Curvy_HideManager", HideManager);
            DefaultGizmoColor = DTUtility.GetPlayerPrefs<Color>("Curvy_DefaultGizmoColor", DefaultGizmoColor);
            DefaultGizmoSelectionColor = DTUtility.GetPlayerPrefs<Color>("Curvy_DefaultGizmoSelectionColor", DefaultGizmoColor);
            DefaultInterpolation = DTUtility.GetPlayerPrefs<CurvyInterpolation>("Curvy_DefaultInterpolation", DefaultInterpolation);
            GizmoControlPointSize = DTUtility.GetPlayerPrefs<float>("Curvy_ControlPointSize", GizmoControlPointSize);
            GizmoOrientationLength = DTUtility.GetPlayerPrefs<float>("Curvy_OrientationLength", GizmoOrientationLength);
            GizmoOrientationColor = DTUtility.GetPlayerPrefs<Color>("Curvy_OrientationColor", GizmoOrientationColor);
            Gizmos = DTUtility.GetPlayerPrefs<CurvySplineGizmos>("Curvy_Gizmos", Gizmos);
            SplineLayer = DTUtility.GetPlayerPrefs<int>("Curvy_SplineLayer", SplineLayer);
        }

        public static void SaveRuntimeSettings()
        {
            DTUtility.SetPlayerPrefs<float>("Curvy_SceneViewResolution", SceneViewResolution);
            DTUtility.SetPlayerPrefs<bool>("Curvy_HideManager", HideManager);
            DTUtility.SetPlayerPrefs<Color>("Curvy_DefaultGizmoColor", DefaultGizmoColor);
            DTUtility.SetPlayerPrefs<Color>("Curvy_DefaultGizmoSelectionColor", DefaultGizmoSelectionColor);
            DTUtility.SetPlayerPrefs<CurvyInterpolation>("Curvy_DefaultInterpolation", DefaultInterpolation);
            DTUtility.SetPlayerPrefs<float>("Curvy_ControlPointSize", GizmoControlPointSize);
            DTUtility.SetPlayerPrefs<float>("Curvy_OrientationLength", GizmoOrientationLength);
            DTUtility.SetPlayerPrefs<Color>("Curvy_OrientationColor", GizmoOrientationColor);
            DTUtility.SetPlayerPrefs<CurvySplineGizmos>("Curvy_Gizmos", Gizmos);
            DTUtility.SetPlayerPrefs<int>("Curvy_SplineLayer", SplineLayer);
            PlayerPrefs.Save();
        }



        public override void MergeDoubleLoaded(IDTSingleton newInstance)
        {
            base.MergeDoubleLoaded(newInstance);

            CurvyGlobalManager other = newInstance as CurvyGlobalManager;
            // Merge connection from a doubled CurvyGlobalManager before it get destroyed by DTSingleton
            CurvyConnection[] otherConnections = other.Connections;
            for (int i = 0; i < otherConnections.Length; i++)
                otherConnections[i].transform.SetParent(this.transform);
        }


        /*! \endcond */
        #endregion



    }
}
