using System;
using UnityEngine;

public class SfxrSynth {

	/**
	 * SfxrSynth
	 *
	 * Copyright 2010 Thomas Vian
	 * Copyright 2013 Zeh Fernando
	 *
	 * Licensed under the Apache License, Version 2.0 (the "License");
	 * you may not use this file except in compliance with the License.
	 * You may obtain a copy of the License at
	 *
	 * 	http://www.apache.org/licenses/LICENSE-2.0
	 *
	 * Unless required by applicable law or agreed to in writing, software
	 * distributed under the License is distributed on an "AS IS" BASIS,
	 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	 * See the License for the specific language governing permissions and
	 * limitations under the License.
	 *
	 */

	/**
	 * SfxrSynth
	 * Generates and plays all necessary audio
	 * 
	 * @author Zeh Fernando
	 */
	
	// Enums
	
	public enum WaveType : int {
		Square = 0,
		Sawtooth = 1,
		Sine = 2,
		Noise = 3
	}

	
	// Sound properties
	private SfxrParams		_params = new SfxrParams();		// Params instance
	
	private GameObject		_gameObject;					// Game object that will contain the audio player script
	private SfxrAudioPlayer	_audioPlayer;					// Audio player script that will be attached to a GameObject to play the sound
	
	private Transform		_parentTransform;				// Parent that will contain the audio (for positional audio)

	private bool			_mutation;						// If the current sound playing or caching is a mutation

	private float[]			_cachedWave;					// Cached wave data from a cacheSound() call
	private uint			_cachedWavePos;					// Equivalent to _cachedWave.position in the old code
	private bool			_cachingNormal;					// If the synth is caching a normal sound

	private int				_cachingMutation;				// Current caching ID
	private float[]			_cachedMutation;				// Current caching wave data for mutation
	private uint			_cachedMutationPos;				// Equivalent to _cachedMutation.position in the old code
	private float[][]		_cachedMutations;				// Cached mutated wave data
	private uint			_cachedMutationsNum;			// Number of cached mutations
	private float			_cachedMutationAmount;			// Amount to mutate during cache

	private bool			_cachingAsync;					// If the synth is currently caching asynchronously

	private float[]			_waveData;						// Full wave, read out in chuncks by the onSampleData method
	private uint			_waveDataPos;					// Current position in the waveData

	private SfxrParams		_original;						// Copied properties for mutation base

	// Synth properies
	private bool		_finished;							// If the sound has finished

	private float		_masterVolume;						// masterVolume * masterVolume (for quick calculations)

	private uint		_waveType;							// The type of wave to generate (see enum WaveType)
	
	private float		_envelopeVolume;					// Current volume of the envelope
	private int			_envelopeStage;						// Current stage of the envelope (attack, sustain, decay, end)
	private float		_envelopeTime;						// Current time through current enelope stage
	private float		_envelopeLength;					// Length of the current envelope stage
	private float		_envelopeLength0;					// Length of the attack stage
	private float		_envelopeLength1;					// Length of the sustain stage
	private float		_envelopeLength2;					// Length of the decay stage
	private float		_envelopeOverLength0;				// 1 / _envelopeLength0 (for quick calculations)
	private float		_envelopeOverLength1;				// 1 / _envelopeLength1 (for quick calculations)
	private float		_envelopeOverLength2;				// 1 / _envelopeLength2 (for quick calculations)
	private uint		_envelopeFullLength;				// Full length of the volume envelop (and therefore sound)

	private float		_sustainPunch;						// The punch factor (louder at begining of sustain)

	private int			_phase;								// Phase through the wave
	private float		_pos;								// Phase expresed as a Number from 0-1, used for fast sin approx
	private float		_period;							// Period of the wave
	private float		_periodTemp;						// Period modified by vibrato
	private int			_periodTempInt;						// Period modified by vibrato (as an Int)
	private float		_maxPeriod;							// Maximum period before sound stops (from minFrequency)

	private float		_slide;								// Note slide
	private float		_deltaSlide;						// Change in slide
	private float		_minFrequency;						// Minimum frequency before stopping

