using UnityEngine;
using TMPro;

public class PopupRelatorio : MonoBehaviour
{
    [Header("Referências")]
    public GameObject root;              // o Panel do pop-up
    public TextMeshProUGUI mensagem;     // texto da mensagem

    private string pastaDownloads;

    public void Show(string pasta, string arquivo)
    {
        pastaDownloads = pasta;
        if (mensagem != null)
        {
            mensagem.text =
                "Relatório salvo com sucesso!\n\n" +
                "Arquivo:\n" + arquivo + "\n\n" +
                "Pasta:\n" + pasta;
        }

        Time.timeScale = 0f; // pausa enquanto o pop-up está aberto
        root.SetActive(true);
    }

    public void AbrirPasta()
    {
        if (!string.IsNullOrEmpty(pastaDownloads))
        {
            Application.OpenURL("file://" + pastaDownloads);
        }
    }

    public void ConfirmarESair()
    {
        Time.timeScale = 1f;

        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
