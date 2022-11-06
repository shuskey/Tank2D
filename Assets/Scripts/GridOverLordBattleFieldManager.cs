using UnityEngine;

public class GridOverLordBattleFieldManager : Singleton<EventManager>
{    
    [SerializeField] private LayerMask[] layersToAvoidPlacingAssetsOn;
    private const int worldUnitsPerGrid = 3;
    private static Vector3 playerOneBaseCampPosition;
    private static Vector3 playerTwoBaseCampPosition;

    private void Start()
    {
        var combinedMask = layersToAvoidPlacingAssetsOn[0] | layersToAvoidPlacingAssetsOn[1];
        var gridPositionOne = BaseCampPlacement.GetRandomViableBasePosition(combinedMask);
        playerOneBaseCampPosition = gridPositionOne * worldUnitsPerGrid;

        var gridPositionTwo = BaseCampPlacement.GetSecondBasePosition(gridPositionOne, combinedMask);
        playerTwoBaseCampPosition = gridPositionTwo * worldUnitsPerGrid;
    }

    public static Vector3 GetPlayerOneBaseCampPosition() => playerOneBaseCampPosition;
    public static Vector3 GetPlayerTwoBaseCampPosition() => playerTwoBaseCampPosition;
}
