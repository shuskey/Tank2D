using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartUpBattleFieldManager : MonoBehaviour
{    
    void Awake()
    {
        GridOverLordBattleFieldManager.InitializeBaseCampPositions();  
    }

}
