using UnityEngine;
using HProt = Hermes.Protocol;
using System.Threading.Tasks;
using System;
using System.Collections;

namespace Manus.Polygon
{
	public static class PolygonValidation
	{
		/// <summary>
		/// Polygon license
		/// </summary>
		public static bool polygonLicensed
		{
			get { return m_LastPolygonLicense; }
		}

		/// <summary>
		/// The coroutine that listens to the latest license state and polygon version
		/// </summary>
		public static Coroutine polygonRoutine = null;
		
		private static bool m_Logged = false;
		private static bool m_LastPolygonLicense = false;

		/// <summary>
		/// Gets called when the license state changes
		/// </summary>
		public static Action<bool> licenseUpdate;

		/// <summary>
		/// Start the polygon license and version check listener
		/// </summary>
		public static IEnumerator AddPolygonCoroutine()
		{
			if (polygonRoutine != null)
				yield break;

			while (true)
			{
				yield return new WaitForSeconds(.1f);

				// License Check
				if (ManusManager.instance?.communicationHub?.deviceLandscape != null)
				{
					LandscapeListener(ManusManager.instance.communicationHub.deviceLandscape);
				}
				else
				{
					if (m_LastPolygonLicense)
					{
						m_LastPolygonLicense = false;
						ToggleLicense(false);
					}
				}
			}
		}

		/// <summary>
		/// Callback for new landscape data
		/// </summary>
		/// <param name="p_Data">landscape data</param>
		private static void LandscapeListener(HProt.Hardware.DeviceLandscape p_Data)
		{
			if (p_Data.License.PolygonData == m_LastPolygonLicense)
				return;

			m_LastPolygonLicense = p_Data.License.PolygonData;

			if (p_Data.License.PolygonData)
			{
				ToggleLicense(true);
			}
			else
			{
				ToggleLicense(false);
			}
		}

		/// <summary>
		/// Log the latest state of the polygon license
		/// </summary>
		/// <param name="p_Licensed">True, polygon is licensed</param>
		private static void ToggleLicense(bool p_Licensed)
		{
			if (p_Licensed)
			{
				Debug.Log("Polygon status: Licensed");
				licenseUpdate?.Invoke(true);
			}
			else
			{
				Debug.Log("Polygon status: Unlicensed");
				licenseUpdate?.Invoke(false);
			}
		}
	}
}
