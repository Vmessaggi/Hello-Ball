using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PerguntaTrigger : MonoBehaviour
{
    [Tooltip("Letra correta: A / W / D (ou o que você usa)")]
    public string respostaCorreta;

    private bool ativado = false;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ativado) return;
        if (!other.CompareTag("Player")) return;

        ativado = true;

        // Inicia cronômetro e zera tentativas
        RegistroRespostas.instancia.IniciarPergunta(respostaCorreta);

        // Se seu Player tem essa flag, você pode travar o movimento aqui
        var player = other.GetComponent<Player>();
        if (player != null) player.aguardandoResposta = true;
    }
}
