using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioClip intro;
    public AudioClip loop;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = intro;
        audioSource.Play();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = loop;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}
