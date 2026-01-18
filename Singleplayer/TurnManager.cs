using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public ShootingManager shootingManager;
    public GridManager playerGrid;
    public GridManager enemyGrid;
    public TextMeshProUGUI sinkedText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI shipsLeftText;
    public EnemyPlacement enemyPlacement;
    public GameObject restartButton;
    public GameObject menuButton;
    public bool isPlayerTurn = true;
    public bool isGameOver = false;

    void Start()
    {
        UpdateBoardVisibility();
        UpdateShipsLeftText();
    }


    public void UpdateShipsLeftText()
    {
        int shipsLeft = 0;

        // loop over all enemy ships
        for (int s = 0; s < enemyPlacement.enemyShips.Count; s++)
        {
            Ship ship = enemyPlacement.enemyShips[s];
            bool shipSunk = true;

            // check all positions of this ship
            for (int i = 0; i < ship.positions.Count; i++)
            {
                Vector2Int pos = ship.positions[i];
                Tiles tile = enemyPlacement.enemyGrid.GetTile(pos.x, pos.y);

                if (tile.state != Tiles.TileState.Hit)
                {
                    shipSunk = false; // still has a part not hit
                    break;
                }
            }

            if (shipSunk)
            {
                if (!ship.IsSunkAnnounced) // only announce once
                {
                    ship.IsSunkAnnounced = true;
                    YouSinkedText(ship);
                }
            }
            else
            {
                shipsLeft++; // still alive
            }

        }


        shipsLeftText.text = $"SHIPS LEFT TO SINK: {shipsLeft}";
    }


    public void YouSinkedText(Ship ship)
    {

        sinkedText.text = $"YOU SINKED {ship.ShipName}";
        sinkedText.gameObject.SetActive(true);

        StartCoroutine(HideSinkedText(2f));
    }

    private IEnumerator HideSinkedText(float delay)
    {
        yield return new WaitForSeconds(delay);
        sinkedText.gameObject.SetActive(false);
    }




    public void PlayerTurn(int pX, int pY)
    {
        if (!isPlayerTurn || isGameOver)
        {

            return;
        }

        isPlayerTurn = false;

        bool hit = shootingManager.ShootEnemyTile(pX, pY); //checking if hit

        if (AllShipsSunk(enemyGrid))
        {
            if (gameOverText != null)
            {
                gameOverText.gameObject.SetActive(true);
                gameOverText.text = "YOU WIN!";
            }
            isGameOver = true;
            restartButton.SetActive(true);
            menuButton.SetActive(true);

            return;
        }
        Invoke(nameof(EndPlayerTurn), 0.5f);
    }



    public void EndPlayerTurn()
    {
        if (isGameOver)
        {
            return;
        }

        isPlayerTurn = false;
        UpdateBoardVisibility();
        Invoke(nameof(EnemyTurn), 0.5f);
    }


    public void EnemyTurn()
    {
        if (isGameOver)
        {
            return;
        }

        List<Vector2Int> availableTargets = new List<Vector2Int>();
        for (int row = 0; row < playerGrid.height; row++)
        {
            for (int col = 0; col < enemyGrid.height; col++)
            {
                Tiles t = playerGrid.GetTile(row, col);
                if (t.state != Tiles.TileState.Hit && t.state != Tiles.TileState.Miss) // if the tile wasnt chosen yet
                {
                    availableTargets.Add(new Vector2Int(row, col));
                }
            }
        }

        if (availableTargets.Count > 0)
        {
            Vector2Int target = availableTargets[Random.Range(0, availableTargets.Count)]; // picks a random position from the list availableTargets and stores in "target"
            Tiles tile = playerGrid.GetTile(target.x, target.y);
            if (tile.state == Tiles.TileState.Ship)
            {
                tile.state = Tiles.TileState.Hit;
                tile.SetTileColor(Color.red);
            }
            else
            {
                tile.state = Tiles.TileState.Miss;
                tile.SetTileColor(Color.blue);
            }
        }

        if (AllShipsSunk(playerGrid))
        {
            if (gameOverText != null)
            {
                gameOverText.gameObject.SetActive(true);
                gameOverText.text = "ENEMY WINS!";
            }
            isGameOver = true;
            restartButton.SetActive(true);
            menuButton.SetActive(true);
            return;
        }
        Invoke(nameof(StartPlayerTurn), 0.5f);
    }

    void StartPlayerTurn()
    {
        isPlayerTurn = true;
        UpdateBoardVisibility();
    }

    public void UpdateBoardVisibility()
    {
        playerGrid.GridPlace.gameObject.SetActive(!isPlayerTurn);
        enemyGrid.GridPlace.gameObject.SetActive(isPlayerTurn);
        if (turnText != null)
        {
            if (isPlayerTurn)
            {
                turnText.text = "MY TURN";
            }
            else
            {
                turnText.text = "ENEMYS TURN";
            }
        }
    }


    public bool AllShipsSunk(GridManager gridManager)
    {
        for (int row = 0; row < gridManager.height; row++)
        {
            for (int col = 0; col < gridManager.width; col++)
            {
                Tiles t = gridManager.GetTile(row, col);

                if (t.state == Tiles.TileState.Ship) // if there is a ship not sunk
                {
                    return false;
                }
            }
        }

        return true;


    }


}
