using UnityEngine;

public class Moeda : MonoBehaviour
{
    public float velocidadeRotacao = 90f; // graus por segundo
    public AudioClip somColeta; // arraste o som no Inspector

    void Update()
    {
        transform.Rotate(0f, velocidadeRotacao * Time.deltaTime, 0f);
    }

    void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.AdicionarMoeda();
            MusicManager.PlaySFX(somColeta, transform.position);
            Destroy(gameObject); // pode destruir imediatamente
        }
    }
}
