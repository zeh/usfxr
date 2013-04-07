using UnityEngine;

public class SfxrParams {

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
	 * SfxrParams
	 * Holds parameters used by SfxrSynth
	 * 
	 * @author Zeh Fernando
	 */

	// Properties
	public bool		paramsDirty;						// Whether If the parameters have been changed since last time (shouldn't used cached sound)

	private uint	_waveType				= 	0;		// Shape of the wave (0:square, 1:saw, 2:sin or 3:noise)

	private float	_masterVolume			=	0.5f;	// Overall volume of the sound (0 to 1)

	private float	_attackTime				=	0.0f;	// Length of the volume envelope attack (0 to 1)
	private float	_sustainTime			=	0.0f;	// Length of the volume envelope sustain (0 to 1)
	private float	_sustainPunch			=	0.0f;	// Tilts the sustain envelope for more 'pop' (0 to 1)
	private float	_decayTime				=	0.0f;	// Length of the volume envelope decay (yes, I know it's called release) (0 to 1)

	private float	_startFrequency			=	0.0f;	// Base note of the sound (0 to 1)
	private float	_minFrequency			=	0.0f;	// If sliding, the sound will stop at this frequency, to prevent really low notes (0 to 1)

	private float	_slide					=	0.0f;	// Slides the note up or down (-1 to 1)
	private float	_deltaSlide				=	0.0f;	// Accelerates the slide (-1 to 1)

	private float	_vibratoDepth			=	0.0f;	// Strength of the vibrato effect (0 to 1)
	private float	_vibratoSpeed			=	0.0f;	// Speed of the vibrato effect (i.e. frequency) (0 to 1)

	private float	_changeAmount			=	0.0f;	// Shift in note, either up or down (-1 to 1)
	private float	_changeSpeed			=	0.0f;	// How fast the note shift happens (only happens once) (0 to 1)

	private float	_squareDuty				=	0.0f;	// Controls the ratio between the up and down states of the square wave, changing the tibre (0 to 1)
	private float	_dutySweep				=	0.0f;	// Sweeps the duty up or down (-1 to 1)

	private float	_repeatSpeed			=	0.0f;	// Speed of the note repeating - certain variables are reset each time (0 to 1)

	private float	_phaserOffset			=	0.0f;	// Offsets a second copy of the wave by a small phase, changing the tibre (-1 to 1)
	private float	_phaserSweep			=	0.0f;	// Sweeps the phase up or down (-1 to 1)

	private float	_lpFilterCutoff			=	0.0f;	// Frequency at which the low-pass filter starts attenuating higher frequencies (0 to 1)
	private float	_lpFilterCutoffSweep	=	0.0f;	// Sweeps the low-pass cutoff up or down (-1 to 1)
	private float	_lpFilterResonance		=	0.0f;	// Changes the attenuation rate for the low-pass filter, changing the timbre (0 to 1)

	private float	_hpFilterCutoff			=	0.0f;	// Frequency at which the high-pass filter starts attenuating lower frequencies (0 to 1)
	private float	_hpFilterCutoffSweep	=	0.0f;	// Sweeps the high-pass cutoff up or down (-1 to 1)
	

	// ================================================================================================================
	// ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------

	/** Shape of the wave (0:square, 1:saw, 2:sin or 3:noise) */
	public uint waveType {
		get { return _waveType; }
		set { _waveType = value > 3 ? 0 : value; paramsDirty = true; }
	}

	/** Overall volume of the sound (0 to 1) */
	public float masterVolume {
		get { return _masterVolume; }
		set { _masterVolume = Clamp1(value); paramsDirty = true; }
	}

	/** Length of the volume envelope attack (0 to 1) */
	public float attackTime {
		get { return _attackTime; }
		set { _attackTime = Clamp1(value); paramsDirty = true; }
	}

	/** Length of the volume envelope sustain (0 to 1) */
	public float sustainTime {
		get { return _sustainTime; }
		set { _sustainTime = Clamp1(value); paramsDirty = true; }
	}

	/** Tilts the sustain envelope for more 'pop' (0 to 1) */
	public float sustainPunch {
		get { return _sustainPunch; }
		set { _sustainPunch = Clamp1(value); paramsDirty = true; }
	}

	/** Length of the volume envelope decay (yes, I know it's called release) (0 to 1) */
	public float decayTime {
		get { return _decayTime; }
		set { _decayTime = Clamp1(value); paramsDirty = true; }
	}

