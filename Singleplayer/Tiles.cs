using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tiles : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum TileState
    {
        Hit, Miss, Ship, Empty
    }

    public enum TileMode
    {
        Placement, Playing
    }

    public TileState state = TileState.Empty;
    public int TileX;
    public int TileY;
    public ShipManager shipManager;
    public TileMode currentMode = TileMode.Placement;
    private Color _originalColor;
    private Image _image;
    private Button _button;
    public Action<int, int> _onClick;


    public void Init(int x, int y, Action<int, int> onClick)
    {
        this.TileX = x;
        this.TileY = y;
        _onClick = onClick;
        _button = GetComponent<Button>();
        _image = GetComponent<Image>(); 
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(OnClick); 

    }

    public void OnClick()
    {


        if (currentMode == TileMode.Placement)
        {
            if (shipManager != null)
                shipManager.PlacementCLickTile(TileX, TileY);
        }
        else if (currentMode == TileMode.Playing)
        {
            _onClick?.Invoke(TileX, TileY);
        }

    }

    private void Awake()
    {
        if (_image == null)
        {
            _image = GetComponent<Image>();

        }

        if (_button == null)
        {
            _button = GetComponent<Button>();

        }


        if (_button != null)
        {
            _button.onClick.AddListener(OnClick);
        }
    }


    public void SetTileColor(Color c)
    {
        if (_image)
        {
            _image.color = c;
        }
    }

    public void HighlightTile(Color c)
    {

        if (_image) _image.color = c;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (shipManager != null && currentMode == TileMode.Placement)
        {
            shipManager.ShowPlacementPreview(TileX, TileY);
        }
    }


    public void OnPointerExit(PointerEventData eventData) //when mouse isnt hovering
    {
        if (shipManager != null)
        {
            shipManager.ClearPreview();
        }
    }


    public void ResetColor()
    {
        if (_image)
        {
            _image.color = new Color32(153, 217, 234, 255);
        }

    }

    public void ResetToStateColor()
    {
        if (_image == null) return;

        switch (state)
        {
            case TileState.Empty:
                _image.color = new Color32(153, 217, 234, 255);
                break;
            case TileState.Ship:
                _image.color = Color.gray;
                break;
            case TileState.Hit:
                _image.color = Color.red;
                break;
            case TileState.Miss:
                _image.color = Color.blue;
                break;
        }
    }

}
