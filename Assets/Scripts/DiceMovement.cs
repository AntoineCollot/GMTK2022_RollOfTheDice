using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceMovement : MonoBehaviour
{
    bool isMoving = false;

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
                rotationAxisWorld = new Vector3(0,0,1);
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

        StartCoroutine(MoveToPos(targetPos, rotationAxisWorld));
    }

    IEnumerator MoveToPos(Vector3 targetPos, Vector3 rotationAxisWorld)
    {
        Vector3 startPos = transform.position;
        isMoving = true;
        float t = 0;
        float lastRot = 0;

        while(t<1)
        {
            t += Time.deltaTime / (GameGrid.PUSH_MOVE_INTERVAl-0.1f);

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

        isMoving = false;
    }
}
