//-----------------------------------------------------------------------
// <summary>
//   SfxrEditor implements a Unity Editor window for SfxrParams values and
//   provides helper methods for custom editors of such sound parameters.
// </summary>
// <copyright file="SfxrEditor.cs" company="Tiaan.com">
//   Copyright 2013 Tiaan Geldenhuys
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Implements a Unity Editor window for SfxrParams values.
/// </summary>
/// <remarks>
/// The editor can be activated from the Window menu, followed by selecting a
/// game-object that has a component with sound-effect parameters.  With the
/// current implementation, to be able to play a sound, the editor must be in
/// play mode itself (i.e., not just in edit mode).  However, the parameter
/// values are not yet persisted, such that one currently still has to use the
/// "Copy Parameters" button to manually copy and paste values after updates.
/// </remarks>
public class SfxrEditor : EditorWindow
{
	protected Vector2 scrollPosition;  // Stores the current scroll-location

	protected bool autoInitParams = false;  // Creates params when a compatible field is found but is unassigned

	protected SfxrItem[] selectedItems;  // References the sounds currently being edited

	private static GUIContent[] waveTypeOptions;  // Options for the wave-type drop-down box

	private static Dictionary<string, bool> foldoutStates;  // Keeps track of GUI expansion states

	/// <summary>
	/// Open the sound-effects editor-window.
	/// </summary>
	[MenuItem("Window/uSFXR Sound Effects")]
	public static void Initialize()
	{
		var window = ScriptableObject.CreateInstance<SfxrEditor>();
		window.title = window.name = "uSFXR Editor";
		window.Show();
	}

	protected virtual void OnSelectionChange()
	{
		this.selectedItems = FindParameters(Selection.objects, this.autoInitParams).ToArray();
		this.Repaint();
	}

	protected class SfxrItem
	{
		public SfxrItem(
			SfxrParams parameters,
			Component gameComponent,
			MemberInfo memberInfo)
		{
			this.parameters = parameters;
			this.gameComponent = gameComponent;
			this.memberInfo = memberInfo;
		}

		public SfxrParams parameters;
		public Component gameComponent;
		public MemberInfo memberInfo;
	}

	protected static IEnumerable<SfxrItem> FindParameters(
		UnityEngine.Object[] objects, bool autoInitParams)
	{
		if (objects != null)
		{
			foreach (UnityEngine.Object @object in objects)
			{
				foreach (var item in FindParameters(@object, autoInitParams))
				{
					yield return item;
				}
			}
		}
	}

