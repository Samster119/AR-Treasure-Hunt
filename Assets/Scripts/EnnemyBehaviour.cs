using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyBehaviour : MonoBehaviour
{
    public int health = 1;
    private PackageBehaviour guardedPackage;

    void Awake()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    public void SetGuardedPackage(PackageBehaviour pkg)
    {
        guardedPackage = pkg;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (guardedPackage != null)
            {
                guardedPackage.Unblock();
            }
            Destroy(gameObject);
        }
    }
}
