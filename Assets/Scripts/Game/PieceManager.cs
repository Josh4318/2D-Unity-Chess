using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PieceManager : MonoBehaviour
{

    [SerializeField] private GameObject blackPawn, blackRook, blackKnight, blackBishop, blackQueen, blackKing;
    [SerializeField] private GameObject whitePawn, whiteRook, whiteKnight, whiteBishop, whiteQueen, whiteKing;

    private BoardManager boardManager;
    private GameManager gameManager;
    private List<Tile> moveTiles = new List<Tile>();

    public void Setup(BoardManager boardManager)
    {
        this.boardManager = boardManager;
        gameManager = FindObjectOfType<GameManager>();
        GeneratePieces();
    }

    void GeneratePieces()
    {
        for (int i = 0; i < 8; i++)
        {
            // Instantiate pawns for both sides
            PlacePiece(whitePawn, i, 1); // White pawns
            PlacePiece(blackPawn, i, 6); // Black pawns, rotated to face the opposite direction
        }

        PlacePiece(whiteRook, 0, 0);
        PlacePiece(whiteRook, 7, 0);
        PlacePiece(blackRook, 0, 7);
        PlacePiece(blackRook, 7, 7);

        PlacePiece(whiteKnight, 1, 0);
        PlacePiece(whiteKnight, 6, 0);
        PlacePiece(blackKnight, 1, 7);
        PlacePiece(blackKnight, 6, 7);

        PlacePiece(whiteBishop, 2, 0);
        PlacePiece(whiteBishop, 5, 0);
        PlacePiece(blackBishop, 2, 7);
        PlacePiece(blackBishop, 5, 7);

        PlacePiece(whiteQueen, 3, 0);
        PlacePiece(whiteKing, 4, 0);
        PlacePiece(blackQueen, 3, 7);
        PlacePiece(blackKing, 4, 7);
    }

    private void PlacePiece(GameObject prefab, int x, int y, bool isBlack = false)
    {
        GameObject piece = Instantiate(prefab, boardManager.GetTileCenter(x, y), Quaternion.identity, transform);
        piece.name = prefab.name;
        Piece pieceComponent = piece.AddComponent<Piece>(); 
        pieceComponent.Init(prefab);

        if (!pieceComponent.IsWhite())
        {
            piece.transform.Rotate(0, 180, 0);
        }
        piece.name = prefab.name;

        // Set sorting layer for pieces
        piece.GetComponent<SpriteRenderer>().sortingLayerName = "Pieces";
        piece.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    public void ShowMovePlates(GameObject chessPiece)
    {
        ClearMovePlates();
        Vector2 piecePosition = chessPiece.transform.position;
        string pieceName = chessPiece.GetComponent<Piece>().GetName();

        Debug.Log(pieceName);
        switch (GetPieceType(pieceName))
        {
            case "Pawn":
                PawnMoves(chessPiece, piecePosition, chessPiece.GetComponent<Piece>().IsWhite());
                return;
            case "Rook":
                RookMoves(piecePosition, chessPiece);
                return;
            case "Knight":
                KnightMoves(piecePosition, chessPiece);
                return;
            case "Bishop":
                BishopMoves(piecePosition, chessPiece);
                return;
            case "Queen":
                QueenMoves(piecePosition, chessPiece);
                return;
            case "King":
                KingMoves(piecePosition, chessPiece);
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

    bool ActivateMovePlate(int x, int y)
    {
        Tile tile = boardManager.getTilePosition(new Vector2(x, y));
        if (tile != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, Vector2.zero);
            if (hit.collider == null || !hit.collider.gameObject.CompareTag("ChessPiece"))
            {
                tile.ShowMovePlate();
                moveTiles.Add(tile);
                return true;
            }
        }
        return false;
    }

    void ActivateCaptureMovePlate(int x, int y, GameObject chessPiece) 
    {
        Tile tile = boardManager.getTilePosition(new Vector2(x, y));
        if (tile != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, Vector2.zero);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("ChessPiece"))
            {
                GameObject hitPiece = hit.collider.gameObject;
                if (hitPiece.GetComponent<Piece>().IsWhite() != chessPiece.GetComponent<Piece>().IsWhite())
                {
                    tile.ShowMovePlate();
                    moveTiles.Add(tile);
                }
            }
        }
    }

    void PawnMoves(GameObject chessPiece, Vector2 piecePosition, bool isWhite) 
    {
        int direction = isWhite ? 1 : -1;
        int startRank = isWhite ? 1 : 6;

        // Regular move
        bool canMoveOneSquare = ActivateMovePlate((int)piecePosition.x, (int)piecePosition.y + direction);

        // Double move from starting position
        if (canMoveOneSquare && (int)piecePosition.y == startRank)
        {
            if (ActivateMovePlate((int)piecePosition.x, (int)piecePosition.y + (2 * direction)))
            {
                gameManager.SetEnPassantMove(new Vector2((int)piecePosition.x, (int)piecePosition.y + direction));
                Debug.Log($"En passant set for {(int)piecePosition.x}, {(int)piecePosition.y + direction}");
            }
        }

        // Capture moves
        ActivateCaptureMovePlate((int)piecePosition.x + 1, (int)piecePosition.y + direction, chessPiece);
        ActivateCaptureMovePlate((int)piecePosition.x - 1, (int)piecePosition.y + direction, chessPiece);

        // En passant
        Vector2 enPassantMove = gameManager.GetEnPassantMove();
        if (enPassantMove == new Vector2(piecePosition.x + 1, piecePosition.y + direction) ||
            enPassantMove == new Vector2(piecePosition.x - 1, piecePosition.y + direction))
        {
            Tile tile = boardManager.getTilePosition(enPassantMove);
            if (tile != null)
            {
                tile.ShowMovePlate();
                moveTiles.Add(tile);
                Debug.Log($"En passant move plate shown for {enPassantMove}");
            }
        }

        // Reset en passant if not a double move
        if ((int)piecePosition.y != startRank || !canMoveOneSquare)
        {
            gameManager.SetEnPassantMove(new Vector2(-1, -1)); // Reset if no en passant is possible
            Debug.Log("En passant reset");
        }
    }

    void RookMoves(Vector2 piecePosition, GameObject chessPiece)
    {
        for (int i = 1; i < 8; i++)
        {
            if (!ActivateMovePlateOrBreak((int)piecePosition.x + i, (int)piecePosition.y, chessPiece)) break;
        }
        for (int i = 1; i < 8; i++)
        {
            if (!ActivateMovePlateOrBreak((int)piecePosition.x - i, (int)piecePosition.y, chessPiece)) break;
        }
        for (int i = 1; i < 8; i++)
        {
            if (!ActivateMovePlateOrBreak((int)piecePosition.x, (int)piecePosition.y + i, chessPiece)) break;
        }
        for (int i = 1; i < 8; i++)
        {
            if (!ActivateMovePlateOrBreak((int)piecePosition.x, (int)piecePosition.y - i, chessPiece)) break;
        }
    }

    void KnightMoves(Vector2 piecePosition, GameObject chessPiece) 
    {
        ActivateMovePlateOrCapturePlate((int)piecePosition.x + 1, (int)piecePosition.y + 2, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x + 1, (int)piecePosition.y - 2, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x - 1, (int)piecePosition.y + 2, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x - 1, (int)piecePosition.y - 2, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x + 2, (int)piecePosition.y + 1, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x + 2, (int)piecePosition.y - 1, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x - 2, (int)piecePosition.y + 1, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x - 2, (int)piecePosition.y - 1, chessPiece);
    }

    void BishopMoves(Vector2 piecePosition, GameObject chessPiece) 
    {
        for (int i = 1; i < 8; i++)
        {
            if (!ActivateMovePlateOrBreak((int)piecePosition.x + i, (int)piecePosition.y + i, chessPiece)) break;
        }
        for (int i = 1; i < 8; i++)
        {
            if (!ActivateMovePlateOrBreak((int)piecePosition.x + i, (int)piecePosition.y - i, chessPiece)) break;
        }
        for (int i = 1; i < 8; i++)
        {
            if (!ActivateMovePlateOrBreak((int)piecePosition.x - i, (int)piecePosition.y + i, chessPiece)) break;
        }
        for (int i = 1; i < 8; i++)
        {
            if (!ActivateMovePlateOrBreak((int)piecePosition.x - i, (int)piecePosition.y - i, chessPiece)) break;
        }
    }

    void QueenMoves(Vector2 piecePosition, GameObject chessPiece) 
    {
        RookMoves(piecePosition, chessPiece);
        BishopMoves(piecePosition, chessPiece);
    }

    void KingMoves(Vector2 piecePosition, GameObject chessPiece) 
    {
        ActivateMovePlateOrCapturePlate((int)piecePosition.x + 1, (int)piecePosition.y, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x - 1, (int)piecePosition.y, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x, (int)piecePosition.y + 1, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x, (int)piecePosition.y - 1, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x + 1, (int)piecePosition.y + 1, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x - 1, (int)piecePosition.y + 1, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x + 1, (int)piecePosition.y - 1, chessPiece);
        ActivateMovePlateOrCapturePlate((int)piecePosition.x - 1, (int)piecePosition.y - 1, chessPiece);
    }

    /*if a piece is found move plates will stop being generated*/
    bool ActivateMovePlateOrBreak(int x, int y, GameObject chessPiece)
    {
        Tile tile = boardManager.getTilePosition(new Vector2(x, y));
        if (tile != null)
        {
            Debug.Log("Checking tile at: " + x + ", " + y);
            int layerMask = LayerMask.GetMask("Pieces");
            RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, Vector2.zero, Mathf.Infinity, layerMask);
            Debug.Log(hit);
            if (hit.collider == null)
            {
                tile.ShowMovePlate();
                moveTiles.Add(tile);
                Debug.Log("Move plate activated at: " + x + ", " + y);
                return true;
            }
            else if (hit.collider.gameObject.CompareTag("ChessPiece"))
            {
                GameObject hitPiece = hit.collider.gameObject;
                Debug.Log("Piece detected at: " + x + ", " + y + " - " + hitPiece.name);
                if (hitPiece.GetComponent<Piece>().IsWhite() != chessPiece.GetComponent<Piece>().IsWhite())
                {
                    tile.ShowMovePlate();
                    moveTiles.Add(tile);
                    Debug.Log("Capture move plate activated at: " + x + ", " + y);
                }
                return false;
            }
        }
        else
        {
            Debug.Log("No tile found at: " + x + ", " + y);
        }
        return false;
    }

    void ActivateMovePlateOrCapturePlate(int x, int y, GameObject chessPiece)
    {
        Tile tile = boardManager.getTilePosition(new Vector2(x, y));
        if (tile != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, Vector2.zero);
            if (hit.collider != null)
            {
                tile.ShowMovePlate();
                moveTiles.Add(tile);
            }
            else if (hit.collider.gameObject.CompareTag("Chesspiece"))
            {
                GameObject hitPiece = hit.collider.gameObject;
                if (hitPiece.GetComponent<Piece>().IsWhite() != chessPiece.GetComponent<Piece>().IsWhite())
                {
                    tile.ShowMovePlate();
                    moveTiles.Add(tile);
                }                
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
}
