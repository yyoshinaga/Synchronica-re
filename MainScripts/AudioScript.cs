using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {

    private AudioClip tink; // Sound when note is hit
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = transform.GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        audioSource.Play(0);        
    }

    public void PauseSound()
    {
        audioSource.Pause();
    }
	public void UnPauseSound()
    {
        audioSource.UnPause();
    }
}
