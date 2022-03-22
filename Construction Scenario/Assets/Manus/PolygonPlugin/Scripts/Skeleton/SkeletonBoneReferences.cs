using System.Collections.Generic;
using UnityEngine;
using HProt = Hermes.Protocol;
using Hermes.Protocol.Polygon;
using Manus.Polygon.Utilities;

namespace Manus.Polygon
{
	#region BoneOrganization

	[System.Serializable]
	public class Body : IBoneGroup
	{
		public Bone hip;
		public Bone spine;
		public Bone chest;
		public Bone upperChest;

		/// <summary>
		/// Is spine valid
		/// </summary>
		public bool isValid
		{
			get
			{
				foreach (var t_Bone in GatherBones(GatherType.All))
				{
					if (!t_Bone.Value.optional && t_Bone.Value.bone == null)
						return false;
				}

				return true;
			}
		}

		public Body()
		{
			hip = new Bone(false, BoneType.Hips);
			spine = new Bone(true, BoneType.Spine);

			chest = new Bone(true, BoneType.Chest);
			upperChest = new Bone(true, BoneType.UpperChest);
		}

		/// <summary>
		/// Gathers bones for the spine
		/// </summary>
		/// <param name="p_GatherType">What bones are requested</param>
		/// <returns>Returns dictionary of requested bones</returns>
		public Dictionary<BoneType, Bone> GatherBones(GatherType p_GatherType)
		{
			var t_Bones = new Dictionary<BoneType, Bone>();

			switch (p_GatherType)
			{
				case GatherType.All:
				case GatherType.Animated:
				case GatherType.NoHands:
					t_Bones.Add(hip.type, hip);
					t_Bones.Add(spine.type, spine);
					if (chest.bone) t_Bones.Add(chest.type, chest);
					if (upperChest.bone) t_Bones.Add(upperChest.type, upperChest);
					break;
			}

			return t_Bones;
		}

		/// <summary>
		/// Assign spine bones
		/// </summary>
		/// <param name="p_Hip">Hip bone</param>
		/// <param name="p_Spine">1st Spine bone</param>
		/// <param name="p_Chest">2nd Spine bone</param>
		/// <param name="p_UpperChest">3rd Spine bone</param>
		public void AssignBones(Transform p_Hip, Transform p_Spine, Transform p_Chest, Transform p_UpperChest)
		{
			hip.AssignTransform(p_Hip);
			if (p_Spine) spine.AssignTransform(p_Spine);
			if (p_Chest) chest.AssignTransform(p_Chest);
			if (p_UpperChest) upperChest.AssignTransform(p_UpperChest);
		}
	}

	[System.Serializable]
	public class Head : IBoneGroup
	{
		public Bone neck;
		public Bone head;

		/// <summary>
		/// Is head valid
		/// </summary>
		public bool isValid
		{
			get
			{
				foreach (var t_Bone in GatherBones(GatherType.All))
				{
					if (!t_Bone.Value.optional && t_Bone.Value.bone == null)
						return false;
				}

				return true;
			}
		}

		public Head()
		{
			neck = new Bone(true, BoneType.Neck);
			head = new Bone(false, BoneType.Head);
		}

		/// <summary>
		/// Gathers bones for the head
		/// </summary>
		/// <param name="p_GatherType">What bones are requested</param>
		/// <returns>Returns dictionary of requested bones</returns>
		public Dictionary<BoneType, Bone> GatherBones(GatherType p_GatherType)
		{
			var t_Bones = new Dictionary<BoneType, Bone>();

			switch (p_GatherType)
			{
				case GatherType.All:
				case GatherType.Animated:
				case GatherType.NoHands:
					if (neck.bone != null) t_Bones.Add(neck.type, neck);
					t_Bones.Add(head.type, head);
					break;
			}

			return t_Bones;
		}

		/// <summary>
		/// Assign head bones
		/// </summary>
		/// <param name="p_Neck">Neck bone</param>
		/// <param name="p_Head">Head bone</param>
		public void AssignBones(Transform p_Neck, Transform p_Head)
		{
			if (p_Neck) neck.AssignTransform(p_Neck);
			head.AssignTransform(p_Head);
		}
	}

	[System.Serializable]
	public class Arm : IBoneGroup
	{
		private bool m_Left;

		public Bone shoulder;
		public Bone upperArm;
		public Bone lowerArm;
		public HandBoneReferences hand;

