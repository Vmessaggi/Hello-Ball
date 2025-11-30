using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    PlayerInput playerinput;
    [SerializeField] Transform[] LaneTransforms;
    Vector3 Destination;

    int currentLaneIndex;
    public float forwardSpeed = 7f;
    public float lateralForce = 400f;
    public float jumpForce = 7f;

    Rigidbody rb;
    bool isGrounded = true;

    public bool aguardandoResposta = false;
    private string respostaCorretaAtual = "";

    private bool bloqueadoAposResposta = false;
    private float zInicialBloqueio = 0f;
    private float distanciaParaDesbloquear = 16f;

    public string faixaAtual;

    private int tentativaAtual = 1; // (mantido se quiser usar em UI)
    private float lastX;

    public int moedas = 0;
    public string nomeJogador;

    private float velocidadeOriginal;
    [SerializeField] private GameObject FumacaPrefab;

    public AudioClip somBatida;
    public AudioClip somPulo;
    public AudioClip somErro;
    public AudioClip somAcerto;
    private AudioSource audioSource;

    public TextMeshProUGUI textoMoedasFase;
    private int moedasDuranteFase = 0;

    [Header("PowerUp")]
    [SerializeField] private Slider powerUpBar;
    [SerializeField] private float taxaDeCarga = 10f;
    [SerializeField] private float duracaoInvulneravel = 5f;

    private float valorPowerUp = 0f;
    private bool invulneravel = false;

    private void OnEnable()
    {
        if (playerinput == null)
            playerinput = new PlayerInput();

        playerinput.Enable();
    }

    private void OnDisable()
    {
        playerinput.Disable();
    }

    void Start()
    {
        AtualizarTextoMoedas();
        rb = GetComponent<Rigidbody>();
        velocidadeOriginal = forwardSpeed;

        playerinput.Gameplay.Move.performed += MovePerformed;
        playerinput.Gameplay.Jump.performed += JumpPerformed;

        audioSource = GetComponent<AudioSource>();

        for (int i = 0; i < LaneTransforms.Length; i++)
        {
            if (Mathf.Approximately(LaneTransforms[i].position.x, transform.position.x))
            {
                currentLaneIndex = i;
                Destination = LaneTransforms[i].position;
            }
        }

        AtualizarFaixaAtual();
        lastX = transform.position.x;

        nomeJogador = PlayerPrefs.GetString("NomeJogador", "Visitante");
        moedas = SaveManager.CarregarMoedas(nomeJogador);
    }

    private void MovePerformed(InputAction.CallbackContext obj)
    {
        if (aguardandoResposta || bloqueadoAposResposta) return;

        float InputValue = obj.ReadValue<float>();
        if (InputValue > 0) MoveRight();
        else if (InputValue < 0) MoveLeft();
    }

    private void JumpPerformed(InputAction.CallbackContext obj)
    {
        if (isGrounded && !aguardandoResposta)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            if (somPulo != null && audioSource != null)
                audioSource.PlayOneShot(somPulo);
        }
    }

    private void MoveLeft()
    {
        if (currentLaneIndex == 0) return;
        currentLaneIndex--;
        Destination = LaneTransforms[currentLaneIndex].position;
        AtualizarFaixaAtual();
    }

    private void MoveRight()
    {
        if (currentLaneIndex == LaneTransforms.Length - 1) return;
        currentLaneIndex++;
        Destination = LaneTransforms[currentLaneIndex].position;
        AtualizarFaixaAtual();
    }

    public void AdicionarMoeda()
    {
        moedasDuranteFase++;
        moedas++;

        // Atualiza o total salvo
        int total = PlayerPrefs.GetInt("Moedas", 0) + 1;
        PlayerPrefs.SetInt("Moedas", total);
        PlayerPrefs.Save();

        // Salva no JSON para persistir após fechar o jogo
        string nome = PlayerPrefs.GetString("NomeJogador", "Default");
        SaveManager.SalvarMoedas(nome, total);

        AtualizarTextoMoedas();
    }

    public void AtualizarTextoMoedas()
    {
        if (textoMoedasFase != null)
            textoMoedasFase.text = "Moedas: " + moedasDuranteFase;
    }

    private void AtualizarFaixaAtual()
    {
        if (currentLaneIndex == 0) faixaAtual = "A";
        else if (currentLaneIndex == 1) faixaAtual = "W";
        else if (currentLaneIndex == 2) faixaAtual = "D";
    }

    void Update()
    {
        // Atualiza a barra de PowerUp com o tempo
        if (!invulneravel)
        {
            valorPowerUp += taxaDeCarga * Time.deltaTime;
            valorPowerUp = Mathf.Clamp(valorPowerUp, 0f, 100f);
        }

        if (powerUpBar != null)
        {
            powerUpBar.value = Mathf.Round(valorPowerUp);

            if (powerUpBar.fillRect != null)
            {
                Image fillImage = powerUpBar.fillRect.GetComponent<Image>();
                if (fillImage != null)
                {
                    if (valorPowerUp >= 100f && !invulneravel)
                    {
                        float piscar = Mathf.PingPong(Time.time * 4f, 1f);
                        fillImage.color = Color.Lerp(Color.cyan, Color.white, piscar);
                    }
                    else if (invulneravel)
                    {
                        fillImage.color = Color.cyan;
                    }
                    else
                    {
                        fillImage.color = Color.green;
                    }
                }
            }
        }

        // Ativa invulnerabilidade com tecla P quando cheio
        if (Keyboard.current.pKey.wasPressedThisFrame && !invulneravel && valorPowerUp >= 100f)
        {
            StartCoroutine(AtivarInvulnerabilidade());
        }

        if (aguardandoResposta)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (Keyboard.current.aKey.wasPressedThisFrame) ProcessarResposta("A", 0);
            else if (Keyboard.current.wKey.wasPressedThisFrame) ProcessarResposta("W", 1);
            else if (Keyboard.current.dKey.wasPressedThisFrame) ProcessarResposta("D", 2);
            return;
        }

        if (bloqueadoAposResposta)
        {
            float distanciaPercorrida = transform.position.z - zInicialBloqueio;
            if (distanciaPercorrida >= distanciaParaDesbloquear)
                bloqueadoAposResposta = false;
        }

        if (!aguardandoResposta)
        {
            transform.position += Vector3.forward * forwardSpeed * Time.deltaTime;

            float circumference = 2 * Mathf.PI * (transform.localScale.x / 2f);
            float distanceZ = forwardSpeed * Time.deltaTime;
            float rotationAngleX = distanceZ * 360f / circumference;

            // Apenas rotação no eixo X (frente)
            transform.Rotate(rotationAngleX, 0f, 0f);
            lastX = transform.position.x;

            if (currentLaneIndex >= 0 && currentLaneIndex < LaneTransforms.Length)
            {
                Vector3 target = new Vector3(
                    LaneTransforms[currentLaneIndex].position.x,
                    transform.position.y,
                    transform.position.z
                );
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 5f);
            }
        }
    }

    private IEnumerator AtivarInvulnerabilidade()
    {
        invulneravel = true;
        Debug.Log("JOGADOR INVULNERÁVEL!");

        float tempoPassado = 0f;
        float powerInicial = valorPowerUp;

        while (tempoPassado < duracaoInvulneravel)
        {
            tempoPassado += Time.deltaTime;
            float t = tempoPassado / duracaoInvulneravel;

            valorPowerUp = Mathf.Lerp(powerInicial, 0f, t);

            if (powerUpBar != null)
                powerUpBar.value = Mathf.Round(valorPowerUp);

            yield return null;
        }

        valorPowerUp = 0f;
        invulneravel = false;
    }

    private void ProcessarResposta(string faixaTecla, int faixaIndex)
    {
        // Registra tentativa (certa ou errada)
        if (RegistroRespostas.instancia != null && !string.IsNullOrEmpty(respostaCorretaAtual))
        {
            RegistroRespostas.instancia.RegistrarRespostaTentativa(faixaTecla);
        }

        if (faixaTecla == respostaCorretaAtual)
        {
            Debug.Log("Acertou a resposta!");
            if (somAcerto != null && audioSource != null)
                audioSource.PlayOneShot(somAcerto);

            // Fecha a pergunta (grava 1/n e tempo)
            if (RegistroRespostas.instancia != null)
                RegistroRespostas.instancia.FinalizarPerguntaAcertada();

            ContinuarComFaixa(faixaIndex);
            tentativaAtual = 1;
        }
        else
        {
            Debug.Log("Errou a resposta!");
            if (somErro != null && audioSource != null)
                audioSource.PlayOneShot(somErro);
            tentativaAtual++;
        }
    }

    private void ContinuarComFaixa(int faixaIndex)
    {
        currentLaneIndex = faixaIndex;
        AtualizarFaixaAtual();
        aguardandoResposta = false;
        bloqueadoAposResposta = true;
        zInicialBloqueio = transform.position.z;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }

        if (collision.gameObject.CompareTag("Obstáculo"))
        {
            if (invulneravel)
            {
                Destroy(collision.gameObject);
                return;
            }

            forwardSpeed *= 0.5f;
            MusicManager.PlaySFX(somBatida, transform.position);

            if (FumacaPrefab != null)
                Instantiate(FumacaPrefab, transform.position, Quaternion.identity);

            Destroy(collision.gameObject);
            Invoke("RestaurarVelocidade", 2f);
        }
    }

    void RestaurarVelocidade()
    {
        forwardSpeed = velocidadeOriginal;
    }

    void OnTriggerEnter(Collider other)
    {
        // Início da pergunta (placa)
        PerguntaTrigger pergunta = other.GetComponent<PerguntaTrigger>();
        if (pergunta != null)
        {
            aguardandoResposta = true;
            respostaCorretaAtual = pergunta.respostaCorreta;
            Debug.Log("Parou para responder. Resposta correta: " + respostaCorretaAtual);

            // Inicia cronômetro / zera tentativas no registrador
            if (RegistroRespostas.instancia != null)
                RegistroRespostas.instancia.IniciarPergunta(respostaCorretaAtual);

            return;
        }
    }
}
