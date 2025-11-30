using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider sliderMaster;
    public Slider sliderMusica;
    public Slider sliderEfeitos;

    const string PP_MASTER = "PP_VolumeMaster";
    const string PP_MUSICA = "PP_VolumeMusica";
    const string PP_EFEITOS = "PP_VolumeEfeitos";

    void Start()
    {
        // 1) Carrega valores salvos (padrão = 1f)
        float vMaster = PlayerPrefs.GetFloat(PP_MASTER, 1f);
        float vMusica = PlayerPrefs.GetFloat(PP_MUSICA, 1f);
        float vEfeitos = PlayerPrefs.GetFloat(PP_EFEITOS, 1f);

        // 2) Aplica no mixer
        ApplyToMixer("VolumeMaster", vMaster);
        ApplyToMixer("VolumeMusica", vMusica);
        ApplyToMixer("VolumeEfeitos", vEfeitos);

        // 3) Reflete nos sliders (sem disparar listeners ainda)
        if (sliderMaster) sliderMaster.value = vMaster;
        if (sliderMusica) sliderMusica.value = vMusica;
        if (sliderEfeitos) sliderEfeitos.value = vEfeitos;

        // 4) Só agora registra os listeners
        if (sliderMaster) sliderMaster.onValueChanged.AddListener(SetVolumeMaster);
        if (sliderMusica) sliderMusica.onValueChanged.AddListener(SetVolumeMusica);
        if (sliderEfeitos) sliderEfeitos.onValueChanged.AddListener(SetVolumeEfeitos);
    }

    void SetVolumeMaster(float volume)
    {
        ApplyToMixer("VolumeMaster", volume);
        PlayerPrefs.SetFloat(PP_MASTER, volume);
        PlayerPrefs.Save();
    }

    void SetVolumeMusica(float volume)
    {
        ApplyToMixer("VolumeMusica", volume);
        PlayerPrefs.SetFloat(PP_MUSICA, volume);
        PlayerPrefs.Save();
    }

    void SetVolumeEfeitos(float volume)
    {
        ApplyToMixer("VolumeEfeitos", volume);
        PlayerPrefs.SetFloat(PP_EFEITOS, volume);
        PlayerPrefs.Save();
    }

    // Converte [0..1] em dB e aplica no mixer (parâmetros precisam estar "Exposed" com esses nomes)
    void ApplyToMixer(string exposedParam, float linear01)
    {
        float safe = Mathf.Max(linear01, 0.0001f);
        float dB = Mathf.Log10(safe) * 20f; // 1 => 0 dB, 0.5 ~ -6dB, etc.
        mixer.SetFloat(exposedParam, dB);
    }
}
