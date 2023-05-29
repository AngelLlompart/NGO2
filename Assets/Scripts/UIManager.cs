using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button startServerButton;
    [SerializeField] private Button startHostButton;
    [SerializeField] private Button startClientButton;
    [SerializeField] private TextMeshProUGUI playersInGameText;

    private void Awake()
    {
        Cursor.visible = true;
    }
    

    // Start is called before the first frame update
    void Start()
    {
        startServerButton.onClick.AddListener(StartServerClick);
        startHostButton.onClick.AddListener(StartHostClick);
        startClientButton.onClick.AddListener(StartClientClick);
    }

    // Update is called once per frame
    void Update()
    {
        //playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    private void StartHostClick()
    {
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started...");
        }
        else
        {
            Debug.Log("Host started could not be started...");
        }
    }
    
    private void StartServerClick()
    {
        if (NetworkManager.Singleton.StartServer())
        {
            Debug.Log("Server started...");
        }
        else
        {
            Debug.Log("Server started could not be started...");
        }
    }
    
    private void StartClientClick()
    {
        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client started...");
        }
        else
        {
            Debug.Log("Client started could not be started...");
        }
    }
}
