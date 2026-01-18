using UnityEngine;

public class GridManager : MonoBehaviour
{

    public int width;
    public int height;
    public RectTransform GridPlace;
    public GameObject tilePrefab;
    public int tileSize = 40;
    public Tiles[,] tiles;
    public Tiles GetTile(int x, int y)
    {
        return tiles[x, y];
    }

    void Start()
    {
        GenerateGrid();
    }



    public void GenerateGrid()
    {
        float gridWidth = tileSize * width;
        float gridHeight = tileSize * height;

        //place grid in correct placement
        float offsetX = -gridWidth * GridPlace.pivot.x + tileSize / 2f;
        float offsetY = gridHeight * (1 - GridPlace.pivot.y) - tileSize / 2f;


        tiles = new Tiles[width, height];
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                GameObject t = Instantiate(tilePrefab, GridPlace);
                RectTransform r = t.GetComponent<RectTransform>();
                r.anchoredPosition = new Vector2(offsetX + col * tileSize, offsetY - row * tileSize);        //place tile in correct placement
                Tiles myTile = r.GetComponent<Tiles>();

                if (myTile != null)
                {
                    tiles[row, col] = myTile;
                    myTile.TileX = row;
                    myTile.TileY = col;
                    myTile.shipManager = FindObjectOfType<ShipManager>();
                }
            }
        }

    }
}
