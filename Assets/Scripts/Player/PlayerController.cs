using System;
using UnityEngine;

[RequireComponent((typeof(CharacterController)))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed;
    
    private Transform _transform;
    private CharacterController _characterController;   

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
    _transform = transform;
    }

    private void Update()
    {
        if (_characterController)
        {
            Vector3 playerInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            playerInput *= Time.deltaTime * _speed;

            if (_characterController.isGrounded)
            {
                _characterController.Move(playerInput + Vector3.down);
            }
            else
            {
                _characterController.Move(_characterController.velocity + Physics.gravity*Time.deltaTime);
            }
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.rigidbody != null)
        {
            hit.rigidbody.linearVelocity = Vector3.up * 10f; // или AddForce
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var character = GetComponent<CharacterController>();    
        
        Gizmos.DrawWireCube(transform.position, Vector3.right + Vector3.forward + Vector3.up * character.height);
    }
}