	private float		_vibratoPhase;						// Phase through the vibrato sine wave
	private float		_vibratoSpeed;						// Speed at which the vibrato phase moves
	private float		_vibratoAmplitude;					// Amount to change the period of the wave by at the peak of the vibrato wave

	private float		_changeAmount;						// Amount to change the note by
	private int			_changeTime;						// Counter for the note change
	private int			_changeLimit;						// Once the time reaches this limit, the note changes

	private float		_squareDuty;						// Offset of center switching point in the square wave
	private float		_dutySweep;							// Amount to change the duty by

	private int			_repeatTime;						// Counter for the repeats
	private int			_repeatLimit;						// Once the time reaches this limit, some of the variables are reset

	private bool		_phaser;							// If the phaser is active
	private float		_phaserOffset;						// Phase offset for phaser effect
	private float		_phaserDeltaOffset;					// Change in phase offset
	private int			_phaserInt;							// Integer phaser offset, for bit maths
	private int			_phaserPos;							// Position through the phaser buffer
	private float[]		_phaserBuffer;						// Buffer of wave values used to create the out of phase second wave

	private bool		_filters;							// If the filters are active
	private float		_lpFilterPos;						// Adjusted wave position after low-pass filter
	private float		_lpFilterOldPos;					// Previous low-pass wave position
	private float		_lpFilterDeltaPos;					// Change in low-pass wave position, as allowed by the cutoff and damping
	private float		_lpFilterCutoff;					// Cutoff multiplier which adjusts the amount the wave position can move
	private float		_lpFilterDeltaCutoff;				// Speed of the low-pass cutoff multiplier
	private float		_lpFilterDamping;					// Damping muliplier which restricts how fast the wave position can move
	private bool		_lpFilterOn;						// If the low pass filter is active

	private float		_hpFilterPos;						// Adjusted wave position after high-pass filter
	private float		_hpFilterCutoff;					// Cutoff multiplier which adjusts the amount the wave position can move
	private float		_hpFilterDeltaCutoff;				// Speed of the high-pass cutoff multiplier

	private float[]		_noiseBuffer;						// Buffer of random values used to generate noise

	private float		_superSample;						// Actual sample writen to the wave
	private float		_sample;							// Sub-sample calculated 8 times per actual sample, averaged out to get the super sample

	private System.Random randomGenerator = new System.Random();


	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	/** The sound parameters */
	public SfxrParams parameters {
		get { return _params; }
		set { _params = value; _params.paramsDirty = true; }
	}

	
	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------

	/**
	 * Plays the sound. If the parameters are dirty, synthesises sound as it plays, caching it for later.
	 * If they're not, plays from the cached sound.
	 * Won't play if caching asynchronously.
	 */
	public void Play() {
		if (_cachingAsync) return;

		Stop();

		_mutation = false;

		if (_params.paramsDirty || _cachingNormal || _cachedWave == null) {
			// Needs to cache new data
			_cachedWavePos = 0;
			_cachingNormal = true;
			_waveData = null;
			Reset(true);
			_cachedWave = new float[_envelopeFullLength];
		} else {
			// Play from cached data
			_waveData = _cachedWave;
			_waveDataPos = 0;
		}

		createGameObject();
		
	}
	
	/**
	 * Plays a mutation of the sound.  If the parameters are dirty, synthesises sound as it plays, caching it for later.
	 * If they're not, plays from the cached sound.
	 * Won't play if caching asynchronously.
	 * @param	mutationAmount	Amount of mutation
	 * @param	mutationsNum	The number of mutations to cache before picking from them
	 */
	public void PlayMutated(float __mutationAmount = 0.05f, uint __mutationsNum = 15) {
		Stop();

		if (_cachingAsync) return;

		_mutation = true;

		_cachedMutationsNum = __mutationsNum;

		if (_params.paramsDirty || _cachedMutations == null) {
			// New set of mutations
			_cachedMutations = new float[_cachedMutationsNum][];
			_cachingMutation = 0;
		}

		if (_cachingMutation != -1) {
			// Continuing caching new mutations
			Reset(true); // To get _envelopeFullLength

			_cachedMutation = new float[_envelopeFullLength];
			_cachedMutationPos = 0;
			_cachedMutations[_cachingMutation] = _cachedMutation;
			_waveData = null;

			_original = _params.Clone();
			_params.Mutate(__mutationAmount);

			Reset(true);
		} else {
			// Play from random cached mutation
			_waveData = _cachedMutations[(uint)(_cachedMutations.Length * getRandom())];
			_waveDataPos = 0;
		}
		
		createGameObject();
	}

