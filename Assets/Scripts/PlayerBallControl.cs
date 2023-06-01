using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NetworkObject))]
[RequireComponent(typeof(ClientNetworkTransform))]
public class PlayerBallControl : NetworkBehaviour
{

    [SerializeField] private float speed = 3.5f;

    [SerializeField] private float flySpeed = 4f;
    
    [SerializeField] private float rotationSpeed = 1.5f;
    
    [SerializeField] private Vector2 defaultInitialPlanePosition = new Vector2(-4, 4);

    private Rigidbody ballRigidBody;

    private void Awake()
    {
        ballRigidBody = GetComponent<Rigidbody>();
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
        
    }

    private void ClientInput()
    {
        //player position and rotation input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (vertical > 0 || vertical < 0)
        {
            ballRigidBody.AddForce(vertical > 0 ? Vector3.forward * speed : Vector3.back * speed);
        }
        if (horizontal > 0 || horizontal < 0)
        {
            ballRigidBody.AddForce(horizontal > 0 ? Vector3.right * speed : Vector3.left * speed);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            ballRigidBody.AddForce(Vector3.up * flySpeed);
        }
        

    }
    
}
