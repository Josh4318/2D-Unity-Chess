using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject chessPiece;
    public BoardManager boardManager;
    public PieceManager pieceManager;
    private List<Tile> moveTiles = new List<Tile>();

    private bool isDragging = false;
    private Vector3 initialPosition;

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
        if (isDragging)
        {
            DragPiece();
        }
    }

    void Inputs()
    {
        if (Input.GetMouseButtonDown(1))
        {  // Right-click
            RightClick();
        }
        if (Input.GetMouseButtonDown(0))
        {  // Left-click
            LeftClick();
        }
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            DropPiece();
        }
    }
    void LeftClick()
    {
        Debug.Log("Left Click Detected");
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log($"Mouse Position: {mousePosition}");

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null)
        {
            GameObject selectedObject = hit.collider.gameObject;
            Debug.Log("Object Hit: " + selectedObject.name);
            if (selectedObject.CompareTag("ChessPiece"))
            {
                Debug.Log("ChessPiece Selected: " + selectedObject.name);
                SelectPiece(selectedObject);
            }
            else if (selectedObject.CompareTag("MovePlate"))
            {
                MovePiece(selectedObject.transform.parent.gameObject);
            }
            else
            {
                Debug.Log("Hit Non-Piece Object: " + selectedObject.name);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any objects.");
        }
    }
    void RightClick()
    {
        int layerMask = LayerMask.GetMask("Tiles");
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);
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
        ShowMovePlates();
        Debug.Log("piece size: " + chessPiece.GetComponent<SpriteRenderer>().size);
        piece.GetComponent<SpriteRenderer>().size = new Vector2(1f, 1f);
    }

    void DeselectCurrentPiece()
    {
        if (chessPiece != null)
        {
            ClearMovePlates();
            chessPiece = null;
        }
    }

    void ShowMovePlates()
    {
        Vector2 piecePosition = chessPiece.transform.position;
        ActivateMovePlate((int)piecePosition.x, (int)piecePosition.y + 1);
        ActivateMovePlate((int)piecePosition.x, (int)piecePosition.y - 1);
        ActivateMovePlate((int)piecePosition.x + 1, (int)piecePosition.y);
        ActivateMovePlate((int)piecePosition.x - 1, (int)piecePosition.y);
    }

    void ClearMovePlates()
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

    void DragPiece()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = chessPiece.transform.position.z;
        chessPiece.transform.position = mousePosition;
    }

    void DropPiece()
    {
        isDragging = false;
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        Debug.Log(hit.collider.gameObject.name);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("MovePlate"))
        {
            Debug.Log("droppyyyy");
            MovePiece(hit.collider.gameObject.transform.parent.gameObject);
        }
        else
        {
            // Return to initial position if not dropped on a valid tile
            chessPiece.transform.position = initialPosition;
        }
        DeselectCurrentPiece();
    }

    void MovePiece(GameObject targetTile)
    {
        Vector3 position = targetTile.transform.position;
        position.z = chessPiece.transform.position.z; // Ensure the z position is maintained
        chessPiece.transform.position = position;
        Debug.Log("Moved piece to: " + position);
        DeselectCurrentPiece();
    }
}
