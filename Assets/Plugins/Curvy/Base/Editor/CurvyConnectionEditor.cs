// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.Curvy;
using FluffyUnderware.DevToolsEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace FluffyUnderware.CurvyEditor
{
    [CustomEditor(typeof(CurvyConnection))]
    public class CurvyConnectionEditor : CurvyEditorBase<CurvyConnection>
    {
        /// <summary>
        /// the gui style used in drawing the gizmo's label
        /// </summary>
        private GUIStyle sceneGuiLabelStyle;
        /// <summary>
        /// Used in the inspector, to avoid the margins added by Unity when subdivising a space
        /// </summary>
        private GUIStyle noMarginsStyle;

        private GUIStyle highlightedItemStyle;

        GUIContent selectCpContent;
        GUIContent removeFromConnectionContent;
        GUIContent controlPointTitleContent;
        GUIContent syncPositionTitleContent;
        GUIContent syncPositionContent;
        GUIContent syncRotationTitleContent;
        GUIContent syncRotationContent;
        GUIContent noSyncPresetContent;
        GUIContent positionSyncPresetContent;
        GUIContent rotationSyncPresetContent;
        GUIContent fullSyncPresetContent;

        private GUIContent splineTitleContent;
        private GUIContent endControlPointTitleContent;
        private GUIContent followUpTitleContent;
        private GUIContent headingDirectionTitleContent;

        GUIContent splineStartDirectionContent;
        GUIContent emptyContent;
        GUIContent noDirectionContent;
        GUIContent splineEndDirectionContent;
        GUIContent automaticDirectionContent;
        GUIContent directionUnavailableContent;

        protected override void OnEnable()
        {
            base.OnEnable();
            sceneGuiLabelStyle = new GUIStyle();
            noMarginsStyle = new GUIStyle();
            noMarginsStyle.margin = new RectOffset();
            highlightedItemStyle = new GUIStyle();
            highlightedItemStyle.normal.background = new Texture2D(1, 1);
            highlightedItemStyle.normal.background.SetPixel(0, 0, new Color(62 / 255f, 125 / 255f, 231 / 255f));
            highlightedItemStyle.normal.background.Apply();

            selectCpContent = new GUIContent("Select", "Select the Control Point");
            removeFromConnectionContent = new GUIContent(CurvyStyles.DeleteSmallTexture, "Remove the Control Point from this Connection");
            controlPointTitleContent = new GUIContent("Control Point", "The name of the Control Point. You can select it or remove it from the connection");
            syncPositionTitleContent = new GUIContent("Sync Position", "Synchronize the Control Point's position with the connection's");
            syncPositionContent = new GUIContent("", "Synchronize the Control Point's position with the connection's");
            syncRotationTitleContent = new GUIContent("Sync Rotation", "Synchronize the Control Point's rotation with the connection's");
            syncRotationContent = new GUIContent("", "Synchronize the Control Point's rotation with the connection's");
            noSyncPresetContent = new GUIContent(CurvyStyles.TexConnection, "No synchronization");
            positionSyncPresetContent = new GUIContent(CurvyStyles.TexConnectionPos, "Position only");
            rotationSyncPresetContent = new GUIContent(CurvyStyles.TexConnectionRot, "Rotation only");
            fullSyncPresetContent = new GUIContent(CurvyStyles.TexConnectionFull, "Position and rotation");

            splineTitleContent = new GUIContent("Spline", "The open spline for which the Follow-Up will be defined");
            endControlPointTitleContent = new GUIContent("Control Point", "The spline's end that will have a Follow-Up, either its first or last visible Control Point");
            followUpTitleContent = new GUIContent("Follow-Up", "The Control Point that will act as the continuity of the spline's end");
            headingDirectionTitleContent = new GUIContent("Heading Direction", "In which direction the Follow-Up should continue on");

            splineStartDirectionContent = new GUIContent("To spline's start", "The Follow-Up segment continues towards its spline's start");
            emptyContent = new GUIContent("");
            noDirectionContent = new GUIContent("Nowhere", "There is no continuity for the Follow-Up");
            splineEndDirectionContent = new GUIContent("To spline's end", "The Follow-Up segment continues towards its spline's end");
            automaticDirectionContent = new GUIContent("Automatic", "Automatically selects the best option");
            directionUnavailableContent = new GUIContent("---", "Select a Follow-Up first");
        }

        public new void OnSceneGUI()
        {
            if (SceneView.currentDrawingSceneView.camera && Target)
            {
                int cpsCount = Target.ControlPointsList.Count;
                if (cpsCount > 0)
                {
                    sceneGuiLabelStyle.normal.textColor = Target.GetGizmoColor();

                    int syncedCPsCount = 0;
                    for (int i = 0; i < cpsCount; i++)
                    {
                        CurvySplineSegment controlPoint = Target.ControlPointsList[i];

                        int lineIndex;
                        if (controlPoint.ConnectionSyncPosition)
                        {
                            lineIndex = syncedCPsCount;
                            syncedCPsCount++;
                        }
                        else
                            lineIndex = 0;

                        Handles.Label(DTHandles.TranslateByPixel(controlPoint.transform.position, 12, -12 * (1 + lineIndex)), controlPoint.ToString(), sceneGuiLabelStyle);
                    }
                }
            }
        }

        protected override void OnReadNodes()
        {
            Node.AddSection("Control Point Options", ControlPointsGui);
            Node.AddSection("Follow-Up", FollowUpGui);
            Node.AddSection("Connection Options", ConnectionGui);
        }

        private static float GetNodeWidth(DTInspectorNode node)
        {
            return EditorGUIUtility.currentViewWidth - 2 * node.Level * 15 /*value of EditorGUI.indent*/;
        }

        void ControlPointsGui(DTInspectorNode node)
        {
            float drawingWidth = GetNodeWidth(node);
            float column1Width = drawingWidth * 16 / 32;
            float column2Width = drawingWidth * 8 / 32;
            float column3Width = drawingWidth * 8 / 32;

            //header           
            {
                EditorGUILayout.BeginHorizontal();
                PositionGuiElements(() => GUILayout.Label(controlPointTitleContent, EditorStyles.boldLabel), column1Width, false);
                PositionGuiElements(() => GUILayout.Label(syncPositionTitleContent, EditorStyles.boldLabel), column2Width);
                PositionGuiElements(() => GUILayout.Label(syncRotationTitleContent, EditorStyles.boldLabel), column3Width);
                EditorGUILayout.EndHorizontal();
            }

            //Items
            for (int index = 0; index < Target.ControlPointsList.Count; index++)
            {
                CurvySplineSegment item = Target.ControlPointsList[index];

                bool itemIsSelected = item.gameObject == Selection.activeGameObject;
                EditorGUILayout.BeginHorizontal(itemIsSelected
                    ? highlightedItemStyle
                    : GUIStyle.none);

                {
                    PositionGuiElements(() =>
                                        {
                                            bool clicked = GUILayout.Button(item.ToString(), itemIsSelected
                                                ? EditorStyles.whiteLabel
                                                : EditorStyles.label, GUILayout.MinWidth(column1Width * 0.6f));

                                            if (GUILayout.Button(selectCpContent, GUILayout.MinWidth(50)))
                                                clicked = true;

                                            if (clicked)
                                                DTSelection.SetGameObjects(item);

                                            if (GUILayout.Button(removeFromConnectionContent, CurvyStyles.ImageButton, GUILayout.MinWidth(20), GUILayout.MinHeight(18)))
                                            {
                                                //TODO make the undoing work
                                                //Undo.RegisterCompleteObjectUndo(new UnityEngine.Object[]{
                                                //    item, Target
                                                //}, "Disconnect from Connection");
                                                item.Disconnect();
                                                CurvyProject.Instance.ScanConnections();
                                                GUIUtility.ExitGUI();
                                            }
                                        }, column1Width, false);
                }

                {
                    PositionGuiElements(() =>
                                        {
                                            bool oldValue = item.ConnectionSyncPosition;
                                            item.ConnectionSyncPosition = GUILayout.Toggle(oldValue, syncPositionContent);
                                            if (item.ConnectionSyncPosition != oldValue)
                                                item.Connection.SetSynchronisationPositionAndRotation(item.Connection.transform.position, item.Connection.transform.rotation);
                                        }, column2Width);
                    PositionGuiElements(() =>
                                        {
                                            bool oldValue = item.ConnectionSyncRotation;
                                            item.ConnectionSyncRotation = GUILayout.Toggle(oldValue, syncRotationContent);
                                            if (item.ConnectionSyncRotation != oldValue)
                                                item.Connection.SetSynchronisationPositionAndRotation(item.Connection.transform.position, item.Connection.transform.rotation);
                                        }, column3Width);
                }

                EditorGUILayout.EndHorizontal();
            }

            //Presets
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Synchronization Presets", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();


                EditorGUILayout.BeginHorizontal(GUILayout.Width(drawingWidth));
                bool syncPosition = false;
                bool syncRotation = false;
                bool buttonClicked = false;
                if (GUILayout.Button(noSyncPresetContent))
                {
                    syncPosition = false;
                    syncRotation = false;
                    buttonClicked = true;
                }

                if (GUILayout.Button(positionSyncPresetContent))
                {
                    syncPosition = true;
                    syncRotation = false;
                    buttonClicked = true;
                }

                if (GUILayout.Button(rotationSyncPresetContent))
                {
                    syncPosition = false;
                    syncRotation = true;
                    buttonClicked = true;
                }

                if (GUILayout.Button(fullSyncPresetContent))
                {
                    syncPosition = true;
                    syncRotation = true;
                    buttonClicked = true;
                }

                if (buttonClicked && Target.ControlPointsList.Any())
                {
                    foreach (CurvySplineSegment controlPoint in Target.ControlPointsList)
                    {
                        controlPoint.ConnectionSyncPosition = syncPosition;
                        controlPoint.ConnectionSyncRotation = syncRotation;
                    }

                    Target.SetSynchronisationPositionAndRotation(Target.transform.position, Target.transform.rotation);
                    Target.AutoSetFollowUp();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void PositionGuiElements(Action guiElementsDrawer, float elementsTotalWidth, bool spaceBefore = true, bool spaceAfter = true)
        {
            EditorGUILayout.BeginHorizontal(noMarginsStyle, GUILayout.Width(elementsTotalWidth));
            if (spaceBefore)
                GUILayout.FlexibleSpace();
            guiElementsDrawer();
            if (spaceAfter)
                GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        void FollowUpGui(DTInspectorNode node)
        {
            float drawingWidth = GetNodeWidth(node);
            float column1Width = drawingWidth * 2 / 10;
            float column2Width = drawingWidth * 2 / 10;
            float column3Width = drawingWidth * 3 / 10;
            float column4Width = drawingWidth * 3 / 10;

            bool hasCpWhichCanHaveFollowUp = Target.ControlPointsList.Any(item => item.Spline && item.Spline.CanControlPointHaveFollowUp(item));

            if (hasCpWhichCanHaveFollowUp)
            {
                //header           
                {
                    EditorGUILayout.BeginHorizontal();
                    PositionGuiElements(() =>  GUILayout.Label(splineTitleContent, EditorStyles.boldLabel, GUILayout.MaxWidth(column1Width)), column1Width);
                    PositionGuiElements(() =>  GUILayout.Label(endControlPointTitleContent, EditorStyles.boldLabel, GUILayout.MaxWidth(column2Width)), column2Width);
                    PositionGuiElements(() =>  GUILayout.Label(followUpTitleContent, EditorStyles.boldLabel), column3Width);
                    PositionGuiElements(() =>  GUILayout.Label(headingDirectionTitleContent, EditorStyles.boldLabel), column4Width);
                    EditorGUILayout.EndHorizontal();
                }

                for (int index = 0; index < Target.ControlPointsList.Count; index++)
                {
                    CurvySplineSegment item = Target.ControlPointsList[index];
                    if (item.Spline && item.Spline.CanControlPointHaveFollowUp(item))
                    {
                        EditorGUILayout.BeginHorizontal();

                        PositionGuiElements(() => GUILayout.Label(item.Spline.name, GUILayout.MaxWidth(column1Width)), column1Width);
                        PositionGuiElements(() => GUILayout.Label(String.Format("{0} ({1})", item.name, item.IsLastControlPoint
                                                ? "Last CP"
                                                : "First CP"), GUILayout.MaxWidth(column2Width)), column2Width);
                        PositionGuiElements(() =>
                                            {
                                                List<CurvySplineSegment> possibleTargets = (from cp in Target.ControlPointsList where cp != item select cp).ToList();
                                                int popUpIndex;
                                                {
                                                    if (item.FollowUp == null)
                                                        popUpIndex = 0;
                                                    else
                                                    {
                                                        int followUpIndex = possibleTargets.IndexOf(item.FollowUp);
#if CURVY_SANITY_CHECKS
                                                        Assert.IsTrue(followUpIndex != -1);
#endif
                                                        popUpIndex = followUpIndex + 1;
                                                    }
                                                }

                                                List<string> popUpContent;
                                                {
                                                    popUpContent = (from cp in possibleTargets select cp.ToString()).ToList();
                                                    popUpContent.Insert(0, "No Follow-Up");
                                                }

                                                EditorGUI.BeginChangeCheck();

                                                popUpIndex = EditorGUILayout.Popup(popUpIndex, popUpContent.ToArray(), GUILayout.MaxWidth(column3Width));

                                                if (EditorGUI.EndChangeCheck())
                                                {
                                                    item.SetFollowUp(popUpIndex == 0
                                                        ? null
                                                        : possibleTargets[popUpIndex - 1]);
                                                }
                                            }, column3Width);

                        PositionGuiElements(() =>
                                            {
                                                CurvySplineSegment itemFollowUp = item.FollowUp;
                                                if (itemFollowUp)
                                                {
                                                    bool canHeadToStart = itemFollowUp.Spline.Closed || itemFollowUp.IsFirstControlPoint == false;
                                                    bool canHeadToEnd = itemFollowUp.Spline.Closed || itemFollowUp.IsLastControlPoint == false;

                                                    //Handle cases where external changes makes a heading value invalid
                                                    //Potential BUG: this UI code modifies the FollowUpHeading based on the knowledge that the spline's logic code, at the time this comment was written, handles those invalid values as if the heading was set to Auto
                                                    if (canHeadToStart == false && item.FollowUpHeading == ConnectionHeadingEnum.Minus || canHeadToEnd == false && item.FollowUpHeading == ConnectionHeadingEnum.Plus)
                                                        item.FollowUpHeading = ConnectionHeadingEnum.Auto;

                                                    int popUpIndex = (int)item.FollowUpHeading + 1;
#if CURVY_SANITY_CHECKS
                                                    Assert.IsTrue((int)ConnectionHeadingEnum.Minus == -1);
                                                    Assert.IsTrue((int)ConnectionHeadingEnum.Sharp == 0);
                                                    Assert.IsTrue((int)ConnectionHeadingEnum.Plus == 1);
                                                    Assert.IsTrue((int)ConnectionHeadingEnum.Auto == 2);
#endif


                                                    GUIContent[] popUpContent =
                                                    {
                                                        canHeadToStart
                                                            ? splineStartDirectionContent
                                                            : emptyContent,
                                                        noDirectionContent,
                                                        canHeadToEnd
                                                            ? splineEndDirectionContent
                                                            : emptyContent,
                                                        automaticDirectionContent
                                                    };
                                                    EditorGUI.BeginChangeCheck();
                                                    popUpIndex = EditorGUILayout.Popup(popUpIndex, popUpContent.ToArray(), GUILayout.MaxWidth(column4Width));
                                                    if (EditorGUI.EndChangeCheck())
                                                        item.FollowUpHeading = (ConnectionHeadingEnum)(popUpIndex - 1);
                                                }
                                                else
                                                    GUILayout.Label(directionUnavailableContent);
                                            }, column4Width);

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
                GUILayout.Label("You should connect the first or last Control Point of an open spline to be able to setup Follow-Ups");
        }

        void ConnectionGui(DTInspectorNode node)
        {
            float drawingWidth = GetNodeWidth(node);

            if (GUILayout.Button("Delete Connection", GUILayout.Width(drawingWidth)))
            {
                Target.Delete();
                GUIUtility.ExitGUI();
            }
        }
    }
}