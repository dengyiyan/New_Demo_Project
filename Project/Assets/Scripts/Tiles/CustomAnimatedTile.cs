using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Custom Async Animated Tile", menuName = "Tiles/Custom Async Animated Tile")]
public class CustomAsyncAnimatedTile : AnimatedTile
{
    private static readonly System.Random random = new System.Random();

    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        bool result = base.GetTileAnimationData(position, tilemap, ref tileAnimationData);
        if (result)
        {
            // Randomize the starting frame
            int randomStartFrame = random.Next(tileAnimationData.animatedSprites.Length);
            float animationDuration = tileAnimationData.animatedSprites.Length / tileAnimationData.animationSpeed;
            float randomStartTime = (randomStartFrame / tileAnimationData.animationSpeed) % animationDuration;
            tileAnimationData.animationStartTime = Time.time - randomStartTime;
        }
        return result;
    }
}
