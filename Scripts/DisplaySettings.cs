using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySettings : MonoBehaviour
{
    [Header("UI")]
    public Toggle toggleFullscreen;

    [Header("Ajustes de 'Janela Maximizada'")]
    [Tooltip("Pixels reservados para a barra de tarefas (aprox). Ajuste conforme seu Windows/DPI.")]
    public int taskbarPixels = 48;
    [Tooltip("Padding para bordas/janelas (somatório esquerda+direita / topo+base).")]
    public int windowBorderX = 16;
    public int windowBorderY = 16;

    [Header("Persistência (opcional)")]
    public bool rememberChoice = true;
    const string KEY_FS = "fs_on"; // 1 = fullscreen, 0 = janela

    void Awake()
    {
        // (1) Se não tem chave ainda, força FULLSCREEN no primeiro boot
        if (rememberChoice && !PlayerPrefs.HasKey(KEY_FS))
        {
            PlayerPrefs.SetInt(KEY_FS, 1);
            PlayerPrefs.Save();
        }

        // Tenta achar o Toggle automaticamente se não foi arrastado
        if (toggleFullscreen == null)
            toggleFullscreen = GetComponentInChildren<Toggle>(includeInactive: true);

        // Estado inicial
        bool wantFullscreen = true;
        if (rememberChoice)
            wantFullscreen = PlayerPrefs.GetInt(KEY_FS, 1) == 1;

        if (toggleFullscreen)
        {
            // Seta sem disparar evento
            toggleFullscreen.SetIsOnWithoutNotify(wantFullscreen);
            // (1) Garante que clicar chama OnToggleFullscreen
            toggleFullscreen.onValueChanged.RemoveListener(OnToggleFullscreen);
            toggleFullscreen.onValueChanged.AddListener(OnToggleFullscreen);
        }

        // Aplica no próximo frame (Display.main pronto)
        StartCoroutine(ApplyNextFrame(wantFullscreen));
    }

    void Update()
    {
        // Sincroniza com F11
        if (Input.GetKeyDown(KeyCode.F11))
        {
            bool goingFullscreen = !IsFullscreen();

            if (toggleFullscreen)
                toggleFullscreen.SetIsOnWithoutNotify(goingFullscreen);

            StartCoroutine(ApplyNextFrame(goingFullscreen));

            if (rememberChoice)
            {
                PlayerPrefs.SetInt(KEY_FS, goingFullscreen ? 1 : 0);
                PlayerPrefs.Save();
            }
        }
    }

    // Chamado pelo Toggle via listener
    public void OnToggleFullscreen(bool isOn)
    {
        if (rememberChoice)
        {
            PlayerPrefs.SetInt(KEY_FS, isOn ? 1 : 0);
            PlayerPrefs.Save();
        }
        StartCoroutine(ApplyNextFrame(isOn));
    }

    IEnumerator ApplyNextFrame(bool enableFullscreen)
    {
        yield return null; // espera 1 frame
        Apply(enableFullscreen);

        // Segunda passada (sempre) — alguns drivers só cravam na segunda
        yield return null;
        Apply(enableFullscreen);

        // (2) Se está SAINDO e parece que "não saiu", faz um break agressivo do borderless
        if (!enableFullscreen && (Screen.fullScreen || IsFullscreenTrue()))
        {
            int sw = (Display.main != null) ? Display.main.systemWidth  : Screen.currentResolution.width;
            int sh = (Display.main != null) ? Display.main.systemHeight : Screen.currentResolution.height;

            // passo 1: reduz bem a janela (quebra o estado borderless preso)
            Screen.fullScreen = false;
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(Mathf.Max(800, sw - 200), Mathf.Max(600, sh - 200), false);

            yield return null;

            // passo 2: agora aplica o tamanho "maximizado"
            int targetW = Mathf.Max(640, sw - windowBorderX);
            int targetH = Mathf.Max(360, sh - taskbarPixels - windowBorderY);
            Screen.SetResolution(targetW, targetH, false);
        }
    }

    void Apply(bool enableFullscreen)
    {
#if UNITY_STANDALONE || UNITY_EDITOR
        if (enableFullscreen)
        {
            // === FULLSCREEN SEM BORDAS (borderless) ===
            int sw = (Display.main != null) ? Display.main.systemWidth  : Screen.currentResolution.width;
            int sh = (Display.main != null) ? Display.main.systemHeight : Screen.currentResolution.height;

            Screen.fullScreenMode = FullScreenMode.FullScreenWindow; // borderless
#if UNITY_2022_2_OR_NEWER
            var cur = Screen.currentResolution;
            int hz = Mathf.RoundToInt((float)cur.refreshRateRatio.value);
            Screen.SetResolution(sw, sh, true, new RefreshRate { numerator = hz, denominator = 1 });
#else
            Screen.SetResolution(sw, sh, true, Screen.currentResolution.refreshRate);
#endif
            Screen.fullScreen = true;
        }
        else
        {
            // === JANELA "MAXIMIZADA" (tamanho da área útil) ===
            int sw = (Display.main != null) ? Display.main.systemWidth  : Screen.currentResolution.width;
            int sh = (Display.main != null) ? Display.main.systemHeight : Screen.currentResolution.height;

            Screen.fullScreenMode = FullScreenMode.Windowed;
            int targetW = Mathf.Max(640, sw - windowBorderX);
            int targetH = Mathf.Max(360, sh - taskbarPixels - windowBorderY);

            Screen.SetResolution(targetW, targetH, false);
            Screen.fullScreen = false; // coloca DEPOIS do SetResolution para sobrepor drivers teimosos
        }
#endif
    }

    bool IsFullscreen()
    {
        return Screen.fullScreen ||
               Screen.fullScreenMode == FullScreenMode.FullScreenWindow ||
               Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen;
    }

    bool IsFullscreenTrue()
    {
        int sw = (Display.main != null) ? Display.main.systemWidth  : Screen.currentResolution.width;
        int sh = (Display.main != null) ? Display.main.systemHeight : Screen.currentResolution.height;

        bool sizeOk = Mathf.Abs(Screen.width - sw) <= 2 && Mathf.Abs(Screen.height - sh) <= 2;

        return Screen.fullScreen &&
               (Screen.fullScreenMode == FullScreenMode.FullScreenWindow ||
                Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen) &&
               sizeOk;
    }
}
