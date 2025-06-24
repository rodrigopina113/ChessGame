using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class Chessboard : MonoBehaviour
{
    [System.Serializable]
    public class Tile
    {
        public string name;
        public Vector3 topLeft;
        public Vector3 topRight;
        public Vector3 bottomLeft;
        public Vector3 bottomRight;
        public GameObject highlightImage;

        public Vector3 GetCenter()
        {
            return (topLeft + topRight + bottomLeft + bottomRight) / 4f;
        }

        public void Highlight(bool isHighlighted)
        {
            if (highlightImage != null)
            {
                highlightImage.SetActive(isHighlighted);
            }
        }
    }

    public Tile[] tiles = new Tile[64];
    private Dictionary<string, Tile> tileDictionary;

    private void Awake()
    {

        tileDictionary = tiles.ToDictionary(tile => tile.name, tile => tile);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;


        foreach (Tile tile in tiles)
        {
            if (tile != null)
            {
  
                Gizmos.DrawLine(tile.topLeft, tile.topRight);
                Gizmos.DrawLine(tile.topRight, tile.bottomRight);
                Gizmos.DrawLine(tile.bottomRight, tile.bottomLeft);
                Gizmos.DrawLine(tile.bottomLeft, tile.topLeft);


                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(tile.GetCenter(), 0.1f);
                Gizmos.color = Color.green;

                UnityEditor.Handles.Label(tile.GetCenter(), tile.name);
            }
        }
    }
#endif

    public Vector3 GetCellPosition(string cellName)
    {

        if (tileDictionary.TryGetValue(cellName, out Tile tile))
        {
            return tile.GetCenter();
        }

        return Vector3.zero;
    }

    public Tile GetTile(string cellName)
    {

        if (tileDictionary.TryGetValue(cellName, out Tile tile))
        {
            return tile;
        }

        return null;
    }

    public void HighlightTile(string cellName, bool isHighlighted)
    {

        Tile tile = GetTile(cellName);
        if (tile != null)
        {
            tile.Highlight(isHighlighted);
        }
    }
}
