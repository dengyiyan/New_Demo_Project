using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "CompositeTile", menuName = "Tiles/CompositeTile")]
public class CompositeTile : TileBase
{
    public Sprite[] sprites;
    public int width;
    public int height;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int tilePosition = new Vector3Int(position.x + x, position.y + y, position.z);
                tilemap.RefreshTile(tilePosition);
            }
        }
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = null;

        // Calculate local position
        int localX = position.x % width;
        int localY = position.y % height;

        // Ensure localX and localY are positive
        if (localX < 0) localX += width;
        if (localY < 0) localY += height;

        // Calculate the index
        int index = localY * width + localX;

        // Check bounds before accessing the array
        if (index >= 0 && index < sprites.Length)
        {
            tileData.sprite = sprites[index];
        }
    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
    {
        return true;
    }
}
