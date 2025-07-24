//using System;
using UnityEngine;

public class backgroundMusic : MonoBehaviour
{
    
    public AudioSource musicPlayer;

    [SerializeField] private AudioClip[] musicClips;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        musicPlayer = GetComponent<AudioSource>();
        
        int rand = Random.Range(0, musicClips.Length);
        musicPlayer.clip = musicClips[rand];
        Debug.Log(rand);
        musicPlayer.Play();
    }
}
