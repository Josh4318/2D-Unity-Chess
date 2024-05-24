using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _camera, _background;

    private Dictionary<Vector2, Tile> _tiles;


    // Start is called before the first frame update
    void Start()
    {
        generateBoard();
        PieceManager pieceManager = GetComponent<PieceManager>();
        if (pieceManager != null)
        {
            pieceManager.Setup(this);
        }

        // Set sorting layer for the background
        _background.GetComponent<SpriteRenderer>().sortingLayerName = "Background";
        _background.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    void generateBoard()
    {
        _tiles = new Dictionary<Vector2, Tile>();
        string[] files = { "A", "B", "C", "D", "E", "F", "G", "H" };

        for (int x = 0; x < _width; x++)
        {
            for(int y = 0; y < _height; y++)
            {
                var newTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity, transform);
                newTile.name = $"Tile {files[x]}{y+1}";

                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                newTile.Init(isOffset);

                // Set sorting layer for tiles
                newTile.GetComponent<SpriteRenderer>().sortingLayerName = "Tiles";
                newTile.GetComponent<SpriteRenderer>().sortingOrder = 1;

                _tiles[new Vector2 (x, y)] = newTile;
            }
        }

        _camera.transform.position = new Vector3((float)_width/2 -0.5f, (float)_height / 2 - 0.5f, -10);
        _background.transform.position = new Vector3((float)_width/2 -0.5f, (float)_height / 2 - 0.5f);
    }

    public Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x, y, (float)-0.1);
    }


    public Tile getTilePosition(Vector2 tilePosition)
    {
        if(_tiles.TryGetValue(tilePosition, out var tile))
        {
            return tile;
        }

        return null;
    }
}
