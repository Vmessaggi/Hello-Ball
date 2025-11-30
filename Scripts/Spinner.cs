using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float floatSpeed = 1f;
    public float floatHeight = 0.1f;

    private Vector3 startPos;
    private Vector3 upDirection;
    private Vector3 rotationAxis;

    void Start()
    {
        startPos = transform.position;
        upDirection = transform.up.normalized;

        // Decide qual eixo usar para girar com base no "pra cima"
        if (Vector3.Dot(upDirection, Vector3.up) > 0.9f)
        {
            // Está apontando para Y (normal), gira em torno de Y
            rotationAxis = Vector3.up;
        }
        else if (Vector3.Dot(upDirection, Vector3.forward) > 0.9f)
        {
            // Está apontando para Z, gira em torno de Z
            rotationAxis = Vector3.forward;
        }
        else if (Vector3.Dot(upDirection, Vector3.right) > 0.9f)
        {
            // Caso raro: up é X → gira em X
            rotationAxis = Vector3.right;
        }
        else
        {
            // Caso inclinado, usa eixo cruzado (moeda em rampa, por exemplo)
            rotationAxis = Vector3.Cross(transform.right, upDirection).normalized;
        }
    }

    void Update()
    {
        // Rotação correta para o "plano" do objeto
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.World);

        // Flutuação no eixo "pra cima" do objeto
        float offset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = startPos + upDirection * offset;
    }
}
