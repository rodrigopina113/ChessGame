using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro; // Se você usar TextMeshPro para o texto do painel de vitória

public class ChessManager : MonoBehaviour
{
    // ── New: holds currently active variant rules
    private IChessRules rules;

    /// <summary>
    /// Called by GameManager to inject the chosen rules before starting.
    /// </summary>
    public void SetRules(IChessRules rules)
    {
        this.rules = rules;
    }

    public Chessboard chessboard; // Reference to the Chessboard script
    public GameObject[] whitePiecePrefabs; // Array of white piece prefabs
    public GameObject[] blackPiecePrefabs; // Array of black piece prefabs
    private ChessPiece selectedPiece; // Currently selected piece
    private bool isWhiteTurn = true; // Tracks whose turn it is
    public ChessAI aiPlayer;
    public bool isLocalMultiplayer = false;


    // Promotion UI and Prefabs (assign these in the Inspector)
    public GameObject promotionPanel; // A UI panel with 4 buttons (initially inactive)
    private Pawn pawnToPromote; // The pawn that reached the promotion rank

    // Promotion prefabs for white
    public GameObject whiteQueenPrefab;
    public GameObject whiteRookPrefab;
    public GameObject whiteBishopPrefab;
    public GameObject whiteKnightPrefab;

    // Promotion prefabs for black
    public GameObject blackQueenPrefab;
    public GameObject blackRookPrefab;
    public GameObject blackBishopPrefab;
    public GameObject blackKnightPrefab;
    public bool IsWhiteTurn => isWhiteTurn;

    // ───────────────────────────────────────────────────────────────
    // NOVO: referências ao painel de vitória (WinPanel)
    [Header("UI de Vitória")]
    public GameObject winPanel;          // Arraste aqui o seu GameObject WinPanel (deve estar inicialmente inativo)
    public TextMeshProUGUI winText;      // Opcional: para alterar o texto de vitória (por exemplo, "Brancas venceram!")
    private bool gameEnded = false;      // Impede cliques após o fim de jogo
    // ───────────────────────────────────────────────────────────────
    private int piecesToDrop = 0;  // Total de peças em animação
    private bool boardReady = false;

    public int currentIndex = 0;
    private int nextIndex;

    [Header("Relógio de Xadrez")]
    [SerializeField] private ChessWatch chessWatch;

   private void Start()
    {
        nextIndex = currentIndex + 1;
        // First ensure chessboard exists
        if (chessboard == null)
        {
            chessboard = Object.FindFirstObjectByType<Chessboard>();
            if (chessboard == null)
            {
                Debug.LogError("Chessboard script not found!");
                return;
            }
        }

        // Then initialize AI if needed
        if (!isLocalMultiplayer)
        {
            if (aiPlayer == null)
            {
                aiPlayer = Object.FindFirstObjectByType<ChessAI>();
                if (aiPlayer == null)
                {
                    Debug.LogError("AI Player script not found!");
                    return;
                }
            }
            aiPlayer.Initialize();
        }

        // Then handle UI
        if (winPanel != null)
            winPanel.SetActive(false);

        // Finally start the game if rules exist
        if (rules != null)
        {
            StartGame();
        }

        // Initialize camera
        if (CameraSwitcher.Instance != null)
        {
            CameraSwitcher.Instance.SwitchCamera(isWhiteTurn);
        }
    }

    public void FinishTurn()
    {
        // Finaliza o turno atual e alterna para o próximo turno.


        // Troca a câmera após a jogada
        if (CameraSwitcher.Instance != null)
        {
            CameraSwitcher.Instance.SwitchCamera(isWhiteTurn);
        }

        isWhiteTurn = !isWhiteTurn;
        if (chessWatch != null)
            chessWatch.SwitchTurn();
    }


    /// <summary>
    /// Called by GameManager after SetRules(rules).
    /// </summary>
    public void StartGame()
    {
        if (rules == null)
        {
            Debug.LogError("ChessManager: no IChessRules assigned!");
            return;
        }
        rules.InitializeBoard(this, chessboard);
        if (!isLocalMultiplayer && aiPlayer != null)
            StartCoroutine(AutoPlayAITurn(2f));
    }


