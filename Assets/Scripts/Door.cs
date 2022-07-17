using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    public Vector3Int GridPos => GameGrid.GetGridPos(transform.position);

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        DicePower.Instance.onPowerPerformed.AddListener(OnPowerPerformed);

        GameGrid.Instance.SetCellType(GridPos, GameGrid.CellType.Obstacle);
    }

    private void OnPowerPerformed(DicePower.Power power, Vector3Int powerGridPos)
    {
        if (power != DicePower.Power.Key)
            return;

        //position
        if(GameGrid.AreAdjacent(GridPos, powerGridPos))
        {
            anim.SetBool("IsOpen", true);
            GameGrid.Instance.SetCellType(GridPos, GameGrid.CellType.Ground);
            SoundManager.PlaySound(3);
        }
    }
}