	/**
	 * Stops the currently playing sound
	 */
	public void Stop() {
		if (_audioPlayer != null) {
			_audioPlayer.Destroy();
			_audioPlayer = null;
		}

		if (_original != null) {
			_params.CopyFrom(_original);
			_original = null;
		}
	}

	private int WriteSamples(float[] __originSamples, int __originPos, float[] __targetSamples, int __targetChannels) {
		// Writes raw samples to Unity's format and return number of samples actually written
		int samplesToWrite = __targetSamples.Length / __targetChannels;

		if (__originPos + samplesToWrite > __originSamples.Length) samplesToWrite = __originSamples.Length - __originPos;

		if (samplesToWrite > 0) {
			// Interlaced filling of sample datas (faster?)
			int i, j;
			for (i = 0; i < __targetChannels; i++) {
				for (j = 0; j < samplesToWrite; j++) {
					__targetSamples[(j * __targetChannels) + i] = __originSamples[j + __originPos];
				}
			}
		}

		return samplesToWrite;
	}

	/**
	 * If there is a cached sound to play, reads out of the data.
	 * If there isn't, synthesises new chunch of data, caching it as it goes.
	 * @param	data		Float[] to write data to
	 * @param	channels	Number of channels used
	 * @return	Whether it needs to continue (there are samples left) or not
	 */
	public bool GenerateAudioFilterData(float[] __data, int __channels) {
		bool endOfSamples = false;

		if (_waveData != null) {
			int samplesWritten = WriteSamples(_waveData, (int)_waveDataPos, __data, __channels);
			_waveDataPos += (uint)samplesWritten;
			if (samplesWritten == 0) endOfSamples = true;
		} else {
			if (_mutation) {
				if (_original != null) {
					_waveDataPos = _cachedMutationPos;

					int samplesNeeded = (int)Mathf.Min((__data.Length / __channels), _cachedMutation.Length - _cachedMutationPos);

					if (SynthWave(_cachedMutation, (int)_cachedMutationPos, (uint)samplesNeeded) || samplesNeeded == 0) {
						// Finished
						_params.CopyFrom(_original);
						_original = null;

						_cachingMutation++;

						endOfSamples = true;

						if (_cachingMutation >= _cachedMutationsNum) _cachingMutation = -1;
					} else {
						_cachedMutationPos += (uint)samplesNeeded;
					}

					WriteSamples(_cachedMutation, (int)_waveDataPos, __data, __channels);
				}
			} else {
				if (_cachingNormal) {
					_waveDataPos = _cachedWavePos;

					int samplesNeeded = (int)Mathf.Min((__data.Length / __channels), _cachedWave.Length - _cachedWavePos);

					if (SynthWave(_cachedWave, (int)_cachedWavePos, (uint)samplesNeeded) || samplesNeeded == 0) {
						_cachingNormal = false;
						endOfSamples = true;
					} else {
						_cachedWavePos += (uint)samplesNeeded;
					}

					WriteSamples(_cachedWave, (int)_waveDataPos, __data, __channels);
				}
			}
		}

		return !endOfSamples;
	}
	
	
	// Cache sound methods

