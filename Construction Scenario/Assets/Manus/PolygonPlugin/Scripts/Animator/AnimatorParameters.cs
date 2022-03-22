using Hermes.Protocol.Polygon;
using Manus.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using HProt = Hermes.Protocol;
using HandLocalTo = Hermes.Protocol.Polygon.Retargeting.HandLocalTo;

namespace Manus.Polygon
{
	[System.Serializable]
	public class AnimatorParameters
	{
		[Header("Hips")]

		[Tooltip("0 - 1 value, 0 means keep the scaled speed, 1 move with the same speed as the target")]
		[Range(0f, 1f)] public float matchSameSpeed = 0;
		[Tooltip("multiplier for the height of the hip")]
		[Range(0f, 2f)] public float hipHeightMultiplier = 1;

		[Header("Legs")]
		[Tooltip("Value to move the feet closer together or further apart")]
		[Range(-3f, 3f)] public float legWidth = 0;
		[Tooltip("Knee aim offset")]
		[Range(-180f, 180f)] public float kneeRotation = 0;

		[Header("Arms")]
		[Tooltip("Offset of the shoulders forward rotation")]
		[Range(-180f, 180f)] public float shoulderForwardOffset = 0;
		[Tooltip("Offset of the shoulders up rotation")]
		[Range(-180f, 180f)] public float shoulderHeightOffset = 0;

		[Space]
		[Tooltip("Multiplier of the shoulder forward rotation")]
		[Range(0f, 2f)] public float shoulderForwardRotationMultiplier = 1;
		[Tooltip("Multiplier of the shoulder height rotation")]
		[Range(0f, 2f)] public float shoulderHeightRotationMultiplier = 1;

		[Space]
		[Tooltip("Offset of the elbow aim rotation")]
		[Range(-180f, 180f)] public float elbowRotation = 0;
		[Tooltip("Arm spacing, used to prevent clipping of arms in the body")]
		[Range(-2f, 2f)] public float armSpacing = 0;

		[Space]
		[Tooltip("Multiplier the change the arm length, used to change the arm bend")]
		[Range(0f, 2f)] public float armLengthMultiplier = 1;

		[Tooltip("Hand target position local")]
		public HandLocalTo handLocalPosition = HandLocalTo.Arms;
		[Tooltip("Hand target rotation local")]
		public HandLocalTo handLocalRotation = HandLocalTo.Root;
		[Tooltip("Copy the hand local rotation or world rotation")]
		public bool handRotationLocal = true;

		[Header("Hand IK")]
		[Tooltip("0 - 1, 0 means copy rotations of the target, 1 means copy the hand target")]
		[Range(0f, 1f)] public float armIK = 0;

		[Space]
		[Tooltip("Offset of hands forward")]
		[Range(-1, 1)] public float handForwardOffset = 0;
		[Tooltip("Offset of hands width")]
		[Range(-1, 1)] public float handWidthOffset = 0;
		[Tooltip("Offset of hands height")]
		[Range(-1, 1)] public float handHeightOffset = 0;

		[Space]
		[Tooltip("Multiplier of hands forward of the target")]
		[Range(0f, 2f)] public float handForwardMultiplier = 1;
		[Tooltip("Multiplier of hands width of the target")]
		[Range(0f, 2f)] public float handWidthMultiplier = 1;
		[Tooltip("Multiplier of hands height of the target")]
		[Range(0f, 2f)] public float handHeightMultiplier = 1;

		[Header("Spine")]
		[Tooltip("Default rotation offset of the hips")]
		[Range(-180f, 180f)] public float defaultHipBend = 0;
		[Tooltip("Default spine rotation offset")]
		[Range(-180f, 180f)] public float defaultSpineBend = 0;
		[Tooltip("Default neck rotation offset")]
		[Range(-180f, 180f)] public float defaultNeckBend = 0;

		[Space]
		[Tooltip("Multiplier of the spine bend")]
		[Range(0f, 2f)] public float spineBendMultiplier = 1;
		[Tooltip("Multiplier of the spine side angle")]
		[Range(0f, 2f)] public float spineAngleMultiplier = 1;
		[Tooltip("Multiplier of the spine twist")]
		[Range(0f, 2f)] public float spineTwistMultiplier = 1;

		/// <summary>
		/// Copy the parameters
		/// </summary>
		/// <returns>Copied version of the parameters</returns>
		public AnimatorParameters Copy()
		{
			return new AnimatorParameters
			{
				matchSameSpeed = matchSameSpeed,
				hipHeightMultiplier = hipHeightMultiplier,
				legWidth = legWidth,
				kneeRotation = kneeRotation,
				shoulderForwardOffset = shoulderForwardOffset,
				shoulderHeightOffset = shoulderHeightOffset,
				shoulderForwardRotationMultiplier = shoulderForwardRotationMultiplier,
				shoulderHeightRotationMultiplier = shoulderHeightRotationMultiplier,
				elbowRotation = elbowRotation,
				armSpacing = armSpacing,
				armLengthMultiplier = armLengthMultiplier,
				handLocalPosition = handLocalPosition,
				handLocalRotation = handLocalRotation,
				handRotationLocal = handRotationLocal,
				armIK = armIK,
				handForwardOffset = handForwardOffset,
				handWidthOffset = handWidthOffset,
				handHeightOffset = handHeightOffset,
				handForwardMultiplier = handForwardMultiplier,
				handWidthMultiplier = handWidthMultiplier,
				handHeightMultiplier = handHeightMultiplier,
				defaultHipBend = defaultHipBend,
				defaultSpineBend = defaultSpineBend,
				defaultNeckBend = defaultNeckBend,
				spineBendMultiplier = spineBendMultiplier,
				spineAngleMultiplier = spineAngleMultiplier,
				spineTwistMultiplier = spineTwistMultiplier,
			};
		}

