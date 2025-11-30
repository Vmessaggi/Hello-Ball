using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenus : MonoBehaviour
{
    [Header("Menus")]
    public GameObject pauseMenu;        // já existia
    public GameObject endGameMenu;      // já existia
    public GameObject settingsMenu;     // >>> NOVO: arraste seu PainelConfig aqui

    private bool isPaused = false;

    void Update()
    {
        // Bloqueia ESC quando o menu de fim de fase está aberto
        if (endGameMenu != null && endGameMenu.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Se está no painel de configurações, ESC volta para o menu de pausa
            if (settingsMenu != null && settingsMenu.activeSelf)
            {
                CloseSettings();
                return;
            }

            // Alterna pausa <-> continuar
            if (isPaused) Resume(); else Pause();
        }
    }

    public void Pause()
    {
        if (settingsMenu != null) settingsMenu.SetActive(false); // garante fechado
        if (pauseMenu != null) pauseMenu.SetActive(true);

        Time.timeScale = 0f;
        isPaused = true;
        ShowCursor(true);
    }

    public void Resume()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        ShowCursor(false);
    }

    // >>> NOVO: abrir/fechar configurações a partir do menu de pausa
    public void OpenSettings()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(true);

        // continua pausado (Time.timeScale = 0), apenas troca o painel
        ShowCursor(true);
    }

    public void CloseSettings()
    {
        if (settingsMenu != null) settingsMenu.SetActive(false);
        if (pauseMenu != null) pauseMenu.SetActive(true);
        ShowCursor(true);
    }

    public void ShowEndGameMenu()
    {
        if (endGameMenu != null) endGameMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        ShowCursor(true);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;

        // Garanta que o cursor volte para o estado de menu
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("Menu");
    }


    private void ShowCursor(bool show)
    {
        Cursor.visible = show;
        Cursor.lockState = show ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
