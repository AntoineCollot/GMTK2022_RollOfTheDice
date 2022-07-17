using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessKnightEnemy : MonoBehaviour
{
    public Vector3Int GridPos => GameGrid.GetGridPos(transform.position);
    public Vector3Int AttackGridPos => GameGrid.GetGridPos(transform.position+transform.forward);
    bool isKilled = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerMovement.Instance.onPlayerMovement.AddListener(OnPlayerMovement);
        DicePower.Instance.onPowerPerformed.AddListener(OnPowerPerformed);

        GameGrid.Instance.SetCellType(GridPos, GameGrid.CellType.Obstacle);
    }

    private void OnPowerPerformed(DicePower.Power power, Vector3Int powerGridPos)
    {
        if (power != DicePower.Power.Earth)
            return;

        //position
        if (GameGrid.AreAdjacent(GridPos, powerGridPos))
        {
            GameGrid.Instance.SetCellType(GridPos, GameGrid.CellType.Ground);
            Kill();
        }
    }

    private void OnPlayerMovement(Direction dir, Vector3Int playerGridPos)
    {
        if (isKilled)
            return;

        if (playerGridPos == AttackGridPos)
        {
            GetComponent<Animator>().SetTrigger("Attack");
            PlayerMovement.Instance.Kill();
        }
    }

    public void Kill()
    {
        if (isKilled)
            return;

        PlayerMovement.Instance.onPlayerMovement.RemoveListener(OnPlayerMovement);
        DicePower.Instance.onPowerPerformed.RemoveListener(OnPowerPerformed);

        isKilled = true;

        StartCoroutine(KillAnim());
    }

    IEnumerator KillAnim()
    {
        Material mat = GetComponentInChildren<Renderer>().material;

        float t = 0;
        while(t<1)
        {
            t += Time.deltaTime/2;

            mat.SetFloat("_Dissolve", t);

            yield return null;
        }
    }
}