		/// <summary>
		/// Check if the parameters are the same
		/// </summary>
		/// <param name="p_Other">Other parameters</param>
		/// <returns>Returns true is the settings are the same</returns>
		public bool Simular(AnimatorParameters p_Other)
		{
			return	matchSameSpeed == p_Other.matchSameSpeed &&
					hipHeightMultiplier == p_Other.hipHeightMultiplier &&
					legWidth == p_Other.legWidth &&
					kneeRotation == p_Other.kneeRotation &&
					shoulderForwardOffset == p_Other.shoulderForwardOffset &&
					shoulderHeightOffset == p_Other.shoulderHeightOffset &&
					shoulderForwardRotationMultiplier == p_Other.shoulderForwardRotationMultiplier &&
					shoulderHeightRotationMultiplier == p_Other.shoulderHeightRotationMultiplier &&
					elbowRotation == p_Other.elbowRotation &&
					armSpacing == p_Other.armSpacing &&
					armLengthMultiplier == p_Other.armLengthMultiplier &&
					handLocalPosition == p_Other.handLocalPosition &&
					handLocalRotation == p_Other.handLocalRotation &&
					handRotationLocal == p_Other.handRotationLocal &&
					armIK == p_Other.armIK &&
					handForwardOffset == p_Other.handForwardOffset &&
					handWidthOffset == p_Other.handWidthOffset &&
					handHeightOffset == p_Other.handHeightOffset &&
					handForwardMultiplier == p_Other.handForwardMultiplier &&
					handWidthMultiplier == p_Other.handWidthMultiplier &&
					handHeightMultiplier == p_Other.handHeightMultiplier &&
					defaultHipBend == p_Other.defaultHipBend &&
					defaultSpineBend == p_Other.defaultSpineBend &&
					defaultNeckBend == p_Other.defaultNeckBend &&
					spineBendMultiplier == p_Other.spineBendMultiplier &&
					spineAngleMultiplier == p_Other.spineAngleMultiplier &&
					spineTwistMultiplier == p_Other.spineTwistMultiplier;
		}

		/// <summary>
		/// Convert the paramters to the protocol version
		/// </summary>
		/// <param name="p_Parameters">Parameters</param>
		public static implicit operator HProt.Polygon.Retargeting.Settings(AnimatorParameters p_Parameters)
		{
			return new HProt.Polygon.Retargeting.Settings
			{
				MatchSameSpeed = p_Parameters.matchSameSpeed,
				HipHeightMultiplier = p_Parameters.hipHeightMultiplier,
				LegWidth = p_Parameters.legWidth,
				KneeRotation = p_Parameters.kneeRotation,
				ShoulderForwardOffset = p_Parameters.shoulderForwardOffset,
				ShoulderHeightOffset = p_Parameters.shoulderHeightOffset,
				ShoulderForwardRotationMultiplier = p_Parameters.shoulderForwardRotationMultiplier,
				ShoulderHeightRotationMultiplier = p_Parameters.shoulderHeightRotationMultiplier,
				ElbowRotation = p_Parameters.elbowRotation,
				ArmSpacing = p_Parameters.armSpacing,
				ArmLengthMultiplier = p_Parameters.armLengthMultiplier,
				HandLocalPosition = p_Parameters.handLocalPosition,
				HandLocalRotation = p_Parameters.handLocalRotation,
				HandRotationLocal = p_Parameters.handRotationLocal,
				ArmIK = p_Parameters.armIK,
				HandForwardOffset = p_Parameters.handForwardOffset,
				HandWidthOffset = p_Parameters.handWidthOffset,
				HandHeightOffset = p_Parameters.handHeightOffset,
				HandForwardMultiplier = p_Parameters.handForwardMultiplier,
				HandWidthMultiplier = p_Parameters.handWidthMultiplier,
				HandHeightMultiplier = p_Parameters.handHeightMultiplier,
				DefaultHipBend = p_Parameters.defaultHipBend,
				DefaultSpineBend = p_Parameters.defaultSpineBend,
				DefaultNeckBend = p_Parameters.defaultNeckBend,
				SpineBendMultiplier = p_Parameters.spineBendMultiplier,
				SpineAngleMultiplier = p_Parameters.spineAngleMultiplier,
				SpineTwistMultiplier = p_Parameters.spineTwistMultiplier,
			};
		}
	}
}