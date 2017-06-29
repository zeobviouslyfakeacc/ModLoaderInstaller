using System;
using System.Reflection;
using System.IO;
using UnityEngine;

namespace Rewired
{
	public class InputManager_Base
	{
		public static void Awake()
		{
			// COPY START
			try
			{
				Debug.Log("Loading mod loader...");
				Assembly modLoaderAssembly = Assembly.LoadFrom("tld_Data/Managed/ModLoader.dll");
				Type modLoaderType = modLoaderAssembly.GetType("ModLoader.ModLoader", true);
				MethodInfo startMethod = modLoaderType.GetMethod("Start", new Type[0]);
				startMethod.Invoke(null, new object[0]);
			}
			catch (Exception e)
			{
				Debug.LogError("Could not start mod loader!");
				Debug.LogException(e);
			}
			// COPY END
		}
	}
}
