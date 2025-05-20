using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Chessboard : MonoBehaviour
{
    [System.Serializable]
    public class Tile
    {
        public string name; // Name of the tile (e.g., "a1")
        public Vector3 topLeft;
        public Vector3 topRight;
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
        public GameObject highlightImage; // Drag your UI Image here in the Inspector

        public Vector3 GetCenter()
        {
            return (topLeft + topRight + bottomLeft + bottomRight) / 4f;
        }

        public void Highlight(bool isHighlighted)
        {
            if (highlightImage != null)
            {
                highlightImage.SetActive(isHighlighted); // Show or hide the image
            }
        }
    }

    public Tile[] tiles = new Tile[64]; // Array to hold all 64 tiles
    private Dictionary<string, Tile> tileDictionary; // Dictionary for fast cell lookup

    private void Awake()
    {
        // Initialize the dictionary for quick lookup
        tileDictionary = tiles.ToDictionary(tile => tile.name, tile => tile);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        // Draw all tiles
        foreach (Tile tile in tiles)
        {
            if (tile != null)
            {
                // Draw the outline of the tile
                Gizmos.DrawLine(tile.topLeft, tile.topRight);
                Gizmos.DrawLine(tile.topRight, tile.bottomRight);
                Gizmos.DrawLine(tile.bottomRight, tile.bottomLeft);
                Gizmos.DrawLine(tile.bottomLeft, tile.topLeft);

                // Draw the tile's name at its center
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(tile.GetCenter(), 0.1f); // Visualize the center
                Gizmos.color = Color.green;

#if UNITY_EDITOR
                UnityEditor.Handles.Label(tile.GetCenter(), tile.name);
#endif
            }
        }
    }

    public Vector3 GetCellPosition(string cellName)
    {
        // Use dictionary for fast lookup
        if (tileDictionary.TryGetValue(cellName, out Tile tile))
        {
            return tile.GetCenter();
        }

        return Vector3.zero;
    }

    public Tile GetTile(string cellName)
    {
        // Retrieve the Tile object directly
        if (tileDictionary.TryGetValue(cellName, out Tile tile))
        {
            return tile;
        }

        return null;
    }

    public void HighlightTile(string cellName, bool isHighlighted)
    {
        // Highlight or unhighlight a specific tile
        Tile tile = GetTile(cellName);
        if (tile != null)
        {
            tile.Highlight(isHighlighted);
        }
    }
}
