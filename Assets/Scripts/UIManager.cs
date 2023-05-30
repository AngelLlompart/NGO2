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
    private void Start()
    {
        startServerButton.onClick.AddListener(StartServerClick);
        startHostButton.onClick.AddListener(StartHostClick);
        startClientButton.onClick.AddListener(StartClientClick);
    }

    // Update is called once per frame
    private void Update()
    {
        playersInGameText.text = $"Players in game: {PlayersManager.Instance.PlayersInGame}";
    }

    private void StartHostClick()
    {
        Debug.Log(NetworkManager.Singleton.StartHost() 
            ? "Host started..." 
            : "Host started could not be started...");
    }
    
    private void StartServerClick()
    {
        Debug.Log(NetworkManager.Singleton.StartServer()
            ? "Server started..."
            : "Server started could not be started...");
    }
    
    private void StartClientClick()
    {
        Debug.Log(NetworkManager.Singleton.StartClient()
            ? "Client started..."
            : "Client started could not be started...");
    }
}
