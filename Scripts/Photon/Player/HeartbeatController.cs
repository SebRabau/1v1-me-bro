using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartbeatController : Photon.MonoBehaviour
{

    PhotonHealth playerHealth;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        if(photonView.isMine)
        {
            playerHealth = GetComponentInParent<PhotonHealth>();
            audioSource = GetComponent<AudioSource>();
        }        
    }

    // Update is called once per frame
    void Update()
    {
        if(photonView.isMine)
        {
            if (playerHealth.currentHealth <= 30 && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
            else if (playerHealth.currentHealth > 30 && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }        
    }
}