		/// <summary>
		/// Is arm valid
		/// </summary>
		public bool isValid
		{
			get
			{
				foreach (var t_Bone in GatherBones(GatherType.All))
				{
					if (!t_Bone.Value.optional && t_Bone.Value.bone == null)
						return false;
				}

				return true;
			}
		}

		public Arm(bool p_Left)
		{
			m_Left = p_Left;

			shoulder = new Bone(true, p_Left ? BoneType.LeftShoulder : BoneType.RightShoulder);
			upperArm = new Bone(false, p_Left ? BoneType.LeftUpperArm : BoneType.RightUpperArm);
			lowerArm = new Bone(false, p_Left ? BoneType.LeftLowerArm : BoneType.RightLowerArm);

			hand = new HandBoneReferences(p_Left);
		}

		/// <summary>
		/// Gathers bones for the arm
		/// </summary>
		/// <param name="p_GatherType">What bones are requested</param>
		/// <returns>Returns dictionary of requested bones</returns>
		public Dictionary<BoneType, Bone> GatherBones(GatherType p_GatherType)
		{
			var t_Bones = new Dictionary<BoneType, Bone>();

			switch (p_GatherType)
			{
				case GatherType.All:
				case GatherType.Animated:
					if (shoulder.bone) t_Bones.Add(shoulder.type, shoulder);
					t_Bones.Add(upperArm.type, upperArm);
					t_Bones.Add(lowerArm.type, lowerArm);
					if (hand.wrist.bone != null) t_Bones.Add(hand.wrist.type, hand.wrist);
					break;
				case GatherType.NoHands:
					if (shoulder.bone) t_Bones.Add(shoulder.type, shoulder);
					t_Bones.Add(upperArm.type, upperArm);
					t_Bones.Add(lowerArm.type, lowerArm);
					break;
			}

			return t_Bones;
		}

		/// <summary>
		/// Assign arm bones
		/// </summary>
		/// <param name="p_Shoulder">Shoulder bone</param>
		/// <param name="p_UpperArm">Upper arm bone</param>
		/// <param name="p_LowerArm">Lower arm bone</param>
		public void AssignBones(Transform p_Shoulder, Transform p_UpperArm, Transform p_LowerArm)
		{
			if (p_Shoulder) shoulder.AssignTransform(p_Shoulder);
			upperArm.AssignTransform(p_UpperArm);
			lowerArm.AssignTransform(p_LowerArm);
		}

		/// <summary>
		/// Assign hand bones
		/// </summary>
		/// <param name="p_LowerArm">Wrist parent bone</param>
		/// <param name="p_Animator">Animator with humanoid avatar</param>
		public void AssignHandBones(Transform p_LowerArm, Animator p_Animator)
		{
			hand.PopulateBones(p_Animator, p_LowerArm, m_Left);
		}
	}

	[System.Serializable]
	public class Leg : IBoneGroup
	{
		private bool m_Left = false;

		public Bone upperLeg;
		public Bone lowerLeg;
		public Bone foot;
		public Bone toes;
		public Bone toesEnd;

		/// <summary>
		/// Is leg valid
		/// </summary>
		public bool isValid
		{
			get
			{
				foreach (var t_Bone in GatherBones(GatherType.All))
				{
					if (!t_Bone.Value.optional && t_Bone.Value.bone == null)
						return false;
				}

				return true;
			}
		}

		public Leg(bool p_Left)
		{
			m_Left = p_Left;

			upperLeg = new Bone(false, m_Left ? BoneType.LeftUpperLeg : BoneType.RightUpperLeg);
			lowerLeg = new Bone(false, m_Left ? BoneType.LeftLowerLeg : BoneType.RightLowerLeg);
			foot = new Bone(false, m_Left ? BoneType.LeftFoot : BoneType.RightFoot);

			toes = new Bone(true, m_Left ? BoneType.LeftToes : BoneType.RightToes);
			toesEnd = new Bone(true, m_Left ? BoneType.LeftToesEnd : BoneType.RightToesEnd);


		}

