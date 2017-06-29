using System;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace ModLoader
{
	public class ModLoader
	{
		public static void Start()
		{
			try
			{
				Debug.Log("Loading Harmony...");
				Assembly harmonyAssembly = Assembly.LoadFrom("tld_Data/Managed/Harmony.dll");
				Type harmonyInstanceType = harmonyAssembly.GetType("Harmony.HarmonyInstance", true);
				MethodInfo createMethod = harmonyInstanceType.GetMethod("Create", new Type[] { typeof(string) });
				MethodInfo patchAllMethod = harmonyInstanceType.GetMethod("PatchAll", new Type[] { typeof(Assembly) });
				Debug.Log("Harmony loaded");

				DirectoryInfo dir = new DirectoryInfo("mods");
				FileInfo[] files = dir.GetFiles("*.dll");
				foreach (FileInfo file in files)
				{
					Debug.Log("Loading mod " + file.Name);
					// Load Assembly and dependencies. Just resolve if already loaded (same for dependencies)
					Assembly assembly = Assembly.LoadFrom(file.FullName);

					// Search all Types in Assembly for HarmonyPatch instructions
					object harmonyInstance = createMethod.Invoke(null, new object[] { assembly.FullName });
					patchAllMethod.Invoke(harmonyInstance, new object[] { assembly });

					// Then call static "OnLoad" method on all types where this method is defined
					foreach (Type type in assembly.GetTypes())
					{
						MethodInfo onLoad = type.GetMethod("OnLoad", new Type[0]);
						if (onLoad != null)
						{
							onLoad.Invoke(null, new object[0]);
						}
					}
				}
				Debug.Log("All mods successfully loaded!");
			}
			catch (Exception e)
			{
				Debug.LogError("Loading mods failed!");
				Debug.LogException(e);
			}
		}
	}
}
