using System.Text;
using UnityEngine;

public static class FenUtility
{
    public static string GenerateFEN(ChessManager manager)
    {
        StringBuilder sb = new StringBuilder();

        // 1. Posicionamento das peças
        for (int row = 7; row >= 0; row--)
        {
            int empty = 0;
            for (int col = 0; col < 8; col++)
            {
                string cell = $"{(char)('a' + col)}{(row + 1)}";
                ChessPiece piece = manager.FindPieceAtCell(cell);

                if (piece == null)
                {
                    empty++;
                    continue;
                }

                if (empty > 0)
                {
                    sb.Append(empty);
                    empty = 0;
                }

                char symbol = GetFENChar(piece);
                sb.Append(symbol);
            }

            if (empty > 0)
                sb.Append(empty);

            if (row > 0)
                sb.Append('/');
        }

        // 2. Cor do jogador atual
        sb.Append(' ');
        sb.Append(manager.IsWhiteTurn ? 'w' : 'b');

        // 3. Castling rights
        sb.Append(' ');
        string castling = "";
        if (manager.CanWhiteCastleKingSide) castling += "K";
        if (manager.CanWhiteCastleQueenSide) castling += "Q";
        if (manager.CanBlackCastleKingSide) castling += "k";
        if (manager.CanBlackCastleQueenSide) castling += "q";
        sb.Append(string.IsNullOrEmpty(castling) ? "-" : castling);

        // 4. En passant target square
        sb.Append(' ');
        sb.Append(manager.LastDoubleStepTargetCell ?? "-");

        // 5. Halfmove clock (opcional, fixo)
        sb.Append(" 0");

        // 6. Fullmove number (opcional, fixo por agora)
        sb.Append(" 1");

        return sb.ToString();
    }

    private static char GetFENChar(ChessPiece piece)
    {
        char c = piece switch
        {
            Pawn => 'p',
            Knight => 'n',
            Bishop => 'b',
            Rook => 'r',
            Queen => 'q',
            King => 'k',
            _ => '?'
        };
        return piece.isWhite ? char.ToUpper(c) : c;
    }
}
