using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    private AudioSource audioSource;

    [Header("Faixas Musicais")]
    public AudioClip musicaMenu;
    public AudioClip musicaFases1_4;
    public AudioClip musicaFases5_8;
    public AudioClip musicaFases9_12;

    // Volumes (0..1)
    float masterVol, musicVol, sfxVol;

    // PlayerPrefs
    const string PP_MASTER = "Vol_Master";
    const string PP_MUSIC  = "Vol_Music";
    const string PP_SFX    = "Vol_SFX";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = true;

            // Carrega volumes salvos (padrão 1, 1, 1)
            masterVol = PlayerPrefs.HasKey(PP_MASTER) ? PlayerPrefs.GetFloat(PP_MASTER) : 1f;
            musicVol  = PlayerPrefs.HasKey(PP_MUSIC)  ? PlayerPrefs.GetFloat(PP_MUSIC)  : 1f;
            sfxVol    = PlayerPrefs.HasKey(PP_SFX)    ? PlayerPrefs.GetFloat(PP_SFX)    : 1f;

            AplicarVolumes();

            SceneManager.sceneLoaded += OnSceneLoaded;
            AtualizarMusica(); // tocar já na primeira cena
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene cena, LoadSceneMode modo)
    {
        AtualizarMusica();
        AplicarVolumes();
    }

    private void AtualizarMusica()
    {
        string cena = SceneManager.GetActiveScene().name;
        AudioClip nova = null;

        if (cena == "Base")
        {
            string fase = PlayerPrefs.GetString("FaseSelecionada", "Fase1");
            if (fase.StartsWith("Fase1") || fase.StartsWith("Fase2") || fase.StartsWith("Fase3") || fase.StartsWith("Fase4"))       nova = musicaFases1_4;
            else if (fase.StartsWith("Fase5") || fase.StartsWith("Fase6") || fase.StartsWith("Fase7") || fase.StartsWith("Fase8")) nova = musicaFases5_8;
            else                                                                                                                    nova = musicaFases9_12;
        }
        else
        {
            nova = musicaMenu;
        }

        if (nova != null && audioSource.clip != nova)
        {
            audioSource.Stop();
            audioSource.clip = nova;
            audioSource.Play();
        }
    }

    private void AplicarVolumes()
    {
        AudioListener.volume = Mathf.Clamp01(masterVol);
        if (audioSource != null) audioSource.volume = Mathf.Clamp01(musicVol);
        // sfxVol aplicado no PlaySFX
        Salvar();
    }

    private void Salvar()
    {
        PlayerPrefs.SetFloat(PP_MASTER, masterVol);
        PlayerPrefs.SetFloat(PP_MUSIC,  musicVol);
        PlayerPrefs.SetFloat(PP_SFX,    sfxVol);
        PlayerPrefs.Save();
    }

    // --------- API: setters/gets para sliders e botões ---------
    public void SetMasterVolume(float v) { masterVol = Mathf.Clamp01(v); AplicarVolumes(); }
    public void SetMusicVolume (float v) { musicVol  = Mathf.Clamp01(v); AplicarVolumes(); }
    public void SetSfxVolume   (float v) { sfxVol    = Mathf.Clamp01(v); AplicarVolumes(); }

    public float GetMasterVolume() => masterVol;
    public float GetMusicVolume () => musicVol;
    public float GetSfxVolume   () => sfxVol;

    // --------- SFX central (respeita volumes) ---------
    public static void PlaySFX(AudioClip clip, Vector3 pos, float baseVolume = 1f)
    {
        if (Instance == null || clip == null) return;
        if (Instance.masterVol <= 0f || Instance.sfxVol <= 0f) return;

        float vol = Mathf.Clamp01(Instance.masterVol * Instance.sfxVol * baseVolume);
        AudioSource.PlayClipAtPoint(clip, pos, vol);
    }
}
