using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class End : MonoBehaviour
{
    public Vector3Int GridPos => GameGrid.GetGridPos(transform.position);

    void Start()
    {
        DiceMovement.Instance.onDiceMovementEnded.AddListener(OnDiceMovementEnded);
    }

    private void OnDiceMovementEnded(Direction dir, Vector3Int gridPos)
    {
        if(gridPos==GridPos)
        {
            StartCoroutine(EndAnim());
        }
    }

    IEnumerator EndAnim()
    {
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("WE MADE IT !"));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("So what now ? Is there a...", DialogueSystem.Speaker.Knight));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("IT'S COOL, I CAN HANDLE IT FROM NOW !"));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("A HUGE BATTLE WILL HAPPEN OF SCREEN, THAT I'LL SURELY WIN !"));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("That's convenient", DialogueSystem.Speaker.Knight));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("BYYYYE !"));

        while (DialogueSystem.Instance.isDisplayingDialogues)
            yield return null;

        DiceMovement.Instance.gameObject.SetActive(false);

        yield return new WaitForSeconds(1);

        GameManager.Instance.End();
    }
}
