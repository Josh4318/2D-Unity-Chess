using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceManager : MonoBehaviour
{

    [SerializeField] private GameObject blackPawn, blackRook, blackKnight, blackBishop, blackQueen, blackKing;
    [SerializeField] private GameObject whitePawn, whiteRook, whiteKnight, whiteBishop, whiteQueen, whiteKing;

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

    // Start is called before the first frame update
    void Start()
    {
        
    }
}
