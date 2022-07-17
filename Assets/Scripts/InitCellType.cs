using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitCellType : MonoBehaviour
{
    public GameGrid.CellType type = GameGrid.CellType.Ground;

    // Start is called before the first frame update
    void Start()
    {
        GameGrid.Instance.SetCellType(GameGrid.GetGridPos(transform.position), type, false);
    }
}
