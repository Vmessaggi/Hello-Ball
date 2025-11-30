using UnityEngine;

public class FaseLoader : MonoBehaviour
{
    private string faseSelecionada;
    private FaseData dadosFase;

    void Awake()
    {
        faseSelecionada = PlayerPrefs.GetString("FaseSelecionada", "Fase11");
    }

    void Start()
    {
        CarregarFase();
    }

    void CarregarFase()
    {
        TextAsset json = Resources.Load<TextAsset>("FasesConfig");
        FasesConfig config = JsonUtility.FromJson<FasesConfig>(json.text);

        dadosFase = config.fases.Find(f => f.nomeFase == faseSelecionada);

        if (dadosFase != null)
        {
            // REGISTRA o início da fase
            if (RegistroRespostas.instancia != null)
            {
                RegistroRespostas.instancia.RegistrarFase(dadosFase.nomeFase);
            }
            else
            {
                Debug.LogWarning("RegistroRespostas não encontrado na cena!");
            }

            // Instancia o prefab
            GameObject prefab = Resources.Load<GameObject>("Prefabs/" + dadosFase.prefab);
            GameObject instancia = Instantiate(prefab, Vector3.zero, Quaternion.identity);

            MapGenerator generator = instancia.AddComponent<MapGenerator>();
            generator.Configurar(dadosFase);
        }
        else
        {
            Debug.LogError("Fase não encontrada: " + faseSelecionada);
        }
    }
}
