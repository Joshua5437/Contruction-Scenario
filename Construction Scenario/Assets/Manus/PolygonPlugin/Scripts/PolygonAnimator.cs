using Hermes.Protocol.Polygon;
using Manus.Hermes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using HProt = Hermes.Protocol;

namespace Manus.Polygon
{
	[DisallowMultipleComponent]
	[UnityEngine.AddComponentMenu("Manus/Polygon/Polygon Animator")]
	[RequireComponent(typeof(Polygon))]
	public class PolygonAnimator : MonoBehaviour
	{
		private RetargetOption m_LastRetargetOption = RetargetOption.BodyEstimation;
		[SerializeField] private RetargetOption m_RetargetOption = RetargetOption.BodyEstimation;
		private string m_LastTargetSkeletonName = "";
		public string targetSkeletonName = "";
		
		private int m_LastUserIndex = 0;
		private bool m_LastScaleToUser;

		[SerializeField] private bool m_UseSmoothing = true;
		[SerializeField] private float m_Smoothing = 20f;

		public int userIndex = 0;
		public bool scaleToUser = true;
		public bool rootMotion = true;

		private AnimatorParameters m_LastParameters = null;
		[SerializeField] private AnimatorParameters m_Parameters = new AnimatorParameters();

		private uint m_SkeletonID;

		private Polygon m_Polygon;
		private Manus.Hermes.Polygon.PolygonData m_NewData;

		private Dictionary<BoneType, Bone> m_BoneCollection;
		private SkeletonAnimator m_SkeletonAnimator;

		private Coroutine m_ConnectRoutine;
		private float m_Timeout = 2f;
		private float m_CheckIfSettingsChangedTimer = .1f;

		private Quaternion m_RootParentOffset;

		public Bone[] boneList { get; private set; }

		/// <summary>
		/// Returns true if the animator is playingA
		/// </summary>
		public bool isPlaying { get { return m_SkeletonAnimator != null && m_SkeletonAnimator.isPlaying; } }

		/// <summary>
		/// Returns true if skeleton is scaled to the user
		/// </summary>
		public bool isScaled { get { return scaleToUser; } }

		#region Monobehaviour Callbacks

		private void OnEnable()
		{
			//Set user index for hands as well
			foreach (var t_Hand in GetComponentsInChildren<Hand.Hand>())
			{
				t_Hand.usePositionalData = false;
				t_Hand.userIndex = userIndex;
			}

			m_Timeout = 2f;
			m_Polygon = GetComponent<Polygon>();
			m_BoneCollection = m_Polygon.boneReferences.GatherBones(GatherType.Animated);

			ToggleScaling(scaleToUser);

			if (ManusManager.instance?.communicationHub)
				ManusManager.instance.communicationHub.polygonUpdate += UpdateTargets;

			m_ConnectRoutine = StartCoroutine(OnConnect());
		}

		private void OnDisable()
		{
			if (ManusManager.instance?.communicationHub)
				ManusManager.instance.communicationHub.polygonUpdate -= UpdateTargets;

			if (ManusManager.instance?.communicationHub?.careTaker?.Hermes != null && m_Polygon != null)
				ManusManager.instance?.communicationHub?.careTaker?.Hermes?.RemoveSkeleton(new RemoveSkeletonArgs {  SkeletonID = m_SkeletonID });

			if (m_ConnectRoutine != null) StopCoroutine(m_ConnectRoutine);
			m_ConnectRoutine = null;

			m_Polygon = null;
			m_BoneCollection = null;
			m_SkeletonAnimator?.Dispose();
			m_SkeletonAnimator = null;
		}

		private void Awake()
		{
			m_SkeletonID = (uint)UnityEngine.Random.Range(0, int.MaxValue);
		}

