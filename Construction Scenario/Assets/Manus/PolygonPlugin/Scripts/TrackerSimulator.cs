using UnityEngine;
using System.Collections.Generic;
using Hermes.Protocol;
using Manus.Hermes;
using HProt = Hermes.Protocol;

namespace Manus.Polygon.Prototyping
{
	public class TrackerSimulator : MonoBehaviour
    {
        /// <summary>
        /// Send simulated trackers to hermes
        /// </summary>
        public bool sendToHermes;

        /// <summary>
        /// The frequency to update the trackers
        /// </summary>
		[SerializeField, Range(1, 144), Tooltip("The frequency to send the tracker data")] 
        public int updateRate = 90;

        /// <summary>
        /// The trackers you want to send to hermes
        /// </summary>
        public SimulatedTrackerSet[] trackerSets;

        private float m_UpdateTimer = 0;

        private void Awake()
        {
            m_UpdateTimer = 1f / updateRate;
        }

        private void Update()
        {
            if (sendToHermes)
            {
                m_UpdateTimer -= Time.deltaTime;
                if (m_UpdateTimer <= 0)
                {
                    m_UpdateTimer = 1f / updateRate;
                    SendDataToHermes();
                }

            }
        }

        /// <summary>
        /// Send all data
        /// </summary>
        private void SendDataToHermes()
        {
            // In case some trackers are still null
            GenerateTransformForeachTracker();

            // Convert to proto trackers
            var t_TrackerList = new List<HProt.Tracker>();

            foreach (var t_TrackerSet in trackerSets)
            {
                foreach (var t_Tracker in t_TrackerSet.trackers)
                {
                    var t_ProtoTracker = new HProt.Tracker
                    {
                        ID = t_Tracker.deviceID,
                        UserIndex = t_Tracker.type == TrackerType.Unknown ? -1 : t_TrackerSet.id,
                        Type = t_Tracker.type,
                        Position = new Translation { Full = t_Tracker.tracker.position.ToProto() },
                        Rotation = new Orientation { Full = t_Tracker.tracker.rotation.ToProto() },
                        Quality = TrackingQuality.Trackable
                    };

                    t_TrackerList.Add(t_ProtoTracker);
                }
            }

            // Send to Hermes
            ManusManager.instance.communicationHub.GiveTrackerData(t_TrackerList.ToArray());
        }

        /// <summary>
        /// Generate all transforms for all the trackers
        /// </summary>
        [ContextMenu("Generate Transform Foreach Tracker")]
        private void GenerateTransformForeachTracker()
        {
            foreach (var t_TrackerSet in trackerSets)
            {
                foreach (var t_Tracker in t_TrackerSet.trackers)
                {
                    if (t_Tracker.tracker == null)
                    {
                        t_Tracker.tracker = new GameObject($"{t_Tracker.type} Tracker - {t_Tracker.deviceID} - {t_TrackerSet.id}").transform;
                        t_Tracker.tracker.SetParent(transform, true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set of trackers, used for one body, can only contain one tracker of each type
    /// </summary>
    [System.Serializable]
    public class SimulatedTrackerSet
    {
        public int id;
        public SimulatedTracker[] trackers;
    }

    /// <summary>
    /// Individual trackers
    /// </summary>
    [System.Serializable]
    public class SimulatedTracker
    {
        public TrackerType type = TrackerType.Unknown;
        public string deviceID = string.Empty;
        public Transform tracker;
    }
}