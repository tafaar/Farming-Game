using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedDestroy : MonoBehaviour
{
    public void Remove(float time)
    {
        transform.parent = null;
        Invoke("SelfDestruct", time);
    }

    void SelfDestruct()
    {
        Destroy(gameObject);
    }
}
