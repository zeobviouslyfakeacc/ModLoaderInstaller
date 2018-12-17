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
			GameObject gameObject = __instance.m_VersionLabel;
			UILabel versionLabel = gameObject.GetComponent<UILabel>();
			versionLabel.multiLine = true;
			versionLabel.overflowMethod = UILabel.Overflow.ResizeHeight;
			versionLabel.width = 1100;
			versionLabel.depth = int.MaxValue;

			if (ModLoader.HasUpdate(out string version)) {
				versionLabel.text += "\n\n[url=update]A new version of the Mod Loader is available (v" + version + "). Click here to be taken to the download page.[/url]";

				BoxCollider collider = gameObject.AddComponent<BoxCollider>();
				collider.center = versionLabel.localCenter;
				collider.size = versionLabel.localSize;

				OpenLinkInBrowser opener = gameObject.AddComponent<OpenLinkInBrowser>();
				opener.label = versionLabel;
			}

			if (ModLoader.HasFailed(out string failureMessage)) {
				versionLabel.color = Color.red;
				versionLabel.text += "\n\n" + failureMessage;
			}
		}

		private class OpenLinkInBrowser : MonoBehaviour {

			private const string downloadsPageLink = "https://github.com/zeobviouslyfakeacc/ModLoaderInstaller/releases";
			internal UILabel label;

			private void OnClick() {
				if (label.GetUrlAtPosition(UICamera.lastWorldPosition) == "update") {
					Application.OpenURL(downloadsPageLink);
				}
			}
		}
	}

	[HarmonyPatch(typeof(BasicMenu), "InternalClickAction", new Type[] { typeof(int), typeof(bool) })]
	internal static class DisableStartGameButtons {

		private static bool Prefix(BasicMenu __instance, int index) {
			BasicMenu mainMenu = (BasicMenu) AccessTools.Field(typeof(Panel_MainMenu), "m_BasicMenu")
					.GetValue(InterfaceManager.m_Panel_MainMenu);

			// Skip if mod loading failed and button clicked was Story, Sandbox or Challenge
			return !(__instance == mainMenu && ModLoader.HasFailed() && index < 3);
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

				Assembly unityConnectModule = GetUnityConnectAssembly();
				SetInternalProperty(unityConnectModule, "UnityEngine.Connect.UnityConnectSettings", "enabled", false);
				SetInternalProperty(unityConnectModule, "UnityEngine.Advertisements.UnityAdsSettings", "enabled", false);
			} catch (Exception ex) {
				Debug.LogError("Could not disable player tracking.");
				Debug.LogException(ex);
			}
		}

		private static Assembly GetUnityConnectAssembly() {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach (Assembly assembly in assemblies) {
				if (assembly.GetName().Name == "UnityEngine.UnityConnectModule") {
					return assembly;
				}
			}
			throw new Exception("Could not find the UnityConnectModule Assembly.");
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
