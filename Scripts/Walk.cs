using UnityEngine;

public class Walk : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    private float radius;

    void Start()
    {
        // raio da esfera
        radius = transform.localScale.x * 0.5f;
    }

    void Update()
    {
        // pega entradas do teclado
        float inputZ = Input.GetAxis("Vertical");

        // vetor de movimento
        Vector3 moveDir = new Vector3(0, 0, inputZ).normalized;

        if (moveDir.magnitude > 0.01f)
        {
            // deslocamento
            float distance = speed * Time.deltaTime;

            // movimenta
            transform.position += moveDir * distance;

            // angulo proporcional a distancia percorrida
            float angle = (distance / (2 * Mathf.PI * radius)) * 360f;

            // eixo de rotacao = perpendicular ao movimento
            Vector3 rotationAxis = Vector3.Cross(Vector3.up, moveDir);

            // rotaciona a esfera
            transform.Rotate(rotationAxis, angle, Space.World);
        }
    }
}
