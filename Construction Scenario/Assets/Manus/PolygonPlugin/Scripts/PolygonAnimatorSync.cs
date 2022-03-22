using System.Collections;
using System.Collections.Generic;
using Manus.Networking.Sync;
using UnityEngine;
using Hermes.Protocol.Polygon;
using LidNet = Lidgren.Network;

namespace Manus.Polygon.Networking
{
	[DisallowMultipleComponent]
	[UnityEngine.AddComponentMenu("Manus/Networking/Sync/Polygon Animator (Sync)")]
	[RequireComponent(typeof(Polygon))]
	[RequireComponent(typeof(PolygonAnimator))]
	public class PolygonAnimatorSync : BaseSync
	{
		private Polygon m_Polygon;
		private PolygonAnimator m_PolygonAnimator;

		private bool m_Owned = false;
		private bool m_Scaled = false;
		private SkeletonAnimator m_SkeletonAnimator;

		private Dictionary<BoneType, Bone> m_BoneCollection;

		[SerializeField, Tooltip("Smooth the received data")] private bool m_UseSmoothing = true;
		[SerializeField, Tooltip("The amount of smoothing applied to the received data (lower means more smoothing)")] private float m_Smoothing = 20.0f;

		#region Monobehaviour Callbacks

		private void Update()
		{
			if (m_SkeletonAnimator != null && !m_Owned)
				m_SkeletonAnimator.AnimateBones(Time.deltaTime);
		}

		#endregion

		private void ToggleParents(bool p_UseParents)
		{
			m_SkeletonAnimator?.Reset();
			m_SkeletonAnimator = null;

			if (p_UseParents)
				m_Polygon.boneReferences.GenerateTransformParents();
			else
				m_Polygon.boneReferences.RemoveTransformParents();

			m_Scaled = p_UseParents;
		}

		public override void Initialize(Manus.Networking.NetObject p_Object)
		{
			if (m_Polygon == null) m_Polygon = GetComponent<Polygon>();
			if (m_PolygonAnimator == null) m_PolygonAnimator = GetComponent<PolygonAnimator>();
		}

		public override void Clean()
		{
		}

		public override bool IsDirty()
		{
			return true;
		}

		public override void ReceiveData(LidNet.NetBuffer p_Msg)
		{
			if (p_Msg.ReadBoolean())
			{

				bool t_Scaled = p_Msg.ReadBoolean();
				if (t_Scaled != m_Scaled)
				{
					ToggleParents(t_Scaled);
				}

				if (m_SkeletonAnimator == null)
				{
					m_SkeletonAnimator = new SkeletonAnimator(m_Polygon, m_UseSmoothing, m_Smoothing);
				}

				int t_Count = p_Msg.ReadInt32();
				for (int i = 0; i < t_Count; i++)
				{
					var t_Type = (BoneType)p_Msg.ReadInt32();
					var t_Position = p_Msg.ReadVector3();
					var t_Rotation = p_Msg.ReadQuaternion();
					var t_Scale = p_Msg.ReadFloat();

					m_SkeletonAnimator?.UpdateTarget(t_Type, false, t_Position, t_Rotation, t_Scale);
				}
			} else
			{
				ToggleParents(false);
			}
		}

		public override void WriteData(LidNet.NetBuffer p_Msg)
		{
			if (m_PolygonAnimator.enabled == false || gameObject.activeInHierarchy == false)
			{
				p_Msg.Write(false);
				return;
			}

			if (m_BoneCollection == null || m_Scaled != m_PolygonAnimator.isScaled)
			{
				m_Scaled = m_PolygonAnimator.isScaled;
				m_BoneCollection = m_Polygon.boneReferences.GatherBones(GatherType.Animated);

				p_Msg.Write(false);
			}
			else
			{
				// Write true so the other client knows there will be bone data
				p_Msg.Write(true);

				p_Msg.Write(m_Scaled);
				p_Msg.Write(m_BoneCollection.Count);

				foreach (var bone in m_BoneCollection)
				{
					p_Msg.Write((int)bone.Key);
					p_Msg.Write(bone.Value.bone.localPosition);
					p_Msg.Write(bone.Value.bone.localRotation);
					p_Msg.Write(bone.Value.bone.localScale.z);
				}
			}
		}

		public override void OnGainOwnership(Manus.Networking.NetObject p_Object)
		{
			m_Owned = true;
			ToggleParents(false);

			m_BoneCollection = null;
			if (m_Polygon == null) m_Polygon = GetComponent<Polygon>();
			if (m_PolygonAnimator == null) m_PolygonAnimator = GetComponent<PolygonAnimator>();
			if (m_PolygonAnimator != null) m_PolygonAnimator.enabled = true;
		}

		public override void OnLoseOwnership(Manus.Networking.NetObject _Object)
		{
			m_Owned = false;
			ToggleParents(false);

			m_BoneCollection = null;
			if (m_Polygon == null) m_Polygon = GetComponent<Polygon>();
			if (m_PolygonAnimator == null) m_PolygonAnimator = GetComponent<PolygonAnimator>();
			if (m_PolygonAnimator != null) m_PolygonAnimator.enabled = false;
		}
	}
}

