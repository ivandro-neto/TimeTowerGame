
using System;
using Entities.Pieces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Entities.Game
{
  public class Board
  {
    private readonly int _rows;
    private readonly int _columns;
    private readonly int _tileSize;
    private readonly int _spacing = 4;
    private Texture2D _borderTexture;
    private int _borderWidth = 5;
    private Vector2 _boardPosition;
    private readonly Piece[,] _pieces;
    private double _fallDelayTimer = 0; // Tempo acumulado do delay
    private double _fallDelayDuration = 0; // Delay de 1.5 segundos
    private bool _isReadyToFall = false; // Controle se o delay já passou e peças podem cair
    private bool _isFalling = false; // Controle de queda
    private const float FallSpeed = 300f; // Velocidade de queda em pixels por segundo
    private Vector2[,] _targetPositions; // Para armazenar as posições alvo

    //User Input
    private Piece _selectedPiece = null; // Peça atualmente selecionada
    private Piece _hoveredPiece = null;  // Peça sobre a qual o mouse está
    private MouseState _previousMouseState; // Estado anterior do mouse para detectar cliques
    private bool _isSwapping = false;    // Controle se a troca está em progresso
    private bool _failSwap = false;    // Controle se a troca está em progresso
    private int _matched = 0;    // Controle se a troca está em progresso

    private readonly IPieceFactory _pieceFactory;
    private const float Gravity = 500f; // Constante de gravidade (quanto maior, mais rápido as peças caem)
    private Vector2[,] _fallVelocity; // Matriz para armazenar a velocidade de queda de cada peça

    public Board(int rows, int columns, int tileSize, IPieceFactory factory, Vector2 boardPosition, Texture2D texture)
    {
      _rows = rows;
      _columns = columns;
      _tileSize = tileSize;
      _pieces = new Piece[rows, columns]; // Inicia as peças
      _pieceFactory = factory;
      _boardPosition = boardPosition;
      _borderTexture = texture;
      _targetPositions = new Vector2[_rows, _columns];
      _fallVelocity = new Vector2[_rows, _columns]; // Inicializa a matriz de velocidades
      InitializeBoard();
    }
    // Inicia o tabuleiro com peças aleatórias
    private void InitializeBoard()
    {
      for (int r = 0; r < _rows; r++)
      {
        for (int c = 0; c < _columns; c++)
        {
          if (_pieces[r, c] == null)
          {
            Vector2 position = new Vector2(
                _boardPosition.X + c * (_tileSize + _spacing),
                _boardPosition.Y + r * (_tileSize + _spacing)
            );
            _pieces[r, c] = _pieceFactory.CreatePiece(position);

            _targetPositions[r, c] = position;
          }
        }
      }
    }


    // Desenha a borda do tabuleiro
    private void DrawBoardBorder(SpriteBatch spriteBatch)
    {
      // Calcular o tamanho total do tabuleiro
      int boardPixelWidth = _columns * (_tileSize + _spacing) - _spacing; // Ajuste para compensar o último espaçamento
      int boardPixelHeight = _rows * (_tileSize + _spacing) - _spacing;

      // Desenhar borda superior
      spriteBatch.Draw(_borderTexture, new Rectangle((int)_boardPosition.X - _borderWidth, (int)_boardPosition.Y - _borderWidth, boardPixelWidth + 2 * _borderWidth, _borderWidth), Color.Black);
      // Desenhar borda inferior
      spriteBatch.Draw(_borderTexture, new Rectangle((int)_boardPosition.X - _borderWidth, (int)_boardPosition.Y + boardPixelHeight, boardPixelWidth + 2 * _borderWidth, _borderWidth), Color.Black);
      // Desenhar borda esquerda
      spriteBatch.Draw(_borderTexture, new Rectangle((int)_boardPosition.X - _borderWidth, (int)_boardPosition.Y - _borderWidth, _borderWidth, boardPixelHeight + 2 * _borderWidth), Color.Black);
      // Desenhar borda direita
      spriteBatch.Draw(_borderTexture, new Rectangle((int)_boardPosition.X + boardPixelWidth, (int)_boardPosition.Y - _borderWidth, _borderWidth, boardPixelHeight + 2 * _borderWidth), Color.Black);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
      // Calcular os limites do tabuleiro
      int boardPixelWidth = _columns * (_tileSize + _spacing) - _spacing;
      int boardPixelHeight = _rows * (_tileSize + _spacing) - _spacing;

      // Criar um retângulo que representa a área do tabuleiro
      Rectangle boardRectangle = new Rectangle(
          (int)_boardPosition.X,
          (int)_boardPosition.Y,
          boardPixelWidth,
          boardPixelHeight
      );


      // Configurar o clipe para o retângulo do tabuleiro
      spriteBatch.GraphicsDevice.ScissorRectangle = boardRectangle;

      // Desenhar as peças do fundo para a frente
      for (int r = _rows - 1; r >= 0; r--) // Começa do fundo
      {
        for (int c = 0; c < _columns; c++)
        {
          Piece piece = _pieces[r, c];
          if (piece != null)
          {
            piece.Draw(spriteBatch);

            // Destaque se a peça estiver selecionada
            if (_selectedPiece == piece)
            {
              Rectangle selectionRectangle = new Rectangle(
                  (int)piece.Position.X - 2,
                  (int)piece.Position.Y - 2,
                  _tileSize + 4,
                  _tileSize + 4
              );
              spriteBatch.Draw(_borderTexture, selectionRectangle, Color.White);
            }
          }
        }
      }

      // Desenhar borda do tabuleiro
      DrawBoardBorder(spriteBatch);
    }



    #region GameLogic
    // Verifica todos os tipos de combinações
    public bool CheckMatches()
    {
      return HorizontalMatches() || VerticalMatches();
    }

    public int Score()
    {
      int points = _matched;
      _matched = 0;
      return points;
    }
    public bool FailSwap()
    {
      bool fail = _failSwap;
      _failSwap = false;
      return fail;
    }
    private bool HorizontalMatches()
    {
      for (int r = 0; r < _rows; r++)
      {
        int count = 1; // Contador para peças combinadas
        for (int c = 1; c < _columns; c++)
        {
          // Verifica se a peça atual é do mesmo tipo que a anterior
          if (_pieces[r, c] != null && _pieces[r, c].Type == _pieces[r, c - 1]?.Type)
          {
            count++; // Incrementa o contador
          }
          else
          {
            // Se a contagem for 3 ou mais, marca as peças
            if (count >= 3)
            {
              for (int k = c - 1; k >= c - count; k--) // Marca as peças
              {
                _pieces[r, k].SetMatchState(true);
              }
              return true; // Retorna true ao encontrar uma combinação
            }
            count = 1; // Reinicia o contador se não for uma combinação
          }
        }
        // Verifica no final da linha se contagem é 3 ou mais
        if (count >= 3)
        {
          for (int k = _columns - 1; k >= _columns - count; k--) // Marca as peças
          {
            _pieces[r, k].SetMatchState(true);
          }
          return true; // Retorna true ao encontrar uma combinação
        }
      }
      return false; // Retorna false se não houver combinações
    }


    private bool VerticalMatches()
    {
      for (int c = 0; c < _columns; c++)
      {
        int count = 1; // Contador para peças combinadas
        for (int r = 1; r < _rows; r++)
        {
          // Verifica se a peça atual é do mesmo tipo que a anterior
          if (_pieces[r, c] != null && _pieces[r, c].Type == _pieces[r - 1, c]?.Type)
          {
            count++; // Incrementa o contador
          }
          else
          {
            // Se a contagem for 3 ou mais, marca as peças
            if (count >= 3)
            {
              for (int k = r - 1; k >= r - count; k--) // Marca as peças
              {
                _pieces[k, c].SetMatchState(true);
              }
              return true; // Retorna true ao encontrar uma combinação
            }
            count = 1; // Reinicia o contador se não for uma combinação
          }
        }
        // Verifica no final da coluna se contagem é 3 ou mais
        if (count >= 3)
        {
          for (int k = _rows - 1; k >= _rows - count; k--) // Marca as peças
          {
            _pieces[k, c].SetMatchState(true);
          }
          return true; // Retorna true ao encontrar uma combinação
        }
      }
      return false; // Retorna false se não houver combinações
    }


    public void RemoveMatches()
    {
      for (int r = 0; r < _rows; r++)
      {
        for (int c = 0; c < _columns; c++)
        {
          if (_pieces[r, c]?.IsMatched == true)
          {
            _pieces[r, c].StartFadeOut();
            _pieces[r, c] = null;
            _matched++;
          }
        }
      }
    }

    private void FallPieces()
    {
      for (int c = 0; c < _columns; c++)
      {
        // Primeiro, vamos verificar as peças que devem cair
        for (int r = _rows - 1; r >= 0; r--)
        {
          if (_pieces[r, c] == null)
          {
            // Se encontramos uma posição vazia, vamos procurar a peça acima que deve cair
            for (int aboveRow = r - 1; aboveRow >= 0; aboveRow--)
            {
              if (_pieces[aboveRow, c] != null)
              {
                // Move a peça para a posição vazia
                _targetPositions[r, c] = new Vector2(_boardPosition.X + c * (_tileSize + _spacing), _boardPosition.Y + r * (_tileSize + _spacing));
                _pieces[r, c] = _pieces[aboveRow, c];
                _pieces[aboveRow, c] = null; // Remove a peça de sua posição anterior
                break; // Pare de procurar assim que movermos uma peça
              }
            }
          }
        }

        // Agora, vamos preencher as posições vazias no topo com novas peças
        for (int r = 0; r < _rows; r++)
        {
          if (_pieces[r, c] == null)
          {
            // Se ainda temos um espaço vazio, cria uma nova peça
            Vector2 startPosition = new Vector2(
                _boardPosition.X + c * (_tileSize + _spacing),
                _boardPosition.Y - _tileSize * (r + 1)  // Iniciar acima da tela
            );
            Vector2 targetPosition = new Vector2(
                _boardPosition.X + c * (_tileSize + _spacing),
                _boardPosition.Y + r * (_tileSize + _spacing)
            );

            _pieces[r, c] = _pieceFactory.CreatePiece(startPosition);
            _targetPositions[r, c] = targetPosition;
            _fallVelocity[r, c] = Vector2.Zero;  // Reseta a velocidade para a nova peça
            _pieces[r, c].StartFadeIn(); // Iniciar a animação de fade-in, se necessário
          }
        }
      }
    }


    private void UpdateFall(GameTime gameTime)
    {
      float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

      for (int r = 0; r < _rows; r++)
      {
        for (int c = 0; c < _columns; c++)
        {
          if (_pieces[r, c] != null)
          {
            Vector2 currentPosition = _pieces[r, c].Position;
            Vector2 targetPosition = _targetPositions[r, c];

            if (currentPosition.Y < targetPosition.Y)
            {
              // Aumente a velocidade da peça (simula gravidade)
              _fallVelocity[r, c].Y += Gravity * deltaTime;

              // Atualize a posição da peça com base na velocidade
              currentPosition.Y += _fallVelocity[r, c].Y * deltaTime;

              // Se a peça ultrapassar a posição alvo, ajuste-a para o valor exato
              if (currentPosition.Y >= targetPosition.Y)
              {
                currentPosition.Y = targetPosition.Y;
                _fallVelocity[r, c].Y = 0; // Zera a velocidade ao atingir o destino
              }

              _pieces[r, c].Position = currentPosition;
            }
          }
        }
      }
    }

    private void FallDelay(GameTime gameTime)
    {
      if (!_isReadyToFall)
      {
        _fallDelayTimer += gameTime.ElapsedGameTime.TotalSeconds;
        if (_fallDelayTimer >= _fallDelayDuration)
        {
          _isReadyToFall = true; // Delay terminou
          _fallDelayTimer = 0; // Reset no timer
        }
      }

      if (_isReadyToFall)
      {
        _isFalling = true; // As peças estão caindo
        UpdateFall(gameTime); // Atualiza a queda das peças

        // Se todas as peças estão na posição alvo, resetar o estado para evitar queda infinita
        bool allInPlace = true;
        for (int r = 0; r < _rows; r++)
        {
          for (int c = 0; c < _columns; c++)
          {
            if (_pieces[r, c] != null && Vector2.Distance(_pieces[r, c].Position, _targetPositions[r, c]) > 1f)
            {
              allInPlace = false;
              break;
            }
          }
        }

        if (allInPlace)
        {
          _isFalling = false; // Reset o estado de queda
          _isReadyToFall = false; // Espera até o próximo ciclo
          CheckMatches();
          RemoveMatches();
          FallPieces(); // Remova os matches e inicie novo ciclo
        }
      }
    }

    #endregion



    public void Update(GameTime gameTime)
    {
      HandleInput();
      FallDelay(gameTime); // Chama o delay de queda

    }

    #region Inputs
    private void HandleInput()
    {
      // Obtenha o estado atual do mouse
      if (_isFalling) return;

      MouseState currentMouseState = Mouse.GetState();

      // Se o botão esquerdo do mouse foi pressionado
      if (currentMouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released)
      {
        Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

        // Verifique se o clique foi em uma peça
        for (int r = 0; r < _rows; r++)
        {
          for (int c = 0; c < _columns; c++)
          {
            Piece piece = _pieces[r, c];
            if (piece != null && piece.ContainsPoint(mousePosition))
            {
              if (_selectedPiece == null)
              {
                // Se nenhuma peça foi selecionada ainda, selecione esta peça
                _selectedPiece = piece;
              }
              else if (_selectedPiece == piece)
              {
                // Se o jogador clicou novamente na peça selecionada, deselecione-a
                _selectedPiece = null;
              }
              else
              {
                // O jogador clicou em outra peça; tente trocar as peças
                _hoveredPiece = piece;
                TrySwapPieces(_selectedPiece, _hoveredPiece);
                _selectedPiece = null; // Reseta a seleção
              }
            }
          }
        }
      }

      // Armazene o estado do mouse para a próxima verificação
      _previousMouseState = currentMouseState;
    }
    private void TrySwapPieces(Piece piece1, Piece piece2)
    {
      // Verifica se as peças são adjacentes (horizontal ou verticalmente)
      Vector2 diff = piece1.Position - piece2.Position;
      if (Math.Abs(diff.X) == _tileSize + _spacing && diff.Y == 0 ||
          Math.Abs(diff.Y) == _tileSize + _spacing && diff.X == 0)
      {
        // Tenta trocar as peças
        SwapPieces(piece1, piece2);

        // Verifica se a troca resulta em uma combinação
        bool hasMatches = CheckMatches();

        if (!hasMatches)
        {
          // Se não houver combinação, desfaz a troca
          SwapPieces(piece1, piece2); // Troca novamente para desfazer
          _failSwap = true;
        }
        else
        {
          // Remove as combinações encontradas
          RemoveMatches();
          _failSwap = false;
        }

        _isSwapping = true;
      }
      else
      {
        // Se as peças não forem adjacentes, nada acontece
        _hoveredPiece = null;
      }
    }

    private void SwapPieces(Piece piece1, Piece piece2)
    {
      // Save the current positions of the pieces
      Vector2 position1 = piece1.Position;
      Vector2 position2 = piece2.Position;

      // Find the indices of the pieces in the array
      (int row1, int col1) = FindPiecePosition(piece1);
      (int row2, int col2) = FindPiecePosition(piece2);

      // Swap pieces in the array
      _pieces[row1, col1] = piece2;
      _pieces[row2, col2] = piece1;

      // Update the positions of the pieces
      piece1.Position = position2;
      piece2.Position = position1;

      // Reset the fall velocity for both pieces (if necessary)
      _fallVelocity[row1, col1] = Vector2.Zero;
      _fallVelocity[row2, col2] = Vector2.Zero;
    }

    private (int, int) FindPiecePosition(Piece piece)
    {
      for (int r = 0; r < _rows; r++)
      {
        for (int c = 0; c < _columns; c++)
        {
          if (_pieces[r, c] == piece)
          {
            return (r, c);
          }
        }
      }
      return (-1, -1); // Return an invalid position if not found
    }


    #endregion
  }
}