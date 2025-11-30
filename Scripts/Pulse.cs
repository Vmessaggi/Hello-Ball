using UnityEngine;

public class Pulse : MonoBehaviour
{
    public float escalaMinima = 0.9f;  // Escala mínima
    public float escalaMaxima = 1.1f;  // Escala máxima
    public float velocidade = 1f;      // Velocidade da pulsação

    private Vector3 escalaInicial;
    private float tempo;

    void Start()
    {
        escalaInicial = transform.localScale;
    }

    void Update()
    {
        tempo += Time.deltaTime * velocidade;
        float escala = Mathf.Lerp(escalaMinima, escalaMaxima, (Mathf.Sin(tempo) + 1f) / 2f);
        transform.localScale = escalaInicial * escala;
    }
}
