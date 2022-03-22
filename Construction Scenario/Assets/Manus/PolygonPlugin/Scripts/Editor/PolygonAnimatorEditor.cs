using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Manus.Polygon
{
	[CustomEditor(typeof(PolygonAnimator))]
    public class PolygonAnimatorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var t_RetargetOption = serializedObject.FindProperty("m_RetargetOption");
            EditorGUILayout.PropertyField(t_RetargetOption);
            
            switch((RetargetOption)t_RetargetOption.intValue)
			{
                case RetargetOption.BodyEstimation:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("userIndex"));
                    break;
                case RetargetOption.TargetSkeleton:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("targetSkeletonName"));
                    break;
            }

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scaleToUser"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("rootMotion"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_UseSmoothing"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Smoothing"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Parameters"));

            serializedObject.ApplyModifiedProperties();
        }
    }
}
