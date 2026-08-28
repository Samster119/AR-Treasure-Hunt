using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageBehaviour : MonoBehaviour
{
    private bool blocked = false;

    void Awake()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }
    }

    public void Block()
    {
        blocked = true;
    }

    public void Unblock()
    {
        blocked = false;
    }

    public bool IsBlocked()
    {
        return blocked;
    }
}
