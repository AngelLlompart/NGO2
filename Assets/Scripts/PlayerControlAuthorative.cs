using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerControlAuthorative : NetworkBehaviour
{

    public enum PlayerState
    {
        Idle,
        Walk,
        ReverseWalk
    }
    
    [SerializeField] private float speed = 3.5f;

    [SerializeField] private float rotationSpeed = 1.5f;
    
    [SerializeField] private Vector2 defaultInitialPlanePosition = new Vector2(-4, 4);

    [SerializeField] private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    private CharacterController _characterController;

    private Animator _animator;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (IsClient && IsOwner)
        {
            transform.position = new Vector3(Random.Range(defaultInitialPlanePosition.x, defaultInitialPlanePosition.y), 0, 
                Random.Range(defaultInitialPlanePosition.x, defaultInitialPlanePosition.y));
            
            PlayerCameraFollow.Instance.FollowPlayer(transform.Find("PlayerCameraRoot"));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient && IsOwner)
        {
            ClientInput();
        }
        
        ClientVisuals();
    }

    private void ClientInput()
    {
        //player position and rotation input
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.LeftShift) && forwardInput > 0) forwardInput = 2;
        Vector3 inputPosition = direction * forwardInput;


        //client is responsible for moving itself
        _characterController.SimpleMove(inputPosition * speed);
        transform.Rotate(inputRotation * rotationSpeed, Space.World);

        //player state changes based on input
        if (forwardInput > 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.Walk);
        } 
        else if (forwardInput < 0)
        {
            UpdatePlayerStateServerRpc(PlayerState.ReverseWalk);
        }
        else
        {
            UpdatePlayerStateServerRpc(PlayerState.Idle);
        }
    }
    
    private void ClientVisuals()
    {
        if (networkPlayerState.Value == PlayerState.Walk)
        {
            _animator.SetFloat("Walk", 1);
        }
        else if (networkPlayerState.Value == PlayerState.ReverseWalk)
        {
            _animator.SetFloat("Walk", -1);
        }
        else
        {
            _animator.SetFloat("Walk", 0);
        }
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        networkPlayerState.Value = state;
    }
    
}
