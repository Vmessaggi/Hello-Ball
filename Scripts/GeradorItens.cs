using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeradorItens : MonoBehaviour
{
    public GameObject prefabMoeda;
    public GameObject[] prefabsObstaculos;
    public Transform[] lanes; // esquerda, meio, direita

    public float distanciaMinPlaca = 18f;
    private List<float> posicoesPlacasZ = new List<float>();

    private float inicioZ = 10f;
    private float fimZ = 100f;

    void Start()
    {
        CarregarDadosDaFase();

        HashSet<string> ocupados = new HashSet<string>();
        bool obstaculoAnterior = false;

        for (float z = inicioZ; z < fimZ; z += 2f)
        {
            if (PertoDePlaca(z)) continue;

            List<int> faixasDisponiveis = new List<int> { 0, 1, 2 };
            int numItensParaGerar = Random.Range(1, 3); // até 2 itens por z
            bool obstaculoGeradoNesteZ = false;

            for (int i = 0; i < numItensParaGerar && faixasDisponiveis.Count > 0; i++)
            {
                int indexFaixa = Random.Range(0, faixasDisponiveis.Count);
                int faixa = faixasDisponiveis[indexFaixa];
                faixasDisponiveis.RemoveAt(indexFaixa);

                string chave = $"{faixa}_{z}";
                if (ocupados.Contains(chave)) continue;

                int chance = Random.Range(0, 100);

                if (chance < 50)
                {
                    GerarMoeda(z, faixa);
                }
                else
                {
                    if (!obstaculoAnterior && !obstaculoGeradoNesteZ)
                    {
                        GerarObstaculo(z, faixa);
                        obstaculoGeradoNesteZ = true;
                    }
                    else
                    {
                        GerarMoeda(z, faixa); // substitui obstáculo por moeda
                    }
                }

                ocupados.Add(chave);
            }

            obstaculoAnterior = obstaculoGeradoNesteZ;
        }
    }

    void CarregarDadosDaFase()
    {
        string faseSelecionada = PlayerPrefs.GetString("FaseSelecionada", "Fase1");
        TextAsset json = Resources.Load<TextAsset>("FasesConfig");
        FasesConfig config = JsonUtility.FromJson<FasesConfig>(json.text);
        FaseData dadosFase = config.fases.Find(f => f.nomeFase == faseSelecionada);

        if (dadosFase == null)
        {
            Debug.LogError("Fase não encontrada: " + faseSelecionada);
            return;
        }

        int numeroDeTrechos = dadosFase.numeroDeTrechos;
        fimZ = numeroDeTrechos * 108f;

        posicoesPlacasZ.Clear();
        for (int i = 1; i <= numeroDeTrechos; i++)
        {
            posicoesPlacasZ.Add(i * 108f);
        }

        Debug.Log("Posições de placas carregadas: " + string.Join(", ", posicoesPlacasZ));
    }

    bool PertoDePlaca(float z)
    {
        foreach (float posZ in posicoesPlacasZ)
        {
            if (z >= posZ - distanciaMinPlaca && z < posZ+10)
                return true;
        }
        return false;
    }

    void GerarMoeda(float z, int faixa)
    {
        Vector3 pos = new Vector3(lanes[faixa].position.x, 1f, z);
        Instantiate(prefabMoeda, pos, Quaternion.identity);
    }

    void GerarObstaculo(float z, int faixa)
    {
        GameObject obst = prefabsObstaculos[Random.Range(0, prefabsObstaculos.Length)];
        Vector3 pos = new Vector3(lanes[faixa].position.x, -0.15f, z);
        Instantiate(obst, pos, Quaternion.identity);
    }
}
