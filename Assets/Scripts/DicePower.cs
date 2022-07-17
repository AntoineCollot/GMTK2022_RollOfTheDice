using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DicePower : MonoBehaviour
{
    public Material[] faceMaterials = new Material[6];
    public Texture2D[] eyeTextures = new Texture2D[6];
    public Texture2D[] powerTextures = new Texture2D[6];
    public bool[] facePowerActivated = new bool[6];

    public enum Power { Key,Earth,Ice, None}
    public class PowerEvent : UnityEvent<Power, Vector3Int> { }
    public PowerEvent onPowerPerformed = new PowerEvent();

    public float faceHighlightDuration = 1f;
    DiceMovement movement;

    public static DicePower Instance;

    private void Awake()
    {
        Instance = this;
    }

#if UNITY_EDITOR
    private void OnDisable()
    {
        for (int faceId = 0; faceId < 6; faceId++)
        {
            faceMaterials[faceId].SetTexture("_FaceTex", eyeTextures[faceId]);
        }
    }
#endif

    private void Start()
    {
        movement = GetComponent<DiceMovement>();
        movement.onDiceMovementStarted.AddListener(OnDiceMovementStarted);
        movement.onDiceMovementEnded.AddListener(OnDiceMovementEnded);

        //Reset the values
        UpdateFaceTextures();
    }

    private void OnDiceMovementStarted(Direction dir, Vector3Int toGridPos)
    {
        int faceUpId = GetIncomingFaceUp();
    }

    private void OnDiceMovementEnded(Direction dir, Vector3Int toGridPos)
    {
        int faceUpId = GetFaceUp();

        if (!facePowerActivated[faceUpId])
            return;

        switch (faceUpId)
        {
            case 0:
                onPowerPerformed.Invoke(Power.Key, toGridPos);
                break;
            case 1:
            case 4:
                onPowerPerformed.Invoke(Power.Earth, toGridPos);
                ScreenShaker.Instance.MediumShake();
                break;
            case 2:
                onPowerPerformed.Invoke(Power.Ice, toGridPos);
                break;
        }
        StartCoroutine(HighlightFace(faceUpId));
    }

    public void GetNewPower(int faceId)
    {
        //Do nothing if already has the power
        if (facePowerActivated[faceId])
            return;

        StartCoroutine(NewPowerAnim(faceId));
        StartCoroutine(HighlightFace(faceId));
    }

    IEnumerator HighlightFace(int faceId)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / faceHighlightDuration;
            float pingpong = Mathf.PingPong(t * 2, 1);
            faceMaterials[faceId].SetFloat("_HighlightIntensity", Curves.QuartEaseOut(0, 1, pingpong));

            yield return null;
        }
        faceMaterials[faceId].SetFloat("_HighlightIntensity", 0);
    }

    IEnumerator NewPowerAnim(int faceId)
    {
        PlayerMovement.Instance.lockMovement = true;

        yield return new WaitForSeconds(0.5f);

        facePowerActivated[faceId] = true;
        faceMaterials[faceId].SetTexture("_FaceTex", powerTextures[faceId]);

        yield return new WaitForSeconds(1f);

        PlayerMovement.Instance.lockMovement = false;
    }

    public int GetFaceUp()
    {
        Vector3[] sides = new Vector3[] { Vector3.up, Vector3.forward, Vector3.right }; // 1(+) or 6(-) 2(+) or 5(-) 3(+) or 4(-)

        float maxY = float.NegativeInfinity;
        int result = -1;

        for (var i = 0; i < 3; i++)
        {
            // Transform the vector to world-space:
            Vector3 worldSpace = transform.TransformDirection(sides[i]);
            if (worldSpace.y > maxY)
            {
                result = i + 1; // index 0 is 1
                maxY = worldSpace.y;
            }
            if (-worldSpace.y > maxY)
            { // also check opposite side
                result = 6 - i; // sum of opposite sides = 7
                maxY = -worldSpace.y;
            }
        }
        return result - 1;
    }

    public int GetIncomingFaceUp()
    {
        Vector3[] sides = new Vector3[] { Vector3.up, Vector3.forward, Vector3.right }; // 1(+) or 6(-) 2(+) or 5(-) 3(+) or 4(-)

        float minDist = Mathf.Infinity;
        int result = -1;

        for (var i = 0; i < 3; i++)
        {
            // Transform the vector to world-space:
            Vector3 worldSpacePositive = transform.position + transform.TransformDirection(sides[i]);
            Vector3 worldSpaceNegative = transform.position - transform.TransformDirection(sides[i]);

            //The face that is the closest to the player will be the one being up
            if (Vector3.Distance(PlayerMovement.Instance.transform.position, worldSpacePositive) < minDist)
            {
                result = i + 1; // index 0 is 1
                minDist = Vector3.Distance(PlayerMovement.Instance.transform.position, worldSpacePositive);
            }
            if (Vector3.Distance(PlayerMovement.Instance.transform.position, worldSpaceNegative) < minDist)
            { // also check opposite side
                result = 6 - i; // sum of opposite sides = 7
                minDist = Vector3.Distance(PlayerMovement.Instance.transform.position, worldSpaceNegative);
            }
        }
        return result - 1;
    }

    public void SetFaceUp(int faceId)
    {
        switch (faceId)
        {
            case 0:
                transform.eulerAngles = Vector3.zero;
                break;
            case 1:
                transform.eulerAngles = Vector3.right * -90;
                break;
            case 2:
                transform.eulerAngles = Vector3.forward * 90;
                break;
            case 3:
                transform.eulerAngles = Vector3.forward * -90;
                break;
            case 4:
                transform.eulerAngles = Vector3.right * 90;
                break;
            case 5:
                transform.eulerAngles = Vector3.right * 180;
                break;
        }
    }

    void UpdateFaceTextures()
    {
        for (int faceId = 0; faceId < 6; faceId++)
        {
            if (facePowerActivated[faceId])
                faceMaterials[faceId].SetTexture("_FaceTex", powerTextures[faceId]);
            else
                faceMaterials[faceId].SetTexture("_FaceTex", eyeTextures[faceId]);

            faceMaterials[faceId].SetFloat("_HighlightIntensity", 0);
        }
    }
}
