using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace ModLoader {

	public class DependencyGraph {

		private readonly Vertex[] vertices;
		private readonly Dictionary<string, Vertex> nameLookup;

		public DependencyGraph(List<Assembly> modAssemblies) {
			int size = modAssemblies.Count;
			vertices = new Vertex[size];
			nameLookup = new Dictionary<string, Vertex>(size);

			// Create vertices
			for (int i = 0; i < size; ++i) {
				Assembly modAssembly = modAssemblies[i];
				string modName = Path.GetFileNameWithoutExtension(modAssembly.Location);

				Vertex modVertex = new Vertex(i, modAssembly, modName);
				vertices[i] = modVertex;
				nameLookup[modAssembly.FullName] = modVertex;
			}

			// Add edges
			SortedDictionary<string, List<string>> modsWithMissingDeps = new SortedDictionary<string, List<string>>();
			foreach (Vertex modVertex in vertices) {
				Assembly modAssembly = modVertex.assembly;
				List<string> missingDependencies = new List<string>();

				foreach (AssemblyName dependency in modAssembly.GetReferencedAssemblies()) {
					if (nameLookup.TryGetValue(dependency.FullName, out Vertex dependencyVertex)) {
						modVertex.dependencies.Add(dependencyVertex);
						dependencyVertex.dependents.Add(modVertex);
					} else if (!TryLoad(dependency)) {
						Debug.LogError(modAssembly.FullName + " is missing referenced assembly " + dependency.FullName);
						missingDependencies.Add(dependency.Name + " v" + dependency.Version);
					}
				}

				if (missingDependencies.Count > 0) {
					modsWithMissingDeps.Add(modAssembly.GetName().Name, missingDependencies);
				}
			}

			if (modsWithMissingDeps.Count > 0) {
				// Some mods are missing dependencies, stop mod loading and show error message
				throw new ModLoadingException(BuildFailureMessage(modsWithMissingDeps));
			}
		}

		private static string BuildFailureMessage(SortedDictionary<string, List<string>> modsWithMissingDeps) {
			StringBuilder messageBuilder = new StringBuilder("Some mods could not be loaded due to missing dependencies which you have to install.\n");
			foreach (string modName in modsWithMissingDeps.Keys) {
				messageBuilder.AppendLine("- '" + modName + "' is missing the following dependencies:");
				foreach (string dependencyName in modsWithMissingDeps[modName]) {
					messageBuilder.AppendLine("    - " + dependencyName);
				}
			}
			return messageBuilder.ToString();
		}

		private static bool TryLoad(AssemblyName assembly) {
			try {
				Assembly.Load(assembly);
				return true;
			} catch (FileNotFoundException) {
				return false;
			} catch (Exception ex) {
				Debug.LogException(ex);
				throw new ModLoadingException("Unknown exception when loading mod libraries. Please check your log file.");
			}
		}

		public List<Assembly> TopologicalSort() {
			int[] unloadedDependencies = new int[vertices.Length];
			SortedList<string, Vertex> loadableMods = new SortedList<string, Vertex>();
			List<Assembly> loadedMods = new List<Assembly>(vertices.Length);

			// Initialize the directory
			for (int i = 0; i < vertices.Length; ++i) {
				Vertex vertex = vertices[i];
				int dependencyCount = vertex.dependencies.Count;

				unloadedDependencies[i] = dependencyCount;
				if (dependencyCount == 0)
					loadableMods.Add(vertex.name, vertex);
			}

			// Perform the (reverse) topological sorting
			while (loadableMods.Count > 0) {
				Vertex mod = loadableMods.Values[0];
				loadableMods.RemoveAt(0);
				loadedMods.Add(mod.assembly);

				foreach (Vertex dependent in mod.dependents) {
					unloadedDependencies[dependent.index] -= 1;
					if (unloadedDependencies[dependent.index] == 0) {
						loadableMods.Add(dependent.name, dependent);
					}
				}
			}

			if (loadedMods.Count < vertices.Length)
				throw new ArgumentException("Could not sort dependencies topologically due to a cyclic dependency.");
			return loadedMods;
		}

		private class Vertex {

			internal readonly int index;
			internal readonly Assembly assembly;
			internal readonly string name;

			internal readonly List<Vertex> dependencies;
			internal readonly List<Vertex> dependents;

			internal Vertex(int index, Assembly assembly, string name) {
				this.index = index;
				this.assembly = assembly;
				this.name = name;

				dependencies = new List<Vertex>();
				dependents = new List<Vertex>();
			}
		}
	}
}
