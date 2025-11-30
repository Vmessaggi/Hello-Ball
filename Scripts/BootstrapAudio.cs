using UnityEngine;

public class BootstrapAudio : MonoBehaviour
{
    // Executa automaticamente antes de carregar qualquer cena
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnsureMusicManager()
    {
        // Já existe? Então não faz nada
        if (MusicManager.Instance != null)
            return;

        // Tenta carregar o prefab do MusicManager na pasta Resources
        GameObject prefab = Resources.Load<GameObject>("MusicManager");

        if (prefab != null)
        {
            GameObject instancia = Object.Instantiate(prefab);
            instancia.name = "MusicManager (Bootstrap)";
            Debug.Log("[BootstrapAudio] MusicManager carregado automaticamente a partir de Resources.");
        }
        else
        {
            Debug.LogWarning("[BootstrapAudio] Nenhum prefab 'MusicManager' encontrado em Resources!");
        }
    }
}
