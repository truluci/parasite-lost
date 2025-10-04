
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class BuildSettingsUtility
{
	[MenuItem("Tools/Build Settings/List Scenes in Build Settings")]
	public static void ListScenesInBuildSettings()
	{
		var scenes = EditorBuildSettings.scenes;
		if (scenes == null || scenes.Length == 0)
		{
			Debug.Log("No scenes in Build Settings.");
			return;
		}

		Debug.Log($"Scenes in Build Settings ({scenes.Length}):");
		for (int i = 0; i < scenes.Length; i++)
		{
			Debug.Log($"{i}: {scenes[i].path} (enabled={scenes[i].enabled})");
		}
	}

	[MenuItem("Tools/Build Settings/Add Scenes Under Assets/Scenes")]
	public static void AddScenesUnderAssetsScenes()
	{
		string root = "Assets/Scenes";
		if (!Directory.Exists(root))
		{
			Debug.LogWarning($"Folder '{root}' not found in project.");
			return;
		}

		var files = Directory.GetFiles(root, "*.unity", SearchOption.AllDirectories)
			.Select(p => p.Replace("\\", "/"))
			.ToArray();

		if (files.Length == 0)
		{
			Debug.LogWarning($"No .unity scene files found under {root}.");
			return;
		}

		var existing = EditorBuildSettings.scenes?.ToList() ?? new System.Collections.Generic.List<EditorBuildSettingsScene>();
		int added = 0;
		foreach (var f in files)
		{
			if (!existing.Any(s => s.path == f))
			{
				existing.Add(new EditorBuildSettingsScene(f, true));
				added++;
			}
		}

		EditorBuildSettings.scenes = existing.ToArray();
		Debug.Log($"Added {added} scenes to Build Settings (found {files.Length} scenes under {root}).");
	}
}
