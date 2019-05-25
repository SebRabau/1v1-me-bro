using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonHealth : Photon.MonoBehaviour
{
    private GameObject[] spawnPoints;

    public const int maxHealth = 100;

    public int currentHealth = maxHealth;
    private int enemyHealth;

    public RectTransform healthBar;
    public RectTransform hui;

    public AudioClip hit;
    private AudioSource audioSource;

    void Awake()
    {
        spawnPoints = new GameObject[] { GameObject.Find("Spawn Position 1"), GameObject.Find("Spawn Position 2") };
        //Respawn(); //move to spawn
        GetComponent<PhotonView>().RPC("Respawn", PhotonTargets.AllBuffered);

        if (photonView.isMine)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    [PunRPC]
    public void TakeDamage(int dmg)
    {
        if(photonView.isMine)
        {
            audioSource.PlayOneShot(hit, 0.6f);
        }        
        currentHealth -= dmg;
        changeHealth(currentHealth);
        if (currentHealth <= 0)
        {
            if(photonView.isMine) //If player is killed by enemy
            {
                GetComponent<RoundManager>().addPoint("enemy");
                GetComponent<RoundManager>().addScore("enemy");
            }
            currentHealth = maxHealth;

            //Respawn();
            GetComponent<PhotonView>().RPC("Respawn", PhotonTargets.AllBuffered);
        }
    }

    void changeHealth(int health)
    {
        healthBar.sizeDelta = new Vector2(health, healthBar.sizeDelta.y);
        hui.sizeDelta = new Vector2(health, hui.sizeDelta.y);
        Debug.Log(gameObject.name+"Health = " + currentHealth);
    }

    [PunRPC]
    public void Respawn()
    {
        //StartCoroutine(RespawnPause());

        if(PhotonNetwork.isMasterClient)
        {
            transform.position = spawnPoints[0].transform.position;
        } else
        {
            transform.position = spawnPoints[1].transform.position;
        }

        GetComponent<PhotonGun>().resetAmmo();

        currentHealth = maxHealth;
        changeHealth(currentHealth);
    }

    public IEnumerator RespawnPause()
    {
        Debug.Log("Pausing");
        Time.timeScale = 0;
        Debug.Log("Paused, waiting");
        yield return new WaitForSeconds(1f);
        Debug.Log("Unpausing");
        Time.timeScale = 1;
    }
}
