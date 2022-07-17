using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementEvent : UnityEvent<Direction, Vector3Int> { }
public class DiceMovement : MonoBehaviour
{
    bool isMoving = false;

    public MovementEvent onDiceMovementStarted = new MovementEvent();
    public MovementEvent onDiceMovementEnded = new MovementEvent();
    public Vector3Int GridPos => GameGrid.GetGridPos(transform.position);

    public static DiceMovement Instance;

    void Awake()
    {
        Instance = this;
    }

    public void Move(Direction dir)
    {
        if (isMoving)
            return;

        Vector3 targetPos = transform.position;
        Vector3 rotationAxisWorld = Vector3.zero;
        switch (dir)
        {
            case Direction.Left:
                targetPos.x -= 1;
                rotationAxisWorld = new Vector3(0, 0, 1);
                break;
            case Direction.Right:
                targetPos.x += 1;
                rotationAxisWorld = new Vector3(0, 0, -1);
                break;
            case Direction.Up:
                targetPos.z += 1;
                rotationAxisWorld = Vector3.right;
                break;
            case Direction.Down:
                targetPos.z -= 1;
                rotationAxisWorld = Vector3.left;
                break;
        }

        onDiceMovementStarted.Invoke(dir, GameGrid.GetGridPos(targetPos));

        //Ice
        if (GameGrid.Instance.GetCell(GameGrid.GetGridPos(targetPos)) == GameGrid.CellType.Ice)
        {
            StartCoroutine(IceSlide(dir, rotationAxisWorld));
        }
        else
            StartCoroutine(MoveToPos(dir, targetPos, rotationAxisWorld));
    }

    IEnumerator MoveToPos(Direction dir, Vector3 targetPos, Vector3 rotationAxisWorld)
    {
        Vector3 startPos = transform.position;
        isMoving = true;
        float t = 0;
        float lastRot = 0;
        SoundManager.PlaySound(0);

        while (t < 1)
        {
            t += Time.deltaTime / (GameGrid.PUSH_MOVE_INTERVAl - 0.1f);

            transform.position = Curves.QuartEaseIn(startPos, targetPos, Mathf.Clamp01(t));
            float currentRot = Curves.QuartEaseIn(0, 90, Mathf.Clamp01(t));
            transform.Rotate(rotationAxisWorld, currentRot - lastRot, Space.World);
            lastRot = currentRot;

            yield return null;
        }
        //Get Rid of errors if any
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.x = Mathf.Round(eulerAngles.x);
        eulerAngles.y = Mathf.Round(eulerAngles.y);
        eulerAngles.z = Mathf.Round(eulerAngles.z);
        transform.eulerAngles = eulerAngles;

        onDiceMovementEnded.Invoke(dir, GameGrid.GetGridPos(targetPos));


        isMoving = false;
    }

    IEnumerator MoveToPosFast(Direction dir, Vector3 targetPos, Vector3 rotationAxisWorld, float time)
    {
        Vector3 startPos = transform.position;
        isMoving = true;
        float t = 0;
        float lastRot = 0;
        SoundManager.PlaySound(0);

        while (t < 1)
        {
            t += Time.deltaTime / time;

            transform.position = Curves.Hermite(startPos, targetPos, Mathf.Clamp01(t));
            float currentRot = Curves.Hermite(0, 90, Mathf.Clamp01(t));
            transform.Rotate(rotationAxisWorld, currentRot - lastRot, Space.World);
            lastRot = currentRot;

            yield return null;
        }
        //Get Rid of errors if any
        Vector3 eulerAngles = transform.eulerAngles;
        eulerAngles.x = Mathf.Round(eulerAngles.x);
        eulerAngles.y = Mathf.Round(eulerAngles.y);
        eulerAngles.z = Mathf.Round(eulerAngles.z);
        transform.eulerAngles = eulerAngles;

        onDiceMovementEnded.Invoke(dir, GameGrid.GetGridPos(targetPos));


        isMoving = false;
    }

    IEnumerator IceSlide(Direction dir, Vector3 rotationAxisWorld)
    {
        isMoving = true;
        Vector3 targetPos = transform.position;
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
        targetPos += worldDir;
        yield return MoveToPos(dir, targetPos, rotationAxisWorld);

        while (GameGrid.Instance.GetCell(GameGrid.GetGridPos(targetPos)) == GameGrid.CellType.Ice)
        {
            targetPos += worldDir;
            float t = 0;
            Vector3 startPos = transform.position;
            while (t < 1)
            {
                t += Time.deltaTime / (PlayerMovement.Instance.freeMoveInterval - 0.1f);

                transform.position = Vector3.Lerp(startPos, targetPos, t);

                yield return null;
            }
        }

        if (!GameGrid.Instance.CanMove(GameGrid.GetGridPos(targetPos), out bool isIce))
            GameManager.Instance.FullRespawn();

        targetPos += worldDir;
        isMoving = false;

        if (GameGrid.Instance.CanMove(GameGrid.GetGridPos(targetPos), out isIce))
            StartCoroutine(MoveToPosFast(dir, targetPos, rotationAxisWorld, PlayerMovement.Instance.freeMoveInterval-0.01f));
    }

    public void Teleport(Vector3 pos)
    {
        transform.position = pos + Vector3.up * 0.4f;
    }
}