	/** Base note of the sound (0 to 1) */
	public float startFrequency {
		get { return _startFrequency; }
		set { _startFrequency = Clamp1(value); paramsDirty = true; }
	}

	/** If sliding, the sound will stop at this frequency, to prevent really low notes (0 to 1) */
	public float minFrequency {
		get { return _minFrequency; }
		set { _minFrequency = Clamp1(value); paramsDirty = true; }
	}

	/** Slides the note up or down (-1 to 1) */
	public float slide {
		get { return _slide; }
		set { _slide = Clamp2(value); paramsDirty = true; }
	}

	/** Accelerates the slide (-1 to 1) */
	public float deltaSlide {
		get { return _deltaSlide; }
		set { _deltaSlide = Clamp2(value); paramsDirty = true; }
	}

	/** Strength of the vibrato effect (0 to 1) */
	public float vibratoDepth {
		get { return _vibratoDepth; }
		set { _vibratoDepth = Clamp1(value); paramsDirty = true; }
	}

	/** Speed of the vibrato effect (i.e. frequency) (0 to 1) */
	public float vibratoSpeed {
		get { return _vibratoSpeed; }
		set { _vibratoSpeed = Clamp1(value); paramsDirty = true; }
	}

	/** Shift in note, either up or down (-1 to 1) */
	public float changeAmount {
		get { return _changeAmount; }
		set { _changeAmount = Clamp2(value); paramsDirty = true; }
	}

	/** How fast the note shift happens (only happens once) (0 to 1) */
	public float changeSpeed {
		get { return _changeSpeed; }
		set { _changeSpeed = Clamp1(value); paramsDirty = true; }
	}

	/** Controls the ratio between the up and down states of the square wave, changing the tibre (0 to 1) */
	public float squareDuty {
		get { return _squareDuty; }
		set { _squareDuty = Clamp1(value); paramsDirty = true; }
	}

	/** Sweeps the duty up or down (-1 to 1) */
	public float dutySweep {
		get { return _dutySweep; }
		set { _dutySweep = Clamp2(value); paramsDirty = true; }
	}

	/** Speed of the note repeating - certain variables are reset each time (0 to 1) */
	public float repeatSpeed {
		get { return _repeatSpeed; }
		set { _repeatSpeed = Clamp1(value); paramsDirty = true; }
	}

	/** Offsets a second copy of the wave by a small phase, changing the tibre (-1 to 1) */
	public float phaserOffset {
		get { return _phaserOffset; }
		set { _phaserOffset = Clamp2(value); paramsDirty = true; }
	}

	/** Sweeps the phase up or down (-1 to 1) */
	public float phaserSweep {
		get { return _phaserSweep; }
		set { _phaserSweep = Clamp2(value); paramsDirty = true; }
	}

	/** Frequency at which the low-pass filter starts attenuating higher frequencies (0 to 1) */
	public float lpFilterCutoff {
		get { return _lpFilterCutoff; }
		set { _lpFilterCutoff = Clamp1(value); paramsDirty = true; }
	}

	/** Sweeps the low-pass cutoff up or down (-1 to 1) */
	public float lpFilterCutoffSweep {
		get { return _lpFilterCutoffSweep; }
		set { _lpFilterCutoffSweep = Clamp2(value); paramsDirty = true; }
	}

	/** Changes the attenuation rate for the low-pass filter, changing the timbre (0 to 1) */
	public float lpFilterResonance {
		get { return _lpFilterResonance; }
		set { _lpFilterResonance = Clamp1(value); paramsDirty = true; }
	}

	/** Frequency at which the high-pass filter starts attenuating lower frequencies (0 to 1) */
	public float hpFilterCutoff {
		get { return _hpFilterCutoff; }
		set { _hpFilterCutoff = Clamp1(value); paramsDirty = true; }
	}

	/** Sweeps the high-pass cutoff up or down (-1 to 1) */
	public float hpFilterCutoffSweep {
		get { return _hpFilterCutoffSweep; }
		set { _hpFilterCutoffSweep = Clamp2(value); paramsDirty = true; }
	}


	// ================================================================================================================
	// PUBLIC INTERFACE -----------------------------------------------------------------------------------------------
	
	// Generator methods

