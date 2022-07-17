using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GivePower : MonoBehaviour
{
    [Range(0,5)] public int faceIdPower;

    public Vector3Int GridPos => GameGrid.GetGridPos(transform.position);

    // Start is called before the first frame update
    void Start()
    {
        DiceMovement.Instance.onDiceMovementStarted.AddListener(OnDiceMovement);
    }

    private void OnDiceMovement(Direction dir, Vector3Int toGridPos)
    {
        if (toGridPos == GridPos)
        {
            DicePower.Instance.GetNewPower(faceIdPower);
        }
    }
}
