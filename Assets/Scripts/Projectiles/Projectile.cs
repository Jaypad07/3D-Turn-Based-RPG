using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public int heal;
    // public Effect effectToApply;

    public float moveSpeed;
    private Character target;

    public void Initialize(Character targetChar)
    {
        target = targetChar;
    }

    private void Update()
    {
        if (target != null)
        {
            // On target.transform.position we are adding an additional vector 3 to raise the projectile position to shoot around their stomach and not their feet.
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position + new Vector3(0, 0.5f, 0), moveSpeed * Time.deltaTime); 
        }
    }

    void ImpactTarget()
    {
        if (damage > 0)
        {
            target.TakeDamage(damage);
        }

        if (heal > 0)
        {
            target.Heal(heal);
        }
        
        // apply effect if we have one.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target != null && other.gameObject == target.gameObject)
        {
            ImpactTarget();
            Destroy(gameObject);
        }
    }
}

