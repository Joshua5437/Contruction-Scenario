using Hermes.Protocol.Polygon;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Manus.Polygon
{
	public class SkeletonScaler : IDisposable
	{
		/// <summary>
		/// List off all bone scalers
		/// </summary>
		public Dictionary<BoneType, BoneScaler> boneScalers { get; private set; }
	
		private Polygon m_Polygon = null;
		
		public SkeletonScaler(SkeletonBoneReferences p_References)
		{
			boneScalers = new Dictionary<BoneType, BoneScaler>();
			GenerateScalerBonesForBody(p_References);
		}

		/// <summary>
		/// Reset the scale of the body
		/// </summary>
		public void Reset()
		{
			if (m_Polygon == null)
				return;

			foreach (var t_Scaler in boneScalers.Values)
			{
				t_Scaler.ScaleBone(1f, ScaleAxis.All, ScaleMode.Percentage);
			}
		}

		/// <summary>
		/// Generates all scaler bones for the body
		/// </summary>
		/// <param name="p_Bones">Bone references of polygon body</param>
		private void GenerateScalerBonesForBody(SkeletonBoneReferences p_Bones)
		{
			Transform t_HeadConnection = p_Bones.head.head.bone;
			if (p_Bones.head.neck.bone)
				t_HeadConnection = p_Bones.head.neck.bone;

			Transform t_LeftArmConnection = p_Bones.armLeft.upperArm.bone;
			if (p_Bones.armLeft.shoulder.bone)
				t_LeftArmConnection = p_Bones.armLeft.shoulder.bone;

			Transform t_RightArmConnection = p_Bones.armRight.upperArm.bone;
			if (p_Bones.armRight.shoulder.bone)
				t_RightArmConnection = p_Bones.armRight.shoulder.bone;

			AddScalerBone(BoneType.Root, p_Bones.root.bone);
			AddScalerBone(BoneType.Hips, p_Bones.body.hip.bone, p_Bones.body.chest.bone != null ? new[] { p_Bones.legLeft.upperLeg.bone, p_Bones.legRight.upperLeg.bone, p_Bones.body.spine.bone } : new[] { p_Bones.legLeft.upperLeg.bone, p_Bones.legRight.upperLeg.bone, t_HeadConnection, t_LeftArmConnection, t_RightArmConnection });
			if (p_Bones.body.spine.bone) AddScalerBone(BoneType.Spine, p_Bones.body.spine.bone, p_Bones.body.chest.bone != null ? new[] { p_Bones.body.chest.bone } : new [] { t_HeadConnection, t_LeftArmConnection, t_RightArmConnection });
			if (p_Bones.body.chest.bone) AddScalerBone(BoneType.Chest, p_Bones.body.chest.bone, (p_Bones.body.upperChest.bone != null) ? new[] { p_Bones.body.upperChest.bone } : new[] { t_HeadConnection, t_LeftArmConnection, t_RightArmConnection });
			if (p_Bones.body.upperChest.bone) AddScalerBone(BoneType.UpperChest, p_Bones.body.upperChest.bone, new[] { t_HeadConnection, t_LeftArmConnection, t_RightArmConnection });

			if (p_Bones.head.neck.bone) AddScalerBone(BoneType.Neck, p_Bones.head.neck.bone, new[] { p_Bones.head.head.bone });
			AddScalerBone(BoneType.Head, p_Bones.head.head.bone);

			// Arms
			if (p_Bones.armLeft.shoulder.bone) AddScalerBone(BoneType.LeftShoulder, p_Bones.armLeft.shoulder.bone, new[] { p_Bones.armLeft.upperArm.bone });
			AddScalerBone(BoneType.LeftUpperArm, p_Bones.armLeft.upperArm.bone, new[] { p_Bones.armLeft.lowerArm.bone });
			AddScalerBone(BoneType.LeftLowerArm, p_Bones.armLeft.lowerArm.bone, new[] { p_Bones.armLeft.hand.wrist.bone });

			if (p_Bones.armRight.shoulder.bone) AddScalerBone(BoneType.RightShoulder, p_Bones.armRight.shoulder.bone, new[] { p_Bones.armRight.upperArm.bone });
			AddScalerBone(BoneType.RightUpperArm, p_Bones.armRight.upperArm.bone, new[] { p_Bones.armRight.lowerArm.bone });
			AddScalerBone(BoneType.RightLowerArm, p_Bones.armRight.lowerArm.bone, new[] { p_Bones.armRight.hand.wrist.bone });

			// Legs
			AddScalerBone(BoneType.LeftUpperLeg, p_Bones.legLeft.upperLeg.bone, new[] { p_Bones.legLeft.lowerLeg.bone });
			AddScalerBone(BoneType.LeftLowerLeg, p_Bones.legLeft.lowerLeg.bone, new[] { p_Bones.legLeft.foot.bone });
			AddScalerBone(BoneType.LeftFoot, p_Bones.legLeft.foot.bone);

			AddScalerBone(BoneType.RightUpperLeg, p_Bones.legRight.upperLeg.bone, new[] { p_Bones.legRight.lowerLeg.bone });
			AddScalerBone(BoneType.RightLowerLeg, p_Bones.legRight.lowerLeg.bone, new[] { p_Bones.legRight.foot.bone });
			AddScalerBone(BoneType.RightFoot, p_Bones.legRight.foot.bone);

			// Hands
			AddScalerBone(BoneType.LeftHand, p_Bones.armLeft.hand.wrist.bone);
			//AddScalerBone(BoneType.LeftIndexProximal, bones.armLeft.hand.index.proximal.bone, bones.armLeft.hand.index.proximal.bone.rotation);
			//AddScalerBone(BoneType.LeftIndexMiddle, bones.armLeft.hand.index.middle.bone, bones.armLeft.hand.index.middle.bone.rotation);
			//AddScalerBone(BoneType.LeftIndexDistal, bones.armLeft.hand.index.distal.bone, bones.armLeft.hand.index.distal.bone.rotation);
			//AddScalerBone(BoneType.LeftMiddleProximal, bones.armLeft.hand.middle.proximal.bone, bones.armLeft.hand.middle.proximal.bone.rotation);
			//AddScalerBone(BoneType.LeftMiddleMiddle, bones.armLeft.hand.middle.middle.bone, bones.armLeft.hand.middle.middle.bone.rotation);
			//AddScalerBone(BoneType.LeftMiddleDistal, bones.armLeft.hand.middle.distal.bone, bones.armLeft.hand.middle.distal.bone.rotation);
			//AddScalerBone(BoneType.LeftRingProximal, bones.armLeft.hand.ring.proximal.bone, bones.armLeft.hand.ring.proximal.bone.rotation);
			//AddScalerBone(BoneType.LeftRingMiddle, bones.armLeft.hand.ring.middle.bone, bones.armLeft.hand.ring.middle.bone.rotation);
			//AddScalerBone(BoneType.LeftRingDistal, bones.armLeft.hand.ring.distal.bone, bones.armLeft.hand.ring.distal.bone.rotation);
			//AddScalerBone(BoneType.LeftPinkyProximal, bones.armLeft.hand.pinky.proximal.bone, bones.armLeft.hand.pinky.proximal.bone.rotation);
			//AddScalerBone(BoneType.LeftPinkyMiddle, bones.armLeft.hand.pinky.middle.bone, bones.armLeft.hand.pinky.middle.bone.rotation);
			//AddScalerBone(BoneType.LeftPinkyDistal, bones.armLeft.hand.pinky.distal.bone, bones.armLeft.hand.pinky.distal.bone.rotation);
			//AddScalerBone(BoneType.LeftThumbProximal, bones.armLeft.hand.thumb.proximal.bone, bones.armLeft.hand.thumb.proximal.bone.rotation);
			//AddScalerBone(BoneType.LeftThumbMiddle, bones.armLeft.hand.thumb.middle.bone, bones.armLeft.hand.thumb.middle.bone.rotation);
			//AddScalerBone(BoneType.LeftThumbDistal, bones.armLeft.hand.thumb.distal.bone, bones.armLeft.hand.thumb.distal.bone.rotation);

			AddScalerBone(BoneType.RightHand, p_Bones.armRight.hand.wrist.bone);
			//AddScalerBone(BoneType.RightIndexProximal, bones.armRight.hand.index.proximal.bone, bones.armRight.hand.index.proximal.bone.rotation);
			//AddScalerBone(BoneType.RightIndexMiddle, bones.armRight.hand.index.middle.bone, bones.armRight.hand.index.middle.bone.rotation);
			//AddScalerBone(BoneType.RightIndexDistal, bones.armRight.hand.index.distal.bone, bones.armRight.hand.index.distal.bone.rotation);
			//AddScalerBone(BoneType.RightMiddleProximal, bones.armRight.hand.middle.proximal.bone, bones.armRight.hand.middle.proximal.bone.rotation);
			//AddScalerBone(BoneType.RightMiddleMiddle, bones.armRight.hand.middle.middle.bone, bones.armRight.hand.middle.middle.bone.rotation);
			//AddScalerBone(BoneType.RightMiddleDistal, bones.armRight.hand.middle.distal.bone, bones.armRight.hand.middle.distal.bone.rotation);
			//AddScalerBone(BoneType.RightRingProximal, bones.armRight.hand.ring.proximal.bone, bones.armRight.hand.ring.proximal.bone.rotation);
			//AddScalerBone(BoneType.RightRingMiddle, bones.armRight.hand.ring.middle.bone, bones.armRight.hand.ring.middle.bone.rotation);
			//AddScalerBone(BoneType.RightRingDistal, bones.armRight.hand.ring.distal.bone, bones.armRight.hand.ring.distal.bone.rotation);
			//AddScalerBone(BoneType.RightPinkyProximal, bones.armRight.hand.pinky.proximal.bone, bones.armRight.hand.pinky.proximal.bone.rotation);
			//AddScalerBone(BoneType.RightPinkyMiddle, bones.armRight.hand.pinky.middle.bone, bones.armRight.hand.pinky.middle.bone.rotation);
			//AddScalerBone(BoneType.RightPinkyDistal, bones.armRight.hand.pinky.distal.bone, bones.armRight.hand.pinky.distal.bone.rotation);
			//AddScalerBone(BoneType.RightThumbProximal, bones.armRight.hand.thumb.proximal.bone, bones.armRight.hand.thumb.proximal.bone.rotation);
			//AddScalerBone(BoneType.RightThumbMiddle, bones.armRight.hand.thumb.middle.bone, bones.armRight.hand.thumb.middle.bone.rotation);
			//AddScalerBone(BoneType.RightThumbDistal, bones.armRight.hand.thumb.distal.bone, bones.armRight.hand.thumb.distal.bone.rotation);
		}

		/// <summary>
		/// Remove a scale bone
		/// </summary>
		private void RemoveScaleBones()
		{
			foreach (var t_BoneScaler in boneScalers.Values)
			{
				t_BoneScaler.Dispose();
			}
		}

		/// <summary>
		/// Add a scale bone
		/// </summary>
		/// <param name="p_BoneType">Bonetype</param>
		/// <param name="p_Main">Main transform</param>
		/// <param name="p_Children">Children to counterscale</param>
		private void AddScalerBone(BoneType p_BoneType, Transform p_Main, Transform[] p_Children = null)
		{
			if (!boneScalers.ContainsKey(p_BoneType))
			{
				boneScalers.Add(p_BoneType, new BoneScaler(p_Main, p_Children));
			}
		}

		/// <summary>
		/// Dispose of the scaler
		/// </summary>
		public void Dispose()
		{
			Reset();
			RemoveScaleBones();
		}
	}
}