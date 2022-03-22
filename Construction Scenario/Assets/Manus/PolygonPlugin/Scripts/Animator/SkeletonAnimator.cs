using Hermes.Protocol.Polygon;
using Manus.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public class SkeletonAnimator : IDisposable
	{
		public class AnimatedBone
		{
			public Transform bone;
			public BoneScaler scaleBone;

			public Vector3 startPosition;
			public Quaternion startRotation;

			public bool first;
			public bool instant;

			public bool world;
			public Vector3? targetPosition;
			public Quaternion? targetRotation;
			public float? targetScale;

			public Vector3 position;
			public Quaternion rotation;
			public float scale;

			public MovingAverageVector movingPosition;
			public MovingAverageQuaternion movingRotation;
		}

		[SerializeField] private bool m_UseSmoothing = true;
		[SerializeField] private float m_Smoothing = 20f;
		private Dictionary<BoneType, AnimatedBone> m_AnimatedBones;

		/// <summary>
		/// Returns true if currently playing
		/// </summary>
		public bool isPlaying { get; private set; } = false;

		public SkeletonAnimator(Polygon p_Polygon, bool p_UseSmoothing, float p_Smoothing)
		{
			var t_BoneCollection = p_Polygon.boneReferences.GatherBones(GatherType.Animated);

			isPlaying = true;
			m_UseSmoothing = p_UseSmoothing;
			m_Smoothing = p_Smoothing;

			m_AnimatedBones = new Dictionary<BoneType, AnimatedBone>();
			foreach (var t_BoneItem in t_BoneCollection)
			{
				m_AnimatedBones.Add(t_BoneItem.Key, new AnimatedBone
				{
					bone = t_BoneItem.Value.bone,
					scaleBone = p_Polygon.boneReferences.scaler != null && p_Polygon.boneReferences.scaler.boneScalers.ContainsKey(t_BoneItem.Key) ? p_Polygon.boneReferences.scaler.boneScalers[t_BoneItem.Key] : null,
					first = true,
					instant = false,
					world = false,
					targetPosition = null,
					targetRotation = null,
					targetScale = null,
					startPosition = t_BoneItem.Value.bone.localPosition,
					startRotation = t_BoneItem.Value.bone.localRotation,
					position = t_BoneItem.Value.bone.localPosition,
					rotation = t_BoneItem.Value.bone.localRotation,
					scale = 1,
					movingPosition = new MovingAverageVector(t_BoneItem.Value.bone.localPosition),
					movingRotation = new MovingAverageQuaternion(t_BoneItem.Value.bone.localRotation)
				});
			}
		}

		/// <summary>
		/// Update tracker position/rotation to move towards
		/// </summary>
		/// <param name="p_Type">Trackertype</param>
		/// <param name="p_World">In world space</param>
		/// <param name="p_Position">New position</param>
		/// <param name="p_Rotation">New rotation</param>
		public void UpdateTarget(BoneType p_Type, bool p_World, Vector3? p_Position = null, Quaternion? p_Rotation = null, float? p_Scale = null)
		{
			if (!m_AnimatedBones.ContainsKey(p_Type))
				return;

			var t_Bone = m_AnimatedBones[p_Type];
			t_Bone.instant = false;
			if (t_Bone.first)
			{
				t_Bone.first = false;
				t_Bone.instant = true;
			}

			if (p_Scale != null && p_Scale.Value <= 0)
				p_Scale = 1;

			t_Bone.world = p_World;
			t_Bone.targetPosition = p_Position;
			t_Bone.targetRotation = p_Rotation;
			t_Bone.targetScale = p_Scale;
		}

		/// <summary>
		/// Animate all changed values to the new position/rotation, run this from the update fuction
		/// </summary>
		/// <param name="p_DeltaTime">Time since last update, use Time.deltaTime for the time since last frame</param>
		public void AnimateBones(float p_DeltaTime)
		{
			if (!isPlaying) return;

			foreach (var t_AnimatedBoneKeyValue in m_AnimatedBones)
			{
				var t_AnimatedBone = t_AnimatedBoneKeyValue.Value;
				bool t_Smooth = (!m_UseSmoothing || t_AnimatedBone.instant);

				if (t_AnimatedBone.targetPosition != null)
				{
					t_AnimatedBone.position = t_Smooth ? t_AnimatedBone.targetPosition.Value : Vector3.Lerp(t_AnimatedBone.position, t_AnimatedBone.targetPosition.Value, p_DeltaTime * m_Smoothing);
					t_AnimatedBone.movingPosition.AddData(t_AnimatedBone.position, t_Smooth);
				}

				if (t_AnimatedBone.targetRotation != null)
				{
					var t_NewRotation = t_Smooth ? t_AnimatedBone.targetRotation.Value : Quaternion.Lerp(t_AnimatedBone.rotation, t_AnimatedBone.targetRotation.Value, p_DeltaTime * m_Smoothing);
					if (t_NewRotation.IsValid())
					{
						t_AnimatedBone.rotation = t_NewRotation;
						t_AnimatedBone.movingRotation.AddData(t_NewRotation, t_Smooth);
					}
				}

				if (t_AnimatedBone.targetScale != null && t_AnimatedBone.scaleBone != null)
				{
					t_AnimatedBone.scale = t_Smooth ? t_AnimatedBone.targetScale.Value : Mathf.Lerp(t_AnimatedBone.scale, t_AnimatedBone.targetScale.Value, p_DeltaTime * m_Smoothing);
				}
			}

			ApplyAnimationData();
		}

		/// <summary>
		/// Apply the smoothed data to the skeleton
		/// </summary>
		public void ApplyAnimationData()
		{
			foreach (var t_AnimatedBoneKeyValue in m_AnimatedBones)
			{
				var t_AnimatedBone = t_AnimatedBoneKeyValue.Value;

				if (t_AnimatedBone.world)
				{
					t_AnimatedBone.bone.position = m_UseSmoothing ? t_AnimatedBone.movingPosition.GetValue() : t_AnimatedBone.position;
					t_AnimatedBone.bone.rotation = m_UseSmoothing ? t_AnimatedBone.movingRotation.GetValue() : t_AnimatedBone.rotation;
				}
				else
				{
					t_AnimatedBone.bone.localPosition = m_UseSmoothing ? t_AnimatedBone.movingPosition.GetValue() : t_AnimatedBone.position;
					t_AnimatedBone.bone.localRotation = m_UseSmoothing ? t_AnimatedBone.movingRotation.GetValue() : t_AnimatedBone.rotation;
				}

				if (t_AnimatedBone.scaleBone != null && t_AnimatedBone.scaleBone.scale.z != t_AnimatedBone.scale)
				{
					ScaleAxis t_Axis = ScaleUniformly(t_AnimatedBoneKeyValue.Key) ? ScaleAxis.All : ScaleAxis.Length;
					t_AnimatedBone.scaleBone.ScaleBone(t_AnimatedBone.scale, t_Axis, ScaleMode.Percentage);
				}
			}
		}

		/// <summary>
		/// Set the scale of the bone directly
		/// </summary>
		/// <param name="p_Type">BoneType</param>
		/// <param name="p_Scale">Scale percentage</param>
		public void ApplyScaleInstantly(BoneType p_Type, float p_Scale)
		{
			if (!m_AnimatedBones.TryGetValue(p_Type, out var t_AnimatedBone))
				return;

			if (p_Scale <= 0)
				p_Scale = 1;

			if (t_AnimatedBone.scaleBone != null && t_AnimatedBone.scaleBone.scale.z != p_Scale)
			{
				ScaleAxis t_Axis = ScaleUniformly(p_Type) ? ScaleAxis.All : ScaleAxis.Length;
				t_AnimatedBone.scaleBone.ScaleBone(p_Scale, t_Axis, ScaleMode.Percentage);
			}
		}

		/// <summary>
		/// Toggle the smoothing of the animation
		/// </summary>
		/// <param name="p_UseSmoothing">true for on</param>
		public void ToggleSmoothing(bool p_UseSmoothing)
		{
			m_UseSmoothing = p_UseSmoothing;
		}

		/// <summary>
		/// Set smoothing value
		/// </summary>
		/// <param name="p_SmoothingValue">Value to smooth by (lower is more smoothing)</param>
		public void SetSmoothing(float p_SmoothingValue)
		{
			m_Smoothing = p_SmoothingValue;
		}

		/// <summary>
		/// Play/Resumes the animation
		/// </summary>
		public void Play()
		{
			isPlaying = true;
		}

		/// <summary>
		/// Pauses the animation
		/// </summary>
		public void Pause()
		{
			isPlaying = false;
		}

		/// <summary>
		/// Resets the animation to the start
		/// </summary>
		public void Reset(bool p_AlsoScale = true)
		{
			foreach (var t_AnimatedBone in m_AnimatedBones.Values)
			{
				t_AnimatedBone.world = false;

				t_AnimatedBone.bone.localPosition = t_AnimatedBone.startPosition;
				t_AnimatedBone.bone.localRotation = t_AnimatedBone.startRotation;

				if (p_AlsoScale && t_AnimatedBone.scaleBone != null)
					t_AnimatedBone.scaleBone.ScaleBone(1, ScaleAxis.All, ScaleMode.Percentage);
			}
		}

		/// <summary>
		/// Dispose of the animator
		/// </summary>
		public void Dispose()
		{
			Reset();
		}

		private bool ScaleUniformly(BoneType p_Type)
		{
			return p_Type == BoneType.Root
				|| p_Type == BoneType.Head
				|| p_Type == BoneType.LeftHand
				|| p_Type == BoneType.RightHand
				|| p_Type == BoneType.LeftFoot
				|| p_Type == BoneType.RightFoot;
		}
	}
}