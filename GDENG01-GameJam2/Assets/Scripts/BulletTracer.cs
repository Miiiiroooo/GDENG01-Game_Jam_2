using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTracer : MonoBehaviour
{
    float currentTime;

    private void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime >= 2f)
        {
            Destroy(this.gameObject);
        }
    }
}
