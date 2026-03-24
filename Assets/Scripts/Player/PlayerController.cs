using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent((typeof(CharacterController)))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    [SerializeField] private float _straveSpeed = 7f;
    [SerializeField] private float _jumpSpeed = 9f;
    [SerializeField] private float _gravityFactor = 2f;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float __horizontalSensivity = 7f;
    [SerializeField] private float _verticalSensivity = 10f;
    [SerializeField] private float _verticalMinAngle = -89f;
    [SerializeField] private float _verticalMaxAngle = 89f;
    
    [Header("Weapon")]
        [SerializeField] private Shotgun _shotgun;
    
    
    private Vector3 _verticalVelocity;
    private Transform _transform;
    private CharacterController _characterController;
    private float _cameraAngle = 0;

    private void Awake()
    { 
        _characterController = GetComponent<CharacterController>();
        _transform = transform;
        _cameraAngle = _cameraTransform.localEulerAngles.x;
    }

    private void Update()
    {
    Movement();

    if (Input.GetKeyDown(KeyCode.Mouse0))
    {
        _shotgun.Shoot(_cameraTransform.position, _cameraTransform.forward);
    }
    }
    
    private void Movement()
        {
            Vector3 forward = Vector3.ProjectOnPlane(_cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(_cameraTransform.right, Vector3.up).normalized;
                    
                    _cameraAngle -= Input.GetAxis("Mouse Y") * _verticalSensivity;
                    _cameraAngle = Mathf.Clamp(_cameraAngle, _verticalMinAngle, _verticalMaxAngle);
                    _cameraTransform.localEulerAngles = Vector3.right * _cameraAngle;
                    
                    _transform.Rotate(Vector3.up * __horizontalSensivity * Input.GetAxis("Mouse X"));
                    
                    if (_characterController)
                    {
                        Vector3 playerSpeed = forward * Input.GetAxis("Vertical") * _speed + right * Input.GetAxis("Horizontal") * _straveSpeed;
                        
                        if (_characterController.isGrounded)
                        {
                            if (Input.GetKeyDown(KeyCode.Space))
                            {
                                _verticalVelocity = Vector3.up * _jumpSpeed;
                            }
                            else
                            {
                                _verticalVelocity = Vector3.down;
                            }
                            
                            _characterController.Move((playerSpeed + _verticalVelocity)*Time.deltaTime);
                        }
                        else
                        {
                            Vector3 horizontalVelocity = _characterController.velocity;
                            horizontalVelocity.y = 0;
                            _verticalVelocity += Physics.gravity * Time.deltaTime * _gravityFactor;
                            _characterController.Move((horizontalVelocity + _verticalVelocity) * Time.deltaTime);
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
