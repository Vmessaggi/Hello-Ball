using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereRotate : MonoBehaviour
{
    public float velocidade = 30f;

    void Update()
    {
        transform.Rotate(Vector3.up, velocidade * Time.deltaTime);
    }
}

