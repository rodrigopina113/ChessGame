using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro; // Se você usar TextMeshPro para o texto do painel de vitória

public class ChessManager : MonoBehaviour
{

    private IChessRules rules;


    public void SetRules(IChessRules rules)
    {
        this.rules = rules;
    }

    //CHESS AI
    public int FullMoveNumber => fullMoveCounter;
    private int fullMoveCounter = 1;


    public bool CanWhiteCastleKingSide;
    public bool CanWhiteCastleQueenSide;
    public bool CanBlackCastleKingSide;
    public bool CanBlackCastleQueenSide;

    public string LastDoubleStepTargetCell;
    public bool IsWhiteTurn => isWhiteTurn;



    public Chessboard chessboard;
    public GameObject[] whitePiecePrefabs;
    public GameObject[] blackPiecePrefabs;
    private ChessPiece selectedPiece;
    private bool isWhiteTurn = true;
    public ChessAI aiPlayer;
    public bool isLocalMultiplayer = false;




    public GameObject promotionPanel;
    private Pawn pawnToPromote;


    public GameObject whiteQueenPrefab;
    public GameObject whiteRookPrefab;
    public GameObject whiteBishopPrefab;
    public GameObject whiteKnightPrefab;


    public GameObject blackQueenPrefab;
    public GameObject blackRookPrefab;
    public GameObject blackBishopPrefab;
    public GameObject blackKnightPrefab;


   [Header("UI de Derrota")]
    public GameObject defeatPanel;
    public TMPro.TextMeshProUGUI defeatText;


    [Header("UI de Vitória")]
    public GameObject winPanel;
    public TextMeshProUGUI winText;
    public GameObject statusObject;
    public TMPro.TextMeshProUGUI statusText;
    private bool gameEnded = false;

    private bool isInCheck = false;

    private int piecesToDrop = 0;
    private bool boardReady = false;

    public int currentIndex = 0;
    private int nextIndex;

    [Header("Relógio de Xadrez")]
    [SerializeField] private ChessWatch chessWatch;

    [Header("Som de Movimento")]
    public AudioClip[] moveSounds;
    private AudioSource audioSource;


    private void Start()
    {

        Debug.Log("👁️ isLocalMultiplayer: " + isLocalMultiplayer);
        nextIndex = currentIndex + 1;

        if (chessboard == null)
        {
            chessboard = Object.FindFirstObjectByType<Chessboard>();
            if (chessboard == null)
            {
                Debug.LogError("Chessboard script not found!");
                return;
            }
        }


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


        if (winPanel != null)
            winPanel.SetActive(false);


        if (rules != null)
        {
            StartGame();
        }


        if (CameraSwitcher.Instance != null)
        {
            CameraSwitcher.Instance.SwitchCamera(isWhiteTurn);
        }
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    }



    public void EndTurn()
    {
        bool currentIsWhite = isWhiteTurn;
        isWhiteTurn = !isWhiteTurn;

        if (isWhiteTurn)
            fullMoveCounter++;

        if (CameraSwitcher.Instance != null)
            CameraSwitcher.Instance.SwitchCamera(isWhiteTurn);

        if (chessWatch != null)
            chessWatch.SwitchTurn();

        // ⚠️ Verifica checkmate logo após mudar de turno
        if (rules.IsCheckmate(isWhiteTurn))
        {
            gameEnded = true;
            if (chessWatch != null)
                chessWatch.PauseTimers();

            bool brancasVenceram = isWhiteTurn == false; // Se ia jogar o branco, foi o preto que deu mate
            string vencedor = brancasVenceram ? "Brancas" : "Pretas";
            string mensagemFinal = $"{vencedor} venceram por xeque-mate!";

            if (isLocalMultiplayer)
            {
                if (defeatText != null) defeatText.text = mensagemFinal;
                if (defeatPanel != null) defeatPanel.SetActive(true);
            }
            else
            {
                if (brancasVenceram)
                {
                    if (defeatText != null) defeatText.text = mensagemFinal;
                    if (defeatPanel != null) defeatPanel.SetActive(true);
                }
                else
                {
                    if (winText != null) winText.text = mensagemFinal;
                    if (winPanel != null) winPanel.SetActive(true);
                }
            }

            LevelProgressManager.Instance.UnlockLevel(nextIndex);
            return;
        }

         // ⚠️ Verifica se o próximo jogador está em xeque
        CheckStatusForCurrentTurn();

        // IA joga no turno das brancas (após o jogador, se não for local)
        if (!isLocalMultiplayer && isWhiteTurn && aiPlayer != null)
        {
            StartCoroutine(TriggerAIMoveAfterDelay(2.5f));
        }
    }


