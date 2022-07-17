using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Vector3Int GridPos => GameGrid.GetGridPos(transform.position);
    public Transform playerSpawnPosition;
    [Range(0, 5)] public int diceFaceUp;

    void Start()
    {
        DiceMovement.Instance.onDiceMovementEnded.AddListener(OnDiceMovementEnded);
        GameManager.Instance.onNewCPActivated.AddListener(OnNewCP);
    }

    private void OnNewCP()
    {
        if(GameManager.Instance.activeCP != this)
            GetComponent<Renderer>().materials[1].SetFloat("_Intensity", 0.3f);
    }

    private void OnDiceMovementEnded(Direction dir, Vector3Int gridPos)
    {
        if(gridPos==GridPos)
        {
            ActivateCP();
        }
    }

    public void ActivateCP()
    {
        GameManager.Instance.SetActiveCP(this);

        GetComponent<Renderer>().materials[1].SetFloat("_Intensity", 1);
    }

    public void FullRespawn()
    {
        DiceMovement.Instance.Teleport(GridPos);
        DicePower.Instance.SetFaceUp(diceFaceUp);
        CharacterMovement.Instance.Teleport(playerSpawnPosition.position);
    }
}
