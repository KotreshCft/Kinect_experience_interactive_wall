using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LorealGridManager : MonoBehaviour
{
    private GameObject[,] gridTiles;  // A 2D array to store the grid objects
    public float spacing = 50f;       // Spacing between tiles, assuming tiles are in pixels
    public LorealManager gameManager; // Reference to the GameManager

    private int totalTiles;           // Total number of tiles
    private int whiteTileCount = 0;   // Counter for white tiles
    public Sprite blackTileSprite;    // Black tile sprite for the initial grid
    // Create the initial black grid
    public void CreateBlackGrid(int gridSizeX, int gridSizeY, GameObject imagePrefab)
    {
        RectTransform parentRectTransform = GetComponent<RectTransform>();
        float parentWidth = parentRectTransform.rect.width;  // Width of the parent UI
        float parentHeight = parentRectTransform.rect.height; // Height of the parent UI

        // Get the size of a single tile
        RectTransform tileRectTransform = imagePrefab.GetComponent<RectTransform>();
        float tileWidth = tileRectTransform.rect.width;
        float tileHeight = tileRectTransform.rect.height;

        // Calculate the total space each row/column will take, including spacing
        float gridWidth = (gridSizeX * (tileWidth + spacing)) - spacing;
        float gridHeight = (gridSizeY * (tileHeight + spacing)) - spacing;

        // Calculate the starting position to center the grid
        float startX = -gridWidth / 2 + tileWidth / 2;
        float startY = gridHeight / 2 - tileHeight / 2;

        // Initialize the gridTiles array
        gridTiles = new GameObject[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calculate the position for each tile
                Vector3 position = new Vector3(
                    startX + x * (tileWidth + spacing),  // X position
                    startY - y * (tileHeight + spacing), // Y position (inverted for UI layout)
                    0
                );

                // Instantiate the image prefab and set its position
                GameObject cellObject = Instantiate(imagePrefab, parentRectTransform);
                cellObject.GetComponent<RectTransform>().anchoredPosition = position;

                // Assign the black tile sprite
                Image imageComponent = cellObject.GetComponent<Image>();
                if (imageComponent != null && blackTileSprite != null)
                {
                    imageComponent.sprite = blackTileSprite;
                }

                // Name the tile and store it in the gridTiles array
                cellObject.name = $"Tile_{x}_{y}";
                gridTiles[x, y] = cellObject;
            }
        }
    }

    public void CreateGameGrid(int gridSizeX, int gridSizeY, GameObject imagePrefab, Sprite[] randomImages)
    {
        RectTransform parentRectTransform = GetComponent<RectTransform>();
        float parentWidth = parentRectTransform.rect.width;  // Width of the parent UI
        float parentHeight = parentRectTransform.rect.height; // Height of the parent UI

        // Get the size of a single tile
        RectTransform tileRectTransform = imagePrefab.GetComponent<RectTransform>();
        float tileWidth = tileRectTransform.rect.width;
        float tileHeight = tileRectTransform.rect.height;

        // Calculate the total space each row/column will take, including spacing
        float gridWidth = (gridSizeX * (tileWidth + spacing)) - spacing;
        float gridHeight = (gridSizeY * (tileHeight + spacing)) - spacing;

        // Calculate the starting position to center the grid
        float startX = -gridWidth / 2 + tileWidth / 2;
        float startY = gridHeight / 2 - tileHeight / 2;

        // Initialize the gridTiles array
        gridTiles = new GameObject[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Calculate the position for each tile
                Vector3 position = new Vector3(
                    startX + x * (tileWidth + spacing),  // X position
                    startY - y * (tileHeight + spacing), // Y position (inverted for UI layout)
                    0
                );

                // Instantiate the image prefab and set its position
                GameObject cellObject = Instantiate(imagePrefab, parentRectTransform);
                cellObject.GetComponent<RectTransform>().anchoredPosition = position;

                // Assign a random sprite from the array
                Image imageComponent = cellObject.GetComponent<Image>();
                if (imageComponent != null && randomImages.Length > 0)
                {
                    int randomIndex = Random.Range(0, randomImages.Length);
                    imageComponent.sprite = randomImages[randomIndex];
                }

                // Name the tile and store it in the gridTiles array
                cellObject.name = $"Tile_{x}_{y}";
                gridTiles[x, y] = cellObject;
            }
        }
    }
    /*public void CreateGameGrid(int gridSizeX, int gridSizeY, GameObject imagePrefab, Sprite[] randomImages)
    {
        if (randomImages == null || randomImages.Length == 0) {
            Debug.LogError("Random Images Arry Empty");
        }
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                // Get the existing tile object
                GameObject cellObject = gridTiles[x, y];

                // Assign a random sprite from the array
                Image imageComponent = cellObject.GetComponent<Image>();
                if (imageComponent != null)
                {
                    int randomIndex = Random.Range(0, randomImages.Length);
                    imageComponent.sprite = randomImages[randomIndex];
                }
            }
        }
    }*/

    // This method returns a specific tile at a given position
    public GameObject GetTileAtPosition(int x, int y)
    {
        if (x >= 0 && x < gridTiles.GetLength(0) && y >= 0 && y < gridTiles.GetLength(1))
        {
            return gridTiles[x, y];
        }
        return null;
    }

    // This method returns all tiles in the grid
    public GameObject[,] GetAllTiles()
    {
        return gridTiles;
    }
}