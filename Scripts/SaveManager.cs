using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DadosJogador
{
    public string nome;
    public int moedas;
}

[System.Serializable]
public class ListaDados
{
    public List<DadosJogador> jogadores = new List<DadosJogador>();
}

public static class SaveManager
{
    private static string caminho = Application.persistentDataPath + "/moedas.json";

    public static void SalvarMoedas(string nome, int moedas)
    {
        ListaDados lista = CarregarTodos();
        DadosJogador jogador = lista.jogadores.Find(j => j.nome == nome);

        if (jogador != null)
            jogador.moedas = moedas;
        else
            lista.jogadores.Add(new DadosJogador { nome = nome, moedas = moedas });

        string json = JsonUtility.ToJson(lista, true);
        File.WriteAllText(caminho, json);
    }

    public static int CarregarMoedas(string nome)
    {
        ListaDados lista = CarregarTodos();
        DadosJogador jogador = lista.jogadores.Find(j => j.nome == nome);
        return jogador != null ? jogador.moedas : 0;
    }

    private static ListaDados CarregarTodos()
    {
        if (!File.Exists(caminho))
            return new ListaDados();

        string json = File.ReadAllText(caminho);
        return JsonUtility.FromJson<ListaDados>(json);
    }
}
