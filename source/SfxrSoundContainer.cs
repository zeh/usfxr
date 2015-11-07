using System.Collections.Generic;
using UnityEngine;

public class SfxrSoundContainer {
	private static SfxrSoundContainer container = null;

	public static SfxrSoundContainer Create() {
#if !UNITY_EDITOR
		if (SfxrSoundContainer.container != null)
			return SfxrSoundContainer.container;
#endif

		string paramsList = ReadSoundsFile();
		Dictionary<string, string> configurations = new Dictionary<string, string>();

		string[] pairs = paramsList.Split(new string[] { "\n", "\r\n" }, System.StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < pairs.Length; ++i) {
			string[] vals = pairs[i].Split(':');
			string title = vals[0];
			string parameters = vals[1];

			configurations.Add(title, parameters);
		}

		SfxrSoundContainer.container = new SfxrSoundContainer(configurations);
		return SfxrSoundContainer.container;
	}

	private static string ReadSoundsFile() {
		TextAsset textFile = Resources.Load<TextAsset>("usfxr_sounds");
		return !textFile ? string.Empty : textFile.text;
	}

	private Dictionary<string, string> configs = new Dictionary<string, string>();

	private SfxrSoundContainer(Dictionary<string, string> entries) {
		foreach (var kvp in entries)
			configs.Add(kvp.Key.ToLowerInvariant(), kvp.Value);
	}

	public bool IsEmpty { get { return configs.Count == 0; } }

	public bool Contains(string title) {
		string actualTitle = title.ToLowerInvariant();
		return configs.ContainsKey(actualTitle);
	}

	public string[] GetTitles() {
		string[] titles = new string[configs.Count];

		int i = 0;
		foreach (var title in configs.Keys) {
			titles[i] = title;
			++i;
		}

		return titles;
	}

	public string GetSound(string title) {
		string actualTitle = title.ToLowerInvariant();
		if (configs.ContainsKey(actualTitle))
			return configs[actualTitle];

		Debug.LogError("No sound with title '" + title + "' found. Create it or verify it has not been removed.");
		return ",,,,,,,,,,,,,,,,,,,,,,,";
	}

#if UNITY_EDITOR
	public void AddSound(string title, string parameters) {
		string actualTitle = title.ToLowerInvariant();
		configs.Add(actualTitle, parameters);
		SaveToFile();
	}

	public void ReplaceSound(string title, string parameters) {
		string actualTitle = title.ToLowerInvariant();
		configs[actualTitle] = parameters;
		SaveToFile();
	}

	public void DeleteSound(string title) {
		string actualTitle = title.ToLowerInvariant();
		configs.Remove(actualTitle);
		SaveToFile();
	}

	private void SaveToFile() {
		string contents = ToString();
		string filePath = Application.dataPath + "/Resources/usfxr_sounds.txt";
		System.IO.FileInfo file = new System.IO.FileInfo(filePath);
		if (!file.Directory.Exists)
			this.CreateDirectory(file.Directory);

		System.IO.File.WriteAllText(filePath, contents);
		UnityEditor.AssetDatabase.Refresh();
	}

	public override string ToString() {
		System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
		var titles = new List<string>(configs.Keys);
		titles.Sort();
		foreach (var title in titles)
			strBuilder.Append(title + ":" + configs[title] + System.Environment.NewLine);

		return strBuilder.ToString();
	}

	private void CreateDirectory(System.IO.DirectoryInfo dir) {
		if (!dir.Parent.Exists)
			this.CreateDirectory(dir.Parent);

		dir.Create();
	}
#endif
}