   public void CheckStatusForCurrentTurn()
    {
        Debug.Log($"🕵️‍♂️ CHECK STATUS for {(isWhiteTurn ? "White" : "Black")}");

        bool isCheck = rules.IsKingInCheck(isWhiteTurn);
        bool isMate = rules.IsCheckmate(isWhiteTurn);
        string color = isWhiteTurn ? "Rei Branco" : "Rei Preto";

        Debug.Log($"✅ isCheck = {isCheck}, isMate = {isMate}");

        if (isMate)
        {
            isInCheck = false;
            statusObject.SetActive(true);
            statusText.text = $"{color} em Xeque-mate!";
            winPanel.SetActive(true);
            return;
        }

        if (isCheck)
        {
            isInCheck = true;
            statusObject.SetActive(true);
            statusText.text = $"{color} em Xeque!";
        }
        else
        {
            isInCheck = false;
            statusObject.SetActive(false);
        }
    }






    private void ShowCheckStatus()
    {
        bool currentPlayer = isWhiteTurn;
        bool isCheck = rules.IsKingInCheck(currentPlayer);
        bool isMate = rules.IsCheckmate(currentPlayer);

        string color = currentPlayer ? "Rei Branco" : "Rei Preto";

        if (isMate)
        {
            statusObject.SetActive(true);
            statusText.text = $"{color} em Xeque-mate!";
            winPanel.SetActive(true);
        }
        else if (isCheck)
        {
            statusObject.SetActive(true);
            statusText.text = $"{color} em Xeque!";
        }
        else
        {
            statusObject.SetActive(false);
        }
        if (!statusObject.activeSelf && (isCheck || isMate))
            statusObject.SetActive(true);

    }




    private bool aiScheduled = false;

    private IEnumerator TriggerAIMoveAfterDelay(float delay)
    {
        aiScheduled = true;

        yield return new WaitForSeconds(delay);
        yield return new WaitUntil(() => promotionPanel == null || !promotionPanel.activeSelf);

        if (!gameEnded && isWhiteTurn && aiPlayer != null)
        {
            Debug.Log("🤖 IA vai jogar automaticamente!");
            aiPlayer.MakeMove(() =>
            {
                EndTurn();
            });

        }

        aiScheduled = false;
    }





    public void StartGame()
    {
        if (rules == null)
        {
            Debug.LogError("ChessManager: no IChessRules assigned!");
            return;
        }
        rules.InitializeBoard(this, chessboard);
    }


    public void PlayerMove(string targetCell)
    {
        HandleTileClick(targetCell);
    }

    private IEnumerator AutoPlayAITurn(float delay)
    {
        if (isLocalMultiplayer || aiPlayer == null)
            yield break;


        yield return new WaitUntil(() => boardReady && (promotionPanel == null || !promotionPanel.activeSelf) && aiPlayer != null && aiPlayer.isInitialized);

        yield return new WaitForSeconds(delay);

        if (!gameEnded && isWhiteTurn && aiPlayer != null && (promotionPanel == null || !promotionPanel.activeSelf))
        {
            aiPlayer.MakeMove();
            isWhiteTurn = false;
        }
    }

    public void CheckOpponentStatus()
    {
        bool enemyIsWhite = isWhiteTurn;
        bool isCheck = rules.IsKingInCheck(enemyIsWhite);
        bool isMate = rules.IsCheckmate(enemyIsWhite);

        string color = enemyIsWhite ? "Rei Branco" : "Rei Preto";

        if (isMate)
        {
            statusObject.SetActive(true);
            statusText.text = $"{color} em Xeque-mate!";
            winPanel.SetActive(true);
            return;
        }

        if (isCheck)
        {
            statusObject.SetActive(true);
            statusText.text = $"{color} em Xeque!";
        }
        else
        {
            statusObject.SetActive(false);
        }
    }

