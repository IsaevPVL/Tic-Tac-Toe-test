using UnityEngine;

public sealed class XOPlacer : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] CellManager cellManager;
    [SerializeField] PlayArea playArea;

    [SerializeField] float cellOffset = 0.02f;
    float size;

    [Header("Line Renderers"), SerializeField] Material lineMaterial;
    [SerializeField] float lineWidth = 0.1f;

    GameObject symbol;
    LineRenderer currentLR;
    Vector3[] oPositions = new Vector3[19];
    Vector3 pos = Vector3.zero;

    public void PlaceSymbolOnGrid(Vector3Int gridCoordinate)
    {
        Vector3 worldCoordinate = playArea.GridToWorld(gridCoordinate);

        cellManager.PopulateCell(gridCoordinate.x, gridCoordinate.y, gameManager.CurrentSymbol);

        if (gameManager.CurrentSymbol == 1)
        {
            MakeX(worldCoordinate);
        }
        else
        {
            MakeO(worldCoordinate);
        }
    }

    void MakeX(Vector3 center)
    {
        //Draw first half of X
        symbol = new GameObject("x1");
        symbol.transform.SetParent(gameManager.transform);

        currentLR = symbol.AddComponent<LineRenderer>();
        SetUpLineRenderer(currentLR);
        currentLR.positionCount = 2;
        currentLR.SetPosition(0, center + (Vector3.left + Vector3.up) * size);
        currentLR.SetPosition(1, center + (Vector3.right + Vector3.down) * size);

        //Draw second half of X
        symbol = new GameObject("x2");
        symbol.transform.SetParent(gameManager.transform);

        currentLR = symbol.AddComponent<LineRenderer>();
        SetUpLineRenderer(currentLR);
        currentLR.positionCount = 2;
        currentLR.SetPosition(0, center + (Vector3.left + Vector3.down) * size);
        currentLR.SetPosition(1, center + (Vector3.right + Vector3.up) * size);

        gameManager.StartNextTurn();
    }

    void MakeO(Vector3 center)
    {
        symbol = new GameObject("o");
        symbol.transform.SetParent(gameManager.transform);

        currentLR = symbol.AddComponent<LineRenderer>();
        SetUpLineRenderer(currentLR);
        currentLR.positionCount = oPositions.Length;

        float angle = 0f;
        for (int i = 0; i < oPositions.Length; i++)
        {
            pos.x = Mathf.Sin(Mathf.Deg2Rad * angle) * size;
            pos.y = Mathf.Cos(Mathf.Deg2Rad * angle) * size;

            angle += 360f / (oPositions.Length - 1);

            oPositions[i] = pos + center;
        }
        currentLR.SetPositions(oPositions);

        gameManager.StartNextTurn();
    }

    void SetUpLineRenderer(LineRenderer lr)
    {
        lr.material = lineMaterial;
        lr.startWidth = currentLR.endWidth = lineWidth;
        lr.alignment = LineAlignment.TransformZ;
        lr.numCapVertices = 3;
    }

    public void SetCellSize(float cellSize)
    {
        size = (cellSize / 2f) - cellOffset;
    }
}