	/**
	 * Cache the sound for speedy playback.
	 * If a callback is passed in, the caching will be done asynchronously, taking maxTimePerFrame milliseconds
	 * per frame to cache, them calling the callback when it's done.
	 * If not, the whole sound is cached immediately - can freeze the player for a few seconds, especially in debug mode.
	 * @param	callback			Function to call when the caching is complete
	 * @param	maxTimePerFrame		Maximum time in milliseconds the caching will use per frame
	 */
	public void CacheSound(Action __callback = null, bool __isFromCoroutine = false) {
		Stop();

		if (_cachingAsync && !__isFromCoroutine) return;

		if (__callback != null) {
			_mutation = false;
			_cachingNormal = true;
			_cachingAsync = true;

			GameObject _surrogateObj = new GameObject("SfxrGameObjectSurrogate-" + (Time.realtimeSinceStartup));
			SfxrCacheSurrogate _surrogate = _surrogateObj.AddComponent<SfxrCacheSurrogate>();
			_surrogate.CacheSound(this, __callback);
		} else {
			Reset(true);

			_cachedWave = new float[_envelopeFullLength];

			SynthWave(_cachedWave, 0, _envelopeFullLength);

			_cachingNormal = false;
			_cachingAsync = false;
		}
	}

	/**
	 * Caches a series of mutations on the source sound.
	 * If a callback is passed in, the caching will be done asynchronously, taking maxTimePerFrame milliseconds
	 * per frame to cache, them calling the callback when it's done.
	 * If not, the whole sound is cached immediately - can freeze the player for a few seconds, especially in debug mode.
	 * @param	mutationsNum		Number of mutations to cache
	 * @param	mutationAmount		Amount of mutation
	 * @param	callback			Function to call when the caching is complete
	 * @param	maxTimePerFrame		Maximum time in milliseconds the caching will use per frame
	 */
	public void CacheMutations(uint __mutationsNum = 15, float __mutationAmount = 0.05f, Action __callback = null, bool __isFromCoroutine = false) {
		Stop();

		if (_cachingAsync && !__isFromCoroutine) return;

		_cachedMutationsNum = __mutationsNum;
		_cachedMutations = new float[_cachedMutationsNum][];

		if (__callback != null) {
			_mutation = true;
			_cachingAsync = true;

			GameObject _surrogateObj = new GameObject("SfxrGameObjectSurrogate-" + (Time.realtimeSinceStartup));
			SfxrCacheSurrogate _surrogate = _surrogateObj.AddComponent<SfxrCacheSurrogate>();
			_surrogate.CacheMutations(this, __mutationsNum, __mutationAmount, __callback);
		} else {
			Reset(true);

			SfxrParams original = _params.Clone();

			for (uint i = 0; i < _cachedMutationsNum; i++) {
				_params.Mutate(__mutationAmount);
				CacheSound();
				_cachedMutations[i] = _cachedWave;
				_params.CopyFrom(original);
			}

			_cachingAsync = false;
			_cachingMutation = -1;
		}
	}

