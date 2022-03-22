using Hermes.Protocol.Polygon;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Manus.Polygon
{
	[RequireComponent(typeof(Polygon))]
	public class ColliderSetup : MonoBehaviour
	{
		[SerializeField] private bool m_Trigger = false;

		[Header("Limbs")]
		[SerializeField, Range(.1f, 3f)] private float m_LegThickness = 1f;
		[SerializeField, Range(.1f, 3f)] private float m_ArmThickness = 1f;

		[Header("Torso")]
		[SerializeField, Range(.1f, 3f)] private float m_BodyWidth = 1f;
		[SerializeField, Range(.1f, 3f)] private float m_BodyThickness = 1f;
		[SerializeField, Range(.1f, 3f)] private float m_LastFill = 1f;

		[Header("Head")]
		[SerializeField, Range(.1f, 3f)] private float m_NeckThickness = 1f;
		[SerializeField] private Vector3 m_HeadOffset = new Vector3(0, 0, 0);
		[SerializeField, Range(.1f, 3f)] private float m_HeadSize = 1f;

		private Dictionary<BoneType, ColliderInfo> m_AllColliders;

		[SerializeField, HideInInspector] private Collider[] m_Colliders = new Collider[0];

		private Polygon m_Polygon;

		/// <summary>
		/// Auto create colliders for a polygon character
		/// </summary>
		[ContextMenu("Setup")]
		private void Setup()
		{
			if (m_Polygon == null) m_Polygon = GetComponent<Polygon>();
			ResetAll();

			m_AllColliders = new Dictionary<BoneType, ColliderInfo>();

			// Create Legs
			m_AllColliders.Add(BoneType.LeftUpperLeg, CreateCapsule(m_Polygon.boneReferences.legLeft.upperLeg.bone, m_Polygon.boneReferences.legLeft.lowerLeg.bone, .07f, m_LegThickness));
			m_AllColliders.Add(BoneType.LeftLowerLeg, CreateCapsule(m_Polygon.boneReferences.legLeft.lowerLeg.bone, m_Polygon.boneReferences.legLeft.foot.bone, .05f, m_LegThickness));

			m_AllColliders.Add(BoneType.RightUpperLeg, CreateCapsule(m_Polygon.boneReferences.legRight.upperLeg.bone, m_Polygon.boneReferences.legRight.lowerLeg.bone, .07f, m_LegThickness));
			m_AllColliders.Add(BoneType.RightLowerLeg, CreateCapsule(m_Polygon.boneReferences.legRight.lowerLeg.bone, m_Polygon.boneReferences.legRight.foot.bone, .05f, m_LegThickness));

			// Create Arms
			m_AllColliders.Add(BoneType.LeftShoulder, CreateCapsule(m_Polygon.boneReferences.armLeft.shoulder.bone, m_Polygon.boneReferences.armLeft.upperArm.bone, .06f, m_ArmThickness));
			m_AllColliders.Add(BoneType.LeftUpperArm, CreateCapsule(m_Polygon.boneReferences.armLeft.upperArm.bone, m_Polygon.boneReferences.armLeft.lowerArm.bone, .06f, m_ArmThickness));
			m_AllColliders.Add(BoneType.LeftLowerArm, CreateCapsule(m_Polygon.boneReferences.armLeft.lowerArm.bone, m_Polygon.boneReferences.armLeft.hand.wrist.bone, .03f, m_ArmThickness));

			m_AllColliders.Add(BoneType.RightShoulder, CreateCapsule(m_Polygon.boneReferences.armRight.shoulder.bone, m_Polygon.boneReferences.armRight.upperArm.bone, .06f, m_ArmThickness));
			m_AllColliders.Add(BoneType.RightUpperArm, CreateCapsule(m_Polygon.boneReferences.armRight.upperArm.bone, m_Polygon.boneReferences.armRight.lowerArm.bone, .06f, m_ArmThickness));
			m_AllColliders.Add(BoneType.RightLowerArm, CreateCapsule(m_Polygon.boneReferences.armRight.lowerArm.bone, m_Polygon.boneReferences.armRight.hand.wrist.bone, .03f, m_ArmThickness));

			// Create Spine
			m_AllColliders.Add(BoneType.Hips, CreateBoxCollider(m_Polygon.boneReferences.body.hip.bone, m_Polygon.boneReferences.body.spine.bone, new Vector3(.3f, 0.15f, 1f), new Vector3(m_BodyWidth, m_BodyThickness, 1f)));
			bool t_Chest = m_Polygon.boneReferences.body.chest.bone != null;
			m_AllColliders.Add(BoneType.Spine, CreateBoxCollider(m_Polygon.boneReferences.body.spine.bone, t_Chest ? m_Polygon.boneReferences.body.chest.bone : m_Polygon.boneReferences.head.neck.bone, new Vector3(.26f, .15f, t_Chest ? 1f : .7f), new Vector3(m_BodyWidth, m_BodyThickness, t_Chest ? 1f : m_LastFill)));
			bool t_UpperChest = m_Polygon.boneReferences.body.upperChest.bone != null;
			if (t_Chest) m_AllColliders.Add(BoneType.Chest, CreateBoxCollider(m_Polygon.boneReferences.body.chest.bone, t_UpperChest ? m_Polygon.boneReferences.body.upperChest.bone : m_Polygon.boneReferences.head.neck.bone, new Vector3(.28f, .15f, t_UpperChest ? 1f : .7f), new Vector3(m_BodyWidth, m_BodyThickness, t_UpperChest ? 1 : m_LastFill)));
			if (t_UpperChest) m_AllColliders.Add(BoneType.UpperChest, CreateBoxCollider(m_Polygon.boneReferences.body.upperChest.bone, m_Polygon.boneReferences.head.neck.bone, new Vector3(.3f, .15f, .7f), new Vector3(m_BodyWidth, m_BodyThickness, m_LastFill)));

			// Create Head
			m_AllColliders.Add(BoneType.Neck, CreateCapsule(m_Polygon.boneReferences.head.neck.bone, m_Polygon.boneReferences.head.head.bone, .05f, m_NeckThickness));
			m_AllColliders.Add(BoneType.Head, CreateSphereCollider(m_Polygon.boneReferences.head.head.bone, new Vector3(.12f, 1f, 1f), new Vector3(m_HeadSize, 1f, 1f), new Vector3(0, .065f, 0.02f), m_HeadOffset));


			Collider[] m_Colliders = new Collider[m_AllColliders.Count];
			var t_AllValues = m_AllColliders.Values.ToArray();
			for (int i = 0; i < m_Colliders.Length; i++)
			{
				m_Colliders[i] = t_AllValues[i].collider;
			}
		}

		private ColliderInfo CreateCapsule(Transform p_Parent, Transform p_Child, float p_OneWidth, float p_Width)
		{
			var t_Col = p_Parent.gameObject.AddComponent<CapsuleCollider>();
			t_Col.isTrigger = m_Trigger;
			t_Col.direction = 2;

			var t_ColInfo = new ColliderInfo(t_Col, p_Parent, p_Child, new Vector3(p_OneWidth, 1f, 1f), new Vector3(p_Width, 1f, 1f), Vector3.zero, Vector3.zero);

			return t_ColInfo;
		}

		private ColliderInfo CreateBoxCollider(Transform p_Parent, Transform p_Child, Vector3 p_OneSize, Vector3 p_Size)
		{
			var t_Col = p_Parent.gameObject.AddComponent<BoxCollider>();
			t_Col.isTrigger = m_Trigger;

			var t_ColInfo = new ColliderInfo(t_Col, p_Parent, p_Child, p_OneSize, p_Size, Vector3.zero, Vector3.zero);

			return t_ColInfo;
		}

		private ColliderInfo CreateSphereCollider(Transform p_Parent, Vector3 p_OneSize, Vector3 p_Size, Vector3 p_DefaultOffset, Vector3 p_Offset)
		{
			var t_Col = p_Parent.gameObject.AddComponent<SphereCollider>();
			t_Col.isTrigger = m_Trigger;

			var t_ColInfo = new ColliderInfo(t_Col, p_Parent, null, p_OneSize, p_Size, p_DefaultOffset, p_Offset);

			return t_ColInfo;
		}

		/// <summary>
		/// Remove all auto generated colliders
		/// </summary>
		[ContextMenu("Reset")]
		private void ResetAll()
		{
			if (m_AllColliders == null) return;

			foreach (var t_Col in m_AllColliders)
			{
				DestroyImmediate(t_Col.Value.collider);
			}

			foreach (var t_Col in m_Colliders)
			{
				DestroyImmediate(t_Col);
			}

			m_Colliders = new Collider[0]; 
			m_AllColliders = new Dictionary<BoneType, ColliderInfo>();
		}

		private void OnValidate()
		{
			if (m_AllColliders == null || m_AllColliders.Count <= 0)
				return;

			m_AllColliders[BoneType.LeftUpperLeg].Scale(new Vector2(m_LegThickness, 1f), Vector3.zero);
			m_AllColliders[BoneType.LeftLowerLeg].Scale(new Vector2(m_LegThickness, 1f), Vector3.zero);
			m_AllColliders[BoneType.RightUpperLeg].Scale(new Vector2(m_LegThickness, 1f), Vector3.zero);
			m_AllColliders[BoneType.RightLowerLeg].Scale(new Vector2(m_LegThickness, 1f), Vector3.zero);

			m_AllColliders[BoneType.LeftShoulder].Scale(new Vector2(m_ArmThickness, 1f), Vector3.zero);
			m_AllColliders[BoneType.LeftUpperArm].Scale(new Vector2(m_ArmThickness, 1f), Vector3.zero);
			m_AllColliders[BoneType.LeftLowerArm].Scale(new Vector2(m_ArmThickness, 1f), Vector3.zero);
			m_AllColliders[BoneType.RightShoulder].Scale(new Vector2(m_ArmThickness, 1f), Vector3.zero);
			m_AllColliders[BoneType.RightUpperArm].Scale(new Vector2(m_ArmThickness, 1f), Vector3.zero);
			m_AllColliders[BoneType.RightLowerArm].Scale(new Vector2(m_ArmThickness, 1f), Vector3.zero);

			m_AllColliders[BoneType.Hips].Scale(new Vector3(m_BodyWidth, m_BodyThickness, 1f), Vector3.zero);
			m_AllColliders[BoneType.Spine].Scale(new Vector3(m_BodyWidth, m_BodyThickness, m_AllColliders.ContainsKey(BoneType.Chest) ? 1f : m_LastFill), Vector3.zero);
			if (m_AllColliders.ContainsKey(BoneType.Chest)) m_AllColliders[BoneType.Chest].Scale(new Vector3(m_BodyWidth, m_BodyThickness, m_AllColliders.ContainsKey(BoneType.UpperChest) ? 1f : m_LastFill), Vector3.zero);
			if (m_AllColliders.ContainsKey(BoneType.UpperChest)) m_AllColliders[BoneType.UpperChest].Scale(new Vector3(m_BodyWidth, m_BodyThickness, m_LastFill), Vector3.zero);

			m_AllColliders[BoneType.Neck].Scale(new Vector2(m_NeckThickness, 1f), Vector3.zero);
			m_AllColliders[BoneType.Head].Scale(new Vector3(m_HeadSize, 1f, 1f), m_HeadOffset);
		}
	}

	/// <summary>
	/// All data needed for the colliders created for the polygon skeleton
	/// </summary>
	public struct ColliderInfo
	{
		public Collider collider;
		public Vector3 oneValues;
		public Vector3 scaleMultiplier;

		public Vector3 defaultOffset;
		public Vector3 offset;

		public Transform parent;
		public Transform child;

		public ColliderInfo(Collider p_Collider, Transform p_Parent, Transform p_Child, Vector3 p_OneValues, Vector3 p_Scale, Vector3 p_DefaultOffset, Vector3 p_Offset)
		{
			collider = p_Collider;
			oneValues = p_OneValues;
			scaleMultiplier = p_Scale;

			defaultOffset = p_DefaultOffset;
			offset = p_Offset;

			parent = p_Parent;
			child = p_Child;

			Scale(p_Scale, p_Offset);
		}

		public void Scale(Vector3 p_NewScale, Vector3 p_Offset)
		{
			scaleMultiplier = p_NewScale;
			offset = p_Offset;

			Vector3 t_Scale = new Vector3(oneValues.x * scaleMultiplier.x, oneValues.y * scaleMultiplier.y, oneValues.z * scaleMultiplier.z);

			if (typeof(CapsuleCollider) == collider.GetType())
			{
				var t_Capsule = (CapsuleCollider)collider;
				t_Capsule.radius = t_Scale.x;
				t_Capsule.height = Vector3.Distance(parent.position, child.position) + t_Scale.x;
				Matrix4x4 t_ParentMatrix = Matrix4x4.TRS(parent.position, parent.rotation, parent.localScale).inverse;
				t_Capsule.center = t_ParentMatrix.MultiplyPoint3x4(child.position) / 2f;
			}
			else if (typeof(BoxCollider) == collider.GetType())
			{
				var t_Box = (BoxCollider)collider;
				Matrix4x4 t_ParentMatrix = Matrix4x4.TRS(parent.position, parent.rotation, parent.localScale).inverse;
				Vector3 t_LocalPosition = t_ParentMatrix.MultiplyPoint3x4(child.position);
				t_Box.center = t_LocalPosition / 2f * t_Scale.z;
				t_Box.size = new Vector3(t_Scale.x, t_Scale.y, t_LocalPosition.magnitude * 1.1f * t_Scale.z);
			}
			else if (typeof(SphereCollider) == collider.GetType())
			{
				var t_Sphere = (SphereCollider)collider;
				t_Sphere.center = defaultOffset + p_Offset;
				t_Sphere.radius = t_Scale.x;
			}
		}
	}
}