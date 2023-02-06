using UnityEngine;

public sealed class CellManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;

    int gridSize;
    int?[,] grid;// 0 - O, 1 - X, NULL - empty
    int?[] cellLine;
    int movesLeft;

    public void SetUpGrid(int gridSize)
    {
        grid = new int?[gridSize, gridSize];
        this.gridSize = gridSize;

        cellLine = new int?[gridSize];

        movesLeft = gridSize * gridSize;
    }

    public bool IsCellEmpty(int x, int y)
    {
        return grid[x, y] == null ? true : false;
    }

    public void PopulateCell(int x, int y, int value)
    {
        if (value != 0 && value != 1)
        {
            return;
        }

        grid[x, y] = value;
        movesLeft--;
    }

    public void EraseCell(int x, int y)
    {
        if (IsCellEmpty(x, y))
        {
            return;
        }
        grid[x, y] = null;
        movesLeft++;
    }

    public bool MovesAvailable()
    {
        return movesLeft != 0;
    }

    public int MovesLeft()
    {
        return movesLeft;
    }

    public int CheckForWins()
    {
        //Колонки
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                cellLine[y] = grid[x, y];
            }
            if (CheckCellLine())
            {
                // print($"Have victory in column {x}");
                return 1;
            }
        }

        //Ряды
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                cellLine[x] = grid[x, y];
            }
            if (CheckCellLine())
            {
                // print($"Have victory in row {y}");
                return 1;
            }
        }

        //Диагональ снизу-вверх
        for (int d = 0; d < gridSize; d++)
        {
            cellLine[d] = grid[d, d];
        }
        if (CheckCellLine())
        {
            // print("Have victory in diagonal bottom to top");
            return 1;
        }

        //Диагональ сверху-вниз
        for (int d = 0; d < gridSize; d++)
        {
            cellLine[d] = grid[d, gridSize - 1 - d];
        }
        if (CheckCellLine())
        {
            // print("Have victory in diagonal top to bottom");
            return 1;
        }

        return 0;
    }

    bool CheckCellLine()
    {
        for (int i = 0; i < gridSize - 1; i++)
        {
            if (cellLine[i] != cellLine[i + 1] || cellLine[i + 1] == null)
            {
                return false;
            }
        }
        return true;
    }
}