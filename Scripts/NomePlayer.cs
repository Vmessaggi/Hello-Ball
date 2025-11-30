using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NomePlayer : MonoBehaviour
{
    public TMP_InputField Nome;

    public void SalvarNome()
    {
        string nome = Nome.text;

        if (string.IsNullOrEmpty(nome))
        {
            Debug.Log("O nome não pode ser vazio!");
            return;
        }

        PlayerPrefs.SetString("NomeJogador", nome);

        int moedas = SaveManager.CarregarMoedas(nome);
        PlayerPrefs.SetInt("Moedas", moedas);

        PlayerPrefs.Save();
        Debug.Log($"Nome salvo: {nome}, Moedas carregadas: {moedas}");
        SceneManager.LoadScene("Menu");
    }

    public void SairDoJogo()
    {
        Debug.Log("Saindo do jogo...");

        Application.Quit();

    #if UNITY_EDITOR
            // Isso faz parar o modo Play quando estiver testando dentro da Unity
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}
