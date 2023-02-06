using UnityEngine;
using System.Collections;

public sealed class CPU : MonoBehaviour
{
    [SerializeField] CellManager cellManager;
    [SerializeField] XOPlacer xoPlacer;
    [SerializeField] PlayArea playArea;
    int gridSize;
    int maximiserSymbol;
    int minimiserSymbol;
    int bestMinimaxerResult;
    Vector3Int move = Vector3Int.zero;
    float timeToThink = 5f;
    WaitForSeconds fakeThinkingTime = new WaitForSeconds(0.1f);

    public void SetGridSize(int size)
    {
        gridSize = size;
    }

    public void MakeMove(int symbol, int currentTurn)
    {   
        //Победить в игре можно с любым первым ходом, поэтому не рассчитываем его, а делаем случайный
        if (currentTurn == 1)
        {
            move.x = Random.Range(0, gridSize);
            move.y = Random.Range(0, gridSize);
            xoPlacer.PlaceSymbolOnGrid(move);

            return;
        }

        //Запоминаем кто чем ходит для проверки, сбрасываем результаты предыдущей
        maximiserSymbol = symbol;
        minimiserSymbol = (symbol == 1) ? 0 : 1;

        bestMinimaxerResult = int.MinValue;
        move.x = -1;
        move.y = -1;

        StartCoroutine(CalculateBestMove());
    }

    IEnumerator CalculateBestMove()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!cellManager.IsCellEmpty(x, y))
                {
                    continue;
                }

                //Выбираем на доске клетку чтобы визульлизировать мыслительный процесс CPU
                playArea.SelectCell(new Vector3Int(x, y, 0));
                yield return fakeThinkingTime;

                //Делаем пробный ход
                cellManager.PopulateCell(x, y, maximiserSymbol);

                //Проверяем результат пробного хода
                int currentMinimaxerResult = Minimaxer(0, false, int.MinValue, int.MaxValue, Time.realtimeSinceStartup + timeToThink / cellManager.MovesLeft());

                //Удалаем пробный ход
                cellManager.EraseCell(x, y);

                // print($"cell[{x}, {y}] score is {currentMinimaxerResult}");
                
                //Если результат хода лучше, запоминаем его
                if (currentMinimaxerResult > bestMinimaxerResult)
                {
                    move.x = x;
                    move.y = y;
                    bestMinimaxerResult = currentMinimaxerResult;
                }
                //Если результат хода такой же, то с верояностью 50% делаем его, для разнообразия в игре
                else if (currentMinimaxerResult == bestMinimaxerResult)
                {
                    if (Random.value > 0.5f)
                    {
                        move.x = x;
                        move.y = y;
                        bestMinimaxerResult = currentMinimaxerResult;
                    }
                }
            }
        }

        //Делаем ход
        xoPlacer.PlaceSymbolOnGrid(move);
    }

    /* Рекурсивно проверяем все возможные ходы и находим оптимальный
    
        Данный алгоритм подразумевает что оба оппонента будут играть оптимально.

        При оптимальной игре в крестики нолики игрок, делающий первый ход не может проиграть,
        т.е если компьютер ходит первым то он либо выиграет, либо будет ничья.

        Игрок делающий второй ход и играющий оптимально может только сыграть в ничью, т.е компьютер ходящий вторым не видит возможной победы, но и не пытается проиграть, что по-моему мнению можно считать за максимальное усложнение жизни игроку.

    */
    int Minimaxer(int depth, bool maximiser, int alpha, int beta, float endTime)
    {
        //Проверяем нет ли на поле победы
        int result = cellManager.CheckForWins() * 10;

        //Приводим знак результата в соответствие тому, кто ходит
        if (maximiser)
        {
            result *= -1;
        }

        //Если есть победа возвращаем результат
        //Учитывем насколько скоро была достигнута победа чтобы найти оптимальный ход
        if (result == 10)
        {
            return result - depth;
        }
        if (result == -10)
        {
            return result + depth;
        }

        //Если ничья возвращаем 0
        if (!cellManager.MovesAvailable())
        {
            return 0;
        }

        //Если выделенное для проверки время вышло заканчиваем ее
        if (Time.realtimeSinceStartup >= endTime)
        {
            return 0;
        }

        //Если никакого определенного результата нет, проверяем ходы дальше
        int bestResult = maximiser ? int.MinValue : int.MaxValue;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (!cellManager.IsCellEmpty(x, y))
                {
                    continue;
                }

                cellManager.PopulateCell(x, y, maximiser ? maximiserSymbol : minimiserSymbol);


                //alpha и beta значения позволяют не продолжать проверку тех возможных ходов, которые априори хуже уже найденных
                if (maximiser)
                {
                    bestResult = Mathf.Max(bestResult, Minimaxer(depth + 1, !maximiser, alpha, beta, endTime));

                    alpha = Mathf.Max(bestResult, alpha);

                    if (alpha >= beta)
                    {
                        cellManager.EraseCell(x, y);
                        return alpha;
                    }
                }
                else
                {
                    bestResult = Mathf.Min(bestResult, Minimaxer(depth + 1, !maximiser, alpha, beta, endTime));

                    beta = Mathf.Min(bestResult, beta);

                    if (beta <= alpha)
                    {
                        cellManager.EraseCell(x, y);
                        return beta;
                    }
                }

                cellManager.EraseCell(x, y);
            }
        }
        return bestResult;
    }
}