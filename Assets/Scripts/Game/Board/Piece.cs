using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
    int value; //e.g. pawn = 1, rook = 5, Knight = 3, Bishop = 3, Queen = 9, King = Infinity
    new string name;
    bool team; //if white team = 1 

    public void Init(GameObject prefab) 
    {
        this.team = prefab.name.Contains("w");
        Debug.Log(this.team);
        string piece = prefab.name;
        piece = piece[1..];

        switch(piece)
        {
            case "P": 
                this.value = 1;
                this.name = "Pawn";
                return;
            case "R":
                this.value = 5;
                this.name = "Rook";
                return;
            case "N":
                this.value = 3;
                this.name = "Knight";
                return;
            case "B":
                this.value = 3;
                this.name = "Bishop";
                return;
            case "Q":
                this.value = 9;
                this.name = "Queen";
                return;
            case "K":
                this.value = 999999;
                this.name = "King";
                return;
        }
    }

    public string GetName() { return this.name; }
    public int GetValue() { return this.value; }
    public bool IsWhite() { return this.team; }

}