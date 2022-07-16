using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public enum Direction { Left,Right, Up, Down,None}
public class CharacterMovement : MonoBehaviour
{
    public float moveTime = 0.5f;
    bool isMoving = false;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
            return;

        if(Input.GetKey(KeyCode.UpArrow))
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
    }

    public void Move(Direction dir)
    {
        if (isMoving)
            return;

        Vector3 targetPos = transform.position;
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

        StartCoroutine(MoveToPos(targetPos));
    }

    IEnumerator MoveToPos(Vector3 targetPos)
    {
        anim.SetTrigger("Walk");
        isMoving = true;
        transform.LookAt(targetPos);
        Vector3 startPos = transform.position;
        float t = 0;
        while(t<1)
        {
            t += Time.deltaTime / moveTime;
            transform.position = Curves.QuadEaseInOut(startPos, targetPos, Mathf.Clamp01(t));

            yield return null;
        }
        isMoving = false;
    }
}
