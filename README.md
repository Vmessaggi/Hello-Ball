# 🎮 **Hello Ball – Jogo Educacional 3D sobre Sólidos Geométricos**

Bem-vindo ao repositório oficial do **Hello Ball**, um jogo digital em 3D desenvolvido como ferramenta educacional para apoio ao ensino e à avaliação da aprendizagem de **sólidos geométricos** no Ensino Médio.  
O jogo combina elementos de *endless runner*, desafios conceituais e mecânicas interativas para tornar o estudo de Geometria Espacial mais dinâmico, visual e acessível.

Este repositório foi criado para disponibilizar:

- Os **arquivos necessários para executar o jogo** (compilado para Windows)  
- Todos os **scripts C# utilizados no desenvolvimento**  
- Informações técnicas e orientações de uso  

---

## 📦 **Download do Jogo (Build Windows)**

O jogo completo para execução está disponível no link abaixo:

🔗 **Download (Google Drive):**  
https://drive.google.com/file/d/1X5QzuBuOTupgvrSGCSQBR1Wa2RqxWEmk/view?usp=drive_link

O arquivo ZIP contém todos os componentes gerados pela Unity necessários para rodar o jogo diretamente no Windows.

---

## 📁 **Conteúdo do ZIP**

Dentro do arquivo ZIP você encontrará os seguintes itens:

### **1. 🗂️ Pasta `HELLO_BALL_Data`**
Contém todos os dados do jogo, incluindo:

- Assets processados pela Unity  
- Modelos, texturas e áudios convertidos  
- Cenas compiladas  
- Scripts embutidos no build  
- Arquivos internos utilizados pelo mecanismo Unity para execução  

É a pasta **principal** que acompanha qualquer build Unity.

---

### **2. 🗂️ Pasta `MonoBleedingEdge`**
Inclui a distribuição de runtime **Mono** usada pela Unity para executar o jogo.  
Contém:

- DLLs internas  
- Bibliotecas padrão  
- Compatibilidade com .NET/Mono

Sem essa pasta, o jogo não inicia — ela garante o funcionamento correto do build.

---

### **3. 🎮 `HELLO BALL.exe`**
O executável principal do jogo.  
Clique duas vezes para jogar.

É o arquivo que carrega a engine Unity, os assets e inicia a experiência do Hello Ball.

---

### **4. ⚙️ `UnityPlayer.dll`**
Uma biblioteca essencial da Unity que faz a interface entre o executável e o motor do jogo.  
Ela é responsável por:

- Renderização  
- Áudio  
- Input  
- Execução de scripts compilados  

É carregada automaticamente pelo `HELLO BALL.exe`.

---

### **5. 🧩 `UnityCrashHandler64.exe`**
Ferramenta interna da Unity para captura de falhas.  
Ela **não é executada manualmente** — serve apenas para registrar logs caso ocorra algum erro inesperado na execução do jogo.

---

## 🧠 **Sobre o Hello Ball**

O **Hello Ball** foi projetado como parte de um projeto acadêmico voltado ao ensino de Geometria Espacial.  
O jogo apresenta:

- Fases temáticas com conteúdos de sólidos geométricos  
- Perguntas conceituais integradas ao percurso  
- Registro de desempenho do aluno  
- Ambientes interativos com visual 3D  
- Mecânica de desvio, coleta e escolha de respostas  
- Exportação de informações (tentativas, tempo, acertos)

O objetivo é tornar o ensino de GE mais intuitivo, visual e motivador.

---

## 💻 **Scripts do Projeto**

Além do build, este repositório também contém uma pasta com os **scripts C# utilizados no desenvolvimento do jogo**, permitindo:

- Consultar a lógica das mecânicas  
- Revisar o funcionamento dos sistemas  
- Auxiliar em estudos, manutenção ou futuras versões  

Qualquer melhoria ou estudo pode ser feito analisando estes scripts.

---

## ▶️ **Como Jogar**

1. Baixe o arquivo ZIP.  
2. Extraia tudo para uma pasta de sua preferência.  
3. Execute **HELLO BALL.exe**.  
4. Divirta-se e aprenda! 🎓

---

## 🧩 **Requisitos Mínimos**

- Windows 10 ou superior  
- 4 GB de RAM (8 GB recomendado)  
- GPU integrada ou dedicada  
- ~500 MB de armazenamento livre  

---

## 🤝 **Contribuições**

Contribuições, sugestões e melhorias são bem-vindas!  
Este projeto tem finalidade educacional e pode ser expandido ou adaptado para novos conteúdos.

---

## 📬 **Contato**

Caso encontre algum erro ou deseje contribuir, sinta-se à vontade para abrir uma *issue* ou entrar em contato pelo GitHub.

---
