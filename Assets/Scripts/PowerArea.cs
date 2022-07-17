using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerArea : MonoBehaviour
{
    public Material highlightMaterial;
    public Material iconMaterial;

    [Header("Icons")]
    public Texture2D iconKey;
    public Texture2D iconEarth;
    public Texture2D iconIce;

    [Header("Colors")]
    [ColorUsage(true, true)] public Color keyColor;
    [ColorUsage(true, true)] public Color earthColor;
    [ColorUsage(true, true)] public Color iceColor;

    [Header("Cells")]
    public GameObject[] cells = new GameObject[4];
    public float highlightDuration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        DicePower.Instance.onPowerPerformed.AddListener(OnPowerPerformed);
    }

    private void OnPowerPerformed(DicePower.Power power, Vector3Int powerGridPos)
    {
        transform.position = powerGridPos;
        Vector3Int[] directions = new Vector3Int[4] { Vector3Int.left, Vector3Int.right, Vector3Int.forward, Vector3Int.back };

        switch (power)
        {
            case DicePower.Power.Key:
                iconMaterial.SetTexture("_MainTex", iconKey);
                iconMaterial.SetColor("_Color", keyColor);
                highlightMaterial.SetColor("_Color", keyColor);

                for (int i = 0; i < 4; i++)
                {
                    GameGrid.CellType cell = GameGrid.Instance.GetCell(powerGridPos + directions[i]);
                    cells[i].SetActive(cell == GameGrid.CellType.Ground || cell == GameGrid.CellType.Obstacle);
                }
                break;
            case DicePower.Power.Earth:
                iconMaterial.SetTexture("_MainTex", iconEarth);
                iconMaterial.SetColor("_Color", earthColor);
                highlightMaterial.SetColor("_Color", earthColor);

                for (int i = 0; i < 4; i++)
                {
                    GameGrid.CellType cell = GameGrid.Instance.GetCell(powerGridPos + directions[i]);
                    cells[i].SetActive(cell == GameGrid.CellType.Ground || cell == GameGrid.CellType.Ice || cell == GameGrid.CellType.Obstacle);
                }
                break;
            case DicePower.Power.Ice:
                iconMaterial.SetTexture("_MainTex", iconIce);
                iconMaterial.SetColor("_Color", iceColor);
                highlightMaterial.SetColor("_Color", iceColor);

                for (int i = 0; i < 4; i++)
                {
                    GameGrid.CellType cell = GameGrid.Instance.GetCell(powerGridPos + directions[i]);
                    cells[i].SetActive(cell == GameGrid.CellType.Water || cell == GameGrid.CellType.Ground || cell == GameGrid.CellType.Ice);
                }
                break;
        }

        StopAllCoroutines();
        StartCoroutine(FadeCellOutline());
        StartCoroutine(FadeCellIcons());
    }

    IEnumerator FadeCellOutline()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / highlightDuration;
            float pingpong = Mathf.PingPong(t * 2, 1);
            highlightMaterial.SetFloat("_Intensity", Curves.QuartEaseOut(0, 1, pingpong));

            yield return null;
        }
        highlightMaterial.SetFloat("_Intensity", 0);
    }

    IEnumerator FadeCellIcons()
    {
        iconMaterial.SetFloat("_Intensity", 0);
        yield return new WaitForSeconds(0.1f);
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / highlightDuration;
            float pingpong = Mathf.PingPong(t * 2, 1);
            iconMaterial.SetFloat("_Intensity", Curves.QuartEaseOut(0, 1, pingpong));

            yield return null;
        }
        iconMaterial.SetFloat("_Intensity", 0);

        for (int i = 0; i < 4; i++)
        {
            cells[i].SetActive(false);
        }
    }
}
