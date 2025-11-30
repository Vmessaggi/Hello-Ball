using UnityEngine;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class ExportadorPDF : MonoBehaviour
{
    public void GerarPDF()
    {
        string nomeJogador = PlayerPrefs.GetString("NomeJogador", "Default");
        string pastaDownloads = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile), "Downloads");

        // 1) Prioriza o ultimo TXT salvo pelo GeradorTXT
        string caminhoTxt = PlayerPrefs.GetString("UltimoCaminhoTXT", "");

        // 2) Se nao houver, tenta o padrao antigo
        if (string.IsNullOrEmpty(caminhoTxt))
        {
            string padraoAntigo = Path.Combine(pastaDownloads, nomeJogador + "_respostas.txt");
            if (File.Exists(padraoAntigo))
            {
                caminhoTxt = padraoAntigo;
            }
            else
            {
                // 3) Como fallback, pega o mais recente do padrao nome_respostas*.txt
                var candidatos = Directory.GetFiles(pastaDownloads, $"{nomeJogador}_respostas*.txt");
                if (candidatos.Length > 0)
                    caminhoTxt = candidatos.OrderByDescending(File.GetLastWriteTime).First();
            }
        }

        string caminhoExe = Path.Combine(Application.streamingAssetsPath, "ConversorParaPDF.exe");

        UnityEngine.Debug.Log("Chamando Conversor: " + caminhoExe);
        UnityEngine.Debug.Log("Arquivo TXT: " + caminhoTxt);

        if (File.Exists(caminhoExe) && File.Exists(caminhoTxt))
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = caminhoExe,
                Arguments = $"\"{caminhoTxt}\"", // o conversor gera o PDF ao lado do TXT
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(startInfo);
        }
        else
        {
            UnityEngine.Debug.LogError("Erro: EXE ou TXT nao encontrado.");
        }
    }
}
