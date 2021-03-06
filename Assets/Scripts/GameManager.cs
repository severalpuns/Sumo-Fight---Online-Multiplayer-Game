﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{

    [SerializeField] GameObject[] arrayOfPlayer;
    public GameObject[] playerSpawnPoint;
    [SerializeField] GameObject staminaBar;
    public UnityEngine.UI.Button btnPush;
    public UnityEngine.UI.Button btnWin;
    public UnityEngine.UI.Button btnLose;

    private const bool isHost = true;
    private const float MAX_STAMINA = 1000, MAX_PUSH_POINTS = 2000, STAMINA_REGEN = 1, PUSH_STAMINA_COST = 100;

    
    private float stamina, pushPoints;

    public float Stamina { get => stamina; set => stamina = value; }
    public float PushPoints { get => pushPoints; set => pushPoints = value; }
 

    // Start is called before the first frame update
    void Start()
    {
        InitPlayers();
        if (PhotonNetwork.IsMasterClient)
        {
            arrayOfPlayer[0] = PhotonNetwork.Instantiate(arrayOfPlayer[0].name, playerSpawnPoint[0].transform.position, Quaternion.identity, 0);
        }
        else
        {
            arrayOfPlayer[1] = PhotonNetwork.Instantiate(arrayOfPlayer[1].name, playerSpawnPoint[1].transform.position, Quaternion.identity, 0);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 2)
            btnPush.gameObject.SetActive(true);

        if (CheckEndGame())
            UpdateStamina();
    }

    public void OnClickPushButton()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            if (Stamina >= PUSH_STAMINA_COST)
            {
                Stamina -= 100;
                arrayOfPlayer[0].GetComponent<Transform>().Translate(new Vector3(25 * Time.deltaTime, 0, 0));
                
            }
        } else
        {
            if (Stamina >= PUSH_STAMINA_COST)
            {
                Stamina -= 100;
                arrayOfPlayer[1].GetComponent<Transform>().Translate(new Vector3(-25 * Time.deltaTime, 0, 0));
              
            }
        }

    }

    private void InitPlayers()
    {
        Stamina = MAX_STAMINA;
        PushPoints = MAX_PUSH_POINTS/2;
    }

    private bool CheckEndGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (PushPoints > MAX_PUSH_POINTS)
            {
                btnPush.gameObject.SetActive(false);
                btnWin.gameObject.SetActive(true);
                return false;
            }
            else if (PushPoints < 0)
            {
                btnPush.gameObject.SetActive(false);
                btnLose.gameObject.SetActive(true);
                return false;
            }
        } else
        {
            if (PushPoints > MAX_PUSH_POINTS)
            {
                btnPush.gameObject.SetActive(false);
                btnLose.gameObject.SetActive(true);
                return false;
            }
            else if (PushPoints < 0)
            {
                btnPush.gameObject.SetActive(false);
                btnWin.gameObject.SetActive(true);
                return false;
            }
        }


        return true;
    }

    private void UpdateStamina()
    {
        int randomStaminaRegenPoint;
        randomStaminaRegenPoint = Random.Range(1, 3);

        if(Stamina< MAX_STAMINA)
            Stamina += STAMINA_REGEN * randomStaminaRegenPoint;

        if (Stamina >= MAX_STAMINA)
            Stamina = MAX_STAMINA;

        staminaBar.transform.localScale = new Vector3(Stamina / MAX_STAMINA, 1);
    }
}
