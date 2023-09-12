using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayableObject : MonoBehaviour
{
    public GameObject KeyInteractionImage;
    public GameObject Strobe;
    public AudioSource AudioSong;

    private bool playerNear=false;
    private bool play = false;

    void Start()
    {
        if (KeyInteractionImage != null)
            KeyInteractionImage.SetActive(false);
        if (Strobe != null)
            Strobe.SetActive(false);
    }

    void Update()
    {
        //VISIBLE KEY INTERACTION
        if (playerNear)
        {
            if (KeyInteractionImage != null)
                KeyInteractionImage.SetActive(true);

            if (Input.GetButtonDown("Interact")) //COMMAND TO PLAY OR STOP SONG
                if (!play)
                {
                    //PLAY
                    AudioSong.Play();
                    play = true;
                    if (Strobe != null)
                        Strobe.SetActive(true);
                }
                else
                {
                    //STOP
                    AudioSong.Stop();
                    play = false;
                    if (Strobe != null)
                        Strobe.SetActive(false);
                }
            
        }

        //HIDE KEY INTERACTION
        if (!playerNear)
        {
            if (KeyInteractionImage != null)
                KeyInteractionImage.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            playerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerNear = false;
        }
    }

}
