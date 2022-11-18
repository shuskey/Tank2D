using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridOverLordBattleFieldManager : Singleton<EventManager>
{
    private const int worldUnitsPerGrid = 3;
    private static Vector3 playerOneBaseCampPosition;
    private static Vector3 playerTwoBaseCampPosition;

    private static string gridLayerGroundOverlay = "Ground Overlay";

    public static void InitializeBaseCampPositions()
    {
        var combinedMask = LayerMask.NameToLayer("Stops Movement") | LayerMask.NameToLayer("InFrontOfPlayer");
        var gridPositionOne = BaseCampPlacement.GetRandomViableBasePosition(combinedMask);
        playerOneBaseCampPosition = gridPositionOne * worldUnitsPerGrid;

        var gridPositionTwo = BaseCampPlacement.GetSecondBasePosition(gridPositionOne, combinedMask);
        playerTwoBaseCampPosition = gridPositionTwo * worldUnitsPerGrid;
    }

    public static void NeutralizeOilDrips(Tile green, Tile purple, Tile neutral)
    {
        Tilemap tilemap = GameObject.FindObjectsOfType<Tilemap>().Where<Tilemap>(i => i.name == gridLayerGroundOverlay).FirstOrDefault();
        for (int y = (int)BaseCampPlacement.GetMinimumGripPosition().y; y <= (int)BaseCampPlacement.GetMaximumGripPosition().y; y++)
        {
            for (int x = (int)BaseCampPlacement.GetMinimumGripPosition().x; x <= (int)BaseCampPlacement.GetMaximumGripPosition().x; x++)
            {
                var mapCoordinates = new Vector3Int(x, y, 0);
                var whatIsThere = tilemap.GetTile(mapCoordinates);
                if (whatIsThere == purple || whatIsThere == green)
                    tilemap.SetTile(mapCoordinates, neutral);
        
           }
        }
    }

    public static Vector3 GetPlayerOneBaseCampPosition() => playerOneBaseCampPosition;

    public static Vector3 GetPlayerTwoBaseCampPosition() => playerTwoBaseCampPosition;
}
