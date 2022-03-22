using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HProt = Hermes.Protocol;
using BoneType = Hermes.Protocol.Polygon.BoneType;
using Manus.Hermes;

namespace Manus.Polygon
{
	/// <summary>
	/// Tranform values including a position and rotation
	/// </summary>
	public class TransformValues
	{
		public Vector3 position;
		public Quaternion rotation;
	}

	[RequireComponent(typeof(Polygon))]
	public class AnimationRetargetTarget : MonoBehaviour
	{
		/// <summary>
		/// The name (ID) of the skeleton
		/// </summary>
		public string targetName;

		private HProt.Polygon.TargetSkeleton m_StartSkeleton;

		private Polygon m_Polygon;
		private Dictionary<BoneType, TransformValues> m_Skeleton;
		private Dictionary<BoneType, Bone> m_BoneCollection;

		private Quaternion m_RootParentOffset;
		private Coroutine m_StartCoroutine;

		private void Awake()
		{
			m_Polygon = GetComponent<Polygon>();
			m_Skeleton = new Dictionary<BoneType, TransformValues>();

			Bone t_RootBone = m_Polygon.boneReferences.root;
			Transform t_RootParent = t_RootBone.bone.parent;
			float t_ContainerMatrixScale = t_RootParent.lossyScale.z / m_Polygon.boneReferences.setupScale; 
			m_RootParentOffset = Quaternion.Inverse(Quaternion.Inverse(t_RootBone.bone.rotation * (t_RootBone.isReorientated ? Quaternion.identity : t_RootBone.desiredRotationOffset)) * t_RootParent.rotation);

			m_BoneCollection = m_Polygon.boneReferences.GatherBones(GatherType.Animated);
			bool t_AddOffsets = !m_Polygon.boneReferences.generatedParents;

			// Generate target dictionary
			Matrix4x4 t_ContainerMatrixInverse = Matrix4x4.TRS(t_RootParent.position, t_RootParent.rotation * m_RootParentOffset, Vector3.one * t_ContainerMatrixScale).inverse;
			foreach (var t_Bone in m_BoneCollection)
			{
				BoneType t_Type = t_Bone.Key;

				Vector3 t_Position = t_ContainerMatrixInverse.MultiplyPoint3x4(t_Bone.Value.bone.position);
				Quaternion t_Rotation = t_ContainerMatrixInverse.rotation * (t_Bone.Value.bone.rotation * (t_AddOffsets ? t_Bone.Value.desiredRotationOffset : Quaternion.identity));

				if (!m_Skeleton.ContainsKey(t_Bone.Key))
					m_Skeleton.Add(t_Type, new TransformValues { position = t_Position, rotation = t_Rotation });
			}

			m_StartSkeleton = GetProtoSkeleton();
		}

		private void OnEnable()
		{
			if (m_StartCoroutine != null)
				StopCoroutine(m_StartCoroutine);

			m_StartCoroutine = StartCoroutine(OnConnect());
		}

		private void OnDisable()
		{
			if (m_StartCoroutine != null)
				StopCoroutine(m_StartCoroutine);
			m_StartCoroutine = null;

			if (ManusManager.instance?.communicationHub?.careTaker?.Hermes == null || !ManusManager.instance.communicationHub.careTaker.connected)
				return;

			ManusManager.instance.communicationHub.careTaker.Hermes.RemoveTargetSkeleton(new Google.Protobuf.WellKnownTypes.StringValue { Value = targetName });
		}

		private void Update()
		{
			if ((ManusManager.instance.communicationHub.careTaker == null || !ManusManager.instance.communicationHub.careTaker.connected) && m_StartCoroutine == null)
				m_StartCoroutine = StartCoroutine(OnConnect());

			UpdatePoints();
		}

		/// <summary>
		/// On connect add the target skeleton
		/// </summary>
		private IEnumerator OnConnect()
		{
			while (ManusManager.instance.communicationHub.careTaker == null || !ManusManager.instance.communicationHub.careTaker.connected)
			{
				yield return new WaitForSeconds(.2f);
			}
			
			ManusManager.instance.communicationHub.careTaker.Hermes.AddTargetSkeleton( m_StartSkeleton );

			m_StartCoroutine = null;
		}

		/// <summary>
		/// Update the target skeleton in manus-core
		/// </summary>
		private void UpdatePoints()
		{
			Transform t_RootParent = m_Polygon.boneReferences.root.bone.parent;
			float t_ContainerMatrixScale = t_RootParent.lossyScale.z / m_Polygon.boneReferences.setupScale;
			Matrix4x4 t_ContainerMatrixInverse = Matrix4x4.TRS(t_RootParent.position, t_RootParent.rotation * m_RootParentOffset, Vector3.one * t_ContainerMatrixScale).inverse;

			bool t_AddOffsets = !m_Polygon.boneReferences.generatedParents;

			foreach (var t_Bone in m_BoneCollection)
			{
				m_Skeleton[t_Bone.Key] = new TransformValues
				{
					position = t_ContainerMatrixInverse.MultiplyPoint3x4(t_Bone.Value.bone.position),
					rotation = t_ContainerMatrixInverse.rotation * t_Bone.Value.bone.rotation * (t_AddOffsets ? t_Bone.Value.desiredRotationOffset : Quaternion.identity)
				};
			}

			if (ManusManager.instance?.communicationHub?.careTaker?.Hermes == null || !ManusManager.instance.communicationHub.careTaker.connected)
				return;

			ManusManager.instance.communicationHub.careTaker.Hermes.UpdateTargetSkeleton( GetProtoSkeleton() );
		}

		/// <summary>
		/// Convert skeleton to proto skeleton
		/// </summary>
		/// <returns>Protocol version of the skeleton</returns>
		private HProt.Polygon.TargetSkeleton GetProtoSkeleton()
		{
			if (m_Skeleton == null)
				return new HProt.Polygon.TargetSkeleton();

			var t_SkeletonProt = new HProt.Polygon.TargetSkeleton
			{
				Name = targetName,
				Data = new HProt.Polygon.SkeletonData { Height = m_Polygon.boneReferences.height }
			};

			foreach (var t_Bone in m_Skeleton)
			{
				t_SkeletonProt.Bones.Add(new HProt.Polygon.Bone
				{
					Type = t_Bone.Key,
					Position = new HProt.Translation { Full = t_Bone.Value.position.ToProto() },
					Rotation = new HProt.Orientation { Full = t_Bone.Value.rotation.ToProto() },
					Scale = 1
				});
			}

			return t_SkeletonProt;
		}

	}
}