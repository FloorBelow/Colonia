using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioMangagerScript : MonoBehaviour {

    [System.Serializable]
    public struct soundDictionaryItem {
        public string key;
        public AudioClip value;
    }
    public AudioClip building01Test;
    public soundDictionaryItem[] sfxItems;
    Dictionary<string, AudioClip> sfx;
    AudioSource source;

    void Start() {
        source = gameObject.GetComponent<AudioSource>();
        sfx = new Dictionary<string, AudioClip>();
        foreach(soundDictionaryItem item in sfxItems) {
            sfx.Add(item.key, item.value);
            
        }
        

    }

    void Update() {
        //source.PlayOneShot(sfx["building01"], 1.0f);
    }


	public void PlaySFX(string sfxID, float volume) {
        source.PlayOneShot(sfx[sfxID], volume);
    }

	public void PlaySFX(string sfxID) {
		PlaySFX(sfxID, 1.0f);
	}
}
