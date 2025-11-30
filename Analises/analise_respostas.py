import os
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt

CAMINHO_CSV = "respostas_hello_ball.csv"
PASTA_GRAFICOS = os.path.join("figuras", "graficos_pergunta")
ARQ_ESTATISTICAS = "estatisticas_likert.csv"
ARQ_CORRELACAO = "correlacao_spearman.csv"

def criar_pasta_se_nao_existir(caminho_pasta: str) -> None:
    if not os.path.exists(caminho_pasta):
        os.makedirs(caminho_pasta)


def carregar_dados(caminho_csv: str) -> pd.DataFrame:
    df = pd.read_csv(caminho_csv)
    return df


def selecionar_colunas_likert(df: pd.DataFrame) -> pd.DataFrame:
    colunas_numericas = df.select_dtypes(include=["int64", "float64"]).columns
    df_likert = df[colunas_numericas].copy()
    return df_likert


def calcular_estatisticas_descritivas(df_likert: pd.DataFrame) -> pd.DataFrame:
    estatisticas = df_likert.agg(["mean", "median", "std"]).T
    estatisticas.columns = ["media", "mediana", "desvio_padrao"]
    return estatisticas


def calcular_correlacao_spearman(df_likert: pd.DataFrame) -> pd.DataFrame:
    corr = df_likert.corr(method="spearman")
    return corr


def gerar_graficos_distribuicao(df_likert: pd.DataFrame,
                                pasta_graficos: str) -> None:
    criar_pasta_se_nao_existir(pasta_graficos)

    for coluna in df_likert.columns:
        contagens = df_likert[coluna].value_counts().sort_index()
        plt.figure()
        contagens.plot(kind="bar")
        plt.title(coluna)
        plt.xlabel("Resposta (1 a 5)")
        plt.ylabel("Frequência")
        plt.xticks(rotation=0)
        plt.tight_layout()
        nome_arquivo = coluna.replace(" ", "_")
        nome_arquivo = nome_arquivo.replace("/", "_")
        nome_arquivo = nome_arquivo.replace("\n", "_")

        caminho_arquivo = os.path.join(pasta_graficos,
                                       f"grafico_{nome_arquivo}.png")
        
        plt.savefig(caminho_arquivo, dpi=300)
        plt.close()


def salvar_tabelas(estatisticas: pd.DataFrame,
                   correlacao: pd.DataFrame,
                   arq_estatisticas: str,
                   arq_correlacao: str) -> None:
    estatisticas.to_csv(arq_estatisticas, encoding="utf-8", sep=";")
    correlacao.to_csv(arq_correlacao, encoding="utf-8", sep=";")

def main():
    df = carregar_dados(CAMINHO_CSV)
    df_likert = selecionar_colunas_likert(df)
    estatisticas = calcular_estatisticas_descritivas(df_likert)
    correlacao = calcular_correlacao_spearman(df_likert)
    gerar_graficos_distribuicao(df_likert, PASTA_GRAFICOS)
    salvar_tabelas(estatisticas,
                   correlacao,
                   ARQ_ESTATISTICAS,
                   ARQ_CORRELACAO)
    print("Estatísticas descritivas (primeiras linhas):")
    print(estatisticas.head())
    print("\nMatriz de correlação de Spearman (primeiras linhas):")
    print(correlacao.head())

if __name__ == "__main__":
    main()