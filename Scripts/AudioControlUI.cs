using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AudioControlUI : MonoBehaviour
{
    public enum Tipo { Master, Musica, SFX }
    [Header("Canal")]
    public Tipo canal;

    [Header("Referências UI")]
    public Button botaoMute;        // BtnMutarGeral (ou Música/SFX)
    public Image iconeImagem;       // o filho "Image" (ícone)
    public Sprite iconeAtivo;       // ligado
    public Sprite iconeMutado;      // mutado
    public Slider sliderVolume;     // SliderVolumeGeral/Musica/SFX

    [Header("Comportamento")]
    [Range(0f,1f)] public float valorAoDesmutar = 0.5f; // 50%
    public bool normalizarSlider01 = true; // garante min=0, max=1, wholeNumbers=false

    void Awake()
    {
        // fallback: se esquecer de arrastar o ícone, tenta achar o filho "Image"
        if (iconeImagem == null)
        {
            var tf = transform.Find("Image");
            if (tf) iconeImagem = tf.GetComponent<Image>();
        }
    }

    void OnEnable()
    {
        // Garante refs e listeners
        PrepararUI();

        // Sincroniza na próxima frame pra dar tempo do MusicManager surgir
        StartCoroutine(SyncProxFrame());
    }

    IEnumerator SyncProxFrame()
    {
        // espera até MusicManager existir (em caso de abrir Menu direto)
        float t = 0f;
        while (MusicManager.Instance == null && t < 2f)
        {
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        SincronizarDoManager();
        AtualizarIcone();
    }

    void PrepararUI()
    {
        if (normalizarSlider01 && sliderVolume != null)
        {
            sliderVolume.minValue = 0f;
            sliderVolume.maxValue = 1f;
            sliderVolume.wholeNumbers = false;
        }

        if (botaoMute != null)
        {
            botaoMute.onClick.RemoveListener(OnClickMute);
            botaoMute.onClick.AddListener(OnClickMute);
        }

        if (sliderVolume != null)
        {
            sliderVolume.onValueChanged.RemoveListener(OnSliderChanged);
            sliderVolume.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    // === Eventos ===
    void OnClickMute()
    {
        if (MusicManager.Instance == null || sliderVolume == null) return;

        float volAtual = ObterVolumeAtual();
        bool estaMutado = volAtual <= 0f;

        float novoVol = estaMutado ? valorAoDesmutar : 0f;
        AplicarVolume(novoVol);
        sliderVolume.SetValueWithoutNotify(novoVol); // move slider sem loop de evento
        AtualizarIcone();
    }

    void OnSliderChanged(float v)
    {
        if (MusicManager.Instance == null) return;

        AplicarVolume(v);
        AtualizarIcone();
    }

    // === Núcleo ===
    float ObterVolumeAtual()
    {
        if (MusicManager.Instance == null) return 0f;
        switch (canal)
        {
            case Tipo.Master: return MusicManager.Instance.GetMasterVolume();
            case Tipo.Musica: return MusicManager.Instance.GetMusicVolume();
            case Tipo.SFX:    return MusicManager.Instance.GetSfxVolume();
        }
        return 0f;
    }

    void AplicarVolume(float v)
    {
        v = Mathf.Clamp01(v);
        switch (canal)
        {
            case Tipo.Master: MusicManager.Instance.SetMasterVolume(v); break;
            case Tipo.Musica: MusicManager.Instance.SetMusicVolume (v); break;
            case Tipo.SFX:    MusicManager.Instance.SetSfxVolume   (v); break;
        }
    }

    void SincronizarDoManager()
    {
        if (MusicManager.Instance == null || sliderVolume == null) return;

        float v = ObterVolumeAtual();
        sliderVolume.SetValueWithoutNotify(v); // evita loop
    }

    public void AtualizarIcone()
    {
        if (iconeImagem == null) return;
        bool mutado = ObterVolumeAtual() <= 0f;
        iconeImagem.sprite = mutado ? iconeMutado : iconeAtivo;
    }
}
