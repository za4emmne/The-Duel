using System;
using UnityEditor.UIElements;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _maxDistance = 500f;
    [SerializeField] private LayerMask _layerMask;
    
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private float _velocity = 100f;

    private Vector3 _startPoint;
    private Vector3 _direction;
    private Collider _collider;

    public void Initialize(CharacterController controller)
    {
        _collider = controller as Collider;
    }
    
public void Shoot(Vector3 startPoint, Vector3 direction)
    {
        _startPoint = startPoint;
        _direction = direction;
        
        ProjectileShoot(startPoint, direction * _velocity);
    }

    private void ProjectileShoot(Vector3 startPoint, Vector3 velocity)
    {
        var projectile = Instantiate(_projectilePrefab);
        projectile.Initialize(_damage, _collider);
        
        projectile.Shoot(startPoint, velocity);
    }

    private void RaycastShoot(Vector3 startPoint, Vector3 direction)
    {
        if (Physics.Raycast(startPoint, direction, out RaycastHit hitInfo, _maxDistance, _layerMask, QueryTriggerInteraction.Ignore))
        {
            var health = hitInfo.collider.GetComponentInParent<AbstractHealth>();

            if (health)
                health.TakeDamage(_damage);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        if (Physics.Raycast(_startPoint, _direction, out RaycastHit hitInfo, _maxDistance, _layerMask, QueryTriggerInteraction.Ignore))
        {
            Gizmos.DrawLine(_startPoint, hitInfo.point);
        }
    }
}
