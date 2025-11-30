using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class RegistroRespostas : MonoBehaviour
{
    public static RegistroRespostas instancia;

    public string nomeJogador;
    public List<string> respostas = new List<string>();

    // Sessão (para carimbo no TXT)
    public System.DateTime inicioSessao { get; private set; } = System.DateTime.MinValue;

    // Estado da fase atual (buffer)
    private string faseIdAtual = null;
    private string nomeFaseAtual = null;
    private bool faseEmAndamento = false;
    private readonly List<string> bufferFase = new List<string>();
    private int numeroPerguntaFase = 1;

    // Estado da pergunta atual
    private float inicioPerguntaTime = -1f;
    private int tentativasPerguntaAtual = 0;
    public string respostaCorretaAtual { get; private set; } = null;

    [Header("Formato")]
    [SerializeField] private int larguraMaxCabecalho = 60; // quebra de linha se exceder (0 = desativa)

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
            if (inicioSessao == System.DateTime.MinValue)
                inicioSessao = System.DateTime.Now;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        nomeJogador = PlayerPrefs.GetString("NomeJogador", "Jogador");
        Debug.Log("[RegistroRespostas] Nome carregado: " + nomeJogador);
    }

    // ===== FASE =====

    // Chame 1x quando a fase for carregada
    public void RegistrarFase(string faseId)
    {
        faseIdAtual = faseId;
        nomeFaseAtual = ObterNomeFaseFormatado(faseId);
        faseEmAndamento = true;

        bufferFase.Clear();
        numeroPerguntaFase = 1;

        inicioPerguntaTime = -1f;
        tentativasPerguntaAtual = 0;
        respostaCorretaAtual = null;

        Debug.Log($"[RegistroRespostas] Fase iniciada (buffer): {nomeFaseAtual}");
    }

    // Chame ao terminar a fase (no trigger de fim)
    public void FinalizarFase(bool concluida)
    {
        if (!faseEmAndamento) return;

        if (concluida && bufferFase.Count > 0)
        {
            string cab = "Fase: " + (larguraMaxCabecalho > 0
                          ? QuebrarLinha(nomeFaseAtual, larguraMaxCabecalho)
                          : nomeFaseAtual);
            respostas.Add(cab);
            respostas.AddRange(bufferFase);
            Debug.Log($"[RegistroRespostas] Fase CONCLUÍDA → commit de {bufferFase.Count} linhas.");
        }
        else
        {
            Debug.Log("[RegistroRespostas] Fase NÃO concluída → descartado buffer.");
        }

        // limpa estado
        faseEmAndamento = false;
        faseIdAtual = null;
        nomeFaseAtual = null;
        bufferFase.Clear();

        inicioPerguntaTime = -1f;
        tentativasPerguntaAtual = 0;
        respostaCorretaAtual = null;
    }

    // ===== PERGUNTA =====

    // OnTriggerEnter da placa
    public void IniciarPergunta(string respostaCorreta)
    {
        inicioPerguntaTime = Time.time; // use Time.unscaledTime se quiser contar em pause
        tentativasPerguntaAtual = 0;
        respostaCorretaAtual = respostaCorreta;
        Debug.Log($"[RegistroRespostas] IniciarPergunta #{numeroPerguntaFase} | correta={respostaCorreta}");
    }

    // a cada passagem numa alternativa
    public void RegistrarRespostaTentativa(string respostaJogador)
    {
        tentativasPerguntaAtual++;
        bufferFase.Add($"Pergunta {numeroPerguntaFase} tentativa {tentativasPerguntaAtual}: esperado {respostaCorretaAtual}, respondeu {respostaJogador}");
    }

    // quando acertar
    public void FinalizarPerguntaAcertada()
    {
        int n = (tentativasPerguntaAtual > 0) ? tentativasPerguntaAtual : 1;
        float duracao = (inicioPerguntaTime >= 0f) ? (Time.time - inicioPerguntaTime) : 0f;
        int percentual = Mathf.RoundToInt(100f / n);

        bufferFase.Add($"Pergunta {numeroPerguntaFase} RESULTADO: 1/{n} = {percentual}% | Tempo: {duracao:0.00}s");

        // próxima pergunta (apenas dentro da fase atual)
        numeroPerguntaFase++;
        inicioPerguntaTime = -1f;
        tentativasPerguntaAtual = 0;
        respostaCorretaAtual = null;
    }

    // ===== UTIL =====

    private string ObterNomeFaseFormatado(string faseId)
    {
        switch (faseId)
        {
            case "Fase1":  return "Fase 1 - Geometria Plana: Identificação de figuras";
            case "Fase2":  return "Fase 2 - Geometria Plana: Identificação de lados";
            case "Fase3":  return "Fase 3 - Geometria Plana: Identificação de área";
            case "Fase4":  return "Fase 4 - Geometria Espacial: Identificação de sólidos";
            case "Fase5":  return "Fase 5 - Geometria Espacial: Quantidade de arestas";
            case "Fase6":  return "Fase 6 - Geometria Espacial: Identificação de volume";
            case "Fase7":  return "Fase 7 - Geometria Espacial: Poliedros (Convexos x Côncavos)"; // versão curta
            case "Fase8":  return "Fase 8 - Geometria Espacial - Poliedros e Não Poliedros";
            case "Fase9":  return "Fase 9 - Geometria Espacial - Poliedros Regulares/Irregulares";
            case "Fase10": return "Fase 10 - Geometria Espacial - Planificação de Poliedros";
            case "Fase11": return "Fase 11 - Geometria Espacial - Pirâmides";
            case "Fase12": return "Fase 12 - Geometria Espacial - Corpos Redondos";
            default: return faseId;
        }
    }

    // Quebra por espaços, com indentação na continuação
    private string QuebrarLinha(string texto, int max)
    {
        if (max <= 0 || string.IsNullOrEmpty(texto) || texto.Length <= max) return texto;

        StringBuilder sb = new StringBuilder();
        int i = 0;
        while (i < texto.Length)
        {
            int take = Mathf.Min(max, texto.Length - i);
            int end = i + take;

            if (end < texto.Length)
            {
                int lastSpace = texto.LastIndexOf(' ', end, take);
                if (lastSpace > i + (max / 3)) end = lastSpace; // evita quebrar cedo demais
            }

            sb.Append(texto.Substring(i, end - i).Trim());
            if (end < texto.Length) sb.Append("\n    "); // indent na nova linha
            i = end;

            while (i < texto.Length && texto[i] == ' ') i++;
        }
        return sb.ToString();
    }

    // (Compat) se ainda existir no seu projeto
    public void AvancarPergunta()
    {
        numeroPerguntaFase++;
        inicioPerguntaTime = -1f;
        tentativasPerguntaAtual = 0;
        respostaCorretaAtual = null;
    }
}
