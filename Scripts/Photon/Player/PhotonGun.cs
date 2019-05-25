using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonGun : Photon.MonoBehaviour
{
    //Audio
    private AudioSource audioSource;
    public AudioClip Shot;
    public AudioClip Reload;

    //Gun vars
    private float nextFire;
    private int currentAmmo;
    private bool rldn = false;
    private bool isFiring = false;
    public float fireRate = 0.08f; //Lower = faster
    public float weaponRange = 50f;
    public int maxAmmo = 25;
    public Animator gunAnim;
    public ParticleSystem muzzleFlash;
    public GameObject impactStone;
    public GameObject impactMetal;
    public GameObject impactWood;
    public GameObject impactSand;
    public GameObject impactPlayer;

    //UI
    public RawImage cHit;
    public Text currentAmmoUI;
    public Text reloadUI;
    public Text ammoFullUI;

    //Cam
    public Camera cam;

    //Accuracy
    private int hitCount;
    private int shootCount;

    //other
    private bool inMenu = false;
    private bool inChat = false;

    public void Awake()
    {
        if (photonView.isMine)
        {
            audioSource = GetComponent<AudioSource>();
            currentAmmo = maxAmmo;
            currentAmmoUI.text = maxAmmo.ToString();
        }
    }

    void Update()
    {
        if (!photonView.isMine)
        {
            return;
        }

        //Player enters menu
        if (Input.GetKeyDown(KeyCode.Escape) && !inMenu)
        {
            inMenu = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && inMenu)
        {
            inMenu = false;
        }

        //Player enters chat
        if (Input.GetKeyDown(KeyCode.Return) && !inChat)
        {
            inChat = true;
        }
        else if (Input.GetKeyDown(KeyCode.Return) && inChat)
        {
            inChat = false;
        }

        if (!inMenu && !inChat)
        {
            if (Input.GetKey(KeyCode.Mouse0) && Time.time > nextFire && currentAmmo > 0 && !rldn)
            {
                shootCount++;

                //Find the RPC reference and call the 'shoot' function on all clients
                GetComponent<PhotonView>().RPC("shoot", PhotonTargets.AllBuffered);

                nextFire = Time.time + fireRate;
                Fire();
                currentAmmo--;
                currentAmmoUI.text = currentAmmo.ToString();
                isFiring = true;
            }
            else if (Input.GetKey(KeyCode.Mouse0) && currentAmmo <= 0)
            {
                StartCoroutine(HoldTime(reloadUI.gameObject, 2));
                isFiring = false;
            }
            isFiring = false;

            if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !rldn)
            {
                rldn = true;
                GetComponent<PhotonView>().RPC("reload", PhotonTargets.AllBuffered);
                //StartCoroutine(Reloader());
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(HoldTime(ammoFullUI.gameObject, 2));
            }
        }        
    }

    void Fire()
    {
        RaycastHit hit = new RaycastHit();

        Ray ray = new Ray(cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0)), cam.transform.TransformDirection(Vector3.forward));

        //Test Hit
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log(hit.collider.gameObject.tag);
        }

        if (Physics.Raycast(ray, out hit, weaponRange))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                hitCount++;

                GameObject impactObj = Instantiate(impactPlayer, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactObj, 2f);

                PhotonHealth health = hit.collider.gameObject.GetComponent<PhotonHealth>();

                int dmg = Random.Range(5, 10);

                if (health != null)
                {
                    if (health.currentHealth - dmg <= 0)
                    {
                        GetComponent<RoundManager>().addPoint("player");
                        GetComponent<RoundManager>().addScore("player");
                        //GetComponent<PhotonHealth>().Respawn();
                        GetComponent<PhotonHealth>().currentHealth = 100;
                        health.currentHealth = 100;
                        GetComponent<PhotonView>().RPC("Respawn", PhotonTargets.AllBuffered);
                        hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.AllBuffered, dmg);
                    } else
                    {
                        hit.transform.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.AllBuffered, dmg);
                    }  
                }

                StartCoroutine(HoldTime(cHit.gameObject, 0.5f));
            }
            else if(hit.collider.gameObject.tag == "Stone")
            {
                GameObject impactObj = Instantiate(impactStone, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactObj, 2f);
            }
            else if (hit.collider.gameObject.tag == "Wood")
            {
                GameObject impactObj = Instantiate(impactWood, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactObj, 2f);
            }
            else if (hit.collider.gameObject.tag == "Metal")
            {
                GameObject impactObj = Instantiate(impactMetal, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactObj, 2f);
            }
            else if (hit.collider.gameObject.tag == "Sand")
            {
                GameObject impactObj = Instantiate(impactSand, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactObj, 2f);
            }
        }
    }

    //PunRPC declares to the Photon server that this is an RPC function
    [PunRPC]
    private void shoot()
    {
        GetComponent<AudioSource>().PlayOneShot(Shot);
        muzzleFlash.Play();
    }

    [PunRPC]
    private void reload()
    {
        GetComponent<AudioSource>().PlayOneShot(Reload);
        StartCoroutine(Reloader());
    }

    public void resetAmmo()
    {
        currentAmmo = maxAmmo;
    }

    private IEnumerator Reloader()
    {
        gunAnim.SetBool("R", true);
        yield return new WaitForSeconds(Reload.length - 0.25f);
        gunAnim.SetBool("R", false);
        yield return new WaitForSeconds(0.25f);
        resetAmmo();
        currentAmmoUI.text = currentAmmo.ToString();
        rldn = false;
    }

    private IEnumerator HoldTime(GameObject a, float b)
    {
        a.SetActive(true);
        yield return new WaitForSeconds(b);
        a.SetActive(false);
    }    

    public double getAccuracy()
    {
        //Debug.Log("shoot: " + shootCount + " hit: " + hitCount+" Accuracy: "+ (double) (hitCount / shootCount) * 100);
        //if(shootCount == 0)
        //{
        //    return 0.0;
        //}
        //return (double) (hitCount / shootCount)*100;

        return 0.0;
    }
}