    public void PlayerMove(string targetCell)
    {
        HandleTileClick(targetCell); // Execute the player's move
    }

    private IEnumerator AutoPlayAITurn(float delay)
    {
        if (isLocalMultiplayer || aiPlayer == null)
            yield break;

        // Wait for promotion panel to be inactive AND AI to be initialized
        yield return new WaitUntil(() => boardReady && (promotionPanel == null || !promotionPanel.activeSelf) && aiPlayer != null && aiPlayer.isInitialized);

        yield return new WaitForSeconds(delay);
        
        if (!gameEnded && isWhiteTurn && aiPlayer != null && (promotionPanel == null || !promotionPanel.activeSelf))
        {
            aiPlayer.MakeMove();
            isWhiteTurn = false;
        }
    }




    private void Update()
    {
        // Se o jogo acabou, não processe cliques
        if (gameEnded)
            return;

            //CÓDIGO PARA A DEMONSTRAÇÃO, QUANDO CLICA P MOSTRA O WINPANEL
            if (Input.GetKeyDown(KeyCode.P))
        {
            LevelProgressManager.Instance.UnlockLevel(nextIndex);
            if (winPanel != null)
                winPanel.SetActive(true); // Mostra o painel ao apertar P
        }

        // If the player right-clicks, deselect any currently selected piece.
        if (Input.GetMouseButtonDown(1))
        {
            if (selectedPiece != null)
            {
                selectedPiece = null;
                HighlightValidMoves(null); // Clear any highlighted moves.
            }
            return; // Skip further processing this frame.
        }

        // Process left-clicks only.
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Use the hit point to determine the nearest cell.
                string clickedCell = FindNearestCell(hit.point);
                if (!string.IsNullOrEmpty(clickedCell))
                {
                    PlayerMove(clickedCell);
                }
            }
        }
    }

    public void PlacePiece(GameObject prefab, string cellName, float dropDelay)
    {
        Vector3 targetPosition = chessboard.GetCellPosition(cellName);
        Vector3 startPosition = targetPosition + Vector3.up * 5f; // Float above

        // Instantiate the piece at the floating start position
        GameObject pieceObj = Instantiate(prefab, startPosition, Quaternion.identity);

        ChessPiece piece = pieceObj.GetComponent<ChessPiece>();
        piece.transform.localScale = Vector3.one * 20f;
        piece.CurrentCell = cellName;
        piece.isWhite = prefab.name.Contains("white");
        piece.chessManager = this;

        if (piece is Knight)
        {
            // black knights always face “down”
            // white knights only face “down” in Racing Kings variant
            bool rotateWhite = rules is RacingKingsRules && piece.isWhite;
            if (!piece.isWhite || rotateWhite)
                pieceObj.transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        piecesToDrop++;
        StartCoroutine(AnimatePieceDrop(pieceObj, startPosition, targetPosition, dropDelay));

        // In 3D, we no longer adjust size based on row
        // piece.AdjustSizeBasedOnRow();
    }

    private IEnumerator AnimatePieceDrop(
        GameObject pieceObj,
        Vector3 start,
        Vector3 end,
        float delay
    )
    {
        float initialGlobalDelay = 1.5f; // tempo de espera antes de qualquer peça cair
        yield return new WaitForSeconds(initialGlobalDelay + delay);

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (pieceObj == null)
                yield break;
            //pieceObj.transform.position = Vector3.Lerp(start, end, elapsed / duration);
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0, 1, Mathf.SmoothStep(0, 1, t)); // Makes the motion more natural
            pieceObj.transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (pieceObj != null)
            pieceObj.transform.position = end; // Snap to final position

        if (pieceObj != null)
            pieceObj.transform.position = end;

        piecesToDrop--;
        if (piecesToDrop <= 0)
        {
            boardReady = true;
            if (chessWatch != null)
                chessWatch.StartTimers();
        }    

    }

    private void HandleTileClick(string cellName)
    {
        // Se o jogo já acabou, ignore
        if (gameEnded)
            return;

        // AI plays White
        if (isWhiteTurn && !isLocalMultiplayer && aiPlayer != null)
        {
            aiPlayer.MakeMove();
            isWhiteTurn = false;
            return;
        }

        // If we already have a selection, try to move it
        if (selectedPiece != null)
        {
            // convert "e4" → (4,3)
            Vector2Int target = new Vector2Int(cellName[0] - 'a', cellName[1] - '1');

            // use rules.IsMoveValid instead of piece.IsValidMove
            if (
                rules.IsMoveValid(selectedPiece, target, chessboard)
                && (
                    !CheckIfKingInCheck()
                    || (selectedPiece is King && DoesMoveRemoveCheck((King)selectedPiece, cellName))
                    || IsMoveSafeForKing(selectedPiece, cellName)
                )
            )
            {
                ChessPiece targetPiece = FindPieceAtCell(cellName);

                // capture logic
                if (targetPiece != null)
                {
                    if (targetPiece.isWhite != selectedPiece.isWhite)
                    {
                        if (targetPiece is King)
                        {
                            // ───> Em vez de ResetGame(), exiba o painel de vitória
                            if (gameEnded) return;
                            gameEnded = true;
                            if (chessWatch != null)
                                chessWatch.PauseTimers();

                            string vencedor = targetPiece.isWhite ? "Pretas" : "Brancas";
                            if (winText != null)
                                winText.text = $"{vencedor} venceram!";
                            winPanel.SetActive(true);
                            LevelProgressManager.Instance.UnlockLevel(nextIndex);
                            return;
                        }
                        Destroy(targetPiece.gameObject);
                        Debug.Log($"Captured {targetPiece.name} at {cellName}");
                    }
                    else
                    {
                        Debug.Log("Cannot move to a cell occupied by your own piece!");
                        selectedPiece = null;
                        HighlightValidMoves(null);
                        return;
                    }
                }

                // castling vs normal move
                if (selectedPiece is King king && Mathf.Abs(cellName[0] - king.CurrentCell[0]) == 2)
                    king.MoveTo(cellName);
                else
                    MovePiece(selectedPiece, cellName);

                // post-move endgame checks
                if (CheckForCheckmate())
                {
                    // ───> Em vez de ResetGame(), exiba o painel de vitória
                    if (gameEnded) return;
                    gameEnded = true;
                    if (chessWatch != null)
                        chessWatch.PauseTimers();

                    // Quem deu o xeque-mate? Se isWhiteTurn == true, então as brancas deram mate agora.
                    string vencedor = isWhiteTurn ? "Brancas" : "Pretas";
                    if (winText != null)
                        winText.text = $"{vencedor} venceram por xeque-mate!";
                    winPanel.SetActive(true);
                    LevelProgressManager.Instance.UnlockLevel(nextIndex);
                    return;
                }
                if (CheckForStalemate())
                {
                    // ───> Em vez de ResetGame(), exiba o painel de empate (ou vitória genérica)
                    if (gameEnded) return;
                    gameEnded = true;
                    if (chessWatch != null)
                        chessWatch.PauseTimers();

                    if (winText != null)
                        winText.text = $"Empate por stalemate!";
                    winPanel.SetActive(true);
                    LevelProgressManager.Instance.UnlockLevel(nextIndex);
                    return;
                }

                // finish turn
                isWhiteTurn = !isWhiteTurn;
                if (chessWatch != null)
                    chessWatch.SwitchTurn();
                selectedPiece = null;
                HighlightValidMoves(null);
                Debug.Log($"Moved piece to {cellName}");
            }
            else
            {
                Debug.Log("Invalid move!");
                selectedPiece = null;
                HighlightValidMoves(null);
            }
        }
        else
        {
            // No selection yet: try to pick up a piece
            ChessPiece piece = FindPieceAtCell(cellName);

            if (piece != null)
            {
                if (piece.isWhite != isWhiteTurn)
                {
                    Debug.Log("Not your turn!");
                    return;
                }

                // If in check, limit selection to King or moves that block/capture
                if (CheckIfKingInCheck())
                {
                    King kingInCheck = FindObjectsByType<King>(FindObjectsSortMode.None)
                        .FirstOrDefault(k => k.isWhite == isWhiteTurn);
                    if (kingInCheck != null)
                    {
                        var threats = GetThreatsToKing(kingInCheck);
                        if (threats.Count == 1)
                        {
                            // allow King or piece that can block/capture
                            if (piece is King || IsMoveSafeForKing(piece, threats[0].CurrentCell))
                            {
                                selectedPiece = piece;
                                HighlightValidMoves(piece);
                                chessboard.HighlightTile(selectedPiece.CurrentCell, true);
                            }
                            else
                            {
                                Debug.Log(
                                    "Only valid moves to capture or block the threat are allowed."
                                );
                            }
                        }
                        else
                        {
                            Debug.Log("Multiple threats! Only the King can move.");
                        }
                    }
                }
                else
                {
                    // normal selection: highlight via rules
                    selectedPiece = piece;
                    if (piece is King)
                    {
                        // for King – only show moves that also remove check if needed
                        foreach (var tile in chessboard.tiles)
                        {
                            Vector2Int coord = new Vector2Int(
                                tile.name[0] - 'a',
                                tile.name[1] - '1'
                            );
                            bool ok =
                                rules.IsMoveValid(piece, coord, chessboard)
                                && DoesMoveRemoveCheck((King)piece, tile.name);
                            tile.Highlight(ok);
                        }
                        chessboard.HighlightTile(selectedPiece.CurrentCell, true);
                    }
                    else
                    {
                        // for others – delegate to HighlightValidMoves (also uses rules)
                        HighlightValidMoves(piece);
                    }
                }
            }
            else
            {
                Debug.Log("No piece selected and clicked on an empty cell");
                HighlightValidMoves(null);
            }
        }
    }

    public void ReplaceBlackPieces()
    {
        // 1) Find all existing black ChessPiece instances in the scene.
        var blackPieces = Object
            .FindObjectsByType<ChessPiece>(FindObjectsSortMode.None)
            .Where(p => !p.isWhite)
            .ToList();

        foreach (var oldPiece in blackPieces)
        {
            string cell = oldPiece.CurrentCell;

            // 2) Determine which prefab index to use based on type
            int prefabIndex = -1;
            if (oldPiece is Rook) prefabIndex = 0;
            else if (oldPiece is Knight) prefabIndex = 1;
            else if (oldPiece is Bishop) prefabIndex = 2;
            else if (oldPiece is Queen) prefabIndex = 3;
            else if (oldPiece is King) prefabIndex = 4;
            else if (oldPiece is Pawn) prefabIndex = 5;
            else continue; // skip unknown piece types

            // 3) Capture the existing piece's runtime scale
            Vector3 oldScale = oldPiece.transform.localScale;

            // 4) Destroy the old piece GameObject
            Destroy(oldPiece.gameObject);

            // 5) Instantiate the new prefab at the same board cell position
            Vector3 worldPos = chessboard.GetCellPosition(cell);
            GameObject prefab = blackPiecePrefabs[prefabIndex];
            GameObject newObj = Instantiate(prefab, worldPos, Quaternion.identity);

            // 6) Re-initialize the new ChessPiece component
            var newPiece = newObj.GetComponent<ChessPiece>();
            newPiece.CurrentCell = cell;
            newPiece.isWhite = false;
            newPiece.chessManager = this;

            // 7) **Apply the old piece's scale** (instead of a fixed Vector3.one * 20f)
            newObj.transform.localScale = oldScale;

            // 8) Rotate knights so they face “downward” for black side
            if (newPiece is Knight)
            {
                newObj.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }


    private List<ChessPiece> GetThreatsToKing(King king)
    {
        List<ChessPiece> threats = new List<ChessPiece>();
        foreach (var piece in GetAllPieces(!king.isWhite))
        {
            if (
                piece.IsValidMove(king.CurrentCell)
                && IsPathClear(piece.CurrentCell, king.CurrentCell, piece)
            )
            {
                threats.Add(piece);
            }
        }
        return threats;
    }

    private List<string> GetBlockingCells(King king, ChessPiece checker)
    {
        if (checker is Knight)
            return new List<string>();
        return GetCellsBetween(checker.CurrentCell, king.CurrentCell);
    }

    private bool IsMoveSafeForKing(ChessPiece piece, string targetCell)
    {
        King king = FindObjectsByType<King>(FindObjectsSortMode.None)
            .FirstOrDefault(k => k.isWhite == isWhiteTurn);

        if (king == null)
            return false;

        List<ChessPiece> threats = GetThreatsToKing(king);

        if (threats.Count == 1) // Single-check scenarios
        {
            ChessPiece checker = threats[0];

            // If the King is trying to move along the checking line, invalidate it
            if (piece is King && GetBlockingCells(king, checker).Contains(targetCell))
            {
                return false; // The King cannot block the line of check
            }

            // Allow capturing the checker
            if (checker.CurrentCell == targetCell)
            {
                return true;
            }

            // Allow blocking the check (non-King pieces)
            if (!(piece is King) && GetBlockingCells(king, checker).Contains(targetCell))
            {
                return true;
            }
        }

        return false; // Not a valid move
    }

    public bool IsPathClear(string startCell, string endCell, ChessPiece piece)
    {
        // Knights don't require path checks
        if (piece is Knight)
            return true;

        // Get all cells between start and end
        var cellsBetween = GetCellsBetween(startCell, endCell);

        foreach (var cell in cellsBetween)
        {
            ChessPiece blockingPiece = FindPieceAtCell(cell);
            if (blockingPiece != null) // If any piece is blocking the path
            {
                return false;
            }
        }

        return true; // Path is clear
    }

    public List<string> GetCellsBetween(string startCell, string endCell)
    {
        List<string> cellsBetween = new List<string>();

        int startRow = startCell[1] - '1';
        int startCol = startCell[0] - 'a';

        int endRow = endCell[1] - '1';
        int endCol = endCell[0] - 'a';

        int rowDirection = Mathf.Clamp(endRow - startRow, -1, 1);
        int colDirection = Mathf.Clamp(endCol - startCol, -1, 1);

        int currentRow = startRow + rowDirection;
        int currentCol = startCol + colDirection;

        while (currentRow != endRow || currentCol != endCol)
        {
            string cell = $"{(char)(currentCol + 'a')}{(char)(currentRow + '1')}";
            cellsBetween.Add(cell);

            currentRow += rowDirection;
            currentCol += colDirection;
        }

        return cellsBetween;
    }

    private void HighlightValidMoves(ChessPiece piece)
    {
        foreach (var tile in chessboard.tiles)
        {
            bool highlight = false;

            if (piece != null)
            {
                // Always highlight the cell where the piece is located.
                if (tile.name == piece.CurrentCell)
                {
                    highlight = true;
                }
                else
                {
                    var coord = new Vector2Int(tile.name[0] - 'a', tile.name[1] - '1');
                    if (
                        rules.IsMoveValid(piece, coord, chessboard)
                        && (
                            FindPieceAtCell(tile.name) is ChessPiece occ
                                && occ.isWhite != piece.isWhite
                            || FindPieceAtCell(tile.name) == null
                        )
                    )
                        highlight = true;
                }
            }

            tile.Highlight(highlight);
        }
    }

    public void MovePiece(ChessPiece piece, string targetCell)
    {

        if (piece == null || chessboard == null)
        {
            Debug.LogError("MovePiece called with null piece or chessboard");
            return;
        }

        ChessPiece targetPiece = FindPieceAtCell(targetCell);


        // Sempre termina o jogo se um Rei for capturado.
        if (targetPiece != null && targetPiece is King)
        {
            // ───> Em vez de ResetGame(), exiba o painel de vitória
            if (gameEnded) return;
            gameEnded = true;
            if (chessWatch != null)
                chessWatch.PauseTimers();

            // Determina vencedor: se o rei capturado era branco, as pretas vencem
            string vencedor = targetPiece.isWhite ? "Pretas" : "Brancas";
            if (winText != null)
                winText.text = $"{vencedor} venceram!";
            winPanel.SetActive(true);
            LevelProgressManager.Instance.UnlockLevel(nextIndex);
            return;
        }

        if (targetPiece != null)
        {
            if (targetPiece.isWhite == piece.isWhite)
            {
                Debug.Log("Cannot capture your own piece!");
                return;
            }
            // Normal capture.
            Destroy(targetPiece.gameObject);
            Debug.Log($"Captured piece at {targetCell}");
        }

        // Animate the move.
        StartCoroutine(MovePieceRoutine(piece, targetCell));

        selectedPiece = null;

        if(isLocalMultiplayer)
            CameraSwitcher.Instance.SwitchCamera(!isWhiteTurn);
            
        HighlightValidMoves(null);
    }

    private IEnumerator MovePieceRoutine(ChessPiece piece, string targetCell)
    {
        Vector3 startPos = piece.transform.position;
        Vector3 endPos = chessboard.GetCellPosition(targetCell);
        float duration = 0.3f;
        float elapsed = 0f;



        while (elapsed < duration)
        {
            piece.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        piece.transform.position = endPos;
        piece.CurrentCell = targetCell;

        // ── Fog of War: rebuild now que piece.CurrentCell está correto
        if (rules is FogOfWarRules fow)
            fow.UpdateFog(this, chessboard);

        // Pawn promotion
        if (piece is Pawn)
        {
            if ((piece.isWhite && targetCell[1] == '8') || (!piece.isWhite && targetCell[1] == '1'))
            {
                ShowPromotionPanel((Pawn)piece);
            }
        }

        // Racing Kings: win by getting your King to rank 1
        if (rules is RacingKingsRules && piece is King king && targetCell[1] == '1')
        {
            // ───> Em vez de ResetGame(), exiba o painel de vitória
            if (gameEnded) yield break;
            gameEnded = true;
            if (chessWatch != null)
             chessWatch.PauseTimers();

            string vencedor = king.isWhite ? "Brancas" : "Pretas";
            if (winText != null)
                winText.text = $"{vencedor} venceram em Racing Kings!";
            winPanel.SetActive(true);
            LevelProgressManager.Instance.UnlockLevel(nextIndex);
            yield break;
        }

        // Continue play
        if (!gameEnded && isWhiteTurn && !isLocalMultiplayer && aiPlayer != null)
            StartCoroutine(AutoPlayAITurn(3f));


        yield break;
    }

    // Called to show the promotion UI panel and store the pawn to promote.
    public void ShowPromotionPanel(Pawn pawn)
    {
        pawnToPromote = pawn;
        promotionPanel.SetActive(true);
        // Optionally disable further input until promotion is complete.
    }

    // Called by your UI buttons (OnClick events) com uma string parameter: "Queen", "Rook", "Bishop" ou "Knight".
    public void PromotePawn(string pieceType)
    {
        if (pawnToPromote == null)
            return;

        Vector3 pos = chessboard.GetCellPosition(pawnToPromote.CurrentCell);
        GameObject newPieceObj = null;

        if (pawnToPromote.isWhite)
        {
            if (pieceType == "Queen")
                newPieceObj = Instantiate(whiteQueenPrefab, pos, Quaternion.identity);
            else if (pieceType == "Rook")
                newPieceObj = Instantiate(whiteRookPrefab, pos, Quaternion.identity);
            else if (pieceType == "Bishop")
                newPieceObj = Instantiate(whiteBishopPrefab, pos, Quaternion.identity);
            else if (pieceType == "Knight")
                newPieceObj = Instantiate(whiteKnightPrefab, pos, Quaternion.identity);
        }
        else
        {
            if (pieceType == "Queen")
                newPieceObj = Instantiate(blackQueenPrefab, pos, Quaternion.identity);
            else if (pieceType == "Rook")
                newPieceObj = Instantiate(blackRookPrefab, pos, Quaternion.identity);
            else if (pieceType == "Bishop")
                newPieceObj = Instantiate(blackBishopPrefab, pos, Quaternion.identity);
            else if (pieceType == "Knight")
                newPieceObj = Instantiate(blackKnightPrefab, pos, Quaternion.identity);
        }

        if (newPieceObj != null)
        {
            ChessPiece newPiece = newPieceObj.GetComponent<ChessPiece>();
            newPiece.CurrentCell = pawnToPromote.CurrentCell;
            newPiece.isWhite = pawnToPromote.isWhite;
            newPiece.chessManager = this;

            // Set the scale to match your original pieces.
            newPieceObj.transform.localScale = Vector3.one * 20f;

            // For black knights, rotate them by 180° on the Y axis.
            if (!pawnToPromote.isWhite && pieceType == "Knight")
            {
                newPieceObj.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            Destroy(pawnToPromote.gameObject);
        }

        promotionPanel.SetActive(false);
        pawnToPromote = null;
    }

    private bool DoesMoveRemoveCheck(King king, string targetCell)
    {
        // Simulate the move
        string originalCell = king.CurrentCell;
        ChessPiece targetPiece = FindPieceAtCell(targetCell);

        // Temporarily move the King para o targetCell
        king.CurrentCell = targetCell;
        if (targetPiece != null)
            Destroy(targetPiece.gameObject); // Temporarily remove captured piece

        // Check se o King ainda está em check
        bool isStillInCheck = king.IsKingInCheck();

        // Restore o estado original
        king.CurrentCell = originalCell;
        if (targetPiece != null)
        {
            Instantiate(
                    targetPiece.gameObject,
                    chessboard.GetCellPosition(targetCell),
                    Quaternion.identity
                )
                .GetComponent<ChessPiece>()
                .CurrentCell = targetCell;
        }

        // Return se o movimento removeu o check
        return !isStillInCheck;
    }

    public ChessPiece FindPieceAtCell(string cellName)
    {
        foreach (var piece in Object.FindObjectsByType<ChessPiece>(FindObjectsSortMode.None))
        {
            if (piece.CurrentCell == cellName)
                return piece;
        }
        return null;
    }

    private string FindNearestCell(Vector3 position)
    {
        string nearestCell = null;
        float minDistance = float.MaxValue;

        foreach (var tile in chessboard.tiles)
        {
            float distance = Vector3.Distance(position, tile.GetCenter());
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestCell = tile.name;
            }
        }

        return nearestCell;
    }

    private bool CheckIfKingInCheck()
    {
        King king = FindObjectsByType<King>(FindObjectsSortMode.None)
            .FirstOrDefault(k => k.isWhite == isWhiteTurn);
        return king != null && king.IsKingInCheck();
    }

    private bool CheckForStalemate()
    {
        King king = FindObjectsByType<King>(FindObjectsSortMode.None)
            .FirstOrDefault(k => k.isWhite == isWhiteTurn);
        if (king == null || king.IsKingInCheck())
            return false;

        return !FindObjectsByType<ChessPiece>(FindObjectsSortMode.None)
            .Any(p => p.isWhite == isWhiteTurn && chessboard.tiles.Any(t => p.IsValidMove(t.name)));
    }

    private bool CheckForCheckmate()
    {
        King king = FindObjectsByType<King>(FindObjectsSortMode.None)
            .FirstOrDefault(k => k.isWhite == isWhiteTurn);
        return king != null && king.IsKingInCheck() && !king.GetValidMoves().Any();
    }

    public void ResetGame()
    {
        // Se o painel de vitória estiver ativo, esconda antes de resetar
        if (winPanel != null && winPanel.activeSelf)
            winPanel.SetActive(false);

        gameEnded = false;

        ClearAllPieces();
        if (rules != null)
            rules.InitializeBoard(this, chessboard);

        isWhiteTurn = true;
    }

    /// <summary>
    /// Destroys every ChessPiece on the board e limpa destaques.
    /// </summary>
    public void ClearAllPieces()
    {
        foreach (var piece in Object.FindObjectsByType<ChessPiece>(FindObjectsSortMode.None))
            Destroy(piece.gameObject);
        HighlightValidMoves(null);
    }

    /// <summary>
    /// Called by a rules cartridge after setup to reset turn/promotion state.
    /// </summary>
    public void FinishSetup()
    {
        if (promotionPanel != null)
            promotionPanel.SetActive(false);

        // Ajustar a câmara para as brancas no início
        if (CameraSwitcher.Instance != null)
            CameraSwitcher.Instance.SwitchCamera(true); // Começa com as brancas
        
        if (chessWatch != null)
            chessWatch.StartTimers();
    }


    public List<ChessPiece> GetAllPieces(bool isWhite)
    {
        return FindObjectsByType<ChessPiece>(FindObjectsSortMode.None)
            .Where(p => p.isWhite == isWhite)
            .ToList();
    }
}
