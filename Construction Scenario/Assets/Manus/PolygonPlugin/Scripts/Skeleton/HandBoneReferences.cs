using System.Collections.Generic;
using UnityEngine;
using Hermes.Protocol.Polygon;
using Manus.Utility;

namespace Manus.Polygon
{
	[System.Serializable]
	public class Finger
	{
		public Bone proximal;
		public Bone middle;
		public Bone distal;
		public Bone tip;

		/// <summary>
		/// Is finger valid
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

		public Finger(bool p_Left, int p_Index)
		{
			string t_EnumName = p_Left ? "Left" : "Right";

			if (p_Index == 0) t_EnumName += "Thumb";
			else if (p_Index == 1) t_EnumName += "Index";
			else if (p_Index == 2) t_EnumName += "Middle";
			else if (p_Index == 3) t_EnumName += "Ring";
			else if (p_Index == 4) t_EnumName += "Pinky";

			proximal = new Bone(true, (BoneType)System.Enum.Parse(typeof(BoneType), t_EnumName + "Proximal"));
			middle = new Bone(true, (BoneType)System.Enum.Parse(typeof(BoneType), t_EnumName + "Middle"));
			distal = new Bone(true, (BoneType)System.Enum.Parse(typeof(BoneType), t_EnumName + "Distal"));
			tip = new Bone(true, (BoneType)System.Enum.Parse(typeof(BoneType), t_EnumName + "Tip"));
		}

		/// <summary>
		/// Gathers all finger bones
		/// </summary>
		/// <param name="p_GatherType">Which bones to return</param>
		/// <returns>Dictionary of all requested bones</returns>
		public Dictionary<BoneType, Bone> GatherBones(GatherType p_GatherType)
		{
			var t_Bones = new Dictionary<BoneType, Bone>();

			switch (p_GatherType)
			{
				case GatherType.All:
					if (proximal.bone) t_Bones.Add(proximal.type, proximal);
					if (middle.bone) t_Bones.Add(middle.type, middle);
					if (distal.bone) t_Bones.Add(distal.type, distal);
					if (tip.bone) t_Bones.Add(tip.type, tip);

					break;
				case GatherType.Animated:
					break;
			}

			return t_Bones;
		}

		/// <summary>
		/// Assign bones of the finger
		/// </summary>
		/// <param name="p_Proximal">1st part of the finger</param>
		/// <param name="p_Middle">2nd part of the finger</param>
		/// <param name="p_Distal">3rd part of the finger</param>
		/// <param name="p_Tip">tip of the finger</param>
		public void AssignBones(Transform p_Proximal, Transform p_Middle, Transform p_Distal, Transform p_Tip)
		{
			proximal.AssignTransform(p_Proximal);
			middle.AssignTransform(p_Middle);
			distal.AssignTransform(p_Distal);
			tip.AssignTransform(p_Tip);
		}
	}

	[System.Serializable]
	public class HandBoneReferences
	{
		private bool _Left;

		public Bone wrist;

		public Finger index;
		public Finger middle;
		public Finger ring;
		public Finger pinky;
		public Finger thumb;

		/// <summary>
		/// Is hand valid
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

		public HandBoneReferences(bool p_Left)
		{
			_Left = p_Left;
			ClearBoneReferences();
		}

