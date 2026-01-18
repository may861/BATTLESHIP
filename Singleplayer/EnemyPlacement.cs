using System.Collections.Generic;
using UnityEngine;

public class EnemyPlacement : MonoBehaviour
{
    public GridManager enemyGrid;
    public List<Ship> enemyShips;


    public void PlaceAllEnemyShips()
    {
        enemyShips = new List<Ship>()
        {
            new Ship() { ShipName = "CARRIER", ShipSize = 5 },
            new Ship() { ShipName = "BATTLESHIP", ShipSize = 4 },
            new Ship() { ShipName = "CRUISER", ShipSize = 3 },
            new Ship() { ShipName = "SUBMARINE", ShipSize = 3 },
            new Ship() { ShipName = "DESTROYER", ShipSize = 2 }
        };

        enemyGrid.GenerateGrid();

        foreach (Ship ship in enemyShips)
        {
            PlaceShipRandomly(ship);
        }
    }


    public void PlaceShipRandomly(Ship ship)
    {
        bool placed = false;
        while (!placed)
        {
            bool horizontal = Random.value < 0.5f;
            int row = Random.Range(0, enemyGrid.height);
            int col = Random.Range(0, enemyGrid.width);

            if (CanPlaceShip(row, col, ship.ShipSize, horizontal))
            {
                ship.isHorizontal = horizontal;
                ship.positions.Clear();

                for (int i = 0; i < ship.ShipSize; i++)
                {
                    int posRow = row;
                    int posCol = col;

                    if (horizontal) //if its horizontal see it this way
                    {
                        posCol += i;
                    }

                    else //if its not horizontal see it this way
                    {
                        posRow += i;
                    }

                    Tiles tile = enemyGrid.GetTile(posRow, posCol); 
                    tile.state = Tiles.TileState.Ship;
                    ship.positions.Add(new Vector2Int(posRow, posCol));

                    Debug.Log($"enemy Placed {ship.ShipName} part at [{posRow},{posCol}] state={tile.state}");

                }
                placed = true;
            }

        }

    }

    public bool CanPlaceShip(int startRow, int startCol, int size, bool horizontal)
    {
        for (int i = 0; i < size; i++)
        {
            int row = startRow;
            int col = startCol;

            if (horizontal) //if its horizontal see it this way
            {
                col += i;
            }

            else //if its not horizontal see it this way
            {
                row += i;
            }


            if (row < 0 || row >= enemyGrid.height || col < 0 || col >= enemyGrid.width)
            {
                return false;
            }

            if (enemyGrid.GetTile(row, col).state != Tiles.TileState.Empty) //if theres a ship there
            {
                return false;
            }


        }
        return true;
    }



}
