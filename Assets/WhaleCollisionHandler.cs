using UnityEngine;

public class WhaleCollisionHandler : MonoBehaviour
{
    [SerializeField] private int collisionDamage;
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log("[WhaleCollider: ]" + " Collided with: " + collision.collider.name);
            damageable.TakeDamage(collisionDamage);
        }
    }
}
