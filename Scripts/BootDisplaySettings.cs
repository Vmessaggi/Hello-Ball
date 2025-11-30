using UnityEngine;
public class BootDisplaySettings : MonoBehaviour
{
    void Awake()
    {
        if (!PlayerPrefs.HasKey("vid_fullscreen"))
            PlayerPrefs.SetInt("vid_fullscreen", 1); // padrão: fullscreen
    }
}
