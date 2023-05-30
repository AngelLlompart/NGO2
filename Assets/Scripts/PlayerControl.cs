using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerControl : NetworkBehaviour
{

    public enum PlayerState
    {
        Idle,
        Walk,
        ReverseWalk
    }
    
    [SerializeField] private float speed = 0.05f;

    [SerializeField] private float rotationSpeed = 1.5f;
    
    [SerializeField] private Vector2 defaultInitialPlanePosition = new Vector2(-4, 4);

    [SerializeField] private NetworkVariable<Vector3> networkPositionDirection = new NetworkVariable<Vector3>(); 
    
    [SerializeField] private NetworkVariable<Vector3> networkRoatationDirection = new NetworkVariable<Vector3>();

    [SerializeField] private NetworkVariable<PlayerState> networkPlayerState = new NetworkVariable<PlayerState>();

    private CharacterController _characterController;

    private Animator _animator;
    //client caching
    private Vector3 oldInputPosition;
    private Vector3 oldInputRotation;
    
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
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClient && IsOwner)
        {
            ClientInput();
        }
        
        ClientMoveAndRotate();
        ClientVisuals();
    }

    private void ClientInput()
    {
        //player position and rotation input
        Vector3 inputRotation = new Vector3(0, Input.GetAxis("Horizontal"), 0);

        Vector3 direction = transform.TransformDirection(Vector3.forward);
        float forwardInput = Input.GetAxis("Vertical");
        Vector3 inputPosition = direction * forwardInput;

        if (oldInputPosition != inputPosition || oldInputRotation != inputRotation)
        {
            oldInputPosition = inputPosition;
            oldInputRotation = inputRotation;
            UpdateClientPositionAndRotationServerRpc(inputPosition * speed, inputRotation * rotationSpeed);
        }

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

    private void ClientMoveAndRotate()
    {
        if (networkPositionDirection.Value != Vector3.zero)
        {
            _characterController.SimpleMove(networkPositionDirection.Value);
        }
        if (networkRoatationDirection.Value != Vector3.zero)
        {
            transform.Rotate(networkRoatationDirection.Value);
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
    
    /*private void UpdateServer()
    {
        transform.position = new Vector3(transform.position.x + leftRightPosition.Value, transform.position.y,
            transform.position.z + forwardBackPosition.Value);
    }*/

    /*private void UpdateClient()
    {
        float forwardBackward = 0;
        float leftRight = 0;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            forwardBackward += speed;
        } 
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            forwardBackward -= speed;
        } 
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            leftRight -= speed;
        } 
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            leftRight += speed;
        }

        if (oldForwardBackPosition != forwardBackward || oldLeftRightPosition != leftRight)
        {
            oldForwardBackPosition = forwardBackward;
            oldLeftRightPosition = leftRight;
            //update the server
            UpdateClientPositionServerRpc(forwardBackward, leftRight);
        }
    }*/

    /*[ServerRpc]
    public void UpdateClientPositionServerRpc(float forwardBackward, float leftRight)
    {
        forwardBackPosition.Value = forwardBackward;
        leftRightPosition.Value = leftRight;
    }*/

    [ServerRpc]
    public void UpdateClientPositionAndRotationServerRpc(Vector3 newPosition, Vector3 newRotation)
    {
        networkPositionDirection.Value = newPosition;
        networkRoatationDirection.Value = newRotation;
    }

    [ServerRpc]
    public void UpdatePlayerStateServerRpc(PlayerState state)
    {
        networkPlayerState.Value = state;
    }
    
}
