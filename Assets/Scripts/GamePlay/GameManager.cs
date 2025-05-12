using System;
using Unity.Netcode;
using UnityEngine;

public enum SquareState
{
    None,
    Cross,
    Circle,
}

// O가 승자, X가 승자, 무승부, 게임 오버 x
public enum GameOverState
{
    NotOver,
    Cross,
    Circle,
    Tie,
}

/// <summary>
/// 틱택토 게임을 진행한다. => 비즈니스 로직 => 핵심 모듈
/// </summary>
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    // 순서대로 O X 를 착수하는 게임
    // 3x3 보드 판
    private NetworkVariable<SquareState>[,] board = new NetworkVariable<SquareState>[3, 3];


    // 보드의 좌표, SquareState
    public event Action<int, int, SquareState> OnBoardChanged;

    public event Action<GameOverState> OnGameOver;

    public event Action<SquareState> OnTurnChanged;

    // 현재 턴
    private NetworkVariable<SquareState> turnState = new();
    public NetworkVariable<GameOverState> gameOverState = new();
    private SquareState _localPlayerType = SquareState.None;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                board[y, x] = new NetworkVariable<SquareState>();
                board[y, x].Value = SquareState.None;
            }
        }
        NetworkManager.Singleton.OnConnectionEvent += (networkManager, connectionEventData) =>
        {
            Logger.Info($"Client {connectionEventData.ClientId} {connectionEventData.EventType}");
            if (networkManager.ConnectedClients.Count == 2)
            {
                StartGame();
            }
        };
    }

    public void StartGame()
    {
        //gameOverState.Value = GameOverState.NotOver;
        OnTurnChanged?.Invoke(SquareState.Cross);
        if (IsHost)
        {
            _localPlayerType = SquareState.Cross;
            turnState.Value = SquareState.Cross;
        }
        else if (IsClient)
        {
            _localPlayerType = SquareState.Circle;
        }
        turnState.OnValueChanged += LogChangedValue;
    }

    private void LogChangedValue(SquareState previousValue, SquareState newValue)
    {
        OnTurnChanged?.Invoke(newValue);
    }

    // 서버에게 입력에 대해 요청
    // 입력: 좌표 값
    // 출력: x
    [Rpc(SendTo.Server)]
    public void ReqValidateRpc(int x, int y, SquareState state)
    {
        // 어떤 클라이언트든 요청할 수 있다.
        // 입력이 유효한가?
        if (_localPlayerType != turnState.Value)
        {
            return;
        }

        if (board[y, x].Value != SquareState.None)
        {
            return;
        }

        ChangeBoardStateRpc(x, y, state);

        if (turnState.Value == SquareState.Cross)
        {
            turnState.Value = SquareState.Circle;
        }
        else if (turnState.Value == SquareState.Circle)
        {
            turnState.Value = SquareState.Cross;
        }
    }

    [Rpc(SendTo.Everyone)]
    public void ChangeBoardStateRpc(int x, int y, SquareState state)
    {
        board[y, x].Value = state;
        OnBoardChanged?.Invoke(y, x, state);
    }

    private bool IsValidPlayerMarker(int x, int y)
    {
        return gameOverState.Value == GameOverState.NotOver &&
            turnState.Value == _localPlayerType &&
            board[y, x].Value == SquareState.None;
    }

    public void PlayMarker(int x, int y)
    {
        if (IsValidPlayerMarker(x, y))
        {
            // 서버에게 입력이 유효한지 요청
            ReqValidateRpc(x, y, _localPlayerType);
        }

        //if (gameOverState != GameOverState.NotOver)
        //{
        //    return;
        //}

        //if (board[y, x] != SquareState.None)
        //{
        //    Logger.Info($"Already Exist {board[y, x]}");
        //    return;
        //}

        //board[y, x] = turnState.Value;
        //OnBoardChanged?.Invoke(y, x, turnState.Value);

        //gameOverState = TestGameOver();
        //if (gameOverState != GameOverState.NotOver)
        //{
        //    Logger.Info($"{gameOverState} is Winner");
        //    OnGameOver?.Invoke(gameOverState);
        //}
    }

    private GameOverState TestGameOver()
    {
        for (int y = 0; y < 3; y++)
        {
            if (board[y, 0].Value != SquareState.None)
            {
                if (board[y, 0] == board[y, 1] && board[y, 1] == board[y, 2])
                {
                    if (turnState.Value == SquareState.Cross)
                    {
                        return GameOverState.Cross;
                    }
                    if (turnState.Value == SquareState.Circle)
                    {
                        return GameOverState.Circle;
                    }
                }
            }
        }

        for (int x = 0; x < 3; x++)
        {
            if (board[0, x].Value != SquareState.None)
            {
                if (board[0, x] == board[1, x] && board[1, x] == board[2, x])
                {
                    if (turnState.Value == SquareState.Cross)
                    {
                        return GameOverState.Cross;
                    }
                    if (turnState.Value == SquareState.Circle)
                    {
                        return GameOverState.Circle;
                    }
                }
            }
        }

        if (board[1, 1].Value == turnState.Value)
        {
            if (board[0, 0].Value == turnState.Value && board[2, 2].Value == turnState.Value)
            {
                if (turnState.Value == SquareState.Cross)
                {
                    return GameOverState.Cross;
                }
                if (turnState.Value == SquareState.Circle)
                {
                    return GameOverState.Circle;
                }
            }

            if (board[0, 2].Value == turnState.Value && board[2, 0].Value == turnState.Value)
            {
                if (turnState.Value == SquareState.Cross)
                {
                    return GameOverState.Cross;
                }
                if (turnState.Value == SquareState.Circle)
                {
                    return GameOverState.Circle;
                }
            }
        }

        if (isFull())
        {
            return GameOverState.Tie;
        }
        else
        {
            return GameOverState.NotOver;
        }
    }

    private bool isFull()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 3; x++)
            {
                if (board[y, x].Value == SquareState.None)
                {
                    return false;
                }
            }
        }
        return true;
    }


}
