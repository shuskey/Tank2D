using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OilSpillOnTileMap : MonoBehaviour
{
    [SerializeReference] private Tile oilSpillTile;
    private string groundOverlayTilemapName = "Ground Overlay";
    private string inFrontOfPlayerTilemapName = "In Front of Player";

    // Update is called once per frame
    public void DropAnOilSpill()
    {      
         var tankAssetTransform = transform.Find("Canvas");

        Tilemap tilemap = GameObject.FindObjectsOfType<Tilemap>().Where<Tilemap>(i => i.name == inFrontOfPlayerTilemapName).FirstOrDefault();
        var currentTileCellCoordinates = tilemap.WorldToCell(tankAssetTransform.position);
        tilemap.SetTile(currentTileCellCoordinates, null);  // delete anything OVER the player

        tilemap = GameObject.FindObjectsOfType<Tilemap>().Where<Tilemap>(i => i.name == groundOverlayTilemapName).FirstOrDefault();
        currentTileCellCoordinates = tilemap.WorldToCell(tankAssetTransform.position);
        tilemap.SetTile(currentTileCellCoordinates, oilSpillTile);                
    }
}
