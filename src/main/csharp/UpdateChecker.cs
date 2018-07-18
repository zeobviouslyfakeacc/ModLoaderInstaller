using System;
using System.Net;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace ModLoader {

	// No async & await in .NET 3.5 :(
	internal class UpdateChecker {

		private const int JOIN_TIMEOUT = 1000; // ms
		private const string VERSION_FILE_URL = "https://raw.githubusercontent.com/zeobviouslyfakeacc/ModLoaderInstaller/master/version.txt";

		private readonly Thread thread;
		private string updateVersion = "";

		internal UpdateChecker() {
			thread = new Thread(DoCheck);
			thread.Start();
		}

		internal string GetUpdateVersion() {
			if (!thread.Join(JOIN_TIMEOUT)) {
				Debug.LogWarning("Mod Loader update checker timed out");
			}
			return updateVersion;
		}

		private void DoCheck() {
			// XXX: Mono version used by Unity doesn't support TLS properly.
			// XXX: TLD doesn't ship with System.Security, which causes spammy log messages, but everything seems to work
			var regularCallback = ServicePointManager.ServerCertificateValidationCallback;
			try {
				ServicePointManager.ServerCertificateValidationCallback = (obj, certificate, chain, sslPolicyErrors) => true;

				using (WebClient webClient = new WebClient()) {
					string currentVersion = VersionToString(Assembly.GetExecutingAssembly().GetName().Version);
					string webVersion = webClient.DownloadString(VERSION_FILE_URL);

					if (currentVersion != webVersion) {
						updateVersion = webVersion;
						Debug.Log("Mod Loader update available. Current version: " + currentVersion + ", web version: " + webVersion);
					}
				}
			} catch (Exception e) {
				Debug.LogWarning("Mod Loader update checker failed:");
				Debug.LogException(e);
			} finally {
				ServicePointManager.ServerCertificateValidationCallback = regularCallback;
			}
		}

		private static string VersionToString(Version version) {
			if (version.Build <= 0)
				return version.ToString(2);
			if (version.Revision <= 0)
				return version.ToString(3);
			else
				return version.ToString(4);
		}
	}
}
