using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkinSelector : MonoBehaviour
{
    [Header("Setup")]
    public Material materialParaAplicar;
    public GameObject jogador;
    public int precoDaSkin = 50;

    [Header("UI")]
    public Button botao;            // Botão "Comprar / Selecionar"
    public TextMeshProUGUI precoUI; // Texto amarelo com preço

    private string nomeSkin;
    private string nomeJogador;
    private UIControllerSkins uiController;
    private Renderer rendererDoJogador;

    // Chaves de PlayerPrefs (por jogador)
    private string KeySkinSelecionada => $"SkinSelecionada_{nomeJogador}";
    private string KeySkinDesbloqueada => $"SkinDesbloqueada_{nomeJogador}_{nomeSkin}";

    void Awake()
    {
        // Nome do jogador precisa existir antes de montar as chaves
        nomeJogador = PlayerPrefs.GetString("NomeJogador", "Default");
        nomeSkin = materialParaAplicar != null ? materialParaAplicar.name : "SkinSemNome";
    }

    void Start()
    {
        uiController = FindObjectOfType<UIControllerSkins>();

        // Cache do renderer (tenta no objeto e nos filhos)
        if (jogador != null)
        {
            rendererDoJogador = jogador.GetComponent<Renderer>();
            if (rendererDoJogador == null)
                rendererDoJogador = jogador.GetComponentInChildren<Renderer>();
        }

        // Ao entrar na cena, se esta for a skin salva, aplica automaticamente
        AplicarSeEstaForASkinSalva();

        // Atualiza UI conforme estado atual (bloqueada/desbloqueada/selecionada)
        AtualizarUI();
    }

    void OnEnable()
    {
        // Se voltar para a cena, re-sincroniza UI
        AtualizarUI();
    }

    public void AplicarMaterial()
    {
        int moedas = PlayerPrefs.GetInt("Moedas", 0);

        // Se ainda não está desbloqueada, tenta comprar
        if (!SkinDesbloqueada())
        {
            if (moedas < precoDaSkin)
            {
                Debug.Log("Moedas insuficientes para desbloquear esta skin.");
                return;
            }

            moedas -= precoDaSkin;
            PlayerPrefs.SetInt("Moedas", moedas);
            SaveManager.SalvarMoedas(nomeJogador, moedas);

            PlayerPrefs.SetInt(KeySkinDesbloqueada, 1);
            Debug.Log($"Skin {nomeSkin} desbloqueada.");

            if (uiController != null)
                uiController.AtualizarMoedas();
        }

        // Seleciona esta skin e aplica no jogador
        SelecionarEstaSkin();

        Debug.Log($"Skin {nomeSkin} aplicada.");
        AtualizarUI();
    }

    private void SelecionarEstaSkin()
    {
        // Aplica material no jogador
        AplicarMaterialNoRenderer();

        // Salva a seleção por jogador
        PlayerPrefs.SetString(KeySkinSelecionada, nomeSkin);
        PlayerPrefs.Save();

        var todos = FindObjectsOfType<SkinSelector>(includeInactive: true);
        foreach (var s in todos)
            s.AtualizarUI();
    }

    private void AplicarSeEstaForASkinSalva()
    {
        string skinSalva = PlayerPrefs.GetString(KeySkinSelecionada, string.Empty);
        if (!string.IsNullOrEmpty(skinSalva) && skinSalva == nomeSkin)
        {
            AplicarMaterialNoRenderer();
        }
    }

    private void AplicarMaterialNoRenderer()
    {
        if (rendererDoJogador == null)
        {
            Debug.LogWarning("Renderer do jogador não encontrado. Verifique a referência do 'jogador'.");
            return;
        }

        if (materialParaAplicar == null)
        {
            Debug.LogWarning("Material da skin não definido.");
            return;
        }

        rendererDoJogador.material = materialParaAplicar;
    }

    public void AtualizarUI()
    {
        if (botao == null || precoUI == null) return;

        TextMeshProUGUI textoBotao = botao.GetComponentInChildren<TextMeshProUGUI>();

        bool desbloqueada = SkinDesbloqueada();
        bool selecionada = SkinEstaSelecionada();

        if (desbloqueada)
        {
            if (selecionada)
            {
                // Já selecionada
                precoUI.text = "Desbloqueada";
                precoUI.fontSize = 15;

                textoBotao.text = "Selecionada";
                textoBotao.fontSize = 24;
                botao.interactable = false; // evita clicar de novo
            }
            else
            {
                // Desbloqueada, mas não selecionada
                precoUI.text = "Desbloqueada";
                precoUI.fontSize = 20;

                textoBotao.text = "Selecionar";
                textoBotao.fontSize = 24;
                botao.interactable = true;
            }
        }
        else
        {
            // Ainda bloqueada → mostrar preço e permitir compra
            precoUI.text = $"{precoDaSkin} Moedas";
            textoBotao.text = "Comprar";
            textoBotao.fontSize = 25;
            botao.interactable = true;
        }
    }

    private bool SkinDesbloqueada()
    {
        return PlayerPrefs.GetInt(KeySkinDesbloqueada, 0) == 1;
    }

    private bool SkinEstaSelecionada()
    {
        string skinSalva = PlayerPrefs.GetString(KeySkinSelecionada, string.Empty);
        return !string.IsNullOrEmpty(skinSalva) && skinSalva == nomeSkin;
    }
}
