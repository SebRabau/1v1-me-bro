using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);

        GameObject[] objs = GameObject.FindGameObjectsWithTag("MenuMusic"); 

        if (objs.Length > 1)
        {
            for(int i=0; i < objs.Length; i++)
            {
                if(!objs[i].GetComponent<AudioSource>().isPlaying)
                {
                    objs[i].GetComponent<AudioSource>().Play();
                }
            }

            Destroy(gameObject);
        }
    }
}
