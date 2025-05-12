using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class HUDUI : MonoBehaviour
{
    [SerializeField] private GameObject crossArrow;
    [SerializeField] private GameObject circleArrow;

    private void Start()
    {
        //crossArrow.SetActive(true);
        //circleArrow.SetActive(false);

        GameManager.Instance.OnTurnChanged += ChangeTurnUI;
    }

    private void ChangeTurnUI(SquareState currentTurn)
    {
        if (currentTurn == SquareState.Cross)
        {
            Logger.Info($"Turn Change {SquareState.Cross}");
            crossArrow.SetActive(true);
            circleArrow.SetActive(false);
        }
        else if (currentTurn == SquareState.Circle)
        {
            Logger.Info($"Turn Change {SquareState.Circle}");
            crossArrow.SetActive(false);
            circleArrow.SetActive(true);
        }
        else if (currentTurn == SquareState.None)
        {
            crossArrow.SetActive(false);
            circleArrow.SetActive(false);
        }
        else
        {
            //Logger.Error("잘못된 입력이 들어왔습니다.");
            throw new ArgumentOutOfRangeException($"{(int)currentTurn}");
        }
    }
}
