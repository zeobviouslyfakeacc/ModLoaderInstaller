using Harmony;
using System;
using UnityEngine;

namespace ModLoader {

	[HarmonyPatch(typeof(GameManager), "ReadVersionFile", new Type[0])]
	internal class AddModdedToVersionString {

		private static void Postfix() {
			GameManager.m_GameVersionString = "Modded " + GameManager.m_GameVersionString;
			Debug.Log(" === This game is MODDED. Do not report any issues to Hinterland! === ");
		}
	}

	[HarmonyPatch(typeof(Panel_MainMenu), "SetVersionLabel", new Type[0])]
	internal class AddFailedModsToVersionLabel {

		private static void Postfix(Panel_MainMenu __instance) {
			if (!ModLoader.HasFailed)
				return;

			GameObject gameObject = __instance.m_VersionLabel;
			UILabel versionLabel = gameObject.GetComponent<UILabel>();
			versionLabel.color = Color.red;
			versionLabel.multiLine = true;
			versionLabel.text += "\n\n" + ModLoader.failureMessage;
			versionLabel.depth = int.MaxValue;
		}
	}

	[HarmonyPatch(typeof(BasicMenu), "InternalClickAction", new Type[] { typeof(int), typeof(bool) })]
	internal class DisableStartGameButtons {

		private static bool Prefix(int index) {
			// Skip if mod loading failed and button clicked was Story, Sandbox or Challenge
			return (!ModLoader.HasFailed) || (index > 2);
		}
	}
}
