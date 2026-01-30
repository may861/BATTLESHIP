using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    public EnemyPlacement enemyPlacement;
    public TextMeshProUGUI sinkedText;
    public ShootingManager shootingManager;
    public GridManager playerGrid;
    public GridManager enemyGrid;
    public Button hintBTN;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI gameOverText;
    public GameObject restartButton;
    public GameObject menuButton;

    public bool isPlayerTurn = true;
    public bool isGameOver = false;

    // =========================
    // Enemy AI memory
    // =========================
    private Ship currentTargetShip = null;
    private List<Vector2Int> enemyHits = new();
    private Vector2Int? targetDirection = null;

    private bool forwardBlocked = false;
    private bool backwardBlocked = false;

    void Start()
    {
        Debug.Log("[INIT] TurnManager started");
        UpdateBoardVisibility();
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
                    Debug.Log($"you sinked {ship}");
                }
            }
            else
            {
                shipsLeft++; // still alive
            }

        }

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


    // =========================
    // PLAYER TURN
    // =========================
    public void PlayerTurn(int x, int y)
    {

        if (!isPlayerTurn || isGameOver)
            return;


        bool hit = shootingManager.ShootEnemyTile(x, y);
        UpdateShipsLeftText();

        if (AllShipsSunk(enemyGrid))
        {
            EndGame("YOU WIN!");
            return;
        }

        if (!hit)
            Invoke(nameof(EndPlayerTurn), 0.4f);
    }

    void EndPlayerTurn()
    {
        isPlayerTurn = false;
        hintBTN.gameObject.SetActive(false);

        UpdateBoardVisibility();
        Invoke(nameof(EnemyTurn), 0.4f);
    }

    // =========================
    // ENEMY TURN
    // =========================
    public void EnemyTurn()
    {
        if (isGameOver)
            return;

        Vector2Int? targetOpt = ChooseEnemyTarget();

        if (!targetOpt.HasValue)
        {
            Invoke(nameof(StartPlayerTurn), 0.4f);
            return;
        }

        Vector2Int target = targetOpt.Value;
        Tiles tile = playerGrid.GetTile(target.x, target.y);

        bool hit = false;

        if (tile.state == Tiles.TileState.Ship)
        {
            hit = true;
            tile.state = Tiles.TileState.Hit;
            tile.SetTileColor(Color.red);

            if (currentTargetShip == null)
                currentTargetShip = tile.ownerShip;

            if (tile.ownerShip == currentTargetShip && !enemyHits.Contains(target))
                enemyHits.Add(target);

            if (enemyHits.Count == 2 && targetDirection == null)
            {
                Vector2Int delta = enemyHits[1] - enemyHits[0];
                targetDirection =
                    Mathf.Abs(delta.x) > Mathf.Abs(delta.y)
                        ? new Vector2Int((int)Mathf.Sign(delta.x), 0)
                        : new Vector2Int(0, (int)Mathf.Sign(delta.y));
            }


            if (enemyHits.Count >= currentTargetShip.ShipSize)
            {
                Debug.Log($"[SUNK] {currentTargetShip.ShipName} destroyed");

                currentTargetShip = null;
                enemyHits.Clear();
                targetDirection = null;
                forwardBlocked = false;
                backwardBlocked = false;
            }
        }
        else
        {
            tile.state = Tiles.TileState.Miss;
            tile.SetTileColor(Color.blue);

            if (targetDirection.HasValue && enemyHits.Count >= 2)
            {
                Vector2Int dir = targetDirection.Value;

                enemyHits.Sort((a, b) =>
                    dir.x != 0 ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));

                Vector2Int start = enemyHits[0];
                Vector2Int end = enemyHits[^1];

                if (target == end + dir)
                    forwardBlocked = true;
                else if (target == start - dir)
                    backwardBlocked = true;
            }
        }

        if (AllShipsSunk(playerGrid))
        {
            EndGame("ENEMY WINS!");
            return;
        }

        if (hit)
            Invoke(nameof(EnemyTurn), 0.4f);
        else
            Invoke(nameof(StartPlayerTurn), 0.4f);
    }

    // =========================
    // TARGET SELECTION
    // =========================
    Vector2Int? ChooseEnemyTarget()
    {
        if (currentTargetShip == null)
            return GetRandomAvailableTarget();

        if (enemyHits.Count == 1)
            return GetAdjacentTarget(enemyHits[0]);

        return GetDirectionalTargetSafe();
    }

    Vector2Int? GetDirectionalTargetSafe()
    {
        Vector2Int dir = targetDirection.Value;

        enemyHits.Sort((a, b) =>
            dir.x != 0 ? a.x.CompareTo(b.x) : a.y.CompareTo(b.y));

        Vector2Int start = enemyHits[0];
        Vector2Int end = enemyHits[^1];

        if (!forwardBlocked)
        {
            Vector2Int probe = end + dir;
            while (IsWithinGrid(probe))
            {
                if (IsShootable(probe))
                    return probe;
                probe += dir;
            }
            forwardBlocked = true;
        }

        if (!backwardBlocked)
        {
            Vector2Int probe = start - dir;
            while (IsWithinGrid(probe))
            {
                if (IsShootable(probe))
                    return probe;
                probe -= dir;
            }
            backwardBlocked = true;
        }

        return null;
    }

    Vector2Int GetAdjacentTarget(Vector2Int hit)
    {
        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var d in dirs)
        {
            Vector2Int p = hit + d;
            if (IsShootable(p))
                return p;
        }

        return GetRandomAvailableTarget();
    }

    Vector2Int GetRandomAvailableTarget()
    {
        List<Vector2Int> options = new();

        for (int r = 0; r < playerGrid.height; r++)
            for (int c = 0; c < playerGrid.width; c++)
            {
                Tiles t = playerGrid.GetTile(r, c);
                if (t.state != Tiles.TileState.Hit &&
                    t.state != Tiles.TileState.Miss)
                    options.Add(new Vector2Int(r, c));
            }

        return options[Random.Range(0, options.Count)];
    }

    bool IsShootable(Vector2Int p)
    {
        return IsWithinGrid(p) &&
               playerGrid.GetTile(p.x, p.y).state != Tiles.TileState.Hit &&
               playerGrid.GetTile(p.x, p.y).state != Tiles.TileState.Miss;
    }

    bool IsWithinGrid(Vector2Int p)
    {
        return p.x >= 0 && p.x < playerGrid.height &&
               p.y >= 0 && p.y < playerGrid.width;
    }

    // =========================
    // UI
    // =========================
    void StartPlayerTurn()
    {
        isPlayerTurn = true;
        hintBTN.gameObject.SetActive(true);
        UpdateBoardVisibility();
    }

    void UpdateBoardVisibility()
    {
        playerGrid.GridPlace.gameObject.SetActive(!isPlayerTurn);
        enemyGrid.GridPlace.gameObject.SetActive(isPlayerTurn);

        if (turnText != null)
            turnText.text = isPlayerTurn ? "MY TURN" : "ENEMY TURN";
    }

    void EndGame(string text)
    {
        isGameOver = true;
        gameOverText.text = text;
        gameOverText.gameObject.SetActive(true);
        restartButton.SetActive(true);
        menuButton.SetActive(true);
    }

    public bool AllShipsSunk(GridManager grid)
    {
        for (int r = 0; r < grid.height; r++)
            for (int c = 0; c < grid.width; c++)
                if (grid.GetTile(r, c).state == Tiles.TileState.Ship)
                    return false;
        return true;
    }
}
