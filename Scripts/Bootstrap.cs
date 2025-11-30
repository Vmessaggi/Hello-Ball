using UnityEngine;
using System.IO;

public class Bootstrap : MonoBehaviour
{
    void Awake()
    {
        // Nome padrão se vazio
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("NomeJogador", "")))
            PlayerPrefs.SetString("NomeJogador", "Jogador");

        // Moedas padrão
        if (!PlayerPrefs.HasKey("Moedas"))
            PlayerPrefs.SetInt("Moedas", 0);

        // Garante pasta/arquivo de save acessíveis (opcional)
        var jsonPath = Application.persistentDataPath + "/moedas.json";
        if (!File.Exists(jsonPath))
            File.WriteAllText(jsonPath, "{\"jogadores\":[]}");
        
        PlayerPrefs.Save();
    }
}
