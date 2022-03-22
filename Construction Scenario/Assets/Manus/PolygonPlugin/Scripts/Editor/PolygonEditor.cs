using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using Manus.Polygon.Networking;
using Manus.Utility;
using Manus.Polygon.Utilities;

namespace Manus.Polygon
{
	[CustomEditor(typeof(Polygon))]
	public class PolygonEditor : UnityEditor.Editor
	{
		private float m_Size = 0.03f;

		private Color m_HandlesColor = Color.cyan;
		private Bone m_SelectedBone;
		private bool m_ReorientationPossible = false;

		private bool m_Animator = false;
		private bool m_Networked = false;

		public override void OnInspectorGUI()
		{
			Polygon t_Script = target as Polygon;

			base.OnInspectorGUI();

			if (t_Script.boneReferences.currentSetupState == 0)
			{
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Auto populate bones (make sure this is set correctly) or manually assign them, when finished 'Set up' the skeleton", MessageType.Info, true);

				GUILayout.BeginHorizontal();

				if (!Application.isPlaying)
				{
					if (GUILayout.Button("Reset Pose"))
					{
						SetToBindPose(t_Script);
						EditorUtility.SetDirty(t_Script);
					}
				}

				if (GUILayout.Button("Populate Bones"))
				{
					PopulateBoneReferences(t_Script);
					EditorUtility.SetDirty(t_Script);
				}

				GUILayout.EndHorizontal();

				if (GUILayout.Button("Set Up"))
				{
					if (t_Script.boneReferences.isValid)
					{
						t_Script.boneReferences.CheckRoot();
						t_Script.boneReferences.CheckToeEnds();
						CalculateBoneOrientations(t_Script);
						t_Script.boneReferences.currentSetupState = 1;
						EditorUtility.SetDirty(t_Script);
					}
					else
					{
						Debug.LogWarning("Trying to set up a skeleton, but the references are not valid", t_Script.gameObject);
					}
				}
			} 
			else if (t_Script.boneReferences.currentSetupState == 1)
			{
				EditorGUILayout.Space();
				EditorGUILayout.HelpBox("Check if all bone orientations are correct, if everything is set up correctly, set the bind pose", MessageType.Info, true);

				GUILayout.BeginHorizontal();

				if (GUILayout.Button("Back"))
				{
					ResetDesiredBoneOrientations(t_Script);
					t_Script.boneReferences.currentSetupState = 0;
					EditorUtility.SetDirty(t_Script);
				}

				if (GUILayout.Button("Set Bind Pose"))
				{
					foreach (var t_Renderer in t_Script.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
					{
						t_Renderer.updateWhenOffscreen = true;
					}

					t_Script.boneReferences.setupScale = t_Script.boneReferences.root.bone.parent.lossyScale.z;
					t_Script.boneReferences.currentSetupState = 2;

					EditorUtility.SetDirty(t_Script);
				}

				GUILayout.EndHorizontal();
			} 
			else if (t_Script.boneReferences.currentSetupState >= 2)
			{
				GUILayout.BeginHorizontal();
				
				if (GUILayout.Button("Reset"))
				{
					ResetDesiredBoneOrientations(t_Script);
					t_Script.boneReferences.currentSetupState = 0;
					EditorUtility.SetDirty(t_Script);
				}

				GUILayout.EndHorizontal();
			}

			if (Application.isPlaying)
				return;

			#region Draw ComponentToggles

			if (t_Script.boneReferences.currentSetupState < 2)
				return;

			EditorGUILayout.Space();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			EditorGUILayout.BeginHorizontal();

			bool t_PreviousAnimator = m_Animator;
			m_Animator = t_Script.GetComponent<PolygonAnimator>();
			m_Animator = GUILayout.Toggle(m_Animator, "Animator", EditorStyles.miniButtonMid);

			bool t_PreviousNetworked = m_Networked;
			m_Networked = t_Script.GetComponent<PolygonAnimatorSync>();
			m_Networked = GUILayout.Toggle(m_Networked, "Networking", EditorStyles.miniButtonMid);

			if (m_Animator != t_PreviousAnimator)
			{
				if (m_Animator) // Add animator
				{
					if (t_Script.gameObject.GetComponent<PolygonAnimator>() == null)
						t_Script.gameObject.AddComponent<PolygonAnimator>();

					t_Script.gameObject.GetComponent<PolygonAnimator>().enabled = !m_Networked;
				}
				else // Remove animator
				{
					if (t_Script.gameObject.GetComponent<PolygonAnimatorSync>() != null)
					{
						DestroyImmediate(t_Script.gameObject.GetComponent<PolygonAnimatorSync>());
					}

					if (t_Script.gameObject.GetComponent<PolygonAnimator>() != null)
					{
						DestroyImmediate(t_Script.gameObject.GetComponent<PolygonAnimator>());
					}
				}
			}

			if (m_Networked != t_PreviousNetworked)
			{
				if (m_Networked) // Add networking
				{
					if (t_Script.gameObject.GetComponent<PolygonAnimator>() != null && t_Script.gameObject.GetComponent<PolygonAnimatorSync>() == null)
					{
						t_Script.gameObject.AddComponent<PolygonAnimatorSync>();
						t_Script.gameObject.GetComponent<PolygonAnimator>().enabled = !m_Networked;
					}
				}
				else // Remove networking
				{
					if (t_Script.gameObject.GetComponent<PolygonAnimator>() != null && t_Script.gameObject.GetComponent<PolygonAnimatorSync>() != null)
					{
						DestroyImmediate(t_Script.gameObject.GetComponent<PolygonAnimatorSync>());
						t_Script.gameObject.GetComponent<PolygonAnimator>().enabled = !m_Networked;
					}
				}
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndVertical();

			#endregion
		}

		/// <summary>
		/// Try to find all bone references automatically
		/// </summary>
		/// <param name="p_Polygon"></param>
		private void PopulateBoneReferences(Polygon p_Polygon)
		{
			if (p_Polygon.isAnimatorValid)
			{
				p_Polygon.boneReferences.Populate(p_Polygon.animator);
			}
		}

		/// <summary>
		/// Calculate the desired bone orientation for each bone assigned in the bone references
		/// </summary>
		/// <param name="p_Polygon"></param>
		private void CalculateBoneOrientations(Polygon p_Polygon)
		{
			foreach (var t_Bone in p_Polygon.boneReferences.GatherBones(GatherType.All).Values)
			{
				t_Bone.CalculateOrientation(p_Polygon.boneReferences);
			}
		}

		/// <summary>
		/// Reset the desired orientations
		/// </summary>
		/// <param name="p_Polygon"></param>
		private void ResetDesiredBoneOrientations(Polygon p_Polygon)
		{
			p_Polygon.boneReferences.Reset();
		}

		/// <summary>
		/// Put the character in the T-pose
		/// </summary>
		/// <param name="p_Polygon"></param>
		private void SetToBindPose(Polygon p_Polygon)
		{
			SkeletonBindPoseUtilities.SampleBindPose(p_Polygon.gameObject);
		}

		private void OnSceneGUI()
		{
			Polygon t_Skeleton = target as Polygon;

			if (t_Skeleton == null || !t_Skeleton.boneReferences.isValid) return;

			SkeletonBoneReferences bones = t_Skeleton.boneReferences;

			if (t_Skeleton.boneReferences.isValid) 
				DrawHumanoidSkeletonBones(bones);

			if (Selection.activeGameObject != t_Skeleton.gameObject) m_SelectedBone = null;
			if (m_SelectedBone != null) DrawRotationGizmo(m_SelectedBone, m_Size);
		}

		private void DrawHumanoidSkeletonBones(SkeletonBoneReferences p_Bones)
		{
			m_ReorientationPossible = p_Bones.currentSetupState < 2;

			// Draw whole skeleton
			DrawBone(p_Bones.root, m_Size);
			DrawBodyAndHead(p_Bones.body, p_Bones.head);

			DrawLeg(p_Bones.legLeft, p_Bones.body);
			DrawLeg(p_Bones.legRight, p_Bones.body);

			DrawArm(p_Bones.armLeft, p_Bones.body);
			DrawArm(p_Bones.armRight, p_Bones.body);

			DrawHeightHandle(p_Bones);
		}

		#region drawing the skeleton bones

		private void DrawBodyAndHead(Body p_Body, Head p_Head)
		{
			Bone t_HeadConnection = p_Head.head;
			if (p_Head.neck.bone)
			{
				t_HeadConnection = p_Head.neck;
				ConnectBones(p_Head.neck, p_Head.head);
			}

			// Draw Skeleton
			if (p_Body.spine.bone)
			{
				ConnectBones(p_Body.hip, p_Body.spine);

				if (p_Body.chest.bone)
				{
					ConnectBones(p_Body.spine, p_Body.chest);

					if (p_Body.upperChest.bone)
					{
						ConnectBones(p_Body.chest, p_Body.upperChest);
						ConnectBones(p_Body.upperChest, t_HeadConnection);
					}
					else
					{
						ConnectBones(p_Body.chest, t_HeadConnection);
					}
				}
				else
				{
					ConnectBones(p_Body.spine, t_HeadConnection);
				}
			} else
			{
				ConnectBones(p_Body.hip, t_HeadConnection);
			}

			// Draw Bones
			DrawBone(p_Body.hip, m_Size);
			if (p_Body.spine.bone) DrawBone(p_Body.spine, m_Size);
			if (p_Body.chest.bone) DrawBone(p_Body.chest, m_Size);
			if (p_Body.upperChest.bone) DrawBone(p_Body.upperChest, m_Size);
			if (p_Head.neck.bone) DrawBone(p_Head.neck, m_Size);
			DrawBone(p_Head.head, m_Size);
		}

		private void DrawArm(Arm p_Arm, Body p_Body)
		{
			// Draw Skeleton
			ConnectBones(p_Arm.lowerArm, p_Arm.hand.wrist);

			Bone t_HighestSpine = p_Body.spine;
			if (p_Body.chest.bone) t_HighestSpine = p_Body.chest;
			if (p_Body.upperChest.bone) t_HighestSpine = p_Body.upperChest;

			if (p_Arm.shoulder.bone)
			{
				ConnectBones(t_HighestSpine, p_Arm.shoulder);
				ConnectBones(p_Arm.shoulder, p_Arm.upperArm);
			}
			else
			{
				ConnectBones(t_HighestSpine, p_Arm.upperArm);
			}
			ConnectBones(p_Arm.upperArm, p_Arm.lowerArm);

			// Draw Bones
			DrawHand(p_Arm.hand);

			if (p_Arm.shoulder.bone) DrawBone(p_Arm.shoulder, m_Size);
			DrawBone(p_Arm.upperArm, m_Size);
			DrawBone(p_Arm.lowerArm, m_Size);
		}
		
		private void DrawLeg(Leg p_Leg, Body p_Body)
		{
			// Draw Skeleton
			ConnectBones(p_Body.hip, p_Leg.upperLeg);
			ConnectBones(p_Leg.upperLeg, p_Leg.lowerLeg);
			ConnectBones(p_Leg.lowerLeg, p_Leg.foot);
			if (p_Leg.toes.bone != null) ConnectBones(p_Leg.foot, p_Leg.toes);
			if (p_Leg.toes.bone != null && p_Leg.toesEnd.bone != null) ConnectBones(p_Leg.toes, p_Leg.toesEnd);

			// Draw Bones
			DrawBone(p_Leg.upperLeg, m_Size);
			DrawBone(p_Leg.lowerLeg, m_Size);
			DrawBone(p_Leg.foot, m_Size);
			if (p_Leg.toes.bone != null)  DrawBone(p_Leg.toes, m_Size);
			if (p_Leg.toesEnd.bone != null)  DrawBone(p_Leg.toesEnd, m_Size);
		}

		private void DrawHand(HandBoneReferences p_Hand)
		{
			DrawBone(p_Hand.wrist, m_Size);

			DrawFinger(p_Hand.index, p_Hand);
			DrawFinger(p_Hand.middle, p_Hand);
			DrawFinger(p_Hand.ring, p_Hand);
			DrawFinger(p_Hand.pinky, p_Hand);
			DrawFinger(p_Hand.thumb, p_Hand);
		}

		private void DrawFinger(Finger p_Finger, HandBoneReferences p_Hand)
		{
			var t_Bones = p_Finger.GatherBones(GatherType.All).Values.ToArray();
			for (int i = 0; i < t_Bones.Length; i++)
			{ 
				ConnectBones(t_Bones[i], (i == 0) ? p_Hand.wrist : t_Bones[i - 1]);
				DrawBone(t_Bones[i], m_Size / 3f);
			}
		}

		private void ConnectBones(Bone p_Bone1, Bone p_Bone2)
		{
			Handles.color = m_HandlesColor;
			Handles.DrawLine(p_Bone1.bone.position, p_Bone2.bone.position);
		}

		private void DrawBone(Bone p_Bone, float p_Size)
		{
			Handles.color = m_HandlesColor;

			if (m_SelectedBone == p_Bone)
			{
				Handles.SphereHandleCap(0, p_Bone.bone.position, Quaternion.identity, p_Size, EventType.Repaint);
				DrawDirectionBone(p_Bone, p_Size);
				return;
			}

			if (Handles.Button(p_Bone.bone.position, Quaternion.identity, p_Size, p_Size, Handles.SphereHandleCap))
			{
				if (Event.current.control)
				{
					Selection.activeGameObject = p_Bone.bone.gameObject;
				}
				else
				{
					Selection.activeGameObject = (target as Polygon)?.gameObject;
					m_SelectedBone = p_Bone;
				}
			}

			DrawDirectionBone(p_Bone, p_Size);
		}

		private void DrawDirectionBone(Bone p_Bone, float p_Size)
		{
			if (!p_Bone.desiredRotationOffset.IsValid() || !m_ReorientationPossible) return;

			Quaternion t_BoneRotation = p_Bone.isReorientated ? p_Bone.bone.rotation : p_Bone.bone.rotation * p_Bone.desiredRotationOffset;

			Handles.color = Handles.zAxisColor;
			Handles.ArrowHandleCap(0, p_Bone.bone.position, t_BoneRotation, p_Size, EventType.Repaint);

			Handles.color = Handles.yAxisColor;
			Handles.ArrowHandleCap(0, p_Bone.bone.position, t_BoneRotation * Quaternion.Euler(-90f, 0f, 0f), p_Size, EventType.Repaint);
		}

		private void DrawHeightHandle(SkeletonBoneReferences p_Bones)
		{
			Handles.color = Handles.zAxisColor;

			EditorGUI.BeginChangeCheck();

			float t_RootScale = 1f;
			if (p_Bones.root.bone != null && p_Bones.root.bone.parent != null)
				t_RootScale = p_Bones.root.bone.parent.lossyScale.z / p_Bones.setupScale;

			Vector3 t_Point = Handles.Slider(
				new Vector3(p_Bones.root.bone.position.x, p_Bones.height * t_RootScale, p_Bones.root.bone.position.z) + Vector3.up * m_Size * .5f,
				Vector3.up, 
				m_Size,
				Handles.CubeHandleCap,
				0f);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Changed Height");
				p_Bones.height = t_Point.y / t_RootScale - m_Size * .5f;
			}
		}

		private void DrawRotationGizmo(Bone p_Bone, float p_Size)
		{
			if (!p_Bone.desiredRotationOffset.IsValid() || p_Bone.bone == null || !m_ReorientationPossible) return;

			Quaternion t_BoneRotation = p_Bone.isReorientated ? p_Bone.bone.rotation : p_Bone.bone.rotation * p_Bone.desiredRotationOffset;

			Handles.color = Handles.yAxisColor;
			Handles.CylinderHandleCap(0, p_Bone.bone.position + t_BoneRotation * Vector3.up * p_Size, t_BoneRotation * Quaternion.Euler(0, 90, 0), p_Size / 3f, EventType.Repaint);

			if (p_Bone.isReorientated)
				return;

			EditorGUI.BeginChangeCheck();
			
			Quaternion t_Rot = Handles.Disc(t_BoneRotation, p_Bone.bone.position, t_BoneRotation * Vector3.forward, p_Size, false, 0.01f);
			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject(target, "Rotated Bone");

				p_Bone.desiredRotationOffset = Quaternion.Inverse(p_Bone.bone.rotation) * t_Rot;
				p_Bone.desiredRotationOffset = t_Rot;
			}
		}

		#endregion
	}
}
