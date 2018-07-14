using Harmony;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.CrashReportHandler;

namespace ModLoader {

	[HarmonyPatch(typeof(GameManager), "ReadVersionFile", new Type[0])]
	internal static class AddModdedToVersionString {

		private static void Postfix() {
			GameManager.m_GameVersionString = "Modded " + GameManager.m_GameVersionString;
		}
	}

	[HarmonyPatch(typeof(Panel_MainMenu), "SetVersionLabel", new Type[0])]
	internal static class AddFailedModsToVersionLabel {

		private static void Postfix(Panel_MainMenu __instance) {
			if (!ModLoader.HasFailed)
				return;

			GameObject gameObject = __instance.m_VersionLabel;
			UILabel versionLabel = gameObject.GetComponent<UILabel>();
			versionLabel.color = Color.red;
			versionLabel.multiLine = true;
			versionLabel.overflowMethod = UILabel.Overflow.ResizeHeight;
			versionLabel.width = 1100;
			versionLabel.text += "\n\n" + ModLoader.failureMessage;
			versionLabel.depth = int.MaxValue;
		}
	}

	[HarmonyPatch(typeof(BasicMenu), "InternalClickAction", new Type[] { typeof(int), typeof(bool) })]
	internal static class DisableStartGameButtons {

		private static bool Prefix(BasicMenu __instance, int index) {
			BasicMenu mainMenu = (BasicMenu) AccessTools.Field(typeof(Panel_MainMenu), "m_BasicMenu")
					.GetValue(InterfaceManager.m_Panel_MainMenu);

			// Skip if mod loading failed and button clicked was Story, Sandbox or Challenge
			return !(__instance == mainMenu && ModLoader.HasFailed && index < 3);
		}
	}

	[HarmonyPatch(typeof(Utils), "HasGameBeenModded")]
	internal static class ShortCircuit_HasGameBeenModded {

		private static bool Prefix(ref bool __result) {
			__result = true; // Yes, this game is modded
			return false; // No, we don't need you to load Harmony again
		}
	}

	internal static class DisableUnityTracking {

		public static void OnLoad() {
			Debug.Log(" === This game is MODDED. Do not report any issues to Hinterland! === ");

			try {
				CrashReportHandler.enableCaptureExceptions = false;
				Analytics.enabled = false;
				Analytics.deviceStatsEnabled = false;
				Analytics.limitUserTracking = true;

				Assembly unityConnectModule = typeof(RemoteSettings).Assembly;
				SetInternalProperty(unityConnectModule, "UnityEngine.Connect.UnityConnectSettings", "enabled", false);
				SetInternalProperty(unityConnectModule, "UnityEngine.Advertisements.UnityAdsSettings", "enabled", false);
			} catch (Exception ex) {
				Debug.LogError("Could not disable player tracking.");
				Debug.LogException(ex);
			}
		}

		private static void SetInternalProperty(Assembly assembly, string className, string propertyName, object value) {
			try {
				Type type = assembly.GetType(className, true);
				PropertyInfo prop = type.GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
				prop.SetValue(null, value, null);
			} catch (Exception ex) {
				Debug.LogError("Could not set property '" + propertyName + "' of class '" + className + "'");
				Debug.LogException(ex);
			}
		}
	}
}