		/// <summary>
		/// Gathers bones for the leg
		/// </summary>
		/// <param name="p_GatherType">What bones are requested</param>
		/// <returns>Returns dictionary of requested bones</returns>
		public Dictionary<BoneType, Bone> GatherBones(GatherType p_GatherType)
		{
			var t_Bones = new Dictionary<BoneType, Bone>();

			switch (p_GatherType)
			{
				case GatherType.All:
				case GatherType.NoHands:
					t_Bones.Add(upperLeg.type, upperLeg);
					t_Bones.Add(lowerLeg.type, lowerLeg);
					t_Bones.Add(foot.type, foot);
					if (toes.bone != null) t_Bones.Add(toes.type, toes);
					if (toesEnd.bone != null) t_Bones.Add(toesEnd.type, toesEnd);
					break;
				case GatherType.Animated:
					t_Bones.Add(upperLeg.type, upperLeg);
					t_Bones.Add(lowerLeg.type, lowerLeg);
					t_Bones.Add(foot.type, foot);
					if (toes.bone != null) t_Bones.Add(toes.type, toes);
					break;
			}

			return t_Bones;
		}

		/// <summary>
		/// Assig leg bones
		/// </summary>
		/// <param name="p_UpperLeg">Upper leg bone</param>
		/// <param name="p_LowerLeg">Lower leg bone</param>
		/// <param name="p_Foot">Foot bone</param>
		/// <param name="p_Toes">Toe bone</param>
		/// <param name="p_ToesEnd">Toe tip bone, tries to find one if not supplied</param>
		public void AssignBones(Transform p_UpperLeg, Transform p_LowerLeg, Transform p_Foot, Transform p_Toes, Transform p_ToesEnd)
		{
			upperLeg.AssignTransform(p_UpperLeg);
			lowerLeg.AssignTransform(p_LowerLeg);
			foot.AssignTransform(p_Foot);

			if (p_Toes == null)
			{
				Debug.LogWarning($"No toe bone assigned in the avatar for {p_Foot.name}, consider assigning the foot for a better connection to the ground", p_Foot);
				return;
			}

			toes.AssignTransform(p_Toes);

			if (p_ToesEnd == null && p_Toes.childCount == 0)
			{
				Debug.LogWarning($"No toe end bone assigned in the avatar for {p_Toes.name}, consider assigning the foot for a better connection to the ground", p_Toes);
				return;
			}

			// Find tip
			for (int i = 0; i < this.toes.bone.childCount; i++)
			{
				if (toes.bone.GetChild(i).GetComponent<MeshRenderer>() || this.toes.bone.GetChild(i).GetComponent<SkinnedMeshRenderer>())
					continue;

				toesEnd.AssignTransform(p_ToesEnd ?? toes.bone.GetChild(i));
				return;
			}
			toesEnd.AssignTransform(p_ToesEnd ?? toes.bone.GetChild(0));
		}
	}

	public enum GatherType
	{
		All,
		Animated,
		NoHands,
	}

	#endregion

	[System.Serializable]
	public class SkeletonBoneReferences
	{
		private const GatherType m_UsedBones = GatherType.NoHands;
		[HideInInspector] public float setupScale = 1;
		[HideInInspector] public int currentSetupState = 0;
		[HideInInspector] public bool generatedParents = false;

		[HideInInspector] public float height = 1.80f;
		public SkeletonScaler scaler;

		public Bone root;

		public Head head;
		public Body body;

		public Arm armLeft;
		public Arm armRight;

		public Leg legLeft = new Leg(true);
		public Leg legRight = new Leg(false);

		/// <summary>
		/// Is body valid
		/// </summary>
		public bool isValid
		{
			get { return head.isValid && body.isValid && armLeft.isValid && armRight.isValid && legLeft.isValid && legRight.isValid; }
		}

		public SkeletonBoneReferences()
		{
			Clear();
		}

		/// <summary>
		/// Gathers bones for the body
		/// </summary>
		/// <param name="p_GatherType">What bones are requested</param>
		/// <returns>Returns dictionary of requested bones</returns>
		public Dictionary<BoneType, Bone> GatherBones(GatherType p_GatherType)
		{
			var t_Bones = new Dictionary<BoneType, Bone>();

			t_Bones.Add(root.type, root);
			AddToDictionary(body.GatherBones(p_GatherType));
			AddToDictionary(head.GatherBones(p_GatherType));
			AddToDictionary(armLeft.GatherBones(p_GatherType));
			AddToDictionary(armRight.GatherBones(p_GatherType));
			AddToDictionary(legLeft.GatherBones(p_GatherType));
			AddToDictionary(legRight.GatherBones(p_GatherType));

			AddToDictionary(armLeft.hand.GatherBones(p_GatherType));
			AddToDictionary(armRight.hand.GatherBones(p_GatherType));


			return t_Bones;

			void AddToDictionary(Dictionary<BoneType, Bone> p_BonesToAdd)
			{
				foreach (var t_Bone in p_BonesToAdd)
				{
					if (!t_Bones.ContainsKey(t_Bone.Key))
					{
						t_Bones.Add(t_Bone.Key, t_Bone.Value);
					}
				}
			}
		}