	/**
	 * Sets the parameters to generate a pickup/coin sound
	 */
	public void GeneratePickupCoin() {
		resetParams();

		_startFrequency = 0.4f + getRandom() * 0.5f;

		_sustainTime = getRandom() * 0.1f;
		_decayTime = 0.1f + getRandom() * 0.4f;
		_sustainPunch = 0.3f + getRandom() * 0.3f;

		if (getRandom() < 0.5f) {
			_changeSpeed = 0.5f + getRandom() * 0.2f;
			_changeAmount = 0.2f + getRandom() * 0.4f;
		}
	}

	/**
	 * Sets the parameters to generate a laser/shoot sound
	 */
	public void GenerateLaserShoot() {
		resetParams();

		_waveType = (uint)(getRandom() * 3);
		if (_waveType == 2 && getRandom() < 0.5f) _waveType = (uint)(getRandom() * 2f);

		_startFrequency = 0.5f + getRandom() * 0.5f;
		_minFrequency = _startFrequency - 0.2f - getRandom() * 0.6f;
		if (_minFrequency < 0.2f) _minFrequency = 0.2f;

		_slide = -0.15f - getRandom() * 0.2f;

		if (getRandom() < 0.33f) {
			_startFrequency = 0.3f + getRandom() * 0.6f;
			_minFrequency = getRandom() * 0.1f;
			_slide = -0.35f - getRandom() * 0.3f;
		}

		if (getRandom() < 0.5f) {
			_squareDuty = getRandom() * 0.5f;
			_dutySweep = getRandom() * 0.2f;
		} else {
			_squareDuty = 0.4f + getRandom() * 0.5f;
			_dutySweep =- getRandom() * 0.7f;
		}

		_sustainTime = 0.1f + getRandom() * 0.2f;
		_decayTime = getRandom() * 0.4f;
		if (getRandom() < 0.5f) _sustainPunch = getRandom() * 0.3f;

		if (getRandom() < 0.33f) {
			_phaserOffset = getRandom() * 0.2f;
			_phaserSweep = -getRandom() * 0.2f;
		}

		if (getRandom() < 0.5f) _hpFilterCutoff = getRandom() * 0.3f;
	}

	/**
	 * Sets the parameters to generate an explosion sound
	 */
	public void GenerateExplosion() {
		resetParams();
		_waveType = 3;

		if (getRandom() < 0.5f) {
			_startFrequency = 0.1f + getRandom() * 0.4f;
			_slide = -0.1f + getRandom() * 0.4f;
		} else {
			_startFrequency = 0.2f + getRandom() * 0.7f;
			_slide = -0.2f - getRandom() * 0.2f;
		}

		_startFrequency *= _startFrequency;

		if (getRandom() < 0.2f) _slide = 0.0f;
		if (getRandom() < 0.33f) _repeatSpeed = 0.3f + getRandom() * 0.5f;

		_sustainTime = 0.1f + getRandom() * 0.3f;
		_decayTime = getRandom() * 0.5f;
		_sustainPunch = 0.2f + getRandom() * 0.6f;

		if (getRandom() < 0.5f) {
			_phaserOffset = -0.3f + getRandom() * 0.9f;
			_phaserSweep = -getRandom() * 0.3f;
		}

		if (getRandom() < 0.33f) {
			_changeSpeed = 0.6f + getRandom() * 0.3f;
			_changeAmount = 0.8f - getRandom() * 1.6f;
		}
	}

	/**
	 * Sets the parameters to generate a powerup sound
	 */
	public void GeneratePowerup() {
		resetParams();

		if (getRandom() < 0.5f) {
			_waveType = 1;
		} else {
			_squareDuty = getRandom() * 0.6f;
		}

		if (getRandom() < 0.5f) {
			_startFrequency = 0.2f + getRandom() * 0.3f;
			_slide = 0.1f + getRandom() * 0.4f;
			_repeatSpeed = 0.4f + getRandom() * 0.4f;
		} else {
			_startFrequency = 0.2f + getRandom() * 0.3f;
			_slide = 0.05f + getRandom() * 0.2f;

			if (getRandom() < 0.5f) {
				_vibratoDepth = getRandom() * 0.7f;
				_vibratoSpeed = getRandom() * 0.6f;
			}
		}

		_sustainTime = getRandom() * 0.4f;
		_decayTime = 0.1f + getRandom() * 0.4f;
	}

