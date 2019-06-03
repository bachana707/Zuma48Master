using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance { get; set; }

    public AudioClip shootSound;
    public AudioClip mergeSound;
    public AudioClip winSound;
    public AudioClip loseSound;

    private AudioSource audioSource;
    [HideInInspector] public bool canPlay = true;

    void Awake() {
        if (Instance == null) {
            Instance = this;
        }
        else if (Instance != this) {
            Debug.Log("Game Manager already exists");
            Destroy(gameObject);
        }
        canPlay = AppData.GetSoundInfo() == 0 ? true : false;
        audioSource = GetComponent<AudioSource>();
    }


    public void PlayShootSound() {
        if (canPlay) {
            audioSource.PlayOneShot(shootSound);
        }
    }
    public void PlayMergeSound() {
        if (canPlay) {
            audioSource.PlayOneShot(mergeSound);
        }
    }
    public void PlayWinSound() {
        if (canPlay) {
            audioSource.PlayOneShot(winSound);
        }
    }
    public void PlayLoseSound() {
        if (canPlay) {
            audioSource.PlayOneShot(loseSound);
        }
    }
}
