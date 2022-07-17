using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayDialogues : MonoBehaviour
{
    public DialogueSystem.Dialogue[] dialogues = new DialogueSystem.Dialogue[1];

    public Vector3Int GridPos => GameGrid.GetGridPos(transform.position);

    // Start is called before the first frame update
    void Start()
    {
        DiceMovement.Instance.onDiceMovementEnded.AddListener(OnDiceMovement);
    }

    private void OnDiceMovement(Direction dir, Vector3Int playerGridPos)
    {

        if (playerGridPos == GridPos)
        {
            DiceMovement.Instance.onDiceMovementEnded.RemoveListener(OnDiceMovement);
            for (int i = 0; i < dialogues.Length; i++)
            {
                DialogueSystem.Instance.AddDialogue(dialogues[i]);
            }
        }
    }
}
