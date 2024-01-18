using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhaleController : MonoBehaviour, IDamageable
{
    [SerializeField] private int maxHealth;
    private int Health { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        Health -= damage;
        
        if(Health <= 0)
        {
            Die();
        }
    }

    public void SetHealth(int amount)
    {
        Health = amount;
    }

    public void Die()
    {
        //Die
    }
}