    public void CheckForCheckStatus()
    {
        if (rules == null) return;


        bool isCheck = rules.IsKingInCheck(!isWhiteTurn);
        bool isMate = rules.IsCheckmate(!isWhiteTurn);

        string color = !isWhiteTurn ? "Rei Branco" : "Rei Preto";

        if (isMate)
        {
            statusObject.SetActive(true);
            statusText.text = $"{color} em Xeque-mate!";
            winPanel.SetActive(true);
            return;
        }

        if (isCheck)
        {
            statusObject.SetActive(true);
            statusText.text = $"{color} em Xeque!";
        }
        else
        {
            statusObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameEnded)
            return;


        if (!aiScheduled && !isLocalMultiplayer && isWhiteTurn && aiPlayer != null && boardReady)
        {
            aiScheduled = true;
            Debug.Log("⏳ AI programada para jogar em 2.5s");
            StartCoroutine(TriggerAIMoveAfterDelay(2.5f));
        }



        if (Input.GetKeyDown(KeyCode.P))
        {
            LevelProgressManager.Instance.UnlockLevel(nextIndex);
            if (winPanel != null)
                winPanel.SetActive(true);
        }


        if (Input.GetMouseButtonDown(1))
        {
            if (selectedPiece != null)
            {
                selectedPiece = null;
                HighlightValidMoves(null);
                CheckStatusForCurrentTurn();
            }
            return;
        }


        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                string clickedCell = FindNearestCell(hit.point);
                if (!string.IsNullOrEmpty(clickedCell))
                {
                    PlayerMove(clickedCell);
                }
            }
        }
    }

    public void ReplaceWhitePieces()
    {

        var whitePieces = Object
            .FindObjectsByType<ChessPiece>(FindObjectsSortMode.None)
            .Where(p => p.isWhite)
            .ToList();

        foreach (var oldPiece in whitePieces)
        {
            string cell = oldPiece.CurrentCell;
            int prefabIndex = -1;


            if (oldPiece is Rook) prefabIndex = 0;
            else if (oldPiece is Knight) prefabIndex = 1;
            else if (oldPiece is Bishop) prefabIndex = 2;
            else if (oldPiece is Queen) prefabIndex = 3;
            else if (oldPiece is King) prefabIndex = 4;
            else if (oldPiece is Pawn) prefabIndex = 5;
            else continue;


            Vector3 oldScale = oldPiece.transform.localScale;
            Destroy(oldPiece.gameObject);


            Vector3 worldPos = chessboard.GetCellPosition(cell);
            GameObject prefab = whitePiecePrefabs[prefabIndex];
            GameObject newObj = Instantiate(prefab, worldPos, Quaternion.identity);


            var newPiece = newObj.GetComponent<ChessPiece>();
            newPiece.CurrentCell = cell;
            newPiece.isWhite = true;
            newPiece.chessManager = this;
            newObj.transform.localScale = oldScale;
        }
    }


    public void PlacePiece(GameObject prefab, string cellName, float dropDelay, bool isWhite)
    {
        Vector3 targetPosition = chessboard.GetCellPosition(cellName);
        Vector3 startPosition = targetPosition + Vector3.up * 5f;

        GameObject pieceObj = Instantiate(prefab, startPosition, Quaternion.identity);

        ChessPiece piece = pieceObj.GetComponent<ChessPiece>();
        piece.transform.localScale = Vector3.one * 20f;
        piece.CurrentCell = cellName;
        piece.isWhite = isWhite;
        piece.chessManager = this;

        if (piece is not Pawn && rules is RacingKingsRules)
        {
            bool rotateWhite = rules is RacingKingsRules && piece.isWhite;
            if (!piece.isWhite || rotateWhite)
                pieceObj.transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        // Only flip non-white Knights if the black skin isn’t index 2
        if (piece is Knight && !piece.isWhite)
        {
            int blackSkin = SkinManager.Instance.currentBlackSkinIndex;
            if (blackSkin != 2)
            {
                pieceObj.transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            // else: leave at default rotation for skinIndex == 2
        }


        piecesToDrop++;
        StartCoroutine(AnimatePieceDrop(pieceObj, startPosition, targetPosition, dropDelay));
    }


    private IEnumerator AnimatePieceDrop(
        GameObject pieceObj,
        Vector3 start,
        Vector3 end,
        float delay
    )
    {
        float initialGlobalDelay = 1.5f;
        yield return new WaitForSeconds(initialGlobalDelay + delay);

        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (pieceObj == null)
                yield break;

            float t = elapsed / duration;
            t = Mathf.SmoothStep(0, 1, Mathf.SmoothStep(0, 1, t));
            pieceObj.transform.position = Vector3.Lerp(start, end, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (pieceObj != null)
            pieceObj.transform.position = end;

        if (pieceObj != null)
            pieceObj.transform.position = end;

        piecesToDrop--;


        if (piecesToDrop <= 32)
        {
            boardReady = true;
            Debug.Log("✅ boardReady TRUE após queda de peças.");
            FinishSetup();
        }

    }

    private void HandleTileClick(string cellName)
    {


        if (gameEnded)
            return;

        if (!isLocalMultiplayer && isWhiteTurn && aiPlayer != null)
            return;


        if (selectedPiece != null)
        {

            Vector2Int target = new Vector2Int(cellName[0] - 'a', cellName[1] - '1');


            if (
                rules.IsMoveValid(selectedPiece, target, chessboard)
                && !WouldMoveCauseSelfCheck(selectedPiece, cellName)
                && (
                    !CheckIfKingInCheck()
                    || (selectedPiece is King && DoesMoveRemoveCheck((King)selectedPiece, cellName))
                    || IsMoveSafeForKing(selectedPiece, cellName)
                )
            )
            {
                ChessPiece targetPiece = FindPieceAtCell(cellName);


                if (targetPiece != null)
                {
                    if (targetPiece.isWhite != selectedPiece.isWhite)
                    {
                        if (targetPiece is King)
                        {

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


                if (selectedPiece is King king && Mathf.Abs(cellName[0] - king.CurrentCell[0]) == 2)
                    king.MoveTo(cellName);
                else
                    MovePiece(selectedPiece, cellName);

                CheckStatusForCurrentTurn();
                EndTurn();


                if (CheckForCheckmate())
                {
                    if (gameEnded) return;
                    gameEnded = true;
                    if (chessWatch != null)
                        chessWatch.PauseTimers();

                    bool brancasVenceram = isWhiteTurn; // quem fez o checkmate
                    string vencedor = brancasVenceram ? "Brancas" : "Pretas";
                    string mensagemFinal = $"{vencedor} venceram por xeque-mate!";

                    if (isLocalMultiplayer)
                    {
                        if (defeatText != null) defeatText.text = mensagemFinal;
                        if (defeatPanel != null) defeatPanel.SetActive(true);
                    }
                    else
                    {
                        if (brancasVenceram)
                        {
                            // Brancas (IA) venceram → Pretas perderam
                            if (defeatText != null) defeatText.text = mensagemFinal;
                            if (defeatPanel != null) defeatPanel.SetActive(true);
                        }
                        else
                        {
                            // Pretas venceram a IA
                            if (winText != null) winText.text = mensagemFinal;
                            if (winPanel != null) winPanel.SetActive(true);
                        }
                    }

                    LevelProgressManager.Instance.UnlockLevel(nextIndex);
                    return;
                }

                if (CheckForStalemate())
                {

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
                CheckOpponentStatus();



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

            ChessPiece piece = FindPieceAtCell(cellName);

            if (piece != null)
            {
                if (piece.isWhite != isWhiteTurn)
                {
                    Debug.Log("Not your turn!");
                    return;
                }


                if (CheckIfKingInCheck())
                {
                    King kingInCheck = FindObjectsByType<King>(FindObjectsSortMode.None)
                        .FirstOrDefault(k => k.isWhite == isWhiteTurn);
                    if (kingInCheck != null)
                    {
                        var threats = GetThreatsToKing(kingInCheck);
                        if (threats.Count == 1)
                        {

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

                    selectedPiece = piece;
                    if (piece is King)
                    {

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

        var blackPieces = Object
            .FindObjectsByType<ChessPiece>(FindObjectsSortMode.None)
            .Where(p => !p.isWhite)
            .ToList();

        foreach (var oldPiece in blackPieces)
        {
            string cell = oldPiece.CurrentCell;


            int prefabIndex = -1;
            if (oldPiece is Rook) prefabIndex = 0;
            else if (oldPiece is Knight) prefabIndex = 1;
            else if (oldPiece is Bishop) prefabIndex = 2;
            else if (oldPiece is Queen) prefabIndex = 3;
            else if (oldPiece is King) prefabIndex = 4;
            else if (oldPiece is Pawn) prefabIndex = 5;
            else continue;


            Vector3 oldScale = oldPiece.transform.localScale;


            Destroy(oldPiece.gameObject);


            Vector3 worldPos = chessboard.GetCellPosition(cell);
            GameObject prefab = blackPiecePrefabs[prefabIndex];
            GameObject newObj = Instantiate(prefab, worldPos, Quaternion.identity);


            var newPiece = newObj.GetComponent<ChessPiece>();
            newPiece.CurrentCell = cell;
            newPiece.isWhite = false;
            newPiece.chessManager = this;


            newObj.transform.localScale = oldScale;


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

        if (threats.Count == 1)
        {
            ChessPiece checker = threats[0];


            if (piece is King && GetBlockingCells(king, checker).Contains(targetCell))
            {
                return false;
            }


            if (checker.CurrentCell == targetCell)
            {
                return true;
            }


            if (!(piece is King) && GetBlockingCells(king, checker).Contains(targetCell))
            {
                return true;
            }
        }

        return false;
    }

    public bool IsPathClear(string startCell, string endCell, ChessPiece piece)
    {

        if (piece is Knight)
            return true;


        var cellsBetween = GetCellsBetween(startCell, endCell);

        foreach (var cell in cellsBetween)
        {
            ChessPiece blockingPiece = FindPieceAtCell(cell);
            if (blockingPiece != null)
            {
                return false;
            }
        }

        return true;
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

    
    public bool IsValidAIMove(string from, string to)
    {
        ChessPiece piece = FindPieceAtCell(from);
        if (piece == null)
            return false;

        // Corrigido: usa isWhite, não IsWhite
        if (piece.isWhite != IsWhiteTurn)
            return false;

        return true;
    }

    private bool WouldMoveCauseSelfCheck(ChessPiece piece, string targetCell)
    {

        string originalCell = piece.CurrentCell;
        ChessPiece capturedPiece = FindPieceAtCell(targetCell);


        piece.CurrentCell = targetCell;
        if (capturedPiece != null)
            capturedPiece.gameObject.SetActive(false);

        bool kingInCheck = CheckIfKingInCheck();

        piece.CurrentCell = originalCell;
        if (capturedPiece != null)
            capturedPiece.gameObject.SetActive(true);

        return kingInCheck;
    }

    
    public void MovePiece(ChessPiece piece, string targetCell)
    {

        if (piece == null || chessboard == null)
        {
            Debug.LogError("MovePiece called with null piece or chessboard");
            return;
        }

        ChessPiece targetPiece = FindPieceAtCell(targetCell);


        if (targetPiece != null && targetPiece is King)
        {

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

        if (targetPiece != null)
        {
            if (targetPiece.isWhite == piece.isWhite)
            {
                Debug.Log("Cannot capture your own piece!");
                return;
            }

            Destroy(targetPiece.gameObject);
            Debug.Log($"Captured piece at {targetCell}");
        }


        StartCoroutine(MovePieceRoutine(piece, targetCell));



        HighlightValidMoves(null);
    }

    private IEnumerator MovePieceRoutine(ChessPiece piece, string targetCell)
    {
        Vector3 startPos = piece.transform.position;
        Vector3 endPos = chessboard.GetCellPosition(targetCell);

        string fromCell = piece.CurrentCell;


        if (piece is King)
        {
            if (piece.isWhite)
            {
                CanWhiteCastleKingSide = false;
                CanWhiteCastleQueenSide = false;
            }
            else
            {
                CanBlackCastleKingSide = false;
                CanBlackCastleQueenSide = false;
            }
        }


        if (piece is Rook)
        {
            if (piece.isWhite && piece.CurrentCell == "h1") CanWhiteCastleKingSide = false;
            if (piece.isWhite && piece.CurrentCell == "a1") CanWhiteCastleQueenSide = false;
            if (!piece.isWhite && piece.CurrentCell == "h8") CanBlackCastleKingSide = false;
            if (!piece.isWhite && piece.CurrentCell == "a8") CanBlackCastleQueenSide = false;
        }


        if (moveSounds != null && moveSounds.Length > 0 && audioSource != null)
        {
            int randomIndex = Random.Range(0, moveSounds.Length);
            audioSource.PlayOneShot(moveSounds[randomIndex]);
        }

        float duration = 0.3f;
        float elapsed = 0f;

        if (piece is Pawn && Mathf.Abs(targetCell[1] - fromCell[1]) == 2)
        {
            char col = targetCell[0];
            char midRow = (char)((piece.CurrentCell[1] + targetCell[1]) / 2);
            LastDoubleStepTargetCell = $"{col}{midRow}";
        }
        else
        {
            LastDoubleStepTargetCell = null;
        }

        while (elapsed < duration)
        {
            piece.transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        piece.transform.position = endPos;
        piece.CurrentCell = targetCell;
        boardReady = true;


        if (rules is FogOfWarRules fow)
            fow.UpdateFog(this, chessboard);


        if (piece is Pawn)
        {
            if ((piece.isWhite && targetCell[1] == '8') || (!piece.isWhite && targetCell[1] == '1'))
            {
                ShowPromotionPanel((Pawn)piece);
            }
        }


        if (rules is RacingKingsRules && piece is King king && targetCell[1] == '1')
        {

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

        selectedPiece = null;




        HighlightValidMoves(null);



        yield break;
    }


    public void ShowPromotionPanel(Pawn pawn)
    {
        pawnToPromote = pawn;
        promotionPanel.SetActive(true);

    }


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


            newPieceObj.transform.localScale = Vector3.one * 20f;


            if (!pawnToPromote.isWhite && pieceType == "Knight")
            {
                newPieceObj.transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            Destroy(pawnToPromote.gameObject);
        }

        promotionPanel.SetActive(false);
        pawnToPromote = null;
        EndTurn();
    }

    private bool DoesMoveRemoveCheck(King king, string targetCell)
    {

        string originalCell = king.CurrentCell;
        ChessPiece targetPiece = FindPieceAtCell(targetCell);

        king.CurrentCell = targetCell;
        if (targetPiece != null)
            targetPiece.gameObject.SetActive(false);


        bool isStillInCheck = king.IsKingInCheck();


        king.CurrentCell = originalCell;
        if (targetPiece != null)
            targetPiece.gameObject.SetActive(true);


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

        if (winPanel != null && winPanel.activeSelf)
            winPanel.SetActive(false);

        gameEnded = false;

        ClearAllPieces();
        if (rules != null)
            rules.InitializeBoard(this, chessboard);

        isWhiteTurn = true;
    }


    public void ClearAllPieces()
    {
        foreach (var piece in Object.FindObjectsByType<ChessPiece>(FindObjectsSortMode.None))
            Destroy(piece.gameObject);
        HighlightValidMoves(null);
    }


    public void FinishSetup()
    {
        if (promotionPanel != null)
            promotionPanel.SetActive(false);


        if (CameraSwitcher.Instance != null)
            CameraSwitcher.Instance.SwitchCamera(true);

        if (chessWatch != null)
            chessWatch.StartTimers();
    }


    public List<ChessPiece> GetAllPieces(bool isWhite)
    {
        return Object.FindObjectsByType<ChessPiece>(FindObjectsSortMode.None)
            .Where(p => p != null && p.gameObject != null && p.isWhite == isWhite)
            .ToList();
    }

}
