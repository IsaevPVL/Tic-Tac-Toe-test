using UnityEngine;
using UnityEngine.UI;

public sealed class GameManager : MonoBehaviour
{
    //Dependencies
    [SerializeField] PlayArea playArea;
    [SerializeField] CellManager cellManager;
    [SerializeField] CPU cpu;

    //UI
    [Header("UI"), SerializeField] GameObject gameSettingsPanel;
    [SerializeField] GameObject gameplayPanel;
    [SerializeField] Dropdown sizeSelection; // value: 0 - 3x3, 1 - 4x4, 2 - 5x5
    [SerializeField] Dropdown modeSelection; // value: 0 - pvp, 1 - pvcpu, 2 - cpuvcpu
    [SerializeField] Text currentStatus;
    [SerializeField] Text currentPlayer;

    //Game Flow
    string playerOneLabel;
    string playerTwoLabel;
    bool playerOneIsCPU;
    bool playerTwoIsCPU;
    int currentTurn;

    bool isX;
    public int CurrentSymbol
    {
        get
        {
            return isX ? 1 : 0;
        }
        private set { }
    }

    void Awake()
    {
        playArea.DoInitialSetup();

        NewGame();
    }

    public void StartGame()
    {
        gameSettingsPanel.SetActive(false);
        gameplayPanel.SetActive(true);

        switch (modeSelection.value)
        {
            case 0: // pvp
                playerOneIsCPU = false;
                playerTwoIsCPU = false;

                playerOneLabel = "игрок 1";
                playerTwoLabel = "игрок 2";

                break;
            case 1: // pvcpu
                playerOneIsCPU = Random.value > 0.5f;
                playerTwoIsCPU = !playerOneIsCPU;

                playerOneLabel = playerOneIsCPU ? "CPU" : "игрок";
                playerTwoLabel = playerTwoIsCPU ? "CPU" : "игрок";

                break;
            case 2: // cpuvcpu
                playerOneIsCPU = true;
                playerTwoIsCPU = true;

                playerOneLabel = "CPU 1";
                playerTwoLabel = "CPU 2";

                break;
        }

        currentStatus.text = "Ходит: ";
        currentTurn = 0;
        isX = Random.value > 0.5f;

        StartNextTurn();
    }

    public void NewGame()
    {
        cpu.StopAllCoroutines();

        gameSettingsPanel.SetActive(true);
        gameplayPanel.SetActive(false);

        playArea.StopCellSelection();

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        ChangeGridSize();
    }

    public void RestartGame()
    {
        NewGame();
        StartGame();
    }

    public void StartNextTurn()
    {
        playArea.StopCellSelection();

        // Проверяем победу
        if (cellManager.CheckForWins() == 1)
        {
            currentStatus.text = "Победил: ";
            currentPlayer.text = (currentTurn % 2 != 0) ? playerOneLabel : playerTwoLabel;

            if (modeSelection.value == 2)
            {
                Invoke("RestartGame", .5f);
            }

            return;
        }

        //Проверяем ничью
        if (!cellManager.MovesAvailable())
        {
            currentStatus.text = "Победила ";
            currentPlayer.text = "дружба";

            if (modeSelection.value == 2)
            {
                Invoke("RestartGame", 1f);
            }

            return;
        }

        currentTurn++;
        isX = !isX;

        if (currentTurn % 2 != 0)
        {
            currentPlayer.text = playerOneLabel;

            if (playerOneIsCPU)
            {
                cpu.MakeMove(CurrentSymbol, currentTurn);
            }
            else
            {
                playArea.AllowCellSelection();
            }
        }
        else
        {
            currentPlayer.text = playerTwoLabel;

            if (playerTwoIsCPU)
            {
                cpu.MakeMove(CurrentSymbol, currentTurn);
            }
            else
            {
                playArea.AllowCellSelection();
            }
        }
    }

    public void ChangeGridSize()
    {
        switch (sizeSelection.value)
        {
            case 0: // 3x3
                playArea.CreateGrid(3);
                cellManager.SetUpGrid(3);
                cpu.SetGridSize(3);
                break;
            case 1: // 4x4
                playArea.CreateGrid(4);
                cellManager.SetUpGrid(4);
                cpu.SetGridSize(4);
                break;
            case 2: // 5x5
                playArea.CreateGrid(5);
                cellManager.SetUpGrid(5);
                cpu.SetGridSize(5);
                break;
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}