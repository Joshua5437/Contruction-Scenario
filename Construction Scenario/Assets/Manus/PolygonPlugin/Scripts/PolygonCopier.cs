using Hermes.Protocol.Polygon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manus.Polygon
{
	[DisallowMultipleComponent]
	[UnityEngine.AddComponentMenu("Manus/Polygon/Polygon Copier")]
	[RequireComponent(typeof(Polygon))]
	public class PolygonCopier : MonoBehaviour
	{
		private Polygon m_Polygon;
		private InitialBone[] m_InitialBones;
		private AnimatedBone[] m_AnimatedBones;

		private bool m_ScaledToUser = false;
		public PolygonAnimator target;

		private void Awake()
		{
			m_Polygon = GetComponent<Polygon>();
		}

		private void OnDisable()
		{
			ResetPose();
			m_Polygon.boneReferences.RemoveTransformParents();

			m_InitialBones = null;
			m_AnimatedBones = null;
		}

		public void LateUpdate()
		{
			if (m_InitialBones == null || m_AnimatedBones == null)
			{
				SetupInitialPosition();
				Setup();
			}

			var t_TargetBones = target.boneList;
			if (t_TargetBones == null)
				return;

			if (target.isScaled != m_ScaledToUser)
				Setup();

			for (int i = 0; i < t_TargetBones.Length; i++)
			{
				var t_TargetBone = t_TargetBones[i];
				m_AnimatedBones[i].bone.localPosition = t_TargetBone.bone.localPosition;
				m_AnimatedBones[i].bone.localRotation = t_TargetBone.bone.localRotation;

				if (m_ScaledToUser && t_TargetBone.bone.localScale.z > 0 && m_AnimatedBones[i].scaler != null && m_AnimatedBones[i].bone.localScale.z != t_TargetBone.bone.localScale.z)
					m_AnimatedBones[i].scaler.ScaleBone(t_TargetBone.bone.localScale.z, t_TargetBone.type == BoneType.Root ? ScaleAxis.All : ScaleAxis.Length, ScaleMode.Percentage);
			}
		}

		/// <summary>
		/// Save initial position
		/// </summary>
		private void SetupInitialPosition()
		{
			var t_Bones = m_Polygon.boneReferences.GatherBones(GatherType.Animated).Values.ToArray();
			m_InitialBones = new InitialBone[t_Bones.Length];

			for (int i = 0; i < t_Bones.Length; i++)
			{
				m_InitialBones[i] = new InitialBone();
				m_InitialBones[i].position = t_Bones[i].bone.localPosition;
				m_InitialBones[i].rotation = t_Bones[i].bone.localRotation;
			}
		}

		/// <summary>
		/// Setup the initial bone positions and animated bones
		/// </summary>
		private void Setup()
		{
			var t_Bones = m_Polygon.boneReferences.GatherBones(GatherType.Animated).Values.ToArray();
			m_ScaledToUser = target.isScaled;

			if (m_ScaledToUser)
			{
				ResetPose();
				m_Polygon.boneReferences.GenerateTransformParents();
				SetupInitialPosition();
			}
			else
			{
				ResetPose();
				m_Polygon.boneReferences.RemoveTransformParents();
				SetupInitialPosition();
			}

			m_AnimatedBones = new AnimatedBone[t_Bones.Length];
			for (int i = 0; i < t_Bones.Length; i++)
			{
				m_AnimatedBones[i] = new AnimatedBone();
				m_AnimatedBones[i].bone = t_Bones[i].bone;

				if (m_ScaledToUser && m_Polygon.boneReferences.scaler.boneScalers.ContainsKey(t_Bones[i].type))
					m_AnimatedBones[i].scaler = m_Polygon.boneReferences.scaler.boneScalers[t_Bones[i].type];
			}
		}
	
		/// <summary>
		/// Set character back to the starting pose
		/// </summary>
		private void ResetPose()
		{
			if (m_AnimatedBones == null)
				return;

			for (int i = 0; i < m_AnimatedBones.Length; i++)
			{
				m_AnimatedBones[i].bone.localPosition = m_InitialBones[i].position;
				m_AnimatedBones[i].bone.localRotation = m_InitialBones[i].rotation;

				if (m_AnimatedBones[i].scaler != null)
					m_AnimatedBones[i].scaler.ScaleBone(1, ScaleAxis.All, ScaleMode.Percentage);
			}
		}

	}

	/// <summary>
	/// Bone thats animated
	/// </summary>
	public struct AnimatedBone
	{
		public Transform bone;
		public BoneScaler scaler;
	}

	/// <summary>
	/// Initial bone transform
	/// </summary>
	public struct InitialBone
	{
		public Vector3 position;
		public Quaternion rotation;
	}
}
