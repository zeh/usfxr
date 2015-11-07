using UnityEngine;

[System.Serializable]
public class SfxrSound {
	private SfxrSynth synthesizer = new SfxrSynth();

	[SerializeField]
	private string sound = null;

	[SerializeField]
	private bool cached = true;

	[SerializeField]
	[Range(0, 100)]
	private int mutations = 0;
	private bool HasMutations { get { return mutations > 0; } }

	[SerializeField]
	[Range(0f, 1f)]
	private float mutationFactor = .05f;

	[System.NonSerialized]
	private bool initialized = false;

	public void Play() {
		if (!initialized)
			Initialize();

		if (HasMutations)
			synthesizer.PlayMutated(mutationFactor, (uint)mutations);
		else
			synthesizer.Play();
	}

	private void Initialize() {
		string parameters = SfxrSoundContainer.Create().GetSound(sound);
		synthesizer.parameters.SetSettingsString(parameters);
		if (cached)
			Cache();

		initialized = true;
	}

	private void Cache() {
		if (HasMutations)
			synthesizer.CacheMutations((uint)mutations, mutationFactor, () => synthesizer.PlayMutated(mutationFactor, (uint)mutations));
		else
			synthesizer.CacheSound(() => synthesizer.Play());
	}

}