		/// <summary>
		/// Auto populate bones
		/// </summary>
		/// <param name="p_Animator">Animator with valid humanoid avatar</param>
		public void Populate(Animator p_Animator)
		{
			root.AssignTransform(p_Animator.GetBoneTransform(HumanBodyBones.Hips)?.parent);
			head.AssignBones(
				p_Animator.GetBoneTransform(HumanBodyBones.Neck),
				p_Animator.GetBoneTransform(HumanBodyBones.Head));
			body.AssignBones(
				p_Animator.GetBoneTransform(HumanBodyBones.Hips),
				p_Animator.GetBoneTransform(HumanBodyBones.Spine),
				p_Animator.GetBoneTransform(HumanBodyBones.Chest),
				p_Animator.GetBoneTransform(HumanBodyBones.UpperChest));
			armLeft.AssignBones(
				p_Animator.GetBoneTransform(HumanBodyBones.LeftShoulder),
				p_Animator.GetBoneTransform(HumanBodyBones.LeftUpperArm),
				p_Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm));
			armLeft.AssignHandBones(p_Animator.GetBoneTransform(HumanBodyBones.LeftLowerArm), p_Animator);

			armRight.AssignBones(
				p_Animator.GetBoneTransform(HumanBodyBones.RightShoulder),
				p_Animator.GetBoneTransform(HumanBodyBones.RightUpperArm),
				p_Animator.GetBoneTransform(HumanBodyBones.RightLowerArm));
			armRight.AssignHandBones(p_Animator.GetBoneTransform(HumanBodyBones.RightLowerArm), p_Animator);

