using UnityEngine;

public class GridOverLordBattleFieldManager : Singleton<EventManager>
{    
    private const int worldUnitsPerGrid = 3;
    private static Vector3 playerOneBaseCampPosition;
    private static Vector3 playerTwoBaseCampPosition;

    public static void InitializeBaseCampPositions()
    {
        var combinedMask = LayerMask.NameToLayer("Stops Movement") | LayerMask.NameToLayer("InFrontOfPlayer");
        var gridPositionOne = BaseCampPlacement.GetRandomViableBasePosition(combinedMask);
        playerOneBaseCampPosition = gridPositionOne * worldUnitsPerGrid;

        var gridPositionTwo = BaseCampPlacement.GetSecondBasePosition(gridPositionOne, combinedMask);
        playerTwoBaseCampPosition = gridPositionTwo * worldUnitsPerGrid;
    }

    public static Vector3 GetPlayerOneBaseCampPosition() => playerOneBaseCampPosition;
    public static Vector3 GetPlayerTwoBaseCampPosition() => playerTwoBaseCampPosition;
}
