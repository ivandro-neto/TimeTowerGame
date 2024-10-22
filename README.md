# Relatório do Projeto: TimeTower

## Sumário
- [Relatório do Projeto: TimeTower](#relatório-do-projeto-timetower)
  - [Sumário](#sumário)
  - [Descrição do Projeto](#descrição-do-projeto)
  - [Objetivos](#objetivos)
  - [Tecnologias Utilizadas](#tecnologias-utilizadas)
  - [Estrutura de Pastas](#estrutura-de-pastas)
  - [Design Patterns Utilizados](#design-patterns-utilizados)
  - [Considerações Finais](#considerações-finais)

## Descrição do Projeto
O projeto é um jogo de quebra-cabeça do tipo Match-3, desenvolvido utilizando a framework MonoGame. O objetivo do jogo é combinar três ou mais peças iguais em uma grade, permitindo que o jogador complete níveis e alcance pontuações mais altas.

## Objetivos
- Desenvolver um jogo interativo e envolvente para diversas plataformas.
- Implementar uma mecânica de jogo fluida e intuitiva.
- Criar uma interface de usuário atraente e responsiva.

## Tecnologias Utilizadas
- **C#**: Linguagem de programação utilizada no desenvolvimento do jogo.
- **MonoGame**: Framework para desenvolvimento de jogos 2D.
- **Visual Studio Code**: Editor de código utilizado para desenvolvimento.

## Estrutura de Pastas
TimeTower/<br> 
├── Assets/ # Recursos do jogo (imagens, sons, etc.)<br> 
│ └── Texture/<br>
├── Source/ # Código-fonte do jogo<br> 
│ ├── Core/ # Lógica principal do jogo<br> 
│ ├── Delegators/ # Delegates para chamada de eventos durante o jogo<br> 
│ ├── Entities/ # Entidades do jogo (jogador, peças, etc.)<br> 
│ │ ├── Game/ # Logica do Jogo (Board)<br> 
│ │ ├── GameStateManager/ # Gerenciamento de cenas<br> 
│ │ │ └── Scenes/ # cenas (menus, níveis)<br> 
│ │ │ │ └── GameMode/ # cenas de Modo de jogo (níveis)<br> 
│ │ ├── HUD/ # Serviços e gerenciamento de estado<br> 
│ │ ├── Piece/ # Pecas do tabuleiro<br>
│ │ └── UI/ # Interface do usuário<br> 
│ └── Utilities/ # Serviços de Rede<br> 
└── README.md # Documentação do projeto<br>


## Design Patterns Utilizados
- **Delegator & Event Pattern**: Utilizado para gerenciar eventos dentro do jogo, permitindo que diferentes partes do código se comuniquem de forma eficiente.
- **Abstract Factory**: Empregado para criar famílias de objetos (como diferentes tipos de peças do jogo) sem especificar suas classes concretas.
- **Dependency Injection**: Facilita a gestão de dependências entre classes, promovendo um código mais modular e testável.

## Considerações Finais
O desenvolvimento do jogo TimeTower está em andamento e várias funcionalidades já foram implementadas. O uso de design patterns tem contribuído significativamente para a organização e manutenibilidade do código.
