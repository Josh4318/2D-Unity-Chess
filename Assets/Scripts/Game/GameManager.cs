/***********************************************************************
 *
***********************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject chessPiece;
    public BoardManager boardManager;
    public PieceManager pieceManager;

    private Vector3 initialPosition;
    private bool isWhiteTurn = true;
    private List<GameObject> activeMovePlates = new List<GameObject>();


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
        Debug.Log("piece size: " + chessPiece.GetComponent<SpriteRenderer>().size);
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
        Vector3 position = targetTile.transform.position;
        position.z = chessPiece.transform.position.z; // Ensure the z position is maintained

        CapturePiece(position);
        
        chessPiece.transform.position = position;
        Debug.Log("Moved piece to: " + position);
        DeselectCurrentPiece();
        EndTurn();
    }

    void CapturePiece(Vector3 position)
    {
        Vector3 raycastPosition = position;
        raycastPosition.z = -0.2f; //raycast checks the correct Z position for pieces
        RaycastHit2D hit =  Physics2D.Raycast(raycastPosition, Vector2.zero);
        Debug.Log("Raycast mouse position is: " + raycastPosition);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("ChessPiece"))
        {
            GameObject hitPiece = hit.collider.gameObject;
            Debug.Log("Capturing Piece: " + hitPiece.name);
            if (hitPiece.GetComponent<Piece>().IsWhite() != chessPiece.GetComponent<Piece>().IsWhite()) 
            {
                Destroy(hitPiece); //capture piece
            }
        }
    }

        void EndTurn()
    {
        isWhiteTurn = !isWhiteTurn; //switch turns
        Debug.Log("Turn ended. It is now " + (isWhiteTurn ? "White" : "Black") + "'s turn");
    }

}
