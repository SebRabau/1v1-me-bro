using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoundManager : MonoBehaviour
{
    private int pScore, eScore, pPoint, ePoint;
    public Text pText, eText, winAccuracy, loseAccuracy;
    public Texture red, blue, grey;
    public RawImage left, middle, right;

    public GameObject win, lose;

    private AudioSource audioSource;
    public AudioClip youWin, youLose, gainPoint, losePoint;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void clearPoints()
    {
        ePoint = 0;
        pPoint = 0;

        left.texture = grey;
        middle.texture = grey;
        right.texture = grey;
    }

    public void clearScores()
    {
        pScore = 0;
        eScore = 0;
    }

    public void addScore(string name)
    {
        if(name == "player")
        {
            pScore++;
            pText.text = pScore.ToString();
        } else if(name == "enemy")
        {
            eScore++;
            eText.text = eScore.ToString();
        }
    }

    public void addPoint(string name)
    {
        if (name == "player")
        {
            if(ePoint > 0)
            {
                clearPoints();
            }

            audioSource.PlayOneShot(gainPoint);

            pPoint++;

            switch(pPoint)
            {
                case 1:
                    left.texture = blue;
                    break;
                case 2:
                    middle.texture = blue;
                    break;
                case 3:
                    right.texture = blue;
                    break;
            }

            if(pPoint == 3)
            {
                gameWin();
            }
        }
        else if (name == "enemy")
        {
            if (pPoint > 0)
            {
                clearPoints();
            }

            audioSource.PlayOneShot(losePoint);

            ePoint++;

            switch (ePoint)
            {
                case 1:
                    left.texture = red;
                    break;
                case 2:
                    middle.texture = red;
                    break;
                case 3:
                    right.texture = red;
                    break;
            }

            if (ePoint == 3)
            {
                gameLose();
            }
        }
    }

    private void gameWin()
    {
        audioSource.PlayOneShot(youWin);

        win.SetActive(true);
        winAccuracy.text = "Accuracy: "+GetComponent<PhotonGun>().getAccuracy().ToString()+"%";
        //StartCoroutine(EndWait());
        FindObjectOfType<GameEndManager>().gameEnd();
        Debug.Log(gameObject.name + " Wins!");
    }

    private void gameLose()
    {
        audioSource.PlayOneShot(youLose);

        lose.SetActive(true);
        loseAccuracy.text = "Accuracy: " + GetComponent<PhotonGun>().getAccuracy().ToString() + "%";
        //StartCoroutine(EndWait());
        FindObjectOfType<GameEndManager>().gameEnd();
        Debug.Log(gameObject.name + " Loses...");
    }

    private IEnumerator EndWait()
    {
        yield return new WaitForSeconds(10);
        Cursor.lockState = CursorLockMode.None;
        PhotonNetwork.Destroy(gameObject);
        PhotonNetwork.Disconnect();        
        FindObjectOfType<LevelManager>().toMenu();
    }
}
