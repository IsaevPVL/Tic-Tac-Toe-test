using UnityEngine;

public sealed class PlayArea : MonoBehaviour
{
    // Dependencies
    [SerializeField] CellManager cellManager;
    [SerializeField] XOPlacer xoPlacer;

    // Grid for player input
    [Space, SerializeField] float cellSize = 1;
    int gridSize;
    Grid interactionGrid;
    Vector3Int selectedCell;
    Vector3Int lastSelectedCell;
    bool haveSelection;

    // Reticle
    [Space, SerializeField] GameObject reticle;
    bool canSelect;

    //Line renderer settings
    [Space, SerializeField] float lineWidth = 0.05f;
    [SerializeField] Material lineMaterial;
    int linesNeeded = 12;
    GameObject[] lines;
    LineRenderer currentLR;
    Vector3[] linePositions = new Vector3[2];


    public void DoInitialSetup()
    {
        CreateLineRenderers();

        SetUpInteractionGrid();

        xoPlacer.SetCellSize(cellSize);
    }

    public void CreateGrid(int gridSize)
    {
        this.gridSize = gridSize;

        for (int i = 0; i < linesNeeded; i++)
        {
            if (i >= 2 * gridSize + 2)
            {
                lines[i].SetActive(false);
                continue;
            }

            currentLR = lines[i].GetComponent<LineRenderer>();

            currentLR.positionCount = 2;

            if (i < gridSize + 1)
            {
                linePositions[0] = Vector3.up * cellSize * i;
                linePositions[1] = Vector3.right * cellSize * gridSize + linePositions[0];
            }
            else
            {
                linePositions[0] = Vector3.right * cellSize * (i - gridSize - 1);
                linePositions[1] = Vector3.up * cellSize * gridSize + linePositions[0];
            }

            currentLR.SetPositions(linePositions);

            lines[i].SetActive(true);
        }

        SetUpCamera(gridSize * cellSize / 2f);
    }

    public void SetUpInteractionGrid()
    {
        interactionGrid = GetComponent<Grid>();
        interactionGrid.cellGap = Vector3.one * lineWidth;
        interactionGrid.cellSize = Vector3.one - Vector3.one * lineWidth;
        // interactionGrid.cellSize = new Vector3(1 - lineWidth, 1 - lineWidth, 0);

        transform.position = Vector3.one * lineWidth / 2f;
    }

    void SetUpCamera(float xyPosition)
    {
        Camera.main.transform.position = new Vector3(xyPosition, xyPosition + 0.5f, -10);
    }

    void CreateLineRenderers()
    {
        lines = new GameObject[linesNeeded];

        for (int i = 0; i < linesNeeded; i++)
        {
            lines[i] = new GameObject();
            lines[i].name = "Line";
            lines[i].transform.parent = transform;

            currentLR = lines[i].AddComponent<LineRenderer>();
            currentLR.material = lineMaterial;
            currentLR.startWidth = currentLR.endWidth = lineWidth;
            currentLR.alignment = LineAlignment.TransformZ;
            currentLR.numCapVertices = 3;

            lines[i].SetActive(false);
        }
    }

    public void SelectCell(Vector3Int position)
    {
        selectedCell = position;

        if (selectedCell == lastSelectedCell)
        {
            return;
        }
        else if (selectedCell.x < 0f || selectedCell.y < 0f || selectedCell.x >= gridSize || selectedCell.y >= gridSize || !cellManager.IsCellEmpty(selectedCell.x, selectedCell.y))
        {
            if (reticle.activeSelf == false)
            {
                return;
            }

            lastSelectedCell = selectedCell;

            reticle.SetActive(false);

            haveSelection = false;
        }
        else
        {
            lastSelectedCell = selectedCell;

            reticle.transform.position = interactionGrid.GetCellCenterLocal(selectedCell);
            reticle.SetActive(true);

            haveSelection = true;
        }

    }

    public void SelectCell(Vector3 position)
    {
        if (!canSelect)
        {
            return;
        }

        SelectCell(interactionGrid.WorldToCell(position));
    }

    public void PlaceSymbol()
    {
        if (canSelect && haveSelection)
        {
            StopCellSelection();

            xoPlacer.PlaceSymbolOnGrid(selectedCell);

            haveSelection = false;
        }
    }

    public Vector3 GridToWorld(Vector3Int gridCoordinate)
    {
        return interactionGrid.GetCellCenterLocal(gridCoordinate);
    }

    public void AllowCellSelection()
    {
        canSelect = true;
    }

    public void StopCellSelection()
    {
        reticle.SetActive(false);
        canSelect = false;
    }
}