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
    [SerializeField] private Button executePhysicsButton;

    private bool hasServerStarted;

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
        executePhysicsButton.onClick.AddListener(ExecutePhysics);

        NetworkManager.Singleton.OnServerStarted += () =>
        {
            hasServerStarted = true; 
        };
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
            : "Host could not be started...");
    }
    
    private void StartServerClick()
    {
        Debug.Log(NetworkManager.Singleton.StartServer()
            ? "Server started..."
            : "Server could not be started...");
    }
    
    private void StartClientClick()
    {
        Debug.Log(NetworkManager.Singleton.StartClient()
            ? "Client started..."
            : "Client could not be started...");
    }
    
    private void ExecutePhysics()
    {
        if (!hasServerStarted)
        {
            Debug.Log("Server has not started... ");
            return;
        }
        
        SpawnerControl.Instance.SpawnObjects();
    }
}
