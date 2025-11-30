using UnityEngine;

public class FaseButton : MonoBehaviour
{
    public string faseId = "Fase1"; // setar no Inspector para cada botão

    public void OnClick()
    {
        var controller = FindObjectOfType<MenuController>();
        controller.AbrirExplicacaoFaseById(faseId);
    }
}
