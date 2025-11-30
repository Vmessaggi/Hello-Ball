using System.Collections;                 // Necessário para IEnumerator / coroutines
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;                              // <<=== para TextMeshProUGUI

public class MenuController : MonoBehaviour
{
    [Header("Painéis principais")]
    public GameObject painelFases;
    public GameObject painelConfiguracoes;
    public GameObject painelCreditos;
    public GameObject painelInstrucoes;

    [Header("Objetos do menu para esconder/mostrar")]
    public GameObject[] objetosParaEsconder;
    public GameObject[] objetosParaMostrar;

    [Header("Exportação")]
    [SerializeField] private bool gerarPdfAoSair = true;

    // -------- Painel de Explicação da Fase (novo) --------
    [Header("Painel Explicação de Fase")]
    public GameObject painelExplicacaoFase;          // painel único para todas as fases
    public TextMeshProUGUI tituloFase;               // TMP do título
    public TextMeshProUGUI descricaoFase;            // TMP da descrição
    public TextMeshProUGUI habilidadeBNCC;           // TMP da BNCC

    // cache de dados
    private FasesConfig config;                      // lido do Resources/FasesConfig.json (TextAsset)
    private FaseData faseSelecionadaData;            // dados da fase atualmente selecionada
    private string faseSelecionadaIdTemp;            // ex.: "Fase1"
    [Header("UI")]
    public PopupRelatorio popupRelatorio;
    private void Awake()
    {
        // Carrega o JSON uma vez
        TextAsset json = Resources.Load<TextAsset>("FasesConfig");
        if (json != null)
            config = JsonUtility.FromJson<FasesConfig>(json.text);
        else
            Debug.LogError("FasesConfig.json não encontrado em Resources.");
    }

    // ---------- Painel Fases ----------
    public void AbrirPainelFases()
    {
        painelFases.SetActive(true);
        foreach (GameObject obj in objetosParaEsconder)
            obj.SetActive(false);
    }

    public void FecharPainelFases()
    {
        painelFases.SetActive(false);
        foreach (GameObject obj in objetosParaEsconder)
            obj.SetActive(true);
    }

    // Chamado pelos botões "Fase 1", "Fase 2", ... (via OnClick com parâmetro string)
    public void AbrirExplicacaoFaseById(string faseId) // ex.: "Fase1"
    {
        if (config == null)
        {
            Debug.LogError("Config de fases não carregada.");
            return;
        }

        // Guarda ID temporário e busca dados
        faseSelecionadaIdTemp = faseId;
        faseSelecionadaData = config.fases.Find(f => f.nomeFase == faseId);

        if (faseSelecionadaData == null)
        {
            Debug.LogError("Fase não encontrada: " + faseId);
            return;
        }

        // Preenche UI (com fallback caso título/descrição/BNCC não estejam no JSON)
        tituloFase.text     = string.IsNullOrEmpty(faseSelecionadaData.titulo) ? faseId : faseSelecionadaData.titulo;
        descricaoFase.text  = string.IsNullOrEmpty(faseSelecionadaData.descricao) ? "Descrição indisponível." : faseSelecionadaData.descricao;
        string bncc         = string.IsNullOrEmpty(faseSelecionadaData.habilidadeBNCC) ? "—" : faseSelecionadaData.habilidadeBNCC;
        habilidadeBNCC.text = "Habilidade BNCC: " + bncc;

        // Mostra painel de explicação e esconde o painel de fases (mantém o resto escondido)
        painelFases.SetActive(false);
        painelExplicacaoFase.SetActive(true);
    }

    // Botão "Voltar" dentro do painel de explicação
    public void FecharExplicacaoFase()
    {
        painelExplicacaoFase.SetActive(false);
        painelFases.SetActive(true);  // <<=== volta para o painel de fases
    }

