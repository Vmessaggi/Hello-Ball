using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public float espacamento = 10f;
    public int comprimento = 108;
    private int numeroDeTrechos;
    private GameObject[] objetosPrefabs;

    public void Configurar(FaseData faseData)
    {
        numeroDeTrechos = faseData.numeroDeTrechos;

        // Carrega os objetos dinamicamente
        objetosPrefabs = Resources.LoadAll<GameObject>("Prefabs/Objetos");

        if (objetosPrefabs.Length == 0)
        {
            Debug.LogError("Nenhum objeto encontrado em Prefabs/Objetos!");
            return;
        }

        GerarMapa();
    }

    void GerarMapa()
    {
        int total = 0;
        if (numeroDeTrechos <= 5)
        {
            total = (int)((numeroDeTrechos * espacamento) + (numeroDeTrechos - 1));
        }
        else
        {
            total = (int)((numeroDeTrechos * espacamento) + (numeroDeTrechos - 2));
        }
        

        for (int i = 0; i < total; i++)
        {
            float zPos = i * espacamento;

            // Lado esquerdo
            GameObject objEsquerdo = objetosPrefabs[Random.Range(0, objetosPrefabs.Length)];
            Instantiate(objEsquerdo, new Vector3(-12f, 0f, zPos), Quaternion.Euler(0f, 90f, 0f), this.transform);

            // Lado direito
            GameObject objDireito = objetosPrefabs[Random.Range(0, objetosPrefabs.Length)];
            Instantiate(objDireito, new Vector3(12f, 0f, zPos), Quaternion.Euler(0f, -90f, 0f), this.transform);
        }
    }
}
