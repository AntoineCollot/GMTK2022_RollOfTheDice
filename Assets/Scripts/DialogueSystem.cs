using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueSystem : MonoBehaviour
{
    public enum Speaker { Knight, Dice, None}
    [System.Serializable]
    public struct Dialogue
    {
        public Dialogue(string _text, Speaker _speaker)
        {
            text = _text;
            speaker = _speaker;
        }

        public Dialogue(string _text)
        {
            text = _text;
            speaker = Speaker.Dice;
        }

        public string text;
        public Speaker speaker;
    }

    Queue<Dialogue> dialogues;

    public float typeWritterCharPerSec = 20;
    public TextMeshProUGUI text;
    public bool isDisplayingDialogues { get; private set; }
    public static DialogueSystem Instance;

    public GameObject knightPortrait;
    public GameObject dicePortrait;

    public GameObject canvas;

    public float lastDialogueTime;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        canvas.SetActive(false);
        knightPortrait.SetActive(false);
        dicePortrait.SetActive(false);
        dialogues = new Queue<Dialogue>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDisplayingDialogues)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                NextDialogue();
            }
        }
    }

    void NextDialogue()
    {
        if (Time.time < lastDialogueTime + 0.5f)
            return;

        lastDialogueTime = Time.time;

        if (dialogues.Count > 0)
        {
            isDisplayingDialogues = true;
            canvas.SetActive(true);

            Dialogue dialogue = dialogues.Dequeue();
            text.text = dialogue.text;
            StopAllCoroutines();
            StartCoroutine(TypeWritter(dialogue.text));

            knightPortrait.SetActive(dialogue.speaker == Speaker.Knight);
            dicePortrait.SetActive(dialogue.speaker == Speaker.Dice);
        }
        else
        {
            StopAllCoroutines();
            canvas.SetActive(false);
            isDisplayingDialogues = false;
        }
    }

    public void AddDialogue(Dialogue text)
    {
        dialogues.Enqueue(text);

        if (!isDisplayingDialogues)
            NextDialogue();
    }

    IEnumerator TypeWritter(string str)
    {
        for (int i = 0; i < str.Length; i++)
        {
            text.maxVisibleCharacters = i;

            yield return new WaitForSeconds(1/typeWritterCharPerSec);
        }
            text.maxVisibleCharacters = str.Length+1;
    }
}