			legLeft.AssignBones(
				p_Animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg),
				p_Animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg),
				p_Animator.GetBoneTransform(HumanBodyBones.LeftFoot),
				p_Animator.GetBoneTransform(HumanBodyBones.LeftToes),
				null);
			legRight.AssignBones(
				p_Animator.GetBoneTransform(HumanBodyBones.RightUpperLeg),
				p_Animator.GetBoneTransform(HumanBodyBones.RightLowerLeg),
				p_Animator.GetBoneTransform(HumanBodyBones.RightFoot),
				p_Animator.GetBoneTransform(HumanBodyBones.RightToes),
				null);
		}

		/// <summary>
		/// Checks if your root is valid
		/// </summary>
		/// <returns> returns true is there is a root in the skeleton </returns>
		public bool HasRoot() 
		{
			return root.bone.GetComponent<Polygon>() == null;
		}

		/// <summary>
		/// Check if a rootbone exists, if not, create one
		/// </summary>
		public void CheckRoot()
		{
			Transform t_RootTransform = root.bone;
			if (t_RootTransform.GetComponent<Polygon>() != null)
			{
				GameObject t_NewRoot = new GameObject("root");
				t_NewRoot.transform.SetPositionAndRotation(t_RootTransform.position, t_RootTransform.rotation);
				t_NewRoot.transform.SetParent(t_RootTransform);
				t_RootTransform = t_NewRoot.transform;

				body.hip.bone.SetParent(t_RootTransform);
				root.AssignTransform(t_RootTransform);
			}
		}

		/// <summary>
		/// Check if a toe bones exists, if not, create them
		/// </summary>
		public void CheckToeEnds()
		{
			Vector3 t_Forward = Vector3.Cross(legRight.foot.bone.position - legLeft.foot.bone.position, Vector3.up).normalized;
			float t_MoveDistance = 0.08f;
			if (legLeft.toes.bone != null && legLeft.toesEnd.bone == null)
			{
				Bone t_LeftToesBone = legLeft.toes;
				GameObject t_NewToeEnd = new GameObject("leftToeEnd");
				t_NewToeEnd.transform.SetPositionAndRotation(t_LeftToesBone.bone.position + t_Forward * t_MoveDistance, t_LeftToesBone.bone.rotation);
				t_NewToeEnd.transform.SetParent(legLeft.toes.bone);

				legLeft.toesEnd.AssignTransform(t_NewToeEnd.transform);
			}

			if (legRight.toes.bone != null && legRight.toesEnd.bone == null)
			{
				Bone t_RightToesBone = legRight.toes;
				GameObject t_NewToeEnd = new GameObject("rightToeEnd");
				t_NewToeEnd.transform.SetPositionAndRotation(t_RightToesBone.bone.position + t_Forward * t_MoveDistance, t_RightToesBone.bone.rotation);
				t_NewToeEnd.transform.SetParent(legRight.toes.bone);

				legRight.toesEnd.AssignTransform(t_NewToeEnd.transform);
			}
		}

		/// <summary>
		/// Clear body bones
		/// </summary>
		public void Clear()
		{
			currentSetupState = 0;

			root = new Bone(false, BoneType.Root);
			head = new Head();
			body = new Body();
			armLeft = new Arm(true);
			armRight = new Arm(false);
			legLeft = new Leg(true);
			legRight = new Leg(false);
			
			RemoveTransformParents();
		}

		/// <summary>
		/// Reset skeleton
		/// </summary>
		public void Reset()
		{
			currentSetupState = 0;
			setupScale = 1;

			foreach (var t_Bone in GatherBones(GatherType.All).Values)
			{
				t_Bone.desiredRotationOffset = new Quaternion(0, 0, 0, 0);
			}

			RemoveTransformParents();
		}

		/// <summary>
		/// Generate transforms with proper directions
		/// </summary>
		public void GenerateTransformParents()
		{
			if (!isValid)
				return;

			foreach (var t_Bone in GatherBones(m_UsedBones))
			{
				if (t_Bone.Value.isReorientated)
					continue;

				Transform t_NewParent = new GameObject(t_Bone.Key.ToString()).transform;
				t_NewParent.SetParent(t_Bone.Value.bone.parent);
				t_NewParent.position = t_Bone.Value.bone.position;
				t_NewParent.rotation = t_Bone.Value.bone.rotation * t_Bone.Value.desiredRotationOffset;
				t_NewParent.localScale = Vector3.one;
				t_Bone.Value.bone.SetParent(t_NewParent, true);

				t_Bone.Value.isReorientated = true;
				t_Bone.Value.bone = t_NewParent;
			}

			scaler = new SkeletonScaler(this);
			generatedParents = true;
		}

		/// <summary>
		/// Remove transforms with other directions
		/// </summary>
		public void RemoveTransformParents()
		{
			if (!isValid)
				return;

			scaler?.Dispose();
			scaler = null;

			foreach (var t_Bone in GatherBones(m_UsedBones))
			{
				if (!t_Bone.Value.isReorientated)
					continue;

				Transform t_NewBone = t_Bone.Value.bone.GetChild(0);
				for (int i = 0; i < t_Bone.Value.bone.childCount; i++)
				{
					t_Bone.Value.bone.GetChild(i).SetParent(t_Bone.Value.bone.parent);
				}

				GameObject.DestroyImmediate(t_Bone.Value.bone.gameObject);
				t_Bone.Value.isReorientated = false;
				t_Bone.Value.bone = t_NewBone;
			}

			var t_ScaleBones = GetAllChildrenWithName(root.bone, "_scaleFixBone");
			foreach (var t_ScaleFix in t_ScaleBones)
			{
				var t_ScaleFixBoneTransform = t_ScaleFix.transform;
				for (int i = 0; i < t_ScaleFixBoneTransform.childCount; i++)
				{
					t_ScaleFixBoneTransform.GetChild(i).SetParent(t_ScaleFixBoneTransform.parent, true);
				}

				GameObject.Destroy(t_ScaleFix.gameObject);
			}

			generatedParents = false;
		}

		private GameObject[] GetAllChildrenWithName(Transform p_Parent, string p_Contains)
		{
			var t_Objects = new List<GameObject>();
			GetChildrenWithName(p_Parent, p_Contains, t_Objects);

			return t_Objects.ToArray();
		}

		private void GetChildrenWithName(Transform p_Parent, string p_Contains, List<GameObject> p_List)
		{
			foreach (Transform t_Child in p_Parent)
			{
				if (t_Child.gameObject.name.Contains(p_Contains))
				{
					p_List.Add(t_Child.gameObject);
				}
				GetChildrenWithName(t_Child, p_Contains, p_List);
			}
		}
	}
}