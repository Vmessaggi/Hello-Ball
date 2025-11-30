using UnityEngine;

public class AplicarSkinPlayer : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private string pastaResources = "SkinsPlayer"; // Assets/Resources/SkinsPlayer
    [SerializeField] private string idSkinPadrao = "SkinPadrao";    // deve existir em Resources/SkinsPlayer

    private Renderer targetRenderer;

    void Awake()
    {
        // Garante que vamos achar o renderer mesmo se estiver em um filho
        targetRenderer = GetComponentInChildren<Renderer>();
        if (targetRenderer == null)
        {
            Debug.LogError("[AplicarSkinPlayer] Renderer do player não encontrado.");
        }
    }

    void Start()
    {
        string nomeJogador = PlayerPrefs.GetString("NomeJogador", "Default");
        string idSkin = PlayerPrefs.GetString("SkinSelecionada_" + nomeJogador, idSkinPadrao);

        // Tenta carregar a skin escolhida
        Material mat = CarregarMaterial(idSkin);
        if (mat == null)
        {
            Debug.LogWarning($"[AplicarSkinPlayer] Skin '{idSkin}' não encontrada. Tentando padrão '{idSkinPadrao}'.");
            mat = CarregarMaterial(idSkinPadrao);
        }

        if (mat != null && targetRenderer != null)
        {
            // Use .material (instancia) para cada player; use .sharedMaterial se quiser compartilhar
            targetRenderer.material = mat;
            Debug.Log($"[AplicarSkinPlayer] Skin aplicada: {mat.name}");
        }
        else
        {
            Debug.LogError("[AplicarSkinPlayer] Falha ao aplicar skin (material ou renderer nulo).");
        }
    }

    private Material CarregarMaterial(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        string path = $"{pastaResources}/{id}";
        Material m = Resources.Load<Material>(path);
        if (m == null)
        {
            // Loga o que existe na pasta para diagnosticar rapidamente
            var todos = Resources.LoadAll<Material>(pastaResources);
            string lista = todos != null && todos.Length > 0
                ? string.Join(", ", System.Array.ConvertAll(todos, t => t.name))
                : "(vazio)";
            Debug.LogWarning($"[AplicarSkinPlayer] Material '{path}' não encontrado. Materiais disponíveis: {lista}");
        }
        return m;
    }
}
