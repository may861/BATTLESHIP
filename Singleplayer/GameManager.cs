using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public MenuLogic menuLogic;
    public GameObject MenuScreen;
    public GameObject menuStartingScreen;
    public GameObject SinglePlayer;
    public ShootingManager shootingManager;
    public EnemyPlacement enemyPlacement;
    public GridManager playerGrid;
    public GridManager enemyGrid;
    public TurnManager turnManager;
    public ShipManager shipManager;
    public GameObject startButton;
    public GameObject menuButton;
    public GameObject restartButton;
    public TextMeshProUGUI toggleText;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI shipsLeftText;
    public TextMeshProUGUI shipsunkText;
    public TextMeshProUGUI gameoverText;
    public AudioSource music;

    private void Awake()
    {
        InitStart();
    }


    private void InitStart()
    {
        gameoverText.gameObject.SetActive(false);
        shipsunkText.gameObject.SetActive(false);
        shipsLeftText.gameObject.SetActive(false);
        turnText.gameObject.SetActive(false);
        menuButton.SetActive(false);
        restartButton.SetActive(false);
        turnManager.gameObject.SetActive(false);
        shootingManager.gameObject.SetActive(false);
    }
    public void StartGame()
    {
        if (!shipManager.AllShipsPlaced())
        {
            return;
        }
        shootingManager.gameObject.SetActive(true);
        turnManager.gameObject.SetActive(true);

        playerGrid.GridPlace.gameObject.SetActive(false);
        turnManager.isPlayerTurn = true;

        // switching to playing mode on enemy grid
        for (int row = 0; row < turnManager.enemyGrid.height; row++)
        {
            for (int col = 0; col < turnManager.enemyGrid.width; col++)
            {
                Tiles tile = turnManager.enemyGrid.GetTile(row, col);
                tile.currentMode = Tiles.TileMode.Playing;
                Debug.Log($"{tile.state},{row}{col}");
            }
        }
        shootingManager.SetTileCallbacks(turnManager.enemyGrid);
        startButton.SetActive(false);
        toggleText.gameObject.SetActive(false);
        shipsLeftText.gameObject.SetActive(true);
        enemyGrid.gameObject.SetActive(true); 
        turnText.gameObject.SetActive(true);    
        titleText.gameObject.SetActive(false);    

    }

    public void ResetGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        turnManager.isGameOver = false;
        restartButton.SetActive(false);
        shootingManager.gameObject.SetActive(false);
        turnManager.gameObject.SetActive(false);
        playerGrid.GridPlace.gameObject.SetActive(true);

        for (int row = 0; row < turnManager.enemyGrid.height; row++)
        {
            for (int col = 0; col < turnManager.enemyGrid.width; col++)
            {
                Tiles tile = turnManager.enemyGrid.GetTile(row, col);
                tile.currentMode = Tiles.TileMode.Placement;
                tile.state = Tiles.TileState.Empty;
                tile.ResetToStateColor();


            }
        }

        enemyPlacement.enemyGrid.GenerateGrid();
        enemyPlacement.PlaceAllEnemyShips();


        for (int row = 0; row < turnManager.playerGrid.height; row++)
        {
            for (int col = 0; col < turnManager.playerGrid.width; col++)
            {
                Tiles tile = turnManager.playerGrid.GetTile(row, col);
                tile.currentMode = Tiles.TileMode.Placement;
                tile.state = Tiles.TileState.Empty;
                tile.ResetToStateColor();
                tile.Init(row, col, shipManager.PlacementCLickTile);
                tile.shipManager = shipManager;

            }
        }


        shipManager.ResetShips();
        shipManager.SetTileCallbacks();
        startButton.SetActive(true);
        toggleText.gameObject.SetActive(true);
        titleText.gameObject.SetActive(true);
        turnText.gameObject.SetActive(false);
        gameoverText.gameObject.SetActive(false);
        shipsLeftText.gameObject.SetActive(false);
        shipsunkText.gameObject.SetActive(false);
        menuButton.SetActive(false);
        turnManager.UpdateShipsLeftText();
    }


    public void BackToMenu()
    {
        ResetGame();
        SinglePlayer.SetActive(false);
        music.GetComponent<AudioSource>().Play();
        
        MenuScreen.SetActive(true);
        menuLogic.BackButton();
        menuStartingScreen.SetActive(true);
    }

}
