using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShipManager : MonoBehaviour
{
    public List<Ship> ShipsToPlace;
    public int currShipIndex = 0;
    private bool _isHorizontal = true;
    public GridManager PlayerGrid;
    public TextMeshProUGUI TitleText;


    void Start()
    {

        ShipsToPlace = new List<Ship>() //puts ships in list
        {
            new Ship() { ShipName = "CARRIER", ShipSize = 5 },
            new Ship() { ShipName = "BATTLESHIP", ShipSize = 4 },
            new Ship() { ShipName = "CRUISER", ShipSize = 3 },
            new Ship() { ShipName = "SUBMARINE", ShipSize = 3 },
            new Ship() { ShipName = "DESTROYER", ShipSize = 2 }
        };


        TitleChange(ShipsToPlace[currShipIndex]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleShip();
        }
    }

    public void ToggleShip()
    {
        _isHorizontal = !_isHorizontal;
    }


    public bool CanPlaceShip(int startX, int startY, int size, bool horizontal)
    {
        for (int i = 0; i < size; i++)
        {
            int x = startX;
            int y = startY;

            if (horizontal) //if its horizontal see it this way
            {
                y += i;
            }

            else //if its not horizontal see it this way
            {
                x += i;
            }


            if (x < 0 || x >= PlayerGrid.height || y < 0 || y >= PlayerGrid.width)
            {
                return false;
            }

            if (PlayerGrid.GetTile(x, y).state != Tiles.TileState.Empty) //if theres a ship there
            {
                return false;
            }


        }
        return true;
    }

    public void PlaceShip(int startX, int startY, Ship ship, bool horizontal)
    {
        ship.isHorizontal = horizontal;
        ship.positions.Clear();

        for (int i = 0; i < ship.ShipSize; i++)
        {
            int x = startX;
            int y = startY;

            if (horizontal) //if its horizontal see it this way
            {
                y += i;
            }

            else //if its not horizontal see it this way
            {
                x += i;
            }
            Tiles tile = PlayerGrid.GetTile(x, y);
            tile.state = Tiles.TileState.Ship;
            tile.SetTileColor(Color.gray);
            ship.positions.Add(new Vector2Int(x, y));
        }

    }


    private List<Tiles> _lastPreviewTiles = new List<Tiles>();
    public void ShowPlacementPreview(int startX, int startY)
    {
        ClearPreview();

        if (currShipIndex >= ShipsToPlace.Count)
            return;

        Ship currShip = ShipsToPlace[currShipIndex];
        Color previewColor = CanPlaceShip(startX, startY, currShip.ShipSize, _isHorizontal) ? Color.green : Color.red;
        for (int i = 0; i < currShip.ShipSize; i++)
        {
            int x = startX;
            int y = startY;

            if (_isHorizontal)
            {
                y += i;
            }

            else
            {
                x += i;
            }


            if (x >= 0 && x < PlayerGrid.height && y >= 0 && y < PlayerGrid.width)
            {
                PlayerGrid.GetTile(x, y).HighlightTile(previewColor);
            }
        }
    }




    public void ClearPreview()
    {
        for (int row = 0; row < PlayerGrid.height; row++)
        {
            for (int col = 0; col < PlayerGrid.width; col++)
            {
                Tiles tile = PlayerGrid.GetTile(row, col);

                if (tile.state == Tiles.TileState.Empty)
                {
                    tile.ResetColor();
                }
                else if (tile.state == Tiles.TileState.Ship)
                {
                    tile.ResetToStateColor();
                }
            }
        }
    }



    public void PlacementCLickTile(int x, int y)
    {
        if (currShipIndex >= ShipsToPlace.Count)
        {
            return;
        }

        Ship currShip = ShipsToPlace[currShipIndex];
        if (CanPlaceShip(x, y, currShip.ShipSize, _isHorizontal))
        {
            PlaceShip(x, y, currShip, _isHorizontal);
            currShipIndex++;
        }

        if (currShipIndex >= ShipsToPlace.Count)
        {
            TitleText.text = "START GAME!";
        }
        else
        {
            TitleChange(ShipsToPlace[currShipIndex]);
        }
    }

    public void TitleChange(Ship currShip)
    {
        TitleText.text = $"PLACE: {currShip.ShipName}";
    }


    public bool AllShipsPlaced()
    {
        if (currShipIndex >= ShipsToPlace.Count)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    public void SetTileCallbacks()
    {
        for (int r = 0; r < PlayerGrid.height; r++)
        {
            for (int c = 0; c < PlayerGrid.width; c++)
            {
                Tiles tile = PlayerGrid.GetTile(r, c);

                tile.shipManager = this;

                tile.Init(r, c, PlacementCLickTile);
                tile.currentMode = Tiles.TileMode.Placement;
                tile.ResetToStateColor();
            }
        }
    }


    public void ResetShips()
    {
        currShipIndex = 0;
        foreach (var ship in ShipsToPlace)
        {
            ship.positions.Clear();
        }
        if (ShipsToPlace.Count > 0)
        {
            TitleChange(ShipsToPlace[currShipIndex]);
        }
    }


}
