using UnityEngine;

public class SfxrParams {

	/**
	 * SfxrSynth
	 *
	 * Copyright 2013 Thomas Vian, Zeh Fernando
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
	 * @author Zeh Fernando
	 */

	//--------------------------------------------------------------------------
	//
	//  Properties
	//
	//--------------------------------------------------------------------------

	/** If the parameters have been changed since last time (shouldn't used cached sound) */
	public bool		paramsDirty;

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

	//--------------------------------------------------------------------------
	//
	//  Getters / Setters
	//
	//--------------------------------------------------------------------------

	/** Shape of the wave (0:square, 1:saw, 2:sin or 3:noise) */
	public uint waveType {
		get { return _waveType; }
		set { _waveType = value > 3 ? 0 : value; paramsDirty = true; }
	}

	/** Overall volume of the sound (0 to 1) */
	public float masterVolume {
		get { return _masterVolume; }
		set { _masterVolume = clamp1(value); paramsDirty = true; }
	}

	/** Length of the volume envelope attack (0 to 1) */
	public float attackTime {
		get { return _attackTime; }
		set { _attackTime = clamp1(value); paramsDirty = true; }
	}

	/** Length of the volume envelope sustain (0 to 1) */
	public float sustainTime {
		get { return _sustainTime; }
		set { _sustainTime = clamp1(value); paramsDirty = true; }
	}

	/** Tilts the sustain envelope for more 'pop' (0 to 1) */
	public float sustainPunch {
		get { return _sustainPunch; }
		set { _sustainPunch = clamp1(value); paramsDirty = true; }
	}

	/** Length of the volume envelope decay (yes, I know it's called release) (0 to 1) */
	public float decayTime {
		get { return _decayTime; }
		set { _decayTime = clamp1(value); paramsDirty = true; }
	}

	/** Base note of the sound (0 to 1) */
	public float startFrequency {
		get { return _startFrequency; }
		set { _startFrequency = clamp1(value); paramsDirty = true; }
	}

	/** If sliding, the sound will stop at this frequency, to prevent really low notes (0 to 1) */
	public float minFrequency {
		get { return _minFrequency; }
		set { _minFrequency = clamp1(value); paramsDirty = true; }
	}

	/** Slides the note up or down (-1 to 1) */
	public float slide {
		get { return _slide; }
		set { _slide = clamp2(value); paramsDirty = true; }
	}

	/** Accelerates the slide (-1 to 1) */
	public float deltaSlide {
		get { return _deltaSlide; }
		set { _deltaSlide = clamp2(value); paramsDirty = true; }
	}

	/** Strength of the vibrato effect (0 to 1) */
	public float vibratoDepth {
		get { return _vibratoDepth; }
		set { _vibratoDepth = clamp1(value); paramsDirty = true; }
	}

	/** Speed of the vibrato effect (i.e. frequency) (0 to 1) */
	public float vibratoSpeed {
		get { return _vibratoSpeed; }
		set { _vibratoSpeed = clamp1(value); paramsDirty = true; }
	}

	/** Shift in note, either up or down (-1 to 1) */
	public float changeAmount {
		get { return _changeAmount; }
		set { _changeAmount = clamp2(value); paramsDirty = true; }
	}

	/** How fast the note shift happens (only happens once) (0 to 1) */
	public float changeSpeed {
		get { return _changeSpeed; }
		set { _changeSpeed = clamp1(value); paramsDirty = true; }
	}

	/** Controls the ratio between the up and down states of the square wave, changing the tibre (0 to 1) */
	public float squareDuty {
		get { return _squareDuty; }
		set { _squareDuty = clamp1(value); paramsDirty = true; }
	}

	/** Sweeps the duty up or down (-1 to 1) */
	public float dutySweep {
		get { return _dutySweep; }
		set { _dutySweep = clamp2(value); paramsDirty = true; }
	}

	/** Speed of the note repeating - certain variables are reset each time (0 to 1) */
	public float repeatSpeed {
		get { return _repeatSpeed; }
		set { _repeatSpeed = clamp1(value); paramsDirty = true; }
	}

	/** Offsets a second copy of the wave by a small phase, changing the tibre (-1 to 1) */
	public float phaserOffset {
		get { return _phaserOffset; }
		set { _phaserOffset = clamp2(value); paramsDirty = true; }
	}

	/** Sweeps the phase up or down (-1 to 1) */
	public float phaserSweep {
		get { return _phaserSweep; }
		set { _phaserSweep = clamp2(value); paramsDirty = true; }
	}

