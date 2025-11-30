using UnityEngine;

public class DesaparecerAoTocar : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Certifique-se de que o jogador tenha a tag "Player"
        {
            Destroy(gameObject);
        }
    }
}
