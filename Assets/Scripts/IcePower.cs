using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePower : MonoBehaviour
{
    public Transform icePrefab;
    public Transform spawnParent;

    // Start is called before the first frame update
    void Start()
    {
        DicePower.Instance.onPowerPerformed.AddListener(OnPowerPerformed);
    }

    private void OnPowerPerformed(DicePower.Power power, Vector3Int powerGridPos)
    {
        if (power != DicePower.Power.Ice)
            return;

        Vector3Int[] directions = new Vector3Int[] { Vector3Int.left, Vector3Int.right, Vector3Int.forward, Vector3Int.back };

        foreach(Vector3Int direction in directions)
        {
            if (GameGrid.Instance.GetCell(powerGridPos + direction) == GameGrid.CellType.Water)
            {
                StartCoroutine(InstantiateIceInLine(powerGridPos + direction, direction));
            }
        }
    }

    IEnumerator InstantiateIceInLine(Vector3Int pos, Vector3Int direction)
    {
        while(GameGrid.Instance.GetCell(pos) == GameGrid.CellType.Water)
        {
            Instantiate(icePrefab, pos + icePrefab.position, Quaternion.identity, spawnParent);
            GameGrid.Instance.SetCellType(pos, GameGrid.CellType.Ice, true);

            pos += direction;

            SoundManager.PlaySound(6);

            yield return new WaitForSeconds(0.2f);
        }
    }
}