	protected static IEnumerable<SfxrItem> FindParameters(
		UnityEngine.Object @object, bool autoInitParams)
	{
		var gameObject = (@object as GameObject);
		if (!gameObject)
		{
			yield break;
		}

		foreach (var gameComponent in gameObject.GetComponents<Component>())
		{
			if (!gameComponent)
			{
				continue;
			}

			// Search through all the fields of the game-component
			// and identify all fields that contain SFXR parameters
			FieldInfo[] fieldInfos = gameComponent.GetType().GetFields(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			foreach (var fieldInfo in fieldInfos)
			{
				if (typeof(SfxrSynth).IsAssignableFrom(fieldInfo.FieldType))
				{
					SfxrItem item = FindParameters(gameComponent, fieldInfo, autoInitParams);
					if (item != null)
					{
						yield return item;
					}
				}
				else if (typeof(SfxrParams).IsAssignableFrom(fieldInfo.FieldType))
				{
					SfxrParams parameters = (fieldInfo.GetValue(gameComponent) as SfxrParams);
					if ((parameters == null)
						&& autoInitParams)
					{
						parameters = new SfxrParams();
						fieldInfo.SetValue(gameComponent, parameters);
					}

					if (parameters != null)
					{
						yield return new SfxrItem(parameters, gameComponent, fieldInfo);
					}
				}
			}
		}
	}

	protected static SfxrItem FindParameters(
		Component gameComponent, FieldInfo synthFieldInfo, bool autoInitParams)
	{
		SfxrParams parameters = null;
		var synth = (synthFieldInfo.GetValue(gameComponent) as SfxrSynth);
		if ((synth == null)
			&& autoInitParams)
		{
			synth = new SfxrSynth();
			synthFieldInfo.SetValue(gameComponent, synth);
		}

		if (synth != null)
		{
			parameters = synth.parameters;
			if ((parameters == null)
				&& autoInitParams)
			{
				parameters = new SfxrParams();
				synth.parameters = parameters;
			}
		}

		SfxrItem item = null;
		if (parameters != null)
		{
			item = new SfxrItem(parameters, gameComponent, synthFieldInfo);
		}

		return item;
	}

	protected virtual void OnGUI()
	{
		this.scrollPosition = EditorGUILayout.BeginScrollView(this.scrollPosition);
		try
		{
			SfxrItem[] currentItems = this.selectedItems;
			int itemCount = ((currentItems != null) ? currentItems.Length : 0);
			if (itemCount <= 0)
			{
				EditorGUILayout.HelpBox("Select a game-object that has a component with sound-effect parameters to be viewed or edited.", MessageType.Info, true);
				EditorGUILayout.Space();
			}
			else
			{
				foreach (SfxrItem item in currentItems)
				{
					bool isDetailed;
					string title = string.Format(
						"{0} @ {1}, {2}",
						((item.memberInfo != null) ? item.memberInfo.Name : null),
						((item.gameComponent != null) ? item.gameComponent.name : null),
						((item.gameComponent != null) ? ObjectNames.GetInspectorTitle(item.gameComponent) : null));
					int originalIndentLevel = EditorGUI.indentLevel;
					if (itemCount == 1)
					{
						isDetailed = true;
						GUILayout.Label(new GUIContent(string.Format("Parameters: {0}", title), "The current selection being displayed for editing below"));
					}
					else
					{
						string foldoutKey = string.Format("{0}:{1}", ((item.gameComponent != null) ? item.gameComponent.GetInstanceID() : 0), title);
						isDetailed = GetFoldoutState(foldoutKey);
						isDetailed = EditorGUILayout.Foldout(isDetailed, title);
						SetFoldoutState(foldoutKey, isDetailed);
						if (isDetailed)
						{
							EditorGUI.indentLevel = (originalIndentLevel + 1);
						}
					}

					if (isDetailed)
					{
						RenderParameters(item.parameters);
						EditorGUI.indentLevel = originalIndentLevel;
					}

					EditorGUILayout.Space();
				}
			}

			bool isChanged;
			EditorGUI.BeginChangeCheck();
			try
			{
				this.autoInitParams = EditorGUILayout.Toggle(new GUIContent("Auto-initiate Parameters", "Creates params when a compatible field is found but is unassigned"), this.autoInitParams);
			}
			finally
			{
				isChanged = EditorGUI.EndChangeCheck();
			}

			if (isChanged
				&& this.autoInitParams)
			{
				this.OnSelectionChange();  // Force a data refresh
			}
		}
		finally
		{
			EditorGUILayout.EndScrollView();
		}
	}

	private static Dictionary<string, bool> FoldoutStates
	{
		get
		{
			if (foldoutStates == null)
			{
				foldoutStates =
					new Dictionary<string, bool>(StringComparer.Ordinal);
			}

			return foldoutStates;
		}
	}

	protected static void SetFoldoutState(string key, bool state)
	{
		FoldoutStates[key ?? String.Empty] = state;
	}

	protected static bool GetFoldoutState(string key)
	{
		bool state;
		if (!FoldoutStates.TryGetValue((key ?? String.Empty), out state))
		{
			state = true;
		}

		return state;
	}

	/// <summary>
	/// Renders the specified SFXR parameters in the editor.
	/// </summary>
	/// <param name="parameters">The current parameters to be rendered.</param>
	/// <remarks>
	/// This method is called automatically for the standalone editor window
	/// when a game-object with parameters is selected.  However, this public
	/// method can also be called by CustomEditor implementations for specific
	/// game-components to render the editor in the Inspector window
	/// (see UnityEditor.Editor for details).  Also, this method can be used
	/// from PropertyDrawer implementations; future releases of the code may
	/// include such a default drawer (once SfxrSynth and SfxrParams supports
	/// native serialization for Unity).
	/// </remarks>
	public static void RenderParameters(SfxrParams parameters)
	{
		if (parameters == null)
		{
			return;
		}

		if (waveTypeOptions == null)
		{
			waveTypeOptions =
				new GUIContent[]
					{
						new GUIContent("Square Wave", "Square (0)"),
						new GUIContent("Sawtooth", "Saw (1)"),
						new GUIContent("Sine Wave", "Sin (2)"),
						new GUIContent("Noise", "Noise (3)")
					};
		}

		EditorGUI.BeginChangeCheck();
		try
		{
			RenderHeading("General Settings");
			RenderPopup(waveTypeOptions, ((int)(parameters.waveType)), (value => parameters.waveType = ((uint)(value))), new GUIContent("Wave Type", "Shape of the wave"));
			bool isSquareWaveType = (parameters.waveType == 0);
			RenderSlider(+0, +1, parameters.masterVolume, (value => parameters.masterVolume = value), new GUIContent("Volume", "Overall volume of the sound (0 to 1)"));

			RenderHeading("Wave Envelope");
			RenderSlider(+0, +1, parameters.attackTime, (value => parameters.attackTime = value), new GUIContent("Attack Time", "Length of the volume envelope attack (0 to 1)"));
			RenderSlider(+0, +1, parameters.sustainTime, (value => parameters.sustainTime = value), new GUIContent("Sustain Time", "Length of the volume envelope sustain (0 to 1)"));
			RenderSlider(+0, +1, parameters.sustainPunch, (value => parameters.sustainPunch = value), new GUIContent("Sustain Punch", "Tilts the sustain envelope for more 'pop' (0 to 1)"));
			RenderSlider(+0, +1, parameters.decayTime, (value => parameters.decayTime = value), new GUIContent("Decay Time", "Length of the volume envelope decay (yes, I know it's called release) (0 to 1)"));

			RenderHeading("Frequency");
			RenderSlider(+0, +1, parameters.startFrequency, (value => parameters.startFrequency = value), new GUIContent("Start Frequency", "Base note of the sound (0 to 1)"));
			RenderSlider(+0, +1, parameters.minFrequency, (value => parameters.minFrequency = value), new GUIContent("Minimum Frequency", "If sliding, the sound will stop at this frequency, to prevent really low notes (0 to 1)"));
			RenderSlider(-1, +1, parameters.slide, (value => parameters.slide = value), new GUIContent("Slide", "Slides the note up or down (-1 to 1)"));
			RenderSlider(-1, +1, parameters.deltaSlide, (value => parameters.deltaSlide = value), new GUIContent("Delta Slide", "Accelerates the slide (-1 to 1)"));
			RenderSlider(+0, +1, parameters.vibratoDepth, (value => parameters.vibratoDepth = value), new GUIContent("Vibrato Depth", "Strength of the vibrato effect (0 to 1)"));
			RenderSlider(+0, +1, parameters.vibratoSpeed, (value => parameters.vibratoSpeed = value), new GUIContent("Vibrato Speed", "Speed of the vibrato effect (i.e. frequency) (0 to 1)"));

			RenderHeading("Tone Change");
			RenderSlider(-1, +1, parameters.changeAmount, (value => parameters.changeAmount = value), new GUIContent("Change Amount", "Shift in note, either up or down (-1 to 1)"));
			RenderSlider(+0, +1, parameters.changeSpeed, (value => parameters.changeSpeed = value), new GUIContent("Change Speed", "How fast the note shift happens (only happens once) (0 to 1)"));

			RenderHeading("Square Waves");
			RenderSlider(+0, +1, parameters.squareDuty, (value => parameters.squareDuty = value), new GUIContent("Square Duty", "Controls the ratio between the up and down states of the square wave, changing the tibre (0 to 1)"), isSquareWaveType);
			RenderSlider(-1, +1, parameters.dutySweep, (value => parameters.dutySweep = value), new GUIContent("Duty Sweep", "Sweeps the duty up or down (-1 to 1)"), isSquareWaveType);

			RenderHeading("Repeats");
			RenderSlider(+0, +1, parameters.repeatSpeed, (value => parameters.repeatSpeed = value), new GUIContent("Repeat Speed", "Speed of the note repeating - certain variables are reset each time (0 to 1)"));

			RenderHeading("Phaser");
			RenderSlider(-1, +1, parameters.phaserOffset, (value => parameters.phaserOffset = value), new GUIContent("Phaser Offset", "Offsets a second copy of the wave by a small phase, changing the tibre (-1 to 1)"));
			RenderSlider(-1, +1, parameters.phaserSweep, (value => parameters.phaserSweep = value), new GUIContent("Phaser Sweep", "Sweeps the phase up or down (-1 to 1)"));

			RenderHeading("Filters");
			RenderSlider(+0, +1, parameters.lpFilterCutoff, (value => parameters.lpFilterCutoff = value), new GUIContent("Low-Pass Cutoff", "Frequency at which the low-pass filter starts attenuating higher frequencies (0 to 1)"));
			RenderSlider(-1, +1, parameters.lpFilterCutoffSweep, (value => parameters.lpFilterCutoffSweep = value), new GUIContent("Low-Pass Cutoff Sweep", "Sweeps the low-pass cutoff up or down (-1 to 1)"));
			RenderSlider(+0, +1, parameters.lpFilterResonance, (value => parameters.lpFilterResonance = value), new GUIContent("Low-Pass Resonance", "Changes the attenuation rate for the low-pass filter, changing the timbre (0 to 1)"));
			RenderSlider(+0, +1, parameters.hpFilterCutoff, (value => parameters.hpFilterCutoff = value), new GUIContent("High-Pass Cutoff", "Frequency at which the high-pass filter starts attenuating lower frequencies (0 to 1)"));
			RenderSlider(-1, +1, parameters.hpFilterCutoffSweep, (value => parameters.hpFilterCutoffSweep = value), new GUIContent("High-Pass Cutoff Sweep", "Sweeps the high-pass cutoff up or down (-1 to 1)"));

			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();
			try
			{
				if (CanPlaySound())
				{
					GUILayoutOption buttonHeight = GUILayout.MinHeight(40.0f);
					RenderButton("Play Sound", () => PlaySound(parameters, false), null, buttonHeight);
				}
				else
				{
					EditorGUILayout.HelpBox("To play the sound, the editor must be playing the game.", MessageType.Info, true);
				}
			}
			finally
			{
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();
			RenderHeading("Tools");
			EditorGUILayout.BeginHorizontal();
			try
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.BeginVertical();
				try
				{
					if (RenderButton("Randomize"))
					{
						parameters.Randomize();
						PlaySound(parameters);
					}

					if (RenderButton("Mutate"))
					{
						parameters.Mutate();  // TODO: Allow for mutation-quantity parameter to be specified
						PlaySound(parameters);
					}
				}
				finally
				{
					EditorGUILayout.EndVertical();
				}

				GUILayout.FlexibleSpace();
			}
			finally
			{
				EditorGUILayout.EndHorizontal();
			}

			RenderHeading("Generators");
			EditorGUILayout.BeginHorizontal();
			try
			{
				GUILayout.FlexibleSpace();
				EditorGUILayout.BeginVertical();
				try
				{
					bool play = false;
					play = (RenderButton("Pickup Coin", parameters.GeneratePickupCoin) || play);
					play = (RenderButton("Laser Shoot", parameters.GenerateLaserShoot) || play);
					play = (RenderButton("Explosion", parameters.GenerateExplosion) || play);
					play = (RenderButton("Powerup", parameters.GeneratePowerup) || play);
					play = (RenderButton("Hit Hurt", parameters.GenerateHitHurt) || play);
					play = (RenderButton("Jump", parameters.GenerateJump) || play);
					play = (RenderButton("Blip Select", parameters.GenerateBlipSelect) || play);
					if (play)
					{
						PlaySound(parameters);
					}
				}
				finally
				{
					EditorGUILayout.EndVertical();
				}

				GUILayout.FlexibleSpace();
			}
			finally
			{
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.Space();
			RenderHeading("Clipboard");
			EditorGUILayout.BeginHorizontal();
			try
			{
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Copy Parameters"))
				{
					EditorGUIUtility.systemCopyBuffer = parameters.GetSettingsString();
				}

				if (GUILayout.Button("Paste Parameters"))
				{
					parameters.SetSettingsString(EditorGUIUtility.systemCopyBuffer);
					PlaySound(parameters);
				}

				GUILayout.FlexibleSpace();
			}
			finally
			{
				EditorGUILayout.EndHorizontal();
			}
		}
		finally
		{
			if (EditorGUI.EndChangeCheck())
			{
				parameters.paramsDirty = true;
			}
		}
	}

	/// <summary>
	/// Determines whether the editor can presently play a sound.
	/// </summary>
	/// <returns>
	///   <c>true</c> if a sound can be played using the PlaySound method; otherwise, <c>false</c>.
	/// </returns>
	public static bool CanPlaySound()
	{
		// TODO: Determine if/how sounds can be played without running the game
		return EditorApplication.isPlaying;
	}

	/// <summary>
	/// Plays the sound for the specified parameters.
	/// </summary>
	/// <param name="parameters">The parameters that details the sound to be generated.</param>
	/// <param name="skipIfUnvailable">When set to <c>true</c> and the sound cannot presently be played, simply skip this request.</param>
	/// <returns><c>true</c> if playing was initiated; otherwise, <c>false</c> when no sound was played.</returns>
	public static bool PlaySound(
		SfxrParams parameters, bool skipIfUnvailable = true)
	{
		if (skipIfUnvailable
			&& (!CanPlaySound()))
		{
			return false;  // Nothing was played
		}

		var synth = new SfxrSynth();
		synth.parameters.SetSettingsString(
			parameters.GetSettingsString());
		synth.Play();
		return true;  // Play was initiated
	}

	protected static void RenderHeading(string heading)
	{
		EditorGUILayout.LabelField(heading, EditorStyles.boldLabel);
	}

	protected static bool RenderButton(
		GUIContent content = null,
		Action valueChangeAction = null,
		bool? isEnabled = null,
		params GUILayoutOption[] options)
	{
		if (content == null)
		{
			content = GUIContent.none;
		}

		bool isClicked = false;
		return RenderGenericEditor(
			ref isClicked,
			() => GUILayout.Button(content, options),
			valueChangeAction,
			isEnabled);
	}

	protected static bool RenderButton(
		string text,
		Action valueChangeAction = null,
		bool? isEnabled = null,
		params GUILayoutOption[] options)
	{
		return RenderButton(
			new GUIContent(text), valueChangeAction, isEnabled, options);
	}

	protected static bool RenderPopup(
		GUIContent[] selectionOptions,
		int value,
		Action<int> valueChangeAction = null,
		GUIContent label = null,
		bool? isEnabled = null)
	{
		if (label == null)
		{
			label = GUIContent.none;
		}

		return RenderGenericEditor(
			ref value,
			() => EditorGUILayout.Popup(label, value, selectionOptions),
			valueChangeAction,
			isEnabled);
	}

	protected static bool RenderSlider(
		float minValue,
		float maxValue,
		float value,
		Action<float> valueChangeAction = null,
		GUIContent label = null,
		bool? isEnabled = null)
	{
		if (label == null)
		{
			label = GUIContent.none;
		}

		return RenderGenericEditor(
			ref value,
			() => EditorGUILayout.Slider(label, value, minValue, maxValue),
			valueChangeAction,
			isEnabled);
	}

	private static bool RenderGenericEditor<T>(
		ref T value,
		Func<T> valueEditFunction,
		Action<T> valueChangeAction = null,
		bool? isEnabled = null)
	{
		bool isChanged;
		if (valueEditFunction == null)
		{
			isChanged = false;
		}
		else
		{
			bool? wasEnabled;
			if (isEnabled.HasValue)
			{
				wasEnabled = GUI.enabled;
				GUI.enabled = isEnabled.Value;
			}
			else
			{
				wasEnabled = null;
			}

			try
			{
				EditorGUI.BeginChangeCheck();
				try
				{
					value = valueEditFunction();
				}
				finally
				{
					isChanged = EditorGUI.EndChangeCheck();
				}

				if (isChanged
					&& (valueChangeAction != null))
				{
					valueChangeAction(value);
				}
			}
			finally
			{
				if (wasEnabled.HasValue)
				{
					GUI.enabled = wasEnabled.Value;
				}
			}
		}

		return isChanged;
	}

	private static bool RenderGenericEditor<T>(
		ref T value,
		Func<T> valueEditFunction,
		Action valueChangeAction,
		bool? isEnabled = null)
	{
		Action<T> valueChangeActionWrapped = null;
		if (valueChangeAction != null)
		{
			valueChangeActionWrapped = (dummyValue) => valueChangeAction();
		}

		return RenderGenericEditor(
			ref value, valueEditFunction, valueChangeActionWrapped, isEnabled);
	}
}