	/**
	 * Sets the parameters to generate a hit/hurt sound
	 */
	public void GenerateHitHurt() {
		resetParams();

		_waveType = (uint)(getRandom() * 3f);
		if (_waveType == 2) {
			_waveType = 3;
		} else if (_waveType == 0) {
			_squareDuty = getRandom() * 0.6f;
		}

		_startFrequency = 0.2f + getRandom() * 0.6f;
		_slide = -0.3f - getRandom() * 0.4f;

		_sustainTime = getRandom() * 0.1f;
		_decayTime = 0.1f + getRandom() * 0.2f;

		if (getRandom() < 0.5f) _hpFilterCutoff = getRandom() * 0.3f;
	}

	/**
	 * Sets the parameters to generate a jump sound
	 */
	public void GenerateJump() {
		resetParams();

		_waveType = 0;
		_squareDuty = getRandom() * 0.6f;
		_startFrequency = 0.3f + getRandom() * 0.3f;
		_slide = 0.1f + getRandom() * 0.2f;

		_sustainTime = 0.1f + getRandom() * 0.3f;
		_decayTime = 0.1f + getRandom() * 0.2f;

		if (getRandom() < 0.5f) _hpFilterCutoff = getRandom() * 0.3f;
		if (getRandom() < 0.5f) _lpFilterCutoff = 1.0f - getRandom() * 0.6f;
	}

	/**
	 * Sets the parameters to generate a blip/select sound
	 */
	public void GenerateBlipSelect() {
		resetParams();

		_waveType = (uint)(getRandom() * 2f);
		if (_waveType == 0) _squareDuty = getRandom() * 0.6f;

		_startFrequency = 0.2f + getRandom() * 0.4f;

		_sustainTime = 0.1f + getRandom() * 0.1f;
		_decayTime = getRandom() * 0.2f;
		_hpFilterCutoff = 0.1f;
	}

	/**
	 * Resets the parameters, used at the start of each generate function
	 */
	protected void resetParams() {
		paramsDirty = true;

		_waveType = 0;
		_startFrequency = 0.3f;
		_minFrequency = 0.0f;
		_slide = 0.0f;
		_deltaSlide = 0.0f;
		_squareDuty = 0.0f;
		_dutySweep = 0.0f;

		_vibratoDepth = 0.0f;
		_vibratoSpeed = 0.0f;

		_attackTime = 0.0f;
		_sustainTime = 0.3f;
		_decayTime = 0.4f;
		_sustainPunch = 0.0f;

		_lpFilterResonance = 0.0f;
		_lpFilterCutoff = 1.0f;
		_lpFilterCutoffSweep = 0.0f;
		_hpFilterCutoff = 0.0f;
		_hpFilterCutoffSweep = 0.0f;

		_phaserOffset = 0.0f;
		_phaserSweep = 0.0f;

		_repeatSpeed = 0.0f;

		_changeSpeed = 0.0f;
		_changeAmount = 0.0f;
	}

	
	// Randomization methods

