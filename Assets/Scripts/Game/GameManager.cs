/***********************************************************************
 *
***********************************************************************/

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject chessPiece;
    public BoardManager boardManager;
    public PieceManager pieceManager;
    private List<Move> moveHistory = new List<Move>();
    private Vector3 initialPosition;
    private bool isWhiteTurn = true;
    private Vector2 enPassantMove = new Vector2(-1, -1); //store en passant position

    public struct Move
    {
        public GameObject Piece { get; }
        public Vector2 StartPosition { get; }
        public Vector2 EndPosition { get; }
        public bool WasDoubleMove { get; }

        public Move(GameObject piece, Vector2 startPosition, Vector2 endPosition, bool wasDoubleMove)
        {
            Piece = piece;
            StartPosition = startPosition;
            EndPosition = endPosition;
            WasDoubleMove = wasDoubleMove;
        }
    } 


    // Start is called before the first frame update
    void Start()
    {
        if (boardManager == null)
            boardManager = FindObjectOfType<BoardManager>();
        if (pieceManager == null)
            pieceManager = FindObjectOfType<PieceManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
    }

    void Inputs()
    {
        if (Input.GetMouseButtonDown(1))
        {  
            RightClick();
        }
        if (Input.GetMouseButtonDown(0))
        {  
            LeftClick();
        }
    }
    void LeftClick()
    {
        Debug.Log("Left Click Detected");
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;
        Debug.Log($"Mouse Position: {mousePosition}");

        int movePlateLayerMask = LayerMask.GetMask("MovePlates");
        RaycastHit2D hitMovePlate = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, movePlateLayerMask);

        if (hitMovePlate.collider != null)
        {
            GameObject selectedObject = hitMovePlate.collider.gameObject;
            Debug.Log("MovePLate Hit: " + selectedObject.name);
            MovePiece(selectedObject.transform.parent.gameObject);
        }
        else
        {
            int chessPieceLayerMask = LayerMask.GetMask("Pieces");
            RaycastHit2D hitChessPiece = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, chessPieceLayerMask);
        
            if (hitChessPiece.collider != null)
            {
                GameObject selectedObject = hitChessPiece.collider.gameObject;
                Debug.Log("ChessPiece Hit: " + selectedObject.name);
                Piece piece = selectedObject.GetComponent<Piece>();
                if (piece.IsWhite() == isWhiteTurn)
                {
                    Debug.Log("ChessPiece Selected: " + selectedObject.name);
                    SelectPiece(selectedObject);
                }
            }
            else
            {
                Debug.Log("Raycast did not hit any objects.");
            }
        }
    }
    void RightClick()
    {
        int layerMask = LayerMask.GetMask("Tiles");
        UnityEngine.Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(mousePosition, UnityEngine.Vector2.zero, Mathf.Infinity, layerMask);
        if (hit.collider != null)
        {
            GameObject clickedTile = hit.collider.gameObject;
            if (clickedTile.CompareTag("Tile"))
            {
                Tile tileScript = clickedTile.GetComponent<Tile>();
                if (tileScript != null)
                {
                    Debug.Log("Highlighted " + clickedTile.name);
                    tileScript.RightClickToggle();
                }
                
            }
        }
    }


    void SelectPiece(GameObject piece)
    {
        if (chessPiece != null)
        {
            DeselectCurrentPiece();
        }
        chessPiece = piece;
        initialPosition = piece.transform.position;
        pieceManager.ShowMovePlates(chessPiece);
        Debug.Log("Selected piece: " +(chessPiece != null ? chessPiece.name : "null"));
        Debug.Log("piece size: " + (chessPiece != null ? chessPiece.GetComponent<SpriteRenderer>().size.ToString() : "null"));
        piece.GetComponent<SpriteRenderer>().size = new Vector2(1f, 1f);
    }

    void DeselectCurrentPiece()
    {
        if (chessPiece != null)
        {
            pieceManager.ClearMovePlates();
            chessPiece = null;
        }
    }

    void MovePiece(GameObject targetTile)
    {
        if (chessPiece == null)
        {
            Debug.Log("MovePiece aborted: chessPiece is null.");
            return;
        }

        Debug.Log("MovePiece called with target tile: " + targetTile.name);

        Vector3 position = targetTile.transform.position;
        position.z = chessPiece.transform.position.z;

        Debug.Log("Moving piece to position: " + position);

        // Handle en passant capture
        if (chessPiece.GetComponent<Piece>().GetName() == "Pawn" && IsEnPassantMove(position))
        {
            CaptureEnPassant(position, chessPiece.GetComponent<Piece>().IsWhite());
        }

        // Capture the opponent piece if it exists on the target tile
        CapturePiece(position);

        // Move the piece to the new position
        chessPiece.transform.position = position;

        // Add move to history
        bool wasDoubleMove = chessPiece.GetComponent<Piece>().GetName() == "Pawn" && 
                             Mathf.Abs(position.y - initialPosition.y) == 2;
        moveHistory.Add(new Move(chessPiece, new Vector2(initialPosition.x, initialPosition.y), 
                        new Vector2(position.x, position.y), wasDoubleMove));

        DeselectCurrentPiece();
        EndTurn(); // Switch turns after a move
    }

    bool IsEnPassantMove(Vector3 position)
    {
        if (moveHistory.Count < 2) return false;

        Move lastMove = moveHistory[moveHistory.Count - 1];
        if (lastMove.Piece.GetComponent<Piece>().GetName() == "Pawn" && lastMove.WasDoubleMove)
        {
            if (lastMove.EndPosition == new Vector2(position.x, position.y + (isWhiteTurn ? -1 : 1)))
            {
                return true;
            }
        }
        return false;
    }

    void CapturePiece(Vector3 position)
    {
        Vector3 raycastPosition = position;
        raycastPosition.z = -0.2f; // Ensure the raycast checks the correct Z position for pieces
        int chessPieceLayerMask = LayerMask.GetMask("Pieces");
        Debug.Log("checking smth");
        RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.zero, Mathf.Infinity, chessPieceLayerMask);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("ChessPiece"))
        {
            GameObject hitPiece = hit.collider.gameObject;
            Debug.Log("Capturing piece: " + hitPiece.name);
            if (hitPiece.GetComponent<Piece>().IsWhite() != chessPiece.GetComponent<Piece>().IsWhite())
            {
                Destroy(hitPiece); // Capture (remove) the opponent piece
            }
        }
    }

    void CaptureEnPassant(Vector3 position, bool isWhite)
    {
        int direction = isWhite ? -1 : 1;
        Vector3 capturePosition = new Vector3(position.x, position.y + direction, -0.2f);

        RaycastHit2D hit = Physics2D.Raycast(capturePosition, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("ChessPiece"))
        {
            Destroy(hit.collider.gameObject); // Capture (remove) the opponent piece
            Debug.Log("En Passant capture performed on: " + hit.collider.gameObject.name);
        }
    }

    void EndTurn()
    {
        isWhiteTurn = !isWhiteTurn; //switch turns
        Debug.Log("Turn ended. It is now " + (isWhiteTurn ? "White" : "Black") + "'s turn");
    }

    public void SetEnPassantMove(Vector2 position)
    {
        enPassantMove = position;
    }

    public Vector2 GetEnPassantMove()
    {
        return enPassantMove;
    }

}
