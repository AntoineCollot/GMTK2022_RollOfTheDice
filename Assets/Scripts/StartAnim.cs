using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnim : MonoBehaviour
{
    public Camera mainCam;
    Animator playerAnim;

    // Start is called before the first frame update
    void Start()
    {
        mainCam.gameObject.SetActive(true);
        playerAnim = PlayerMovement.Instance.GetComponent<Animator>();
        playerAnim.SetBool("IsSleeping", true);

        DiceMovement.Instance.gameObject.SetActive(false);
    }

    public void Activate()
    {
        StartCoroutine(Anim());
    }

    IEnumerator Anim()
    {
        yield return new WaitForSeconds(3);

        DiceMovement.Instance.gameObject.SetActive(true);

        yield return new WaitForSeconds(1);

        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("HEY, LISTEN !"));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("Hmmm ?", DialogueSystem.Speaker.Knight));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("WAKE UP !"));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("Hmmm ?", DialogueSystem.Speaker.Knight));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("YOU GOTTA HELP ME TO SAVE THE WORLD !"));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("From wha...", DialogueSystem.Speaker.Knight));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("JUST SAVE IT OK ?!"));
        DialogueSystem.Instance.AddDialogue(new DialogueSystem.Dialogue("PUSH ME AROUND, WE HAVE TO FIND THE SOURCE OF ALL EVIL AND DESTROY IT !"));

        yield return new WaitForSeconds(3);

        playerAnim.SetBool("IsSleeping", false);
        GameManager.Instance.gameHasStarted = true;
    }
}
