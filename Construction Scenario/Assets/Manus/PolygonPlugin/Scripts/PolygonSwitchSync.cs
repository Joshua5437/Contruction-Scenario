using Lidgren.Network;
using Manus.Networking;
using Manus.Networking.Sync;
using UnityEngine;

namespace Manus.Polygon.Networking
{
	[DisallowMultipleComponent]
	[UnityEngine.AddComponentMenu("Manus/Networking/Sync/Polygon Switch (Sync)")]
	[RequireComponent(typeof(PolygonSwitch))]
	public class PolygonSwitchSync : BaseSync
	{
		private bool m_Owned = false;

		private int m_LastCharacterIndex = 0;
		private PolygonSwitch m_Switch = null;

		public override void Initialize(NetObject p_Object)
		{
			m_Switch = GetComponent<PolygonSwitch>();
			m_Switch.LoadCharacter(0);
		}

		public override void Clean()
		{
			m_LastCharacterIndex = m_Switch.currentIndex;
		}

		public override bool IsDirty()
		{
			return m_LastCharacterIndex != m_Switch.currentIndex;
		}

		public override void ReceiveData(NetBuffer p_Msg)
		{
			int t_Index = p_Msg.ReadInt32();

			if (!m_Owned)
				m_Switch.LoadCharacter(t_Index);
		}

		public override void WriteData(NetBuffer p_Msg)
		{
			p_Msg.Write(m_Switch.currentIndex);
		}

		public override void OnGainOwnership(Manus.Networking.NetObject p_Object)
		{
			if (m_Switch == null)
				GetComponent<PolygonSwitch>();

			m_Switch.isLocal = true;
		}

		public override void OnLoseOwnership(Manus.Networking.NetObject p_Object)
		{
			if (m_Switch == null)
				GetComponent<PolygonSwitch>();

			m_Switch.isLocal = false;
		}
	}
}