    // Botão "Iniciar" dentro do painel de explicação
    public void IniciarFaseSelecionada()
    {
        if (string.IsNullOrEmpty(faseSelecionadaIdTemp))
        {
            Debug.LogWarning("Nenhuma fase selecionada.");
            return;
        }

        Time.timeScale = 1f; // garante tempo normal na cena seguinte
        PlayerPrefs.SetString("FaseSelecionada", faseSelecionadaIdTemp);
        SceneManager.LoadScene("Base");
    }

    // ---------- Fluxo antigo (se quiser iniciar sem explicação) ----------
    public void SelecionarFase(string nomeFase)
    {
        Time.timeScale = 1f; // garante tempo normal na cena seguinte
        PlayerPrefs.SetString("FaseSelecionada", nomeFase);
        SceneManager.LoadScene("Base");
    }

    // ---------- Cena de Skins ----------
    public void IrParaCenaSkins()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Skins");
    }

    public void VoltarParaMenuPrincipal()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    // ---------- Painel Configurações ----------
    public void AbrirPainelConfiguracoes()
    {
        painelConfiguracoes.SetActive(true);
        foreach (GameObject obj in objetosParaEsconder)
            obj.SetActive(false);
    }

    public void FecharPainelConfiguracoes()
    {
        painelConfiguracoes.SetActive(false);
        foreach (GameObject obj in objetosParaEsconder)
            obj.SetActive(true);
    }

    // ---------- Painel Créditos ----------
    public void AbrirPainelCreditos()
    {
        painelCreditos.SetActive(true);
        foreach (GameObject obj in objetosParaEsconder)
            obj.SetActive(false);
    }

    public void FecharPainelCreditos()
    {
        painelCreditos.SetActive(false);
        foreach (GameObject obj in objetosParaEsconder)
            obj.SetActive(true);
    }

    // ---------- Painel Instruções ----------
    public void AbrirPainelInstrucoes()
    {
        painelInstrucoes.SetActive(true);
        foreach (GameObject obj in objetosParaEsconder)
            obj.SetActive(false);
    }

    public void FecharPainelInstrucoes()
    {
        painelInstrucoes.SetActive(false);
        foreach (GameObject obj in objetosParaEsconder)
            obj.SetActive(true);
    }

    // ---------- Sair ----------
    public void SairDoJogo()
    {
        StartCoroutine(SairComExportacao());
    }

    private IEnumerator SairComExportacao()
    {
        // Se o jogador sair no meio de uma fase, descartamos o buffer dessa fase
        if (RegistroRespostas.instancia != null)
            RegistroRespostas.instancia.FinalizarFase(false);

        // 1) Gera TXT silenciosamente (não abre pasta) e pega caminhos
        var gerador = FindObjectOfType<GeradorTXT>();
        string caminhoTXT = null;
        string pastaDownloads = null;

        if (gerador != null)
        {
            // IMPORTANTE: use o método que RETORNA o caminho (conforme ajustamos no GeradorTXT)
            caminhoTXT = gerador.GerarTXT(false);
            pastaDownloads = gerador.GetPastaDownloads();
        }
        else
        {
            Debug.LogWarning("GeradorTXT não encontrado na cena.");
        }

        // 1 frame para garantir a escrita no disco
        yield return null;

        // 2) Gera PDF (opcional)
        if (gerarPdfAoSair)
        {
            var exportador = FindObjectOfType<ExportadorPDF>();
            if (exportador != null) exportador.GerarPDF();
        }

        // Pequena espera em tempo real (funciona com timeScale = 0)
        yield return new WaitForSecondsRealtime(0.2f);

        // 3) Em vez de sair aqui, mostramos o POP-UP se possível
        if (popupRelatorio != null && !string.IsNullOrEmpty(pastaDownloads) && !string.IsNullOrEmpty(caminhoTXT))
        {
            popupRelatorio.Show(pastaDownloads, caminhoTXT);
            // NÃO sair agora — o PopupRelatorio.ConfirmarESair() fará o Quit
            yield break;
        }

        // 3) Fecha o app
        Application.Quit();
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}
