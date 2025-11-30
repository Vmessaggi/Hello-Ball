using System.Collections.Generic;

[System.Serializable]
public class FaseData
{
    public string nomeFase;
    public string titulo;
    public string descricao;
    public string habilidadeBNCC; 
    public string prefab;
    public int numeroDeTrechos;
}

[System.Serializable]
public class FasesConfig
{
    public List<FaseData> fases;
}
