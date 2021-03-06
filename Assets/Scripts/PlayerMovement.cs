using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { Left, Right, Up, Down, None }
public class PlayerMovement : MonoBehaviour
{
    public float freeMoveInterval = 0.4f;
    float currentMoveInterval;
    bool isMoving = false;
    Animator anim;
    float lastMoveTime;
    Vector3 currentGridPosition;
    Vector3 refPosition;
    [Range(0, 1)] public float movementSmooth = 0.1f;

    //Push
    float refPush;
    float targetPush;
    float currentPush;

    public bool lockMovement = false;

    public MovementEvent onPlayerMovement = new MovementEvent();

    public static PlayerMovement Instance;

    bool lastMovePushedDice = false;
    public bool IsMoving => Time.time < lastMoveTime + currentMoveInterval;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        currentGridPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z))
        {
            Move(Direction.Up);
        }
        else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q))
        {
            Move(Direction.Left);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            Move(Direction.Right);
        }
        else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            Move(Direction.Down);
        }

        transform.position = Vector3.SmoothDamp(transform.position, currentGridPosition, ref refPosition, movementSmooth);

        //Anim
        anim.SetBool("IsMoving", IsMoving);
        currentPush = Mathf.SmoothDamp(currentPush, targetPush, ref refPush, 0.1f);
        anim.SetFloat("Push", currentPush);
    }

    public void Move(Direction dir)
    {
        if (IsMoving || lockMovement || !GameManager.Instance.GameIsPlayable || DialogueSystem.Instance.isDisplayingDialogues)
            return;

        Vector3 targetPos = currentGridPosition;
        switch (dir)
        {
            case Direction.Left:
                targetPos.x -= 1;
                break;
            case Direction.Right:
                targetPos.x += 1;
                break;
            case Direction.Up:
                targetPos.z += 1;
                break;
            case Direction.Down:
                targetPos.z -= 1;
                break;
        }
        transform.LookAt(targetPos);

        if (!GameGrid.Instance.CanMove(GameGrid.GetGridPos(targetPos), out bool isIce))
            return;

        //Check if there is a dice
        if (GameGrid.HasDice(GameGrid.GetGridPos(targetPos)))
        {
            targetPush = 1;
            currentMoveInterval = GameGrid.PUSH_MOVE_INTERVAl;

            if (!lastMovePushedDice)
                SoundManager.PlaySound(1);
            lastMovePushedDice = true;

            //Check if the dice can move
            if (!GameGrid.Instance.MoveDice(GameGrid.GetGridPos(targetPos), dir))
                return;
        }
        else
        {
            targetPush = 0;
            currentMoveInterval = freeMoveInterval;
            lastMovePushedDice = false;
        }


        currentGridPosition = targetPos;
        lastMoveTime = Time.time;
        onPlayerMovement.Invoke(dir, GameGrid.GetGridPos(targetPos));

        if (isIce)
        {
            StartCoroutine(IceSlide(dir));
        }
    }

    public void Teleport(Vector3 pos)
    {
        transform.position = GameGrid.GetGridPos(pos);
        currentGridPosition = GameGrid.GetGridPos(pos);
    }

    public void Kill()
    {
        StartCoroutine(KillAnim());
    }

    IEnumerator KillAnim()
    {
        lockMovement = true;

        yield return new WaitForSeconds(.35f);
        anim.SetBool("IsKilled", true);

        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.FullRespawn();
        lockMovement = false;
        anim.SetBool("IsKilled", false);
    }

    IEnumerator IceSlide(Direction dir)
    {
        lockMovement = true;
        yield return new WaitForSeconds(0.2f);
        anim.speed = 0;
        Vector3 worldDir = Vector3.zero;
        switch (dir)
        {
            case Direction.Left:
                worldDir.x -= 1;
                break;
            case Direction.Right:
                worldDir.x += 1;
                break;
            case Direction.Up:
                worldDir.z += 1;
                break;
            case Direction.Down:
                worldDir.z -= 1;
                break;
        }

        while (GameGrid.Instance.GetCell(GameGrid.GetGridPos(currentGridPosition)) == GameGrid.CellType.Ice)
        {
            currentGridPosition += worldDir;
            yield return new WaitForSeconds(freeMoveInterval - 0.1f);
        }

        if (GameGrid.HasDice(GameGrid.GetGridPos(currentGridPosition)))
            currentGridPosition -= worldDir;

        if (!GameGrid.Instance.CanMove(GameGrid.GetGridPos(currentGridPosition), out bool isIce))
        {
            anim.speed = 1;
            Kill();
            yield break;
        }

        anim.speed = 1;
        yield return new WaitForSeconds(freeMoveInterval);
        lockMovement = false;
    }
}
