using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int collisionDamage;
    // Start is called before the first frame update

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.TryGetComponent(out IDamageable damageable))
        {
            Debug.Log("[Projectile: ]" + " Collided with: " + collision.collider.name);
            damageable.TakeDamage(collisionDamage);
        }
    }
}
