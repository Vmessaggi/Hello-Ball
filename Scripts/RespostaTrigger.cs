using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RespostaTrigger : MonoBehaviour
{
    [Tooltip("Letra desta faixa/opção: A / W / D (ou o que você usa)")]
    public string minhaResposta;

    [SerializeField] private GameObject Estrelas; // efeito de acerto (opcional)

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Só registra se existe uma pergunta ativa
        if (RegistroRespostas.instancia == null || 
            string.IsNullOrEmpty(RegistroRespostas.instancia.respostaCorretaAtual))
            return;

        // Conta tentativa
        RegistroRespostas.instancia.RegistrarRespostaTentativa(minhaResposta);

        bool acertou = (minhaResposta == RegistroRespostas.instancia.respostaCorretaAtual);

        if (acertou)
        {
            RegistroRespostas.instancia.FinalizarPerguntaAcertada();

            // libera o jogador, se usar essa flag
            var player = other.GetComponent<Player>();
            if (player != null) player.aguardandoResposta = false;

            // efeito de acerto
            if (Estrelas != null)
            {
                var fx = Instantiate(Estrelas, transform.position, Quaternion.identity);
                Destroy(fx, 2f);
            }

            // opcional: remover as 3 opções da placa
            // Destroy(transform.parent.gameObject);
        }
        else
        {
            // efeito/áudio de erro (opcional)
        }
    }
}