	/** Frequency at which the low-pass filter starts attenuating higher frequencies (0 to 1) */
	public float lpFilterCutoff {
		get { return _lpFilterCutoff; }
		set { _lpFilterCutoff = clamp1(value); paramsDirty = true; }
	}

	/** Sweeps the low-pass cutoff up or down (-1 to 1) */
	public float lpFilterCutoffSweep {
		get { return _lpFilterCutoffSweep; }
		set { _lpFilterCutoffSweep = clamp2(value); paramsDirty = true; }
	}

	/** Changes the attenuation rate for the low-pass filter, changing the timbre (0 to 1) */
	public float lpFilterResonance {
		get { return _lpFilterResonance; }
		set { _lpFilterResonance = clamp1(value); paramsDirty = true; }
	}

	/** Frequency at which the high-pass filter starts attenuating lower frequencies (0 to 1) */
	public float hpFilterCutoff {
		get { return _hpFilterCutoff; }
		set { _hpFilterCutoff = clamp1(value); paramsDirty = true; }
	}

	/** Sweeps the high-pass cutoff up or down (-1 to 1) */
	public float hpFilterCutoffSweep {
		get { return _hpFilterCutoffSweep; }
		set { _hpFilterCutoffSweep = clamp2(value); paramsDirty = true; }
	}

	//--------------------------------------------------------------------------
	//
	//  Generator Methods
	//
	//--------------------------------------------------------------------------

	/**
	 * Sets the parameters to generate a pickup/coin sound
	 */
	public void generatePickupCoin() {
		resetParams();
	
		_startFrequency = 0.4f + Random.value * 0.5f;
	
		_sustainTime = Random.value * 0.1f;
		_decayTime = 0.1f + Random.value * 0.4f;
		_sustainPunch = 0.3f + Random.value * 0.3f;
	
		if (Random.value < 0.5f) {
			_changeSpeed = 0.5f + Random.value * 0.2f;
			_changeAmount = 0.2f + Random.value * 0.4f;
		}
	}

	/**
	 * Sets the parameters to generate a laser/shoot sound
	 */
	public void generateLaserShoot() {
		resetParams();
	
		_waveType = (uint)(Random.value * 3);
		if (_waveType == 2 && Random.value < 0.5f) _waveType = (uint)(Random.value * 2f);
	
		_startFrequency = 0.5f + Random.value * 0.5f;
		_minFrequency = _startFrequency - 0.2f - Random.value * 0.6f;
		if (_minFrequency < 0.2f) _minFrequency = 0.2f;
	
		_slide = -0.15f - Random.value * 0.2f;
	
		if (Random.value < 0.33f) {
			_startFrequency = 0.3f + Random.value * 0.6f;
			_minFrequency = Random.value * 0.1f;
			_slide = -0.35f - Random.value * 0.3f;
		}
	
		if (Random.value < 0.5f) {
			_squareDuty = Random.value * 0.5f;
			_dutySweep = Random.value * 0.2f;
		} else {
			_squareDuty = 0.4f + Random.value * 0.5f;
			_dutySweep =- Random.value * 0.7f;
		}
	
		_sustainTime = 0.1f + Random.value * 0.2f;
		_decayTime = Random.value * 0.4f;
		if (Random.value < 0.5f) _sustainPunch = Random.value * 0.3f;
	
		if (Random.value < 0.33f) {
			_phaserOffset = Random.value * 0.2f;
			_phaserSweep = -Random.value * 0.2f;
		}
	
		if (Random.value < 0.5f) _hpFilterCutoff = Random.value * 0.3f;
	}

	/**
	 * Sets the parameters to generate an explosion sound
	 */
	public void generateExplosion() {
		resetParams();
		_waveType = 3;
	
		if (Random.value < 0.5f) {
			_startFrequency = 0.1f + Random.value * 0.4f;
			_slide = -0.1f + Random.value * 0.4f;
		} else {
			_startFrequency = 0.2f + Random.value * 0.7f;
			_slide = -0.2f - Random.value * 0.2f;
		}
	
		_startFrequency *= _startFrequency;
	
		if (Random.value < 0.2f) _slide = 0.0f;
		if (Random.value < 0.33f) _repeatSpeed = 0.3f + Random.value * 0.5f;
	
		_sustainTime = 0.1f + Random.value * 0.3f;
		_decayTime = Random.value * 0.5f;
		_sustainPunch = 0.2f + Random.value * 0.6f;
	
		if (Random.value < 0.5f) {
			_phaserOffset = -0.3f + Random.value * 0.9f;
			_phaserSweep = -Random.value * 0.3f;
		}
	
		if (Random.value < 0.33f) {
			_changeSpeed = 0.6f + Random.value * 0.3f;
			_changeAmount = 0.8f - Random.value * 1.6f;
		}
	}