	/**
	 * Sets the parent transform of this audio, for positional audio
	 * @param	__transform		The transform object of the parent
	 */
	public void SetParentTransform(Transform __transform) {
		_parentTransform = __transform;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	/**
	 * Resets the runing variables from the params
	 * Used once at the start (total reset) and for the repeat effect (partial reset)
	 * @param	totalReset	If the reset is total
	 */
	private void Reset(bool __totalReset) {
		// Shorter reference
		SfxrParams p = _params;

		_period = 100.0f / (p.startFrequency * p.startFrequency + 0.001f);
		_maxPeriod = 100.0f / (p.minFrequency * p.minFrequency + 0.001f);

		_slide = 1.0f - p.slide * p.slide * p.slide * 0.01f;
		_deltaSlide = -p.deltaSlide * p.deltaSlide * p.deltaSlide * 0.000001f;

		if (p.waveType == WaveType.Square) {
			_squareDuty = 0.5f - p.squareDuty * 0.5f;
			_dutySweep = -p.dutySweep * 0.00005f;
		}

		if (p.changeAmount > 0.0) {
			_changeAmount = 1.0f - p.changeAmount * p.changeAmount * 0.9f;
		} else {
			_changeAmount = 1.0f + p.changeAmount * p.changeAmount * 10.0f;
		}

		_changeTime = 0;

		if (p.changeSpeed == 1.0f) {
			_changeLimit = 0;
		} else {
			_changeLimit = (int)((1f - p.changeSpeed) * (1f - p.changeSpeed) * 20000f + 32f);
		}

		if (__totalReset) {
			p.paramsDirty = false;

			_masterVolume = p.masterVolume * p.masterVolume;

			_waveType = p.waveType;

			if (p.sustainTime < 0.01) p.sustainTime = 0.01f;

			float totalTime = p.attackTime + p.sustainTime + p.decayTime;
			if (totalTime < 0.18f) {
				float multiplier = 0.18f / totalTime;
				p.attackTime *= multiplier;
				p.sustainTime *= multiplier;
				p.decayTime *= multiplier;
			}

			_sustainPunch = p.sustainPunch;

			_phase = 0;

			_minFrequency = p.minFrequency;

			_filters = p.lpFilterCutoff != 1.0 || p.hpFilterCutoff != 0.0;

			_lpFilterPos = 0.0f;
			_lpFilterDeltaPos = 0.0f;
			_lpFilterCutoff = p.lpFilterCutoff * p.lpFilterCutoff * p.lpFilterCutoff * 0.1f;
			_lpFilterDeltaCutoff = 1.0f + p.lpFilterCutoffSweep * 0.0001f;
			_lpFilterDamping = 5.0f / (1.0f + p.lpFilterResonance * p.lpFilterResonance * 20.0f) * (0.01f + _lpFilterCutoff);
			if (_lpFilterDamping > 0.8f) _lpFilterDamping = 0.8f;
			_lpFilterDamping = 1.0f - _lpFilterDamping;
			_lpFilterOn = p.lpFilterCutoff != 1.0f;

			_hpFilterPos = 0.0f;
			_hpFilterCutoff = p.hpFilterCutoff * p.hpFilterCutoff * 0.1f;
			_hpFilterDeltaCutoff = 1.0f + p.hpFilterCutoffSweep * 0.0003f;

			_vibratoPhase = 0.0f;
			_vibratoSpeed = p.vibratoSpeed * p.vibratoSpeed * 0.01f;
			_vibratoAmplitude = p.vibratoDepth * 0.5f;

			_envelopeVolume = 0.0f;
			_envelopeStage = 0;
			_envelopeTime = 0;
			_envelopeLength0 = p.attackTime * p.attackTime * 100000.0f;
			_envelopeLength1 = p.sustainTime * p.sustainTime * 100000.0f;
			_envelopeLength2 = p.decayTime * p.decayTime * 100000.0f + 10f;
			_envelopeLength = _envelopeLength0;
			_envelopeFullLength = (uint)(_envelopeLength0 + _envelopeLength1 + _envelopeLength2);

			_envelopeOverLength0 = 1.0f / _envelopeLength0;
			_envelopeOverLength1 = 1.0f / _envelopeLength1;
			_envelopeOverLength2 = 1.0f / _envelopeLength2;

			_phaser = p.phaserOffset != 0.0f || p.phaserSweep != 0.0f;

			_phaserOffset = p.phaserOffset * p.phaserOffset * 1020.0f;
			if (p.phaserOffset < 0.0f) _phaserOffset = -_phaserOffset;
			_phaserDeltaOffset = p.phaserSweep * p.phaserSweep * p.phaserSweep * 0.2f;
			_phaserPos = 0;

			if (_phaserBuffer == null) _phaserBuffer = new float[1024];
			if (_noiseBuffer == null) _noiseBuffer = new float[32];

			uint i;
			for (i = 0; i < 1024; i++) _phaserBuffer[i] = 0.0f;
			for (i = 0; i < 32; i++) _noiseBuffer[i] = getRandom() * 2.0f - 1.0f;

			_repeatTime = 0;

			if (p.repeatSpeed == 0.0) {
				_repeatLimit = 0;
			} else {
				_repeatLimit = (int)((1.0-p.repeatSpeed) * (1.0-p.repeatSpeed) * 20000) + 32;
			}
		}
	}

	/**
	 * Writes the wave to the supplied buffer array of floats (it'll contain the mono audio)
	 * @param	buffer		A float[] to write the wave to
	 * @param	waveData	If the wave should be written for the waveData
	 * @return				If the wave is finished
	 */
	private bool SynthWave(float[] __buffer, int __bufferPos, uint __length) {
		_finished = false;

		int i, j, n;
		int l = (int)__length;
		
		for (i = 0; i < l; i++) {
			if (_finished) return true;

			// Repeats every _repeatLimit times, partially resetting the sound parameters
			if (_repeatLimit != 0) {
				if (++_repeatTime >= _repeatLimit) {
					_repeatTime = 0;
					Reset(false);
				}
			}

			// If _changeLimit is reached, shifts the pitch
			if (_changeLimit != 0) {
				if (++_changeTime >= _changeLimit) {
					_changeLimit = 0;
					_period *= _changeAmount;
				}
			}

			// Acccelerate and apply slide
			_slide += _deltaSlide;
			_period *= _slide;

			// Checks for frequency getting too low, and stops the sound if a minFrequency was set
			if (_period > _maxPeriod) {
				_period = _maxPeriod;
				if (_minFrequency > 0) _finished = true;
			}

			_periodTemp = _period;

			// Applies the vibrato effect
			if (_vibratoAmplitude > 0) {
				_vibratoPhase += _vibratoSpeed;
				_periodTemp = _period * (1.0f + Mathf.Sin(_vibratoPhase) * _vibratoAmplitude);
			}

			_periodTempInt = (int)_periodTemp;
			if (_periodTempInt < 8) _periodTemp = _periodTempInt = 8;

			// Sweeps the square duty
			if (_waveType == WaveType.Square) {
				_squareDuty += _dutySweep;
				if (_squareDuty < 0.0) {
					_squareDuty = 0.0f;
				} else if (_squareDuty > 0.5) {
					_squareDuty = 0.5f;
				}
			}

			// Moves through the different stages of the volume envelope
			if (++_envelopeTime > _envelopeLength) {
				_envelopeTime = 0;

				switch(++_envelopeStage) {
					case 1: _envelopeLength = _envelopeLength1; break;
					case 2: _envelopeLength = _envelopeLength2; break;
				}
			}

			// Sets the volume based on the position in the envelope
			switch(_envelopeStage) {
				case 0: _envelopeVolume = _envelopeTime * _envelopeOverLength0; 										break;
				case 1: _envelopeVolume = 1.0f + (1.0f - _envelopeTime * _envelopeOverLength1) * 2.0f * _sustainPunch;	break;
				case 2: _envelopeVolume = 1.0f - _envelopeTime * _envelopeOverLength2; 									break;
				case 3: _envelopeVolume = 0.0f; _finished = true; 														break;
			}

			// Moves the phaser offset
			if (_phaser) {
				_phaserOffset += _phaserDeltaOffset;
				_phaserInt = (int)_phaserOffset;
				if (_phaserInt < 0) {
					_phaserInt = -_phaserInt;
				} else if (_phaserInt > 1023) {
					_phaserInt = 1023;
				}
			}

			// Moves the high-pass filter cutoff
			if (_filters && _hpFilterDeltaCutoff != 0) {
				_hpFilterCutoff *= _hpFilterDeltaCutoff;
				if (_hpFilterCutoff < 0.00001f) {
					_hpFilterCutoff = 0.00001f;
				} else if (_hpFilterCutoff > 0.1f) {
					_hpFilterCutoff = 0.1f;
				}
			}

			_superSample = 0;
			for (j = 0; j < 8; j++) {
				// Cycles through the period
				_phase++;
				if (_phase >= _periodTempInt) {
					_phase = _phase % _periodTempInt;

					// Generates new random noise for this period
					if (_waveType == WaveType.Noise) {
						for (n = 0; n < 32; n++) _noiseBuffer[n] = getRandom() * 2.0f - 1.0f;
					}
				}

				// Gets the sample from the oscillator
				switch(_waveType) {
					case WaveType.Square: // Square wave
						_sample = ((_phase / _periodTemp) < _squareDuty) ? 0.5f : -0.5f;
						break;
					case WaveType.Sawtooth: // Saw wave
						_sample = 1.0f - (_phase / _periodTemp) * 2.0f;
						break;
					case WaveType.Sine: // Sine wave (fast and accurate approx) {
						_pos = _phase / _periodTemp;
						_pos = _pos > 0.5f ? (_pos - 1.0f) * 6.28318531f : _pos * 6.28318531f;
						_sample = _pos < 0 ? 1.27323954f * _pos + 0.405284735f * _pos * _pos : 1.27323954f * _pos - 0.405284735f * _pos * _pos;
						_sample = _sample < 0 ? 0.225f * (_sample *-_sample - _sample) + _sample : 0.225f * (_sample * _sample - _sample) + _sample;
						break;
					case WaveType.Noise: // Noise
						_sample = _noiseBuffer[(uint)(_phase * 32 / _periodTempInt)];
						break;
				}

				// Applies the low and high pass filters
				if (_filters) {
					_lpFilterOldPos = _lpFilterPos;
					_lpFilterCutoff *= _lpFilterDeltaCutoff;
					if (_lpFilterCutoff < 0.0) {
						_lpFilterCutoff = 0.0f;
					} else if (_lpFilterCutoff > 0.1) {
						_lpFilterCutoff = 0.1f;
					}

					if (_lpFilterOn) {
						_lpFilterDeltaPos += (_sample - _lpFilterPos) * _lpFilterCutoff;
						_lpFilterDeltaPos *= _lpFilterDamping;
					} else {
						_lpFilterPos = _sample;
						_lpFilterDeltaPos = 0.0f;
					}

					_lpFilterPos += _lpFilterDeltaPos;

					_hpFilterPos += _lpFilterPos - _lpFilterOldPos;
					_hpFilterPos *= 1.0f - _hpFilterCutoff;
					_sample = _hpFilterPos;
				}

				// Applies the phaser effect
				if (_phaser) {
					_phaserBuffer[_phaserPos & 1023] = _sample;
					_sample += _phaserBuffer[(_phaserPos - _phaserInt + 1024) & 1023];
					_phaserPos = (_phaserPos + 1) & 1023;
				}

				_superSample += _sample;
			}

			// Averages out the super samples and applies volumes
			_superSample = _masterVolume * _envelopeVolume * _superSample * 0.125f;

			// Clipping if too loud
			if (_superSample < -1f) {
				_superSample = -1f;
			} else if (_superSample > 1f) {
				_superSample = 1f;
			}

			// Writes value to list, ignoring left/right sound channels (this is applied when filtering the audio later)
			__buffer[i + __bufferPos] = _superSample;
		}

		return false;
	}
	
	private void createGameObject() {
		// Create a game object to handle playback
		_gameObject = new GameObject("SfxrGameObject-" + (Time.realtimeSinceStartup));
		fixGameObjectParent();

		// Create actual audio player
    	_audioPlayer = _gameObject.AddComponent<SfxrAudioPlayer>();
		_audioPlayer.SetSfxrSynth(this);
		_audioPlayer.SetRunningInEditMode(Application.isEditor && !Application.isPlaying);
	}


	private void fixGameObjectParent() {
		// Sets the parent of the game object to be the wanted object
		Transform transformToUse = _parentTransform;
		
		// If no parent assigned, use main camera by default
		if (transformToUse == null) transformToUse = Camera.main.transform;
		
		// If has any parent (assigned, or main camera exist) assigns it
		if (transformToUse != null) _gameObject.transform.parent = transformToUse;
		
		// Center in parent (or scene if no parent)
		_gameObject.transform.localPosition = new Vector3(0, 0, 0);
	}

	/**
	 * Returns a random value: 0 <= n < 1
	 * This function is needed so we can follow the original code more strictly; Unity's Random.value returns 0 <= n <= 1
	 */
	private float getRandom() {
		// We can't use Unity's Random.value because it cannot be called from a separate thread
		// (We get the error "get_value can only be called from the main thread" when this is called to generate the soung data)
		return (float)(randomGenerator.NextDouble() % 1);
	}

}