	/**
	 * Randomly adjusts the parameters ever so slightly
	 */
	public void Mutate(float __mutation = 0.05f) {
		if (getRandom() < 0.5f) startFrequency += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) minFrequency += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) slide += 				getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) deltaSlide += 			getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) squareDuty += 			getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) dutySweep += 			getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) vibratoDepth += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) vibratoSpeed += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) attackTime += 			getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) sustainTime += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) decayTime += 			getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) sustainPunch += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) lpFilterCutoff += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) lpFilterCutoffSweep += getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) lpFilterResonance += 	getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) hpFilterCutoff += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) hpFilterCutoffSweep += getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) phaserOffset += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) phaserSweep += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) repeatSpeed += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) changeSpeed += 		getRandom() * __mutation * 2f - __mutation;
		if (getRandom() < 0.5f) changeAmount += 		getRandom() * __mutation * 2f - __mutation;
	}

	/**
	 * Sets all parameters to random values
	 */
	public void Randomize() {
		paramsDirty = true;

		_waveType = (uint)(getRandom() * 4f);

		_attackTime =  		Pow(getRandom()*2f-1f, 4);
		_sustainTime =  	Pow(getRandom()*2f-1f, 2);
		_sustainPunch =  	Pow(getRandom()*0.8f, 2);
		_decayTime =  		getRandom();

		_startFrequency =  	(getRandom() < 0.5f) ? Pow(getRandom()*2f-1f, 2) : (Pow(getRandom() * 0.5f, 3) + 0.5f);
		_minFrequency =  	0.0f;

		_slide =  			Pow(getRandom()*2f-1f, 5);
		_deltaSlide =  		Pow(getRandom()*2f-1f, 3);

		_vibratoDepth =  	Pow(getRandom()*2f-1f, 3);
		_vibratoSpeed =  	getRandom()*2f-1f;

		_changeAmount =  	getRandom()*2f-1f;
		_changeSpeed =  	getRandom()*2f-1f;

		_squareDuty =  		getRandom()*2f-1f;
		_dutySweep =  		Pow(getRandom()*2f-1f, 3);

		_repeatSpeed =  	getRandom()*2f-1f;

		_phaserOffset =  	Pow(getRandom()*2f-1f, 3);
		_phaserSweep =  	Pow(getRandom()*2f-1f, 3);

		_lpFilterCutoff =  		1f - Pow(getRandom(), 3);
		_lpFilterCutoffSweep = 	Pow(getRandom()*2f-1f, 3);
		_lpFilterResonance =  	getRandom()*2f-1f;

		_hpFilterCutoff =  		Pow(getRandom(), 5);
		_hpFilterCutoffSweep = 	Pow(getRandom()*2f-1f, 5);

		if (_attackTime + _sustainTime + _decayTime < 0.2f) {
			_sustainTime = 0.2f + getRandom() * 0.3f;
			_decayTime = 0.2f + getRandom() * 0.3f;
		}

		if ((_startFrequency > 0.7f && _slide > 0.2) || (_startFrequency < 0.2 && _slide < -0.05)) {
			_slide = -_slide;
		}

		if (_lpFilterCutoff < 0.1f && _lpFilterCutoffSweep < -0.05f) {
			_lpFilterCutoffSweep = -_lpFilterCutoffSweep;
		}
	}


	// Setting string methods
	
	/**
	 * Returns a string representation of the parameters for copy/paste sharing
	 * @return	A comma-delimited list of parameter values
	 */
	public string GetSettingsString() {
		string str = waveType.ToString();
		str += "," + To4DP(_attackTime) + 			"," + To4DP(_sustainTime)
			+ "," + To4DP(_sustainPunch) + 			"," + To4DP(_decayTime)
			+ "," + To4DP(_startFrequency) + 		"," + To4DP(_minFrequency)
			+ "," + To4DP(_slide) + 				"," + To4DP(_deltaSlide)
			+ "," + To4DP(_vibratoDepth) + 			"," + To4DP(_vibratoSpeed)
			+ "," + To4DP(_changeAmount) + 			"," + To4DP(_changeSpeed)
			+ "," + To4DP(_squareDuty) + 			"," + To4DP(_dutySweep)
			+ "," + To4DP(_repeatSpeed) + 			"," + To4DP(_phaserOffset)
			+ "," + To4DP(_phaserSweep) + 			"," + To4DP(_lpFilterCutoff)
			+ "," + To4DP(_lpFilterCutoffSweep) + 	"," + To4DP(_lpFilterResonance)
			+ "," + To4DP(_hpFilterCutoff)+ 		"," + To4DP(_hpFilterCutoffSweep)
			+ "," + To4DP(_masterVolume);

		return str;
	}

	/**
	 * Parses a settings string into the parameters
	 * @param	string	Settings string to parse
	 * @return			If the string successfully parsed
	 */
	public bool SetSettingsString(string __string) {
		string[] values = __string.Split(new char[] { ',' });

		if (values.Length != 24) return false;

		// [zeh] fix this
		waveType = 				ParseUint(values[0]);
		attackTime =  			ParseFloat(values[1]);
		sustainTime =  			ParseFloat(values[2]);
		sustainPunch =  		ParseFloat(values[3]);
		decayTime =  			ParseFloat(values[4]);
		startFrequency =  		ParseFloat(values[5]);
		minFrequency =  		ParseFloat(values[6]);
		slide =  				ParseFloat(values[7]);
		deltaSlide =  			ParseFloat(values[8]);
		vibratoDepth =  		ParseFloat(values[9]);
		vibratoSpeed =  		ParseFloat(values[10]);
		changeAmount =  		ParseFloat(values[11]);
		changeSpeed =  			ParseFloat(values[12]);
		squareDuty =  			ParseFloat(values[13]);
		dutySweep =  			ParseFloat(values[14]);
		repeatSpeed =  			ParseFloat(values[15]);
		phaserOffset =  		ParseFloat(values[16]);
		phaserSweep =  			ParseFloat(values[17]);
		lpFilterCutoff =  		ParseFloat(values[18]);
		lpFilterCutoffSweep =	ParseFloat(values[19]);
		lpFilterResonance =  	ParseFloat(values[20]);
		hpFilterCutoff =  		ParseFloat(values[21]);
		hpFilterCutoffSweep =	ParseFloat(values[22]);
		masterVolume = 			ParseFloat(values[23]);

		return true;
	}


	// Copying methods

	/**
	 * Returns a copy of this SfxrParams with all settings duplicated
	 * @return	A copy of this SfxrParams
	 */
	public SfxrParams Clone() {
		SfxrParams outp = new SfxrParams();
		outp.CopyFrom(this);

		return outp;
	}

	/**
	 * Copies parameters from another instance
	 * @param	params	Instance to copy parameters from
	 */
	public void CopyFrom(SfxrParams __params, bool __makeDirty = false) {
		_waveType = 			__params.waveType;
		_attackTime =           __params.attackTime;
		_sustainTime =          __params.sustainTime;
		_sustainPunch =         __params.sustainPunch;
		_decayTime =			__params.decayTime;
		_startFrequency = 		__params.startFrequency;
		_minFrequency = 		__params.minFrequency;
		_slide = 				__params.slide;
		_deltaSlide = 			__params.deltaSlide;
		_vibratoDepth = 		__params.vibratoDepth;
		_vibratoSpeed = 		__params.vibratoSpeed;
		_changeAmount = 		__params.changeAmount;
		_changeSpeed = 			__params.changeSpeed;
		_squareDuty = 			__params.squareDuty;
		_dutySweep = 			__params.dutySweep;
		_repeatSpeed = 			__params.repeatSpeed;
		_phaserOffset = 		__params.phaserOffset;
		_phaserSweep = 			__params.phaserSweep;
		_lpFilterCutoff = 		__params.lpFilterCutoff;
		_lpFilterCutoffSweep = 	__params.lpFilterCutoffSweep;
		_lpFilterResonance = 	__params.lpFilterResonance;
		_hpFilterCutoff = 		__params.hpFilterCutoff;
		_hpFilterCutoffSweep = 	__params.hpFilterCutoffSweep;
		_masterVolume = 		__params.masterVolume;

		if (__makeDirty) paramsDirty = true;
	}


	// Utility methods

	/**
	 * Clams a value to betwen 0 and 1
	 * @param	value	Input value
	 * @return			The value clamped between 0 and 1
	 */
	private float Clamp1(float value) {
		return (value > 1f) ? 1f : ((value < 0f) ? 0f : value);
	}

	/**
	 * Clams a value to betwen -1 and 1
	 * @param	value	Input value
	 * @return			The value clamped between -1 and 1
	 */
	private float Clamp2(float value) {
		return (value > 1f) ? 1f : ((value < -1f) ? -1f : value);
	}

	/**
	 * Quick power function
	 * @param	base		Base to raise to power
	 * @param	power		Power to raise base by
	 * @return				The calculated power
	 */
	private float Pow(float __pbase, int __power) {
		switch(__power) {
			case 2: return __pbase * __pbase;
			case 3: return __pbase * __pbase * __pbase;
			case 4: return __pbase * __pbase * __pbase * __pbase;
			case 5: return __pbase * __pbase * __pbase * __pbase * __pbase;
		}

		return 1f;
	}


	// ================================================================================================================
	// INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

	/**
	 * Returns the number as a string to 4 decimal places
	 * @param	value	Number to convert
	 * @return			Number to 4dp as a string
	 */
	private string To4DP(float __value) {
		if (__value < 0.0001f && __value > -0.0001f) return "";
		return __value.ToString("#.####");
	}

	/**
	 * Added by zeh - 2013 03 11 - temp?
	 */
	private uint ParseUint(string __value) {
		if (__value.Length == 0) return 0;
		return uint.Parse(__value);
	}

	/**
	 * Added by zeh - 2013 03 11 - temp?
	 */
	private float ParseFloat(string __value) {
		if (__value.Length == 0) return 0;
		return float.Parse(__value);
	}
	
	/**
	 * Returns a random value: 0 <= n < 1
	 * This needed to be created to follow the original code more strictly; Unity's Random.value returns 0 <= n <= 1
	 */
	private float getRandom() {
		return Random.value % 1;
	}
}