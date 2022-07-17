using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public const float PUSH_MOVE_INTERVAl = 0.7f;

    public enum CellType { Ground, Water, Obstacle, Empty }
    public CellType[,] cells;
    public const int GRID_SIZE_X = 30;
    public const int GRID_SIZE_Y = 30;

    public static GameGrid Instance;

    private void Awake()
    {
        Instance = this;

        //Init the cells
        cells = new CellType[GRID_SIZE_X, GRID_SIZE_Y];
        for (int y = 0; y < cells.GetLength(1); y++)
        {
            for (int x = 0; x < cells.GetLength(0); x++)
            {

                cells[x, y] = CellType.Empty;
            }
        }
    }

    public void SetCellType(Vector3Int gridCoords, CellType type)
    {
        if (gridCoords.x < 0 || gridCoords.z < 0 || gridCoords.x >= GRID_SIZE_X || gridCoords.z >= GRID_SIZE_Y)
        {
            Debug.LogError(gridCoords + " is out of grid bounds");
            return;
        }
        cells[gridCoords.x, gridCoords.z] = type;
    }

    public static Vector3Int GetGridPos(Vector3 pos)
    {
        return new Vector3Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y), Mathf.RoundToInt(pos.z));
    }

    public static bool HasDice(Vector3Int gridCoords)
    {
        return gridCoords == DiceMovement.Instance.GridPos;
    }

    public bool MoveDice(Vector3Int targetGridPos, Direction dir)
    {
        Vector3Int diceTargetGrisPos = targetGridPos;
        switch (dir)
        {
            case Direction.Left:
                diceTargetGrisPos.x -= 1;
                break;
            case Direction.Right:
                diceTargetGrisPos.x += 1;
                break;
            case Direction.Up:
                diceTargetGrisPos.z += 1;
                break;
            case Direction.Down:
                diceTargetGrisPos.z -= 1;
                break;
        }

        //Check that the dice can move
        if (GetCell(diceTargetGrisPos) != CellType.Ground)
            return false;

        DiceMovement.Instance.Move(dir);
        return true;
    }

    public bool CanMove(Vector3Int toGridCoords)
    {
        return GetCell(toGridCoords) == CellType.Ground;
    }

    public CellType GetCell(Vector3Int gridCoords)
    {
        if (gridCoords.x < 0 || gridCoords.z < 0 || gridCoords.x >= GRID_SIZE_X || gridCoords.z >= GRID_SIZE_Y)
        {
            return CellType.Empty;
        }
        return cells[gridCoords.x, gridCoords.z];
    }

    private void OnDrawGizmos()
    {
        Color color = Color.blue;
        color.a = 0.5f;
        Gizmos.color = color;
        Vector3 pos = Vector3.zero;
        Vector3 size = new Vector3(1, 0, 1);
        for (int x = 0; x < GRID_SIZE_X; x++)
        {
            for (int y = 0; y < GRID_SIZE_Y; y++)
            {
                pos.x = x;
                pos.z = y;
                Gizmos.DrawWireCube(pos, size);
            }
        }
    }

    public static bool AreAdjacent(Vector3Int a, Vector3Int b)
    {
        int dist = Mathf.Abs(a.x - b.x);
        dist += Mathf.Abs(a.z - b.z);

        return dist == 1;
    }

    public static bool AreAdjacentDiagonal(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) <= 1 && Mathf.Abs(a.z - b.z) <= 1;
    }

}
