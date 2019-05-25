using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonMovement : Photon.MonoBehaviour
{
    private int playerCount;
    private bool stepping = false;
    private bool inMenu = false;
    private bool inChat = false;

    //Player objs
    public GameObject gun, target;

    //Player vars
    private float speed = 5f;
    public Animator anim, gunanim;
    private Rigidbody rb;    
    private Vector3 correctPos, oldPos;
    private Quaternion correctRot;
    private float x, z, my, mx;
    private CapsuleCollider col;
    public float lookSpeed = 175.0f;

    //Audio
    public AudioSource audioSource;
    public AudioClip startingClip;
    private bool started = false;

    //Cam
    public Camera cam;

    //UI
    public GameObject canvas, chat;
    public Text serverStats;

    private void Awake()
    {
        if (photonView.isMine)
        {
            cam.gameObject.SetActive(true);
            rb = GetComponent<Rigidbody>();
            canvas.gameObject.SetActive(true);
            chat.gameObject.SetActive(true);

            col = GetComponent<CapsuleCollider>();

            //Photon
            playerCount = PhotonNetwork.room.playerCount;
            serverStats.text = ("Connected to the room: " + PhotonNetwork.room.name + " - Players Online: " + playerCount + " - Master: " + PhotonNetwork.isMasterClient);

            if(playerCount == 2)
            {
                GetComponent<AudioSource>().PlayOneShot(startingClip);
                started = true;
            }

            //Confine cursor to window
            Cursor.lockState = CursorLockMode.Locked;
        } else
        {
            return;
        }
    }

    private void LateUpdate()
    {
        mx = Input.GetAxis("Mouse X") * Time.deltaTime * lookSpeed;

        transform.Rotate(0, mx, 0);
    }

    private void Update()
    {        
        if(photonView.isMine)
        {
            if(!started && PhotonNetwork.room.playerCount == 2)
            {
                GetComponent<AudioSource>().PlayOneShot(startingClip);
                started = true;
            }

            if(!inMenu && !inChat)
            {
                checkInput();
            }

            //Player enters menu
            if (Input.GetKeyDown(KeyCode.Escape) && !inMenu)
            {
                Cursor.lockState = CursorLockMode.None;
                inMenu = true;                
            } else if(Input.GetKeyDown(KeyCode.Escape) && inMenu)
            {
                Cursor.lockState = CursorLockMode.Locked;
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

            if (PhotonNetwork.room.playerCount != playerCount) //Update Player count and master indicator
            {
                playerCount = PhotonNetwork.room.playerCount;
                serverStats.text = ("Connected to the room: " + PhotonNetwork.room.name + " - Players Online: " + playerCount + " - Master: " + PhotonNetwork.isMasterClient);
                //GetComponent<PhotonHealth>().Respawn();
                GetComponent<PhotonView>().RPC("Respawn", PhotonTargets.AllBuffered);
            }

            //if fallen off map
            if(gameObject.transform.position.y < -15)
            {
                //GetComponent<PhotonHealth>().Respawn();
                GetComponent<PhotonView>().RPC("Respawn", PhotonTargets.AllBuffered);
            }
        }   
    }

    private void checkInput()
    {
        if(Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Movement Audio
        if ((!Input.GetAxisRaw("Horizontal").AlmostEquals(0, 0.02f) || !Input.GetAxisRaw("Vertical").AlmostEquals(0, 0.02f)) && grounded())
        {
            if (!stepping)
            {
                GetComponent<PhotonView>().RPC("startWalking", PhotonTargets.AllBuffered);
                stepping = true;
            }
            anim.SetBool("Walking", true);
        }
        else if ((Input.GetAxisRaw("Horizontal").AlmostEquals(0, 0.02f) && Input.GetAxisRaw("Vertical").AlmostEquals(0, 0.02f)))
        {
            GetComponent<PhotonView>().RPC("stopWalking", PhotonTargets.AllBuffered);
            stepping = false;
            anim.SetBool("Walking", false);
        }

        //Run
        if (Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftControl) && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            speed = 8.0f;
            audioSource.pitch = 1.1f;
            anim.SetBool("Run", true);
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 5.0f;
            audioSource.pitch = 1f;
            anim.SetBool("Run", false);
        }

        //Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            speed = 3.0f;
            anim.SetBool("Crouch", true);
            audioSource.pitch = 0.9f;
            col.center.Set(0.0136f, 0.525f, 0);
            col.height = 1.15f;
        }
        else if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            speed = 5.0f;
            anim.SetBool("Crouch", false);
            audioSource.pitch = 1f;
            col.center.Set(0.0136f, 0.9f, 0);
            col.height = 1.9f;
        }

        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && grounded())
        {
            rb.AddForce(new Vector3(0.0f, 6.0f, 0.0f), ForceMode.Impulse);
            audioSource.loop = false;
            stepping = false;
            anim.SetBool("Jump", true);
        } 

        //Zoom
        if (Input.GetMouseButton(1))
        {
            cam.fieldOfView = 45;
            speed = 3.0f;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            cam.fieldOfView = 60;
            speed = 5.0f;
        }
       
        //Move --> Translate to velocity
        x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        transform.Translate(x, 0, z);
    }

    [PunRPC]
    private void startWalking()
    {
        audioSource.loop = true;
        audioSource.Play();
    }

    [PunRPC]
    private void stopWalking()
    {
        audioSource.loop = false;
    }

    private bool grounded()
    {
        if (transform.position.y < -0.8f)
        {
            anim.SetBool("Jump", false);
            return true;
        } else
        {
            return false;
        }
    }
}
