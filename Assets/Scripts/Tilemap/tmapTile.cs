using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New GOL Tile", menuName = "Tile-GOL")]
public class TmapTile : Tile {

    public enum TileState { Empty, Outline, Alive };
    public Sprite spriteEmpty, spriteAlive, spriteOutline;
    private TileState state = TileState.Empty;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        // This is called anytime a Tile is added in Unity Editor or in game

        int i = Random.Range(1, 100);
        if (i < 90) {
            state = TileState.Empty;
            tileData.sprite = spriteEmpty;
        }
        else {
            state = TileState.Alive;
            tileData.sprite = spriteAlive;
        }
    }

    public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go) {
        // This is called once on first frame rendering
        // TODO: ADD THINGS HERE IF NEEDED
        return true;
    }

}