		/// <summary>
		/// Gathers all hand bones
		/// </summary>
		/// <param name="p_GatherType">What bones are requested</param>
		/// <returns>Returns dictionary of requested bones</returns>
		public Dictionary<BoneType, Bone> GatherBones(GatherType p_GatherType)
		{
			var t_Bones = new Dictionary<BoneType, Bone>();

			t_Bones.Add(wrist.type, wrist);
			AddToDictionary(index.GatherBones(p_GatherType));
			AddToDictionary(middle.GatherBones(p_GatherType));
			AddToDictionary(ring.GatherBones(p_GatherType));
			AddToDictionary(pinky.GatherBones(p_GatherType));
			AddToDictionary(thumb.GatherBones(p_GatherType));

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
		/// Populate hand bones
		/// </summary>
		/// <param name="p_LowerArm">Parent of wrist bone</param>
		public void PopulateBones(Transform p_LowerArm)
		{
			PopulateBones(null, p_LowerArm, true);
		}

		/// <summary>
		/// Populate hand bones
		/// </summary>
		/// <param name="p_Animator">Humanoid animator</param>
		/// <param name="p_LowerArm">Parent of wrist bone</param>
		/// <param name="p_Left">Is left hand</param>
		public void PopulateBones(Animator p_Animator, Transform p_LowerArm, bool p_Left)
		{
			bool t_IsLeft = p_Left;

			Transform t_Wrist = p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
			if (t_Wrist == null) t_Wrist = TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "wrist" });
			if (t_Wrist == null) t_Wrist = TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "hand" });
			if (t_Wrist == null) t_Wrist = p_LowerArm.childCount > 0 ? p_LowerArm.GetChild(0) : null;
			wrist.AssignTransform(t_Wrist);

			thumb.AssignBones(
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftThumbProximal : HumanBodyBones.RightThumbProximal)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "thumb", "1" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftThumbIntermediate : HumanBodyBones.RightThumbIntermediate)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "thumb", "2" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "thumb", "3" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal)?.childCount > 0
					? GetMostSuitableChild(p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftThumbDistal : HumanBodyBones.RightThumbDistal))
					: TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "thumb", "4" }));
			index.AssignBones(
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftIndexProximal : HumanBodyBones.RightIndexProximal)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "index", "1" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftIndexIntermediate : HumanBodyBones.RightIndexIntermediate)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "index", "2" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "index", "3" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal)?.childCount > 0
					? GetMostSuitableChild(p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftIndexDistal : HumanBodyBones.RightIndexDistal))
					: TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "index", "4" }));
			middle.AssignBones(
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftMiddleProximal : HumanBodyBones.RightMiddleProximal)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "middle", "1" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftMiddleIntermediate : HumanBodyBones.RightMiddleIntermediate)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "middle", "2" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "middle", "3" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal)?.childCount > 0
					? GetMostSuitableChild(p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftMiddleDistal : HumanBodyBones.RightMiddleDistal))
					: TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "middle", "4" }));
			ring.AssignBones(
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftRingProximal : HumanBodyBones.RightRingProximal)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "ring", "1" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftRingIntermediate : HumanBodyBones.RightRingIntermediate)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "ring", "2" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "ring", "3" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal)?.childCount > 0
					? GetMostSuitableChild(p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftRingDistal : HumanBodyBones.RightRingDistal))
					: TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "ring", "4" }));
			pinky.AssignBones(
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftLittleProximal : HumanBodyBones.RightLittleProximal)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "pinky", "1" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftLittleIntermediate : HumanBodyBones.RightLittleIntermediate)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "pinky", "2" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal)
				?? TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "pinky", "3" }),
				p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal)?.childCount > 0
					? GetMostSuitableChild(p_Animator?.GetBoneTransform(t_IsLeft ? HumanBodyBones.LeftLittleDistal : HumanBodyBones.RightLittleDistal))
					: TransformExtension.FindDeepChildCriteria(p_LowerArm, new string[] { "pinky", "4" }));
		}

		/// <summary>
		/// Find the child bone that is probably the best option for a bone
		/// </summary>
		/// <param name="p_Transform">Parent of child bone</param>
		/// <returns>Returns most suitable child bone</returns>
		private Transform GetMostSuitableChild(Transform p_Transform)
		{
			for (int i = 0; i < p_Transform.childCount; i++)
			{
				if (p_Transform.GetChild(i).GetComponent<MeshRenderer>() || p_Transform.GetChild(i).GetComponent<SkinnedMeshRenderer>())
					continue;

				return p_Transform.GetChild(i);
			}
			return p_Transform.GetChild(0);
		}

		/// <summary>
		/// Clear hand bones
		/// </summary>
		public void ClearBoneReferences()
		{
			wrist = new Bone(false, _Left ? BoneType.LeftHand : BoneType.RightHand);
			thumb = new Finger(_Left, 0);
			index = new Finger(_Left, 1);
			middle = new Finger(_Left, 2);
			ring = new Finger(_Left, 3);
			pinky = new Finger(_Left, 4);
		}
	}
}

