using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FimFase : MonoBehaviour
{
    private bool faseTerminada = false;
    private GameMenus gameMenus;

    [Header("Comportamento")]
    [Tooltip("Pausar o jogo ao terminar a fase (Time.timeScale = 0).")]
    [SerializeField] private bool pausarAoTerminar = true;

    private void Reset()
    {
        // Garante que o collider é trigger
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void Start()
    {
        gameMenus = FindObjectOfType<GameMenus>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (faseTerminada) return;
        if (!other.CompareTag("Player")) return;

        faseTerminada = true;

        // 1) Commit da fase (agora as perguntas desta fase vão para a lista final)
        if (RegistroRespostas.instancia != null)
            RegistroRespostas.instancia.FinalizarFase(true);

        // 2) Opcional: desabilitar o controle do jogador
        var player = other.GetComponent<Player>();
        if (player != null)
            player.enabled = false;

        // 3) Mostrar menu de fim de fase (se existir)
        if (gameMenus != null)
            gameMenus.ShowEndGameMenu();
        else
            Debug.LogWarning("[FimFase] GameMenus não encontrado na cena.");

        // 4) Pausar o jogo (opcional)
        if (pausarAoTerminar)
            Time.timeScale = 0f;
    }
}
