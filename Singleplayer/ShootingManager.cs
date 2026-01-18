using TMPro;
using UnityEngine;

public class ShootingManager : MonoBehaviour
{
    public EnemyPlacement enemyPlacement;
    public GridManager enemyGrid;
    public TurnManager turnManager;

    private void Awake()
    {
        enemyPlacement.PlaceAllEnemyShips();
    }

    void Start()
    {

        for (int r = 0; r < enemyGrid.height; r++)
        {

            for (int c = 0; c < enemyGrid.width; c++)
            {

                Tiles tile = enemyGrid.GetTile(r, c);
                tile.Init(r, c, OnEnemyTileCLick);
                
            }
        }
    }


    public void OnEnemyTileCLick(int tileX, int tileY)
    {
        Tiles tile = enemyGrid.GetTile(tileX, tileY);

        if (tile.state == Tiles.TileState.Hit || tile.state == Tiles.TileState.Miss) //if the tile has alrealdy been clicked on do nothing
        {
            return;
        }

        if (turnManager != null)
        {
            turnManager.PlayerTurn(tileX, tileY); // go to TurnManager
        }
        else
        {
            Debug.LogWarning("TurnManager not assigned in ShootingManager!");
        }
    }


    public bool ShootEnemyTile(int row, int col)
    {
        Tiles tile = enemyGrid.GetTile(row, col);
        Debug.Log($"{tile.state}");

        if (tile.state == Tiles.TileState.Hit || tile.state == Tiles.TileState.Miss)
        {
            Debug.Log($"{tile.state} {row},{col}");
            return false;
        }

        if (tile.state == Tiles.TileState.Ship)
        {
            tile.state = Tiles.TileState.Hit;
            tile.SetTileColor(Color.red);
            turnManager.UpdateShipsLeftText();
            return true;
        }
        else
        {
            tile.state = Tiles.TileState.Miss;
            tile.SetTileColor(Color.blue);

            return false;
        }

    }


    public void SetTileCallbacks(GridManager enemyGrid)
    {
        for (int r = 0; r < enemyGrid.height; r++)
        {
            for (int c = 0; c < enemyGrid.width; c++)
            {
                Tiles tile = enemyGrid.GetTile(r, c);
                tile.Init(r, c, OnEnemyTileCLick); // shooting callback
            }
        }
    }


}
