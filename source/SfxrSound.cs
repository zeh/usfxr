using UnityEngine;

[System.Serializable]
public class SfxrSound {
	private SfxrSynth synthesizer = new SfxrSynth();

	[SerializeField]
	private string sound = null;

	[SerializeField]
	private bool cached = true;

	[SerializeField]
	private uint mutations = 0;
	private bool HasMutations { get { return mutations > 0; } }

	[SerializeField]
	[Range(0f, 1f)]
	private float mutationFactor = .05f;

	private bool initialized = false;

	public void Play() {
		if (!initialized)
			Initialize();

		if (HasMutations)
			synthesizer.PlayMutated(mutationFactor, mutations);
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
			synthesizer.CacheMutations(mutations, mutationFactor, () => synthesizer.PlayMutated(mutationFactor, mutations));
		else
			synthesizer.CacheSound(() => synthesizer.Play());
	}

}
