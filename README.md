# PxG Auto Revive Bot

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white)
![OpenCV](https://img.shields.io/badge/OpenCV-27338e?style=for-the-badge&logo=opencv&logoColor=white)

**PxG Auto Revive** é uma ferramenta de automação para Windows, desenvolvida em C# com Windows Forms, criada como um estudo aprofundado sobre a interação de software com o sistema operacional e outros aplicativos, como jogos.

O principal objetivo deste projeto foi explorar técnicas de **automação de tarefas**, **visão computacional** e o uso da **API do Windows** para simular a entrada do usuário de forma precisa e controlada.

---

## Motivation

A construção deste programa foi motivada pelo desejo de aprender e aplicar na prática os seguintes conceitos:

1.  **Interação com a API do Windows (P/Invoke):** Entender como um programa C# pode chamar funções nativas da `user32.dll` e `kernel32.dll` para realizar tarefas de baixo nível, como:
    *   Listar janelas abertas (`EnumWindows`).
    *   Enviar cliques de mouse (`mouse_event`) e pressionamentos de tecla (`PostMessage`) para uma janela específica, mesmo que ela não esteja em foco.
    *   Instalar hooks globais (`SetWindowsHookEx`) para capturar eventos de teclado e mouse em todo o sistema.
    *   Bloquear a entrada do usuário (`BlockInput`) para garantir a execução de automações sem interferência.

2.  **Visão Computacional com OpenCV:** Utilizar a biblioteca **OpenCvSharp** (um wrapper do OpenCV para .NET) para analisar pixels da tela em tempo real. O bot usa a técnica de *Template Matching* (`Cv2.MatchTemplate`) para identificar se um ícone específico (como o de um Pokémon "desmaiado") está presente em uma determinada área da tela.

3.  **Automação e Simulação de Usuário:** Combinar a API do Windows e a visão computacional para criar uma lógica de automação inteligente. O bot não apenas clica em coordenadas fixas, mas primeiro "vê" a tela para tomar uma decisão antes de agir, tornando a automação mais robusta.

4.  **Boas Práticas de Software:** Estruturar o projeto de forma organizada, separando responsabilidades em diferentes classes (`Handlers`, `Services`, `Models`), gerenciando configurações de forma persistente e criando uma interface de usuário intuitiva com Windows Forms.

---

## Funcionalidades Principais

-   **Seleção de Janela Alvo:** O usuário pode selecionar qualquer janela aberta no sistema para ser o alvo da automação.
-   **Configuração de Teclas:** Permite que o usuário configure teclas de atalho para diferentes ações dentro do jogo (usar um item, selecionar um personagem, etc.).
-   **Captura de Posição:** O usuário pode clicar em qualquer lugar da tela para registrar as coordenadas exatas onde uma ação (como usar um item) deve ocorrer.
-   **Gatilho de Execução Global:** A automação pode ser acionada por uma tecla de atalho configurável (incluindo botões laterais do mouse), que funciona mesmo que a janela do bot não esteja em foco (desde que a janela do jogo esteja ativa).
-   **Lógica de "Revive" Inteligente:**
    1.  O usuário pressiona a tecla de execução.
    2.  O bot captura uma pequena imagem da área onde o status do Pokémon é exibido.
    3.  Usando o OpenCV, ele compara essa imagem com um "template" do ícone de Pokémon desmaiado.
    4.  **Se** o ícone for encontrado, ele executa a sequência de automação: pressiona a tecla do item de "revive" e clica na posição do Pokémon.
    5.  **Se** o ícone não for encontrado, ele apenas pressiona a tecla do Pokémon para trazê-lo de volta, sem gastar o item.
-   **Interface Gráfica Simples:** Uma janela única onde todas as configurações podem ser feitas de forma rápida e fácil.

---

## Estrutura do Projeto

O código está organizado nos seguintes diretórios e classes principais:

-   `PxG/`
    -   **`Views/`**: Contém os formulários do Windows Forms (`ReviveView.cs`), que compõem a interface do usuário.
    -   **`Handlers/`**: Classes responsáveis por interações diretas com o sistema.
        -   `WindowSelector.cs`: Lista as janelas abertas.
        -   `KeyboardHandler.cs`: Envia comandos de teclado.
        -   `CursorPoint.cs`: Controla e simula os cliques do mouse.
        -   `GlobalKeyboardHook.cs` / `GlobalMouseHook.cs`: Capturam entradas globais.
        -   `ScreenAnalyzer.cs`: Usa o OpenCV para análise de imagem.
        -   `RevivePokemonHandler.cs`: Orquestra a sequência de ações para usar um item.
    -   **`Services/`**:
        -   `AutoReviveService.cs`: Gerencia o estado da automação (iniciar/parar) e conecta os hooks da UI com a lógica de execução.
    -   **`Models/`**:
        -   `AppSettings.cs`: Define a estrutura dos dados de configuração.
        -   `WindowInfo.cs`: Representa as informações de uma janela.
    -   **`SettingsManager.cs`**: Salva e carrega as configurações do usuário em um arquivo JSON.

---

## Como Usar

1.  **Compile e execute o projeto.**
2.  **Selecione a Janela:** Na interface, clique em "Atualizar" e selecione a janela do jogo na lista.
3.  **Configure as Teclas:**
    -   Clique em "Capturar Tecla Pokémon" e pressione a tecla de atalho que seleciona o Pokémon.
    -   Clique em "Capturar Tecla Revive" e pressione a tecla de atalho do item de reviver.
    -   Clique em "Capturar Tecla Execução" e pressione a tecla que você usará para acionar o bot.
4.  **Defina a Posição:**
    -   Clique em "Definir Posição na Tela".
    -   Mova o mouse até a barra de status do Pokémon na janela do jogo e clique. As coordenadas serão salvas.
5.  **Inicie o Modo Auto:** Clique em "INICIAR MODO AUTO".
6.  Com a janela do jogo em foco, pressione a tecla de execução que você configurou. O bot fará o resto.

> **Nota de Segurança:** O bot utiliza a função `BlockInput` para evitar que o usuário interfira na automação. Se o programa travar durante essa operação, pode ser necessário usar `Ctrl+Alt+Del` para reiniciar o computador. Execute por sua conta e risco.

---

Este projeto é estritamente para fins educacionais e não deve ser usado para obter vantagens injustas em jogos online.