		private void Update()
		{
			m_Timeout -= Time.deltaTime;
			m_CheckIfSettingsChangedTimer -= Time.deltaTime;

			if (m_LastScaleToUser != scaleToUser)
			{
				ToggleScaling(scaleToUser);
			}

			if (m_LastRetargetOption != m_RetargetOption || m_LastUserIndex != userIndex || m_LastTargetSkeletonName != targetSkeletonName )
			{
				if (m_SkeletonAnimator != null)
				{

					UpdateTarget();

					//Set user index for hands as well
					foreach (var t_Hand in GetComponentsInChildren<Hand.Hand>())
					{
						t_Hand.userIndex = userIndex;
					}
				}
			}

			if (m_SkeletonAnimator != null)
			{
				if (m_NewData != null)
				{
					ProcessSkeletonData(m_NewData);
					m_NewData = null;
				}
				m_SkeletonAnimator?.AnimateBones(Time.deltaTime);
			}

			if (m_Timeout < 0 && m_ConnectRoutine == null)
			{
				m_SkeletonAnimator?.Dispose();
				m_SkeletonAnimator = null;

				m_Timeout = 2f;
				m_ConnectRoutine = StartCoroutine(OnConnect());
			}

			if (m_CheckIfSettingsChangedTimer < 0 && m_ConnectRoutine == null)
			{
				m_CheckIfSettingsChangedTimer = 0.1f;
				UpdateParameters();
			}
		}

