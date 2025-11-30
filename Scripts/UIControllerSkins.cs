using UnityEngine;
using TMPro;

public class UIControllerSkins : MonoBehaviour
{
    public TextMeshProUGUI moedasText;

    void Start()
    {
        AtualizarMoedas();
    }

    public void AtualizarMoedas()
    {
        int moedas = PlayerPrefs.GetInt("Moedas", 0); // Pega moedas salvas
        moedasText.text = "Moedas: " + moedas.ToString();
    }
}
