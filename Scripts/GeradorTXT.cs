using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GeradorTXT : MonoBehaviour
{
    [Header("Comportamento")]
    [Tooltip("Se true, abre a pasta Downloads após salvar.")]
    [SerializeField] private bool abrirPastaAoSalvar = false;

    [Tooltip("Se true, limpa a lista de respostas após salvar.")]
    [SerializeField] private bool limparRespostasAposSalvar = true;

    [Tooltip("Se true, adiciona data/hora no nome do arquivo.")]
    [SerializeField] private bool prefixarDataHoraNoArquivo = true;

    [Header("PDF (opcional)")]
    [Tooltip("Se true, tenta converter o TXT em PDF usando um executável no StreamingAssets.")]
    [SerializeField] private bool chamarConversorPdf = false;

    [Tooltip("Nome do executável dentro de StreamingAssets (ex.: ConversorParaPDF.exe)")]
    [SerializeField] private string nomeExecutavelPdf = "ConversorParaPDF.exe";

    private string downloads;
    private string ultimoCaminhoTXT;

    void Start()
    {
        downloads = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "/Downloads";
    }

    /// <summary>
    /// Salva o TXT e retorna o caminho do arquivo salvo.
    /// Também grava em PlayerPrefs o caminho do último TXT.
    /// </summary>
    /// <param name="abrirPasta">Se true, abre a pasta Downloads após salvar.</param>
    public string GerarTXT(bool abrirPasta = false)
    {
        if (RegistroRespostas.instancia == null)
        {
            Debug.LogWarning("[GeradorTXT] RegistroRespostas.instancia é nulo.");
            return null;
        }

        string nomeJogador = string.IsNullOrWhiteSpace(RegistroRespostas.instancia.nomeJogador)
            ? "Jogador"
            : RegistroRespostas.instancia.nomeJogador;

        string stamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
        string baseName = prefixarDataHoraNoArquivo
            ? $"{nomeJogador}_respostas_{stamp}.txt"
            : $"{nomeJogador}_respostas.txt";

        string caminho = Path.Combine(downloads, baseName);

        var linhas = new List<string>();
        linhas.Add("Nome do Jogador: " + nomeJogador);

        var inicioSessao = RegistroRespostas.instancia.inicioSessao;
        linhas.Add("Início da sessão: " + inicioSessao.ToString("dd/MM/yyyy HH:mm"));
        linhas.Add("Exportado em: " + System.DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
        linhas.Add("Respostas:");

        // Respostas acumuladas
        if (RegistroRespostas.instancia.respostas != null)
            linhas.AddRange(RegistroRespostas.instancia.respostas);

        // Grava tudo (cria/overwrite)
        File.WriteAllLines(caminho, linhas);

        // Guarda para outros scripts (popup/menu/conversor)
        PlayerPrefs.SetString("UltimoCaminhoTXT", caminho);
        PlayerPrefs.Save();
        ultimoCaminhoTXT = caminho;

        // Abrir pasta?
        bool deveAbrir = abrirPasta || abrirPastaAoSalvar;
        if (deveAbrir)
            Application.OpenURL("file://" + downloads);

        // Limpar memória de respostas (opcional)
        if (limparRespostasAposSalvar && RegistroRespostas.instancia.respostas != null)
            RegistroRespostas.instancia.respostas.Clear();

        Debug.Log("[GeradorTXT] Arquivo salvo em: " + caminho);

        // (Opcional) Tentar converter em PDF
        if (chamarConversorPdf)
        {
            string caminhoExe = Path.Combine(Application.streamingAssetsPath, nomeExecutavelPdf);
            if (File.Exists(caminhoExe) && File.Exists(caminho))
            {
                try
                {
                    var startInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = caminhoExe,
                        Arguments = $"\"{caminho}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    System.Diagnostics.Process.Start(startInfo);
                    Debug.Log("[GeradorTXT] Conversor PDF chamado.");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("[GeradorTXT] Falha ao chamar conversor PDF: " + e.Message);
                }
            }
            else
            {
                Debug.LogWarning("[GeradorTXT] Conversor não encontrado ou TXT ausente.");
            }
        }

        return caminho;
    }

    // Atalhos p/ botões (podem ser ligados direto no OnClick)
    public void GerarTXTSemAbrir() => GerarTXT(false);
    public void GerarTXTAbrindoPasta() => GerarTXT(true);

    // --- Getters para o pop-up/menu ---
    public string GetPastaDownloads() => downloads;
    public string GetUltimoCaminhoTXT()
        => string.IsNullOrEmpty(ultimoCaminhoTXT) ? PlayerPrefs.GetString("UltimoCaminhoTXT", "") : ultimoCaminhoTXT;
}
