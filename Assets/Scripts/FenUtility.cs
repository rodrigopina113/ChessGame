using System.Text;
using UnityEngine;

public static class FenUtility
{
    public static string GenerateFEN(ChessManager manager)
    {
        StringBuilder sb = new StringBuilder();

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

        sb.Append(' ');
        sb.Append(manager.IsWhiteTurn ? 'w' : 'b');

        sb.Append(' ');
        string castling = "";
        if (manager.CanWhiteCastleKingSide) castling += "K";
        if (manager.CanWhiteCastleQueenSide) castling += "Q";
        if (manager.CanBlackCastleKingSide) castling += "k";
        if (manager.CanBlackCastleQueenSide) castling += "q";
        sb.Append(string.IsNullOrEmpty(castling) ? "-" : castling);

        sb.Append(' ');
        sb.Append(string.IsNullOrEmpty(manager.LastDoubleStepTargetCell) ? "-" : manager.LastDoubleStepTargetCell);

        sb.Append(" 0");

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
