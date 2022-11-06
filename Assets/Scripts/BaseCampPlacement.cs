using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public static class BaseCampPlacement
{
    private const int worldUnitsPerGrid = 3;
    // inspect your current grid divide by 3 to get to Grid Coordinates
    private static readonly Vector2 minimumGridPosition = new Vector2(-23, -13);
    private static readonly Vector2 maximumGridPosition = new Vector2(56, 30);   

    public static Vector2 GetRandomViableBasePosition(LayerMask layersToAvoid)
    {
        Vector2 basePosition = GetRandomPositionInsideGrid();

        while (!IsLocationViableForBase(basePosition, layersToAvoid))
        {
            basePosition = GetRandomPositionInsideGrid();
        }
        return basePosition;
    }

    public static Vector2 GetSecondBasePosition(Vector2 firstBase, LayerMask layersToAvoid)
    {
        float tooClose = (Vector2.Distance(minimumGridPosition, maximumGridPosition) / 2.0f) - 1.0f;
        Vector2 secondBase = GetRandomPositionInsideGrid();

        while (!IsLocationViableForBase(secondBase, layersToAvoid) || 
            IsSecondBaseTooCloseToFirstBase(firstBase, secondBase, tooClose))
        {
            secondBase = GetRandomPositionInsideGrid();
        }
        return secondBase;
    }    

    private static Vector2 GetRandomPositionInsideGrid()
    {
        int x = Random.Range((int)minimumGridPosition.x, (int)maximumGridPosition.x + 1);
        int y = Random.Range((int)minimumGridPosition.y, (int)maximumGridPosition.y + 1);
        return new Vector2(x, y);
    }

    private static bool IsLocationViableForBase(Vector2 baseGridPosition, LayerMask layersToAvoid)
    {
        for (int deltax = -1; deltax <= 1; deltax++)        
            for (int deltay = -1; deltay <= 1; deltay++)            
                if (Physics2D.OverlapCircle((baseGridPosition + new Vector2(deltax, deltay)) * worldUnitsPerGrid, 0.2f, layersToAvoid))
                    return false;
                            
        return true;
    }

    private static bool IsSecondBaseTooCloseToFirstBase(Vector2 firstBase, Vector2 secondBase, float tooClose)
    {
        return Vector2.Distance(firstBase, secondBase) <= tooClose;        
    }
}
