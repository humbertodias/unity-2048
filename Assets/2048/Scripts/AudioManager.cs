using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	public static AudioManager Instance;
	/*
	public static AudioManager Instance {
		get { return instance; }
	}
	*/

	[SerializeField]
	private AudioSource sfxSource;

	[SerializeField]
	private AudioSource musicSource;

	public bool muteSound = false;
	public bool muteMusic = false;

	//===================================================
	// UNITY METHODS
	//===================================================

	void Awake() {
		/*
		# region - Singleton
		if( instance == null ) {
			instance = this;
		} else if( instance != this ) {
			Destroy( gameObject );
		}
		DontDestroyOnLoad( gameObject );
		# endregion
		*/
		Instance = this;

	}


	//===================================================
	// PUBLIC METHODS
	//===================================================


	/// <summary>
	/// Plays the SFX.
	/// </summary>
	/// <param name="clip">The clip.</param>
	/// <param name="volume">The volume.</param>
	public void PlaySFX( AudioClip clip, float volume = 1.0f, bool loop = false ) {
		if( sfxSource != null && !sfxSource.isPlaying ) {
			sfxSource.clip = clip;
			sfxSource.volume = volume;
			sfxSource.mute = muteSound;
			sfxSource.Play();
		} else {
			PlayDynamicSound( clip, muteSound, volume, loop );
		}
	}

	/// <summary>
	/// Plays the music.
	/// </summary>
	/// <param name="clip">The clip.</param>
	/// <param name="volume">The volume.</param>
	public void PlayMusic( AudioClip clip, bool loop = false, float volume = 1.0f ) {
/*		if( musicSource.isPlaying && musicSource.clip == clip ) {
			return;
		}
*/
		if (musicSource != null && !musicSource.isPlaying) {

			musicSource.clip = clip;
			musicSource.loop = loop;
			musicSource.volume = volume;
			musicSource.mute = muteMusic;
			musicSource.Play ();

		} else {
			PlayDynamicSound( clip, muteMusic, volume, loop );
		}
	}


	//===================================================
	// PRIVATE METHODS
	//===================================================

	/// <summary>
	/// Plays the dynamic sound.
	/// </summary>
	/// <param name="clip">The clip.</param>
	/// <param name="volume">The volume.</param>
	private void PlayDynamicSound( AudioClip clip, bool mute, float volume = 1.0f, bool loop = false) {
		//Create an empty game object
		GameObject sfxGO = new GameObject( "DynamicSound_" + clip.name );
		sfxGO.transform.SetParent( transform );

		//Create the source
		AudioSource source = sfxGO.AddComponent<AudioSource>();
		source.clip = clip;
		source.volume = volume;
		source.mute = mute;
		source.loop = loop;
		source.Play();

		Destroy( sfxGO, clip.length );
	}

	//===================================================
	// EVENTS METHODS
	//===================================================


	public void MuteSoundToggle(){
		muteSound = !muteSound;

		if (sfxSource != null) {
			sfxSource.mute = muteSound;
		}
	}

	public void MuteMusicToggle(){
		muteMusic = !muteMusic;

		if (musicSource != null) {
			musicSource.mute = muteMusic;
		}
	}

}