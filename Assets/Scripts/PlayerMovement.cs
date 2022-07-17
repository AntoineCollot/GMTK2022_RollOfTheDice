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
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Move(Direction.Up);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            Move(Direction.Left);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Move(Direction.Right);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
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
        if (IsMoving || lockMovement)
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

        if (!GameGrid.Instance.CanMove(GameGrid.GetGridPos(targetPos)))
            return;

        //Check if there is a dice
        if(GameGrid.HasDice(GameGrid.GetGridPos(targetPos)))
        {
            targetPush = 1;
            currentMoveInterval = GameGrid.PUSH_MOVE_INTERVAl;

            //Check if the dice can move
            if (!GameGrid.Instance.MoveDice(GameGrid.GetGridPos(targetPos), dir))
                return;
        }
        else
        {
            targetPush = 0;
            currentMoveInterval = freeMoveInterval;
        }

        currentGridPosition = targetPos;
        lastMoveTime = Time.time;
        onPlayerMovement.Invoke(dir, GameGrid.GetGridPos(targetPos));
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
}