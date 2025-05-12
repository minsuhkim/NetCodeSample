using System;
using Unity.Netcode;
using UnityEngine;

public enum SquareState
{
    None,
    Cross,
    Circle,
}

// O�� ����, X�� ����, ���º�, ���� ���� x
public enum GameOverState
{
    NotOver,
    Cross,
    Circle,
    Tie,
}

/// <summary>
/// ƽ���� ������ �����Ѵ�. => ����Ͻ� ���� => �ٽ� ���
/// </summary>
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    // ������� O X �� �����ϴ� ����
    // 3x3 ���� ��
    private NetworkVariable<SquareState>[,] board = new NetworkVariable<SquareState>[3, 3];


    // ������ ��ǥ, SquareState
    public event Action<int, int, SquareState> OnBoardChanged;

    public event Action<GameOverState> OnGameOver;

    public event Action<SquareState> OnTurnChanged;

    // ���� ��
    private NetworkVariable<SquareState> turnState = new();
    private GameOverState gameOverState = GameOverState.NotOver;
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
        for(int y=0; y<3; y++)
        {
            for(int x=0; x<3; x++)
            {
                board[y, x] = new NetworkVariable<SquareState>();
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

    // �������� �Է¿� ���� ��û
    // �Է�: ��ǥ ��
    // ���: x
    [Rpc(SendTo.Server)]
    public void ReqValidateRpc(int x, int y, SquareState state)
    {
        // � Ŭ���̾�Ʈ�� ��û�� �� �ִ�.
        // �Է��� ��ȿ�Ѱ�?
        //Logger.Info($"{nameof(ReqValidateRpc)} {x},{y},{state}");
        //if (state != turnState.Value)
        //{
        //    Logger.Info("Invalidate");
        //}
        //else
        //{
        //    Logger.Info("Validate");
        //}
        if (board[y,x].Value != SquareState.None)
        {
            return;
        }

        board[y, x].Value = state;
        CreateMarkRpc( x,  y, state);

        if (turnState.Value == SquareState.Cross)
        {
            turnState.Value = SquareState.Circle;
        }
        else if (turnState.Value == SquareState.Circle)
        {
            turnState.Value = SquareState.Cross;
        }

    }
    [Rpc(SendTo.ClientsAndHost)]
    public void CreateMarkRpc(int x, int y, SquareState state)
    {
        OnBoardChanged?.Invoke(y, x, state);
    }

    public void PlayMarker(int x, int y)
    {
        if (_localPlayerType == turnState.Value)
        {
            // �������� �Է��� ��ȿ���� ��û
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
