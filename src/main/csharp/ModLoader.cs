﻿using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ModLoader {

	public class ModLoader {

		private static string failureMessage = "";
		internal static bool HasFailed() => failureMessage != "";
		internal static bool HasFailed(out string message) {
			message = failureMessage;
			return message != "";
		}

		private static string updateVersion = "";
		internal static bool HasUpdate(out string version) {
			version = updateVersion;
			return version != "";
		}

		public static void Start() {
			// First of all, disable error reporting and tracking
			DisableUnityTracking.OnLoad();
			// Then patch in Mod Loader helpers
			HarmonyInstance.Create("Mod Loader").PatchAll(Assembly.GetExecutingAssembly());
			// Start the mod loader update check in the background
			UpdateChecker updateChecker = new UpdateChecker();

			// Load mods
			DirectoryInfo modsDir = GetModsDirectory();
			modsDir.Create(); // Does nothing if the directory already exists
			FileInfo[] files = modsDir.GetFiles("*.dll");

			try {
				DependencyGraph dependencyGraph = LoadModAssemblies(files);
				List<Assembly> sortedAssemblies = dependencyGraph.TopologicalSort();

				ApplyHarmonyPatches(sortedAssemblies);
				CallOnLoadMethods(sortedAssemblies);

				Debug.Log("All mods successfully loaded!");
			} catch (ModLoadingException mle) {
				failureMessage = mle.Message;
			}

			updateVersion = updateChecker.GetUpdateVersion();
		}

		private static DirectoryInfo GetModsDirectory() {
			DirectoryInfo dataDir = new DirectoryInfo(Application.dataPath);

			DirectoryInfo tldBaseDirectory;
			if (Application.platform == RuntimePlatform.OSXPlayer) {
				tldBaseDirectory = dataDir.Parent.Parent; // TheLongDark/tld.app/Contents/
			} else {
				tldBaseDirectory = dataDir.Parent; // TheLongDark/tld_Data/
			}

			return new DirectoryInfo(Path.Combine(tldBaseDirectory.FullName, "mods"));
		}

		private static DependencyGraph LoadModAssemblies(FileInfo[] assemblyFiles) {
			Debug.Log("Loading mod assemblies");
			List<Assembly> loadedAssemblies = new List<Assembly>();
			List<string> failedAssemblies = new List<string>();

			foreach (FileInfo file in assemblyFiles) {
				if (file.Extension != ".dll") // GetFiles filter is too inclusive
					continue;

				try {
					Assembly modAssembly = Assembly.LoadFrom(file.FullName);
					loadedAssemblies.Add(modAssembly);
				} catch (Exception e) {
					failedAssemblies.Add(file.Name + ExceptionToString(e));
					Debug.LogError("Loading mod " + file.Name + " failed!");
					Debug.LogException(e);
				}
			}

			if (failedAssemblies.Count > 0) {
				throw new ModLoadingException("The following mods could not be loaded:", failedAssemblies);
			}

			return new DependencyGraph(loadedAssemblies);
		}

		private static void ApplyHarmonyPatches(List<Assembly> modAssemblies) {
			Debug.Log("Applying Harmony patches");
			List<string> failedMods = new List<string>();

			foreach (Assembly modAssembly in modAssemblies) {
				try {
					HarmonyInstance.Create(modAssembly.FullName).PatchAll(modAssembly);
				} catch (Exception e) {
					failedMods.Add(modAssembly.GetName() + ExceptionToString(e));
					Debug.LogError("Patching mod " + modAssembly.GetName() + " failed!");
					Debug.LogException(e);

					if (e is ReflectionTypeLoadException rtle && rtle.LoaderExceptions != null) {
						foreach (Exception loaderException in rtle.LoaderExceptions) {
							Debug.LogException(loaderException);
						}
					}
				}
			}

			if (failedMods.Count > 0) {
				throw new ModLoadingException("The following mods could not be patched:", failedMods);
			}
		}

		private static void CallOnLoadMethods(List<Assembly> modAssemblies) {
			Debug.Log("Calling OnLoad methods");

			foreach (Assembly modAssembly in modAssemblies) {
				foreach (Type type in modAssembly.GetTypes()) {
					try {
						MethodInfo onLoad = type.GetMethod("OnLoad", new Type[0]);
						onLoad?.Invoke(null, new object[0]);
					} catch (Exception e) {
						if (e is TargetInvocationException && e.InnerException != null) {
							e = e.InnerException;
						}

						string message = "OnLoad method failed for type " + type.FullName + " of mod " + modAssembly.GetName().Name;
						Debug.LogError(message);
						Debug.LogException(e);
						throw new ModLoadingException(message + ":\n" + ExceptionToString(e));
					}
				}
			}
		}

		private static string ExceptionToString(Exception ex) {
			return " - " + ex.GetType().Name + ": " + ex.Message;
		}
	}

	internal class ModLoadingException : Exception {

		internal ModLoadingException(string message) : base(message) {
		}

		internal ModLoadingException(string baseMessage, List<string> mods) : base(BuildMessage(baseMessage, mods)) {
		}

		private static string BuildMessage(string baseMessage, List<string> mods) {
			return baseMessage + "\n- " + string.Join("\n- ", mods.ToArray());
		}
	}
}