	/**
	 * Sets the parameters to generate a powerup sound
	 */
	public void generatePowerup() {
		resetParams();
	
		if (Random.value < 0.5f) {
			_waveType = 1;
		} else {
			_squareDuty = Random.value * 0.6f;
		}
	
		if (Random.value < 0.5f) {
			_startFrequency = 0.2f + Random.value * 0.3f;
			_slide = 0.1f + Random.value * 0.4f;
			_repeatSpeed = 0.4f + Random.value * 0.4f;
		} else {
			_startFrequency = 0.2f + Random.value * 0.3f;
			_slide = 0.05f + Random.value * 0.2f;
		
			if (Random.value < 0.5f) {
				_vibratoDepth = Random.value * 0.7f;
				_vibratoSpeed = Random.value * 0.6f;
			}
		}
	
		_sustainTime = Random.value * 0.4f;
		_decayTime = 0.1f + Random.value * 0.4f;
	}

	/**
	 * Sets the parameters to generate a hit/hurt sound
	 */
	public void generateHitHurt() {
		resetParams();
	
		_waveType = (uint)(Random.value * 3f);
		if (_waveType == 2) {
			_waveType = 3;
		} else if (_waveType == 0) {
			_squareDuty = Random.value * 0.6f;
		}
	
		_startFrequency = 0.2f + Random.value * 0.6f;
		_slide = -0.3f - Random.value * 0.4f;
	
		_sustainTime = Random.value * 0.1f;
		_decayTime = 0.1f + Random.value * 0.2f;
	
		if (Random.value < 0.5f) _hpFilterCutoff = Random.value * 0.3f;
	}

	/**
	 * Sets the parameters to generate a jump sound
	 */
	public void generateJump() {
		resetParams();
	
		_waveType = 0;
		_squareDuty = Random.value * 0.6f;
		_startFrequency = 0.3f + Random.value * 0.3f;
		_slide = 0.1f + Random.value * 0.2f;
	
		_sustainTime = 0.1f + Random.value * 0.3f;
		_decayTime = 0.1f + Random.value * 0.2f;
	
		if (Random.value < 0.5f) _hpFilterCutoff = Random.value * 0.3f;
		if (Random.value < 0.5f) _lpFilterCutoff = 1.0f - Random.value * 0.6f;
	}

