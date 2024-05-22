using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColour, _offColour;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _rightClickHighlight;
    [SerializeField] private GameObject _movePlate;

    public void Init(bool isOffset)
    {
        _spriteRenderer.color = isOffset ? _offColour : _baseColour;
        _rightClickHighlight.SetActive(false); // Ensure right-click highlight is off by default
        _movePlate.SetActive(false); // Ensure move plate is off by default
    }

    public void RightClickToggle()
    {
        _rightClickHighlight.SetActive(!_rightClickHighlight.activeSelf);
    }

    public void ShowMovePlate()
    {
        _movePlate.SetActive(true);
    }

    public void HideMovePlate()
    {
        _movePlate.SetActive(false);
    }

}
