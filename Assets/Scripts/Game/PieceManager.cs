using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PieceManager : MonoBehaviour
{

    [SerializeField] private GameObject blackPawn, blackRook, blackKnight, blackBishop, blackQueen, blackKing;
    [SerializeField] private GameObject whitePawn, whiteRook, whiteKnight, whiteBishop, whiteQueen, whiteKing;
    private List<Tile> moveTiles = new List<Tile>();

    private BoardManager boardManager;

    public void Setup(BoardManager boardManager)
    {
        this.boardManager = boardManager;
        GeneratePieces();
    }

    void GeneratePieces()
    {
        for (int i = 0; i < 8; i++)
        {
            // Instantiate pawns for both sides
            PlacePiece(whitePawn, i, 1); // White pawns
            PlacePiece(blackPawn, i, 6, true); // Black pawns, rotated to face the opposite direction
        }

        PlacePiece(whiteRook, 0, 0);
        PlacePiece(whiteRook, 7, 0);
        PlacePiece(blackRook, 0, 7, true);
        PlacePiece(blackRook, 7, 7, true);

        PlacePiece(whiteKnight, 1, 0);
        PlacePiece(whiteKnight, 6, 0);
        PlacePiece(blackKnight, 1, 7, true);
        PlacePiece(blackKnight, 6, 7, true);

        PlacePiece(whiteBishop, 2, 0);
        PlacePiece(whiteBishop, 5, 0);
        PlacePiece(blackBishop, 2, 7, true);
        PlacePiece(blackBishop, 5, 7, true);

        PlacePiece(whiteQueen, 3, 0);
        PlacePiece(whiteKing, 4, 0);
        PlacePiece(blackQueen, 3, 7, true);
        PlacePiece(blackKing, 4, 7, true);
    }

    private void PlacePiece(GameObject prefab, int x, int y, bool isBlack = false)
    {
        GameObject piece = Instantiate(prefab, boardManager.GetTileCenter(x, y), Quaternion.identity, transform);
        if (isBlack)
        {
            piece.transform.Rotate(0, 180, 0);
        }
        piece.name = prefab.name;

        // Set sorting layer for pieces
        piece.GetComponent<SpriteRenderer>().sortingLayerName = "Pieces";
        piece.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    bool IsWhite(GameObject chessPiece) 
    {
        if (chessPiece.name.Contains("White")) return true;
        else return false;
    }

    public void ShowMovePlates(GameObject chessPiece)
    {
        ClearMovePlates();
        Vector2 piecePosition = chessPiece.transform.position;
        string pieceName = chessPiece.name;

        switch (GetPieceType(pieceName))
        {
            case "Pawn":
                PawnMoves(piecePosition, pieceName.Contains("white"));
                return;
            case "Rook":
                RookMoves(piecePosition);
                return;
            case "Knight":
                KnightMoves(piecePosition);
                return;
            case "Bishop":
                BishopMoves(piecePosition);
                return;
            case "Queen":
                QueenMoves(piecePosition);
                return;
            case "King":
                KingMoves(piecePosition);
                return;
        }
    }

    public void ClearMovePlates()
    {
        foreach (Tile tile in moveTiles)
        {
            tile.HideMovePlate();
        }
        moveTiles.Clear();
    }

    void ActivateMovePlate(int x, int y)
    {
        Tile tile = boardManager.getTilePosition(new Vector2(x, y));
        if (tile != null)
        {
            Debug.Log("Activating Move Plate on Tile: " + tile.name);
            tile.ShowMovePlate();
            moveTiles.Add(tile);
        }
    }

    void CaptureMovePlate(int x, int y) {
        
        Tile tile = boardManager.getTilePosition(new Vector2(x, y));
        if (tile != null) {

            RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("ChessPiece"))
            {
                GameObject hitPiece = hit.collider.gameObject;
            }

        }
    }

    string GetPieceType(string pieceName)
    {
        if (pieceName.Contains("Pawn")) return "Pawn";
        if (pieceName.Contains("Rook")) return "Rook";
        if (pieceName.Contains("Knight")) return "Knight";
        if (pieceName.Contains("Bishop")) return "Bishop";
        if (pieceName.Contains("Queen")) return "Queen";
        if (pieceName.Contains("King")) return "King";
        return null;
    }


    void PawnMoves(Vector2 piecePosition, bool isWhite) {
        int direction = isWhite ? 1 : -1;
        int startRank = isWhite ? 1 : 6;
        
        //if pawn is white add tile up 1, if black add 1 down
        ActivateMovePlate((int)piecePosition.x, (int)piecePosition.y + direction);

        //if on starting position allow double move
        if((int)piecePosition.y == startRank) {
            ActivateMovePlate((int)piecePosition.x, (int)piecePosition.y + (2 * direction));
        }

        

    }

    void RookMoves(Vector2 piecePosition) {

    }

    void KnightMoves(Vector2 piecePosition) {

    }

    void BishopMoves(Vector2 piecePosition) {

    }

    void QueenMoves(Vector2 piecePosition) {

    }

    void KingMoves(Vector2 piecePosition) {

    }
}