	/**
	 * Sets the parameters to generate a blip/select sound
	 */
	public void generateBlipSelect() {
		resetParams();
	
		_waveType = (uint)(Random.value * 2f);
		if (_waveType == 0) _squareDuty = Random.value * 0.6f;
	
		_startFrequency = 0.2f + Random.value * 0.4f;
	
		_sustainTime = 0.1f + Random.value * 0.1f;
		_decayTime = Random.value * 0.2f;
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

	//--------------------------------------------------------------------------
	//
	//  Randomize Methods
	//
	//--------------------------------------------------------------------------

	/**
	 * Randomly adjusts the parameters ever so slightly
	 */
	public void mutate(float mutation = 0.05f) {
		if (Random.value < 0.5f) startFrequency += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) minFrequency += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) slide += 				Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) deltaSlide += 			Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) squareDuty += 			Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) dutySweep += 			Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) vibratoDepth += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) vibratoSpeed += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) attackTime += 			Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) sustainTime += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) decayTime += 			Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) sustainPunch += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) lpFilterCutoff += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) lpFilterCutoffSweep += Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) lpFilterResonance += 	Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) hpFilterCutoff += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) hpFilterCutoffSweep += Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) phaserOffset += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) phaserSweep += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) repeatSpeed += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) changeSpeed += 		Random.value * mutation * 2f - mutation;
		if (Random.value < 0.5f) changeAmount += 		Random.value * mutation * 2f - mutation;
	}

	/**
	 * Sets all parameters to random values
	 */
	public void randomize() {
		paramsDirty = true;
	
		_waveType = (uint)(Random.value * 4f);
	
		_attackTime =  		pow(Random.value*2f-1f, 4);
		_sustainTime =  	pow(Random.value*2f-1f, 2);
		_sustainPunch =  	pow(Random.value*0.8f, 2);
		_decayTime =  		Random.value;

		_startFrequency =  	(Random.value < 0.5f) ? pow(Random.value*2f-1f, 2) : (pow(Random.value * 0.5f, 3) + 0.5f);
		_minFrequency =  	0.0f;
	
		_slide =  			pow(Random.value*2f-1f, 5);
		_deltaSlide =  		pow(Random.value*2f-1f, 3);
	
		_vibratoDepth =  	pow(Random.value*2f-1f, 3);
		_vibratoSpeed =  	Random.value*2f-1f;
	
		_changeAmount =  	Random.value*2f-1f;
		_changeSpeed =  	Random.value*2f-1f;
	
		_squareDuty =  		Random.value*2f-1f;
		_dutySweep =  		pow(Random.value*2f-1f, 3);
	
		_repeatSpeed =  	Random.value*2f-1f;
	
		_phaserOffset =  	pow(Random.value*2f-1f, 3);
		_phaserSweep =  	pow(Random.value*2f-1f, 3);
	
		_lpFilterCutoff =  		1f - pow(Random.value, 3);
		_lpFilterCutoffSweep = 	pow(Random.value*2f-1f, 3);
		_lpFilterResonance =  	Random.value*2f-1f;
	
		_hpFilterCutoff =  		pow(Random.value, 5);
		_hpFilterCutoffSweep = 	pow(Random.value*2f-1f, 5);
	
		if (_attackTime + _sustainTime + _decayTime < 0.2f) {
			_sustainTime = 0.2f + Random.value * 0.3f;
			_decayTime = 0.2f + Random.value * 0.3f;
		}
	
		if ((_startFrequency > 0.7f && _slide > 0.2) || (_startFrequency < 0.2 && _slide < -0.05)) {
			_slide = -_slide;
		}
	
		if (_lpFilterCutoff < 0.1f && _lpFilterCutoffSweep < -0.05f) {
			_lpFilterCutoffSweep = -_lpFilterCutoffSweep;
		}
	}

	//--------------------------------------------------------------------------
	//
	//  Settings String Methods
	//
	//--------------------------------------------------------------------------

	/**
	 * Returns a string representation of the parameters for copy/paste sharing
	 * @return	A comma-delimited list of parameter values
	 */
	public string getSettingsString() {
		string str = waveType.ToString();
		str += "," + to4DP(_attackTime) + 			"," + to4DP(_sustainTime)
			+ "," + to4DP(_sustainPunch) + 			"," + to4DP(_decayTime)
			+ "," + to4DP(_startFrequency) + 		"," + to4DP(_minFrequency)
			+ "," + to4DP(_slide) + 				"," + to4DP(_deltaSlide)
			+ "," + to4DP(_vibratoDepth) + 			"," + to4DP(_vibratoSpeed)
			+ "," + to4DP(_changeAmount) + 			"," + to4DP(_changeSpeed)
			+ "," + to4DP(_squareDuty) + 			"," + to4DP(_dutySweep)
			+ "," + to4DP(_repeatSpeed) + 			"," + to4DP(_phaserOffset)
			+ "," + to4DP(_phaserSweep) + 			"," + to4DP(_lpFilterCutoff)
			+ "," + to4DP(_lpFilterCutoffSweep) + 	"," + to4DP(_lpFilterResonance)
			+ "," + to4DP(_hpFilterCutoff)+ 		"," + to4DP(_hpFilterCutoffSweep)
			+ "," + to4DP(_masterVolume);
	
		return str;
	}

	/**
	 * Parses a settings string into the parameters
	 * @param	string	Settings string to parse
	 * @return			If the string successfully parsed
	 */
	public bool setSettingsString(string str) {
		string[] values = str.Split(new char[] { ',' });
	
		if (values.Length != 24) return false;
	
		// [zeh] fix this
		waveType = 				parseUint(values[0]);
		attackTime =  			parseFloat(values[1]);
		sustainTime =  			parseFloat(values[2]);
		sustainPunch =  		parseFloat(values[3]);
		decayTime =  			parseFloat(values[4]);
		startFrequency =  		parseFloat(values[5]);
		minFrequency =  		parseFloat(values[6]);
		slide =  				parseFloat(values[7]);
		deltaSlide =  			parseFloat(values[8]);
		vibratoDepth =  		parseFloat(values[9]);
		vibratoSpeed =  		parseFloat(values[10]);
		changeAmount =  		parseFloat(values[11]);
		changeSpeed =  			parseFloat(values[12]);
		squareDuty =  			parseFloat(values[13]);
		dutySweep =  			parseFloat(values[14]);
		repeatSpeed =  			parseFloat(values[15]);
		phaserOffset =  		parseFloat(values[16]);
		phaserSweep =  			parseFloat(values[17]);
		lpFilterCutoff =  		parseFloat(values[18]);
		lpFilterCutoffSweep =	parseFloat(values[19]);
		lpFilterResonance =  	parseFloat(values[20]);
		hpFilterCutoff =  		parseFloat(values[21]);
		hpFilterCutoffSweep =	parseFloat(values[22]);
		masterVolume = 			parseFloat(values[23]);
	
		return true;
	}


	//--------------------------------------------------------------------------
	//
	//  Copying Methods
	//
	//--------------------------------------------------------------------------

	/**
	 * Returns a copy of this SfxrParams with all settings duplicated
	 * @return	A copy of this SfxrParams
	 */
	public SfxrParams clone() {
		SfxrParams outp = new SfxrParams();
		outp.copyFrom(this);
	
		return outp;
	}

	/**
	 * Copies parameters from another instance
	 * @param	params	Instance to copy parameters from
	 */
	public void copyFrom(SfxrParams pparams, bool makeDirty = false) {
		_waveType = 			pparams.waveType;
		_attackTime =           pparams.attackTime;
		_sustainTime =          pparams.sustainTime;
		_sustainPunch =         pparams.sustainPunch;
		_decayTime =			pparams.decayTime;
		_startFrequency = 		pparams.startFrequency;
		_minFrequency = 		pparams.minFrequency;
		_slide = 				pparams.slide;
		_deltaSlide = 			pparams.deltaSlide;
		_vibratoDepth = 		pparams.vibratoDepth;
		_vibratoSpeed = 		pparams.vibratoSpeed;
		_changeAmount = 		pparams.changeAmount;
		_changeSpeed = 			pparams.changeSpeed;
		_squareDuty = 			pparams.squareDuty;
		_dutySweep = 			pparams.dutySweep;
		_repeatSpeed = 			pparams.repeatSpeed;
		_phaserOffset = 		pparams.phaserOffset;
		_phaserSweep = 			pparams.phaserSweep;
		_lpFilterCutoff = 		pparams.lpFilterCutoff;
		_lpFilterCutoffSweep = 	pparams.lpFilterCutoffSweep;
		_lpFilterResonance = 	pparams.lpFilterResonance;
		_hpFilterCutoff = 		pparams.hpFilterCutoff;
		_hpFilterCutoffSweep = 	pparams.hpFilterCutoffSweep;
		_masterVolume = 		pparams.masterVolume;
	
		if (makeDirty) paramsDirty = true;
	}


	//--------------------------------------------------------------------------
	//
	//  Util Methods
	//
	//--------------------------------------------------------------------------

	/**
	 * Clams a value to betwen 0 and 1
	 * @param	value	Input value
	 * @return			The value clamped between 0 and 1
	 */
	private float clamp1(float value) {
		return (value > 1f) ? 1f : ((value < 0f) ? 0f : value);
	}

	/**
	 * Clams a value to betwen -1 and 1
	 * @param	value	Input value
	 * @return			The value clamped between -1 and 1
	 */
	private float clamp2(float value) {
		return (value > 1f) ? 1f : ((value < -1f) ? -1f : value);
	}

	/**
	 * Quick power function
	 * @param	base		Base to raise to power
	 * @param	power		Power to raise base by
	 * @return				The calculated power
	 */
	private float pow(float pbase, int power) {
		switch(power) {
			case 2: return pbase*pbase;
			case 3: return pbase*pbase*pbase;
			case 4: return pbase*pbase*pbase*pbase;
			case 5: return pbase*pbase*pbase*pbase*pbase;
		}
	
		return 1f;
	}

	/**
	 * Returns the number as a string to 4 decimal places
	 * @param	value	Number to convert
	 * @return			Number to 4dp as a string
	 */
	private string to4DP(float value) {
		if (value < 0.0001f && value > -0.0001f) return "";
		return value.ToString("#.####");
	}

	/**
	 * Added by zeh - 2013 03 11 - temp?
	 */
	private uint parseUint(string value) {
		if (value.Length == 0) return 0;
		return uint.Parse(value);
	}

	/**
	 * Added by zeh - 2013 03 11 - temp?
	 */
	private float parseFloat(string value) {
		if (value.Length == 0) return 0;
		return float.Parse(value);
	}


}