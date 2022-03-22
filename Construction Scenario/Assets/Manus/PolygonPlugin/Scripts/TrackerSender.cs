using System.Collections.Generic;
using UnityEngine;
using Manus.VR;
using Manus.Utility;
using Hermes.Protocol;
using Manus.Hermes;
using HProt = Hermes.Protocol;

namespace Manus.Polygon
{
	public class TrackerSender : MonoBehaviour
	{
		#region Fields

		/// <summary>
		/// Decide if you also want to send the unassigned trackers, so they can be assigned in the Manus Core
		/// </summary>
		[SerializeField, Tooltip("Decide if you also want to send the unassigned trackers, so they can be assigned in the Manus Core")]
		private bool m_AlsoSendUnassigned = true;

		/// <summary>
		/// The trackers that have to be send to hermes
		/// </summary>
		[SerializeField, Tooltip("The trackers that have to be send to hermes")] private VRTrackerType[] m_TrackersToSend =
			{
				VRTrackerType.Head,
				VRTrackerType.LeftHand,
				VRTrackerType.RightHand,
				VRTrackerType.Waist,
				VRTrackerType.LeftFoot,
				VRTrackerType.RightFoot,
				VRTrackerType.LeftUpperArm,
				VRTrackerType.RightUpperArm
			};

		/// <summary>
		/// The tracker sets that are send to manus core, by default send all
		/// </summary>
		[SerializeField, Tooltip("By default all trackers get send, but you can only send the 1st of every tracker, so there will only be one tracker set available in manus core")] 
		private List<int> m_UpdateOnly = null;

		/// <summary>
		/// The frequency to send the tracker data
		/// </summary>
		[SerializeField, Range(1, 144), Tooltip("The frequency to send the tracker data")]
		private int m_UpdateRate = 90;

		private TrackerManager m_TrackerManager;
		private float m_Timer = 0f;

		#endregion

		#region MonoBehaviour Callbacks

		private void OnEnable()
		{
			m_TrackerManager = TrackerManager.instance;
		}

		private void Update()
		{
			m_Timer -= Time.deltaTime;
			if (m_Timer < 0)
			{
				m_Timer = 1f / m_UpdateRate;
				UpdateTrackingDataToHermes();
			}
		}

		#endregion

		/// <summary>
		/// Sends all tracker data to manus core
		/// </summary>
		private void UpdateTrackingDataToHermes()
		{
			if (ManusManager.instance?.communicationHub == null)
				return;

			var t_TrackerList = new List<HProt.Tracker>();

			foreach (VRTrackerType t_Type in m_TrackersToSend)
			{
				if (m_TrackerManager.trackersType[(int)t_Type]?.Count > 0)
				{
					for (int i = 0; i < m_TrackerManager.trackersType[(int)t_Type].Count; i++)
					{
						if (m_UpdateOnly != null && m_UpdateOnly.Count > 0 && !m_UpdateOnly.Contains(i))
							continue;

						if (m_TrackerManager.trackersType[(int)t_Type][i] == null)
							continue;

						TrackerType t_ProtoType = TrackerType.Unknown;
						switch (m_TrackerManager.trackersType[(int)t_Type][i].type)
						{
							case VRTrackerType.LeftHand:
								t_ProtoType = TrackerType.LeftHand;
								break;
							case VRTrackerType.RightHand:
								t_ProtoType = TrackerType.RightHand;
								break;
							case VRTrackerType.LeftFoot:
								t_ProtoType = TrackerType.LeftFoot;
								break;
							case VRTrackerType.RightFoot:
								t_ProtoType = TrackerType.RightFoot;
								break;
							case VRTrackerType.Waist:
								t_ProtoType = TrackerType.Waist;
								break;
							case VRTrackerType.Head:
								t_ProtoType = TrackerType.Head;
								break;
							case VRTrackerType.LeftUpperArm:
								t_ProtoType = TrackerType.LeftUpperArm;
								break;
							case VRTrackerType.RightUpperArm:
								t_ProtoType = TrackerType.RightUpperArm;
								break;
						}

						if (t_ProtoType == TrackerType.Unknown)
							continue;

						var t_ProtoTracker = new HProt.Tracker
						{
							ID = m_TrackerManager.trackersType[(int)t_Type][i].deviceID,
							UserIndex = t_ProtoType == TrackerType.Unknown ? -1 : i,
							Type = t_ProtoType,
							Position = new Translation { Full = m_TrackerManager.trackersType[(int)t_Type][i].position.ToProto() },
							Rotation = new Orientation { Full = m_TrackerManager.trackersType[(int)t_Type][i].rotation.ToProto() },
							Quality = TrackingQuality.Trackable
						};

						t_TrackerList.Add(t_ProtoTracker);
					}
				}
			}

			if (m_AlsoSendUnassigned)
			{
				VRTrackerType t_UnassignedType = VRTrackerType.Other;
				if (m_TrackerManager.trackersType[(int)t_UnassignedType]?.Count > 0)
				{
					for (int i = 0; i < m_TrackerManager.trackersType[(int)t_UnassignedType].Count; i++)
					{
						if (m_TrackerManager.trackersType[(int)t_UnassignedType][i] == null)
							continue;

						var t_ProtoTracker = new HProt.Tracker
						{
							ID = m_TrackerManager.trackersType[(int)t_UnassignedType][i].deviceID.ToString(),
							UserIndex = -1,
							Type = TrackerType.Unknown,
							Position = new Translation { Full = m_TrackerManager.trackersType[(int)t_UnassignedType][i].position.ToProto() },
							Rotation = new Orientation { Full = m_TrackerManager.trackersType[(int)t_UnassignedType][i].rotation.ToProto() },
							Quality = TrackingQuality.Trackable
						};

						t_TrackerList.Add(t_ProtoTracker);
					}
				}
			}
			
			ManusManager.instance.communicationHub.GiveTrackerData(t_TrackerList.ToArray());
		}
	}
}