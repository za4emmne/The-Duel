using System;
using UnityEngine;

public class PlayerRigidbody : MonoBehaviour
{
    [SerializeField] private float _speed = 3f;
   
    
    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>(); 
    }

    private void Update()
    {

        Vector3 playerSpeed = new Vector3(Input.GetAxis("Horizontal") * _speed, 0f, Input.GetAxis("Vertical"));
        var character = GetComponent<CharacterController>();

        _rigidbody.linearVelocity += Physics.gravity;
    }
}