		private void OnValidate()
		{
			if (m_SkeletonAnimator != null)
			{
				m_SkeletonAnimator.ToggleSmoothing(m_UseSmoothing);
				m_SkeletonAnimator.SetSmoothing(m_Smoothing);
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Send skeleton data to hermes and with animating
		/// </summary>
		public void StartAnimating()
		{
			if (m_Polygon == null || m_SkeletonAnimator != null)
				return;

			if (ManusManager.instance?.communicationHub?.careTaker?.Hermes != null && m_Polygon != null)
				ManusManager.instance?.communicationHub?.careTaker?.Hermes?.RemoveSkeleton(new RemoveSkeletonArgs { SkeletonID = m_SkeletonID });

			Bone t_RootBone = m_Polygon.boneReferences.root;
			Transform t_RootParent = t_RootBone.bone.parent;
			float t_ContainerMatrixScale = t_RootParent.lossyScale.z / m_Polygon.boneReferences.setupScale;

			m_RootParentOffset = Quaternion.Inverse(Quaternion.Inverse(t_RootBone.bone.rotation * (t_RootBone.isReorientated ? Quaternion.identity : t_RootBone.desiredRotationOffset)) * t_RootParent.rotation);
			Matrix4x4 t_ContainerMatrixInverse = Matrix4x4.TRS(t_RootParent.position, t_RootParent.rotation * m_RootParentOffset, Vector3.one * t_ContainerMatrixScale).inverse;

			Skeleton t_Skeleton = new Skeleton {
				DeviceID = m_SkeletonID,
				RescaleSkeleton = scaleToUser,
				Data = new SkeletonData { Height = m_Polygon.boneReferences.height },
			};

			// Add bones
			foreach (var t_Bone in m_Polygon.boneReferences.GatherBones(GatherType.All))
			{
				var t_ProtoBone = new HProt.Polygon.Bone 
				{ 
					Type = t_Bone.Value.type,
					Position = new HProt.Translation { Full = t_ContainerMatrixInverse.MultiplyPoint3x4(t_Bone.Value.bone.position).ToProto() },
					Rotation = new HProt.Orientation { Full = (t_ContainerMatrixInverse.rotation * (t_Bone.Value.bone.rotation * (t_Bone.Value.isReorientated ? Quaternion.identity : t_Bone.Value.desiredRotationOffset))).ToProto() }
				};

				t_Skeleton.Bones.Add(t_ProtoBone);
			}

			// Add Target
			switch (m_RetargetOption)
			{
				case RetargetOption.BodyEstimation:
					t_Skeleton.BodyEstimation = new HProt.Polygon.BodyEstimationTarget { User = userIndex };
					break;
				case RetargetOption.TargetSkeleton:
					t_Skeleton.TargetSkeleton = new HProt.Polygon.SkeletonTarget { Name = targetSkeletonName };
					break;
			}

			if (ManusManager.instance?.communicationHub?.careTaker?.Hermes == null || !ManusManager.instance.communicationHub.careTaker.connected)
			{
				Debug.LogError("Hermes not connected");
				return;
			}

			var t_Complete = ManusManager.instance.communicationHub.careTaker.Hermes.AddSkeleton(t_Skeleton).Type;
			m_SkeletonAnimator = new SkeletonAnimator(m_Polygon, m_UseSmoothing, m_Smoothing);

			m_LastRetargetOption = m_RetargetOption;
			m_LastTargetSkeletonName = targetSkeletonName;
			m_LastUserIndex = userIndex;
		}

		/// <summary>
		/// Resumes playing the received data
		/// </summary>
		public void Resume()
		{
			m_SkeletonAnimator?.Play();
		}

		/// <summary>
		/// Pauses the playing of animations
		/// </summary>
		public void Pause()
		{
			m_SkeletonAnimator?.Pause();
		}

		/// <summary>
		/// Set root motion to true or false
		/// </summary>
		/// <param name="p_On">on or off</param>
		public void SetRootMotion(bool  p_On)
		{
			rootMotion = p_On;
		}

		/// <summary>
		/// Set the character to the bindpose (reset pose to default)
		/// </summary>
		[ContextMenu("SetToBindPose")]
		public void SetToBindPose()
		{
			m_SkeletonAnimator.Pause();
			m_SkeletonAnimator.Reset(false);
		}

		/// <summary>
		/// Continue animation
		/// </summary>
		[ContextMenu("ContinueAnimatingFromBindPose")]
		public void ContinueAnimatingFromBindPose()
		{
			m_SkeletonAnimator.ApplyAnimationData();
			m_SkeletonAnimator.Play();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Update the animation target
		/// </summary>
		/// <param name="p_Poly"></param>
		private void UpdateTargets(Manus.Hermes.Polygon.PolygonData p_Poly)
		{
			m_NewData = p_Poly;
		}

		/// <summary>
		/// Waits for connection with hermes to start animating
		/// </summary>
		private IEnumerator OnConnect()
		{
			while (ManusManager.instance.communicationHub.careTaker == null || !ManusManager.instance.communicationHub.careTaker.connected)
			{
				yield return new WaitForSeconds(.2f);
			}

			StartAnimating();
			m_ConnectRoutine = null;
		}

		/// <summary>
		/// Update the parameters in manus-core
		/// </summary>
		private void UpdateParameters()
		{
			if (m_LastParameters != null && m_Parameters.Simular(m_LastParameters))
				return;

			var t_Complete = ManusManager.instance.communicationHub.careTaker.Hermes.SetSkeletonSettings(new HProt.Polygon.Retargeting.SettingsArgs { ID = m_SkeletonID, Settings = m_Parameters}).Type;
			if (t_Complete == HProt.CompleteType.Complete)
				m_LastParameters = m_Parameters.Copy();
		}

		/// <summary>
		/// Add/Remove scale/anti-scale bones in the skeleton
		/// </summary>
		private void ToggleScaling(bool p_Scale)
		{
			m_SkeletonAnimator?.Dispose();
			m_SkeletonAnimator = null;

			if (p_Scale)
			{
				m_Polygon.boneReferences.GenerateTransformParents();
			} 
			m_LastScaleToUser = scaleToUser;

			if (m_ConnectRoutine != null) StopCoroutine(m_ConnectRoutine);
			m_ConnectRoutine = StartCoroutine(OnConnect());

		}

		/// <summary>
		/// Update the skeleton target
		/// </summary>
		private void UpdateTarget()
		{
			var t_Target = new HProt.Polygon.SetTargetArgs { DeviceID = m_SkeletonID };

			switch (m_RetargetOption)
			{
				case RetargetOption.BodyEstimation:
					t_Target.BodyEstimation = new HProt.Polygon.BodyEstimationTarget { User = userIndex };
					break;
				case RetargetOption.TargetSkeleton:
					t_Target.TargetSkeleton = new HProt.Polygon.SkeletonTarget { Name = targetSkeletonName };
					break;
			}

			m_LastRetargetOption = m_RetargetOption;
			m_LastTargetSkeletonName = targetSkeletonName;
			m_LastUserIndex = userIndex;

			if (ManusManager.instance?.communicationHub?.careTaker?.Hermes != null && ManusManager.instance.communicationHub.careTaker.connected)
				ManusManager.instance.communicationHub.careTaker.Hermes.SetSkeletonTarget(t_Target);
		}

		/// <summary>
		/// Converts the received data from hermes to object-local and applies data to the animator
		/// </summary>
		/// <param name="p_Poly">Polygon data received from manus-core</param>
		private void ProcessSkeletonData(Manus.Hermes.Polygon.PolygonData p_Poly)
		{
			foreach (var t_Skeleton in p_Poly.skeletons)
			{
				if (t_Skeleton.id != m_SkeletonID || !isPlaying)
					continue;

				m_Timeout = 2f;
				ApplyDataToSkeleton(t_Skeleton);
				UpdateAnimationTargets();
				m_SkeletonAnimator.ApplyAnimationData();
			}
		}

		#region Apply new animationdata

		/// <summary>
		/// Apply data from manus-core on skeleton
		/// </summary>
		/// <param name="p_Target">Skeleton received from manus-core</param>
		private void ApplyDataToSkeleton(Manus.Hermes.Polygon.Skeleton p_Target)
		{
			if (!isPlaying)
				return;

			// Scale model to base scale
			Transform t_Container = m_Polygon.boneReferences.root.bone.parent;
			float t_ContainerMatrixScale = t_Container.lossyScale.z / m_Polygon.boneReferences.setupScale;

			Matrix4x4 t_ContainerMatrix = Matrix4x4.TRS(t_Container.position, t_Container.rotation * m_RootParentOffset, Vector3.one * t_ContainerMatrixScale);

			// Apply world space data
			for (int i = 0; i < p_Target.bones.Count; i++)
			{
				if (!m_BoneCollection.ContainsKey(p_Target.bones[i].type))
					continue;

				Bone t_Bone = m_BoneCollection[p_Target.bones[i].type];
				
				t_Bone.bone.position = t_ContainerMatrix.MultiplyPoint3x4(p_Target.bones[i].position);
				t_Bone.bone.rotation = t_ContainerMatrix.rotation * p_Target.bones[i].rotation * (t_Bone.isReorientated ? Quaternion.identity : Quaternion.Inverse(t_Bone.desiredRotationOffset));
				m_SkeletonAnimator.ApplyScaleInstantly(p_Target.bones[i].type, p_Target.bones[i].scale);
			}
		}

		/// <summary>
		/// Update the current transforms in the animator
		/// </summary>
		private void UpdateAnimationTargets()
		{
			boneList = m_BoneCollection.Values.ToArray();

			for (int i = 0; i < boneList.Length; i++)
			{
				var t_Bone = boneList[i];
				
				if (!rootMotion && t_Bone.type == BoneType.Root)
					m_SkeletonAnimator?.UpdateTarget(t_Bone.type, false, Vector3.zero, Quaternion.identity * m_RootParentOffset * Quaternion.Inverse(m_Polygon.boneReferences.root.isReorientated ? Quaternion.identity :m_Polygon.boneReferences.root.desiredRotationOffset), t_Bone.bone.localScale.z);
				else
					m_SkeletonAnimator?.UpdateTarget(t_Bone.type, false, t_Bone.bone.localPosition, t_Bone.bone.localRotation, t_Bone.bone.localScale.z);
			}
		}

		#endregion

		#endregion
	}

	/// <summary>
	/// Toggle to select what the skeleton should retarget on
	/// </summary>
	public enum RetargetOption
	{
		BodyEstimation,
		TargetSkeleton
	}
}