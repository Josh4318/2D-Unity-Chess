using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
    int value; //e.g. pawn = 1, rook = 5, Knight = 3, Bishop = 3, Queen = 9, King = Infinity
    bool team; //if white team = 1 

    public void Init(GameObject prefab) 
    {
        this.team = prefab.name.Contains("white") ? true : false;

        string piece = prefab.name;
        piece = piece[1..];
        
        switch(piece)
        {
            case "P": 
                this.value = 1;
                return;
            case "R":
                this.value = 5;
                return;
            case "N":
                this.value = 3;
                return;
            case "B":
                this.value = 3;
                return;
            case "Q":
                this.value = 9;
                return;
            case "K":
                this.value = 999999;
                return;
        }
    }

    public int GetValue() { return this.value; }
    public bool IsWhite() { return this.team; }

}