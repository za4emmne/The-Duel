using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _projectileCollider;

    private float _damage;

    public void Shoot(Vector3 startPoint, Vector3 speed)
    {
        _rigidbody.position = startPoint;
        _rigidbody.linearVelocity = speed;
    }

    internal void Initialize(float damage, Collider collider)
    {
        _damage = damage;
        Physics.IgnoreCollision(_projectileCollider, collider);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider)
        {
            var health = other.collider.GetComponentInParent<AbstractHealth>();

            if (health)
                health.TakeDamage(_damage);
        }
    }
}