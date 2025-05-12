using UnityEngine;

/// <summary>
/// 보드의 상태를 애플리케이션에 출력한다.
/// </summary>
public class GameVisualManager : MonoBehaviour
{
    [SerializeField]private GameObject circlePrefab;
    [SerializeField]private GameObject crossPrefab;

    // 언제 인스턴싱? => 보드의 상태가 바뀌었을 때 => GameManager의 상태가 변경됐을 때

    // A가 B의 상태가 바뀌었는지 어떻게 아는가
    // 1. 폴링(Polling): 주기적으로 검사
    // - 단점: Busy
    // 2. 옵저버 패턴(Observer): B가 능동적으로 자신의 상태를 나한테 알려줬으면 좋겠다.
    // - 단점: 디버깅이 빡셈, 병목 지점이 될 수 있음
    // - 가장 많이 하는 실수: 구독 해지 안하는 것
    // 이벤트

    private float cellSize = 3.3f;

    private void Start()
    {
        // 이벤트를 구독한다.
        // 마커의 생성은 보드가 바뀌었을 때만 이루어진다.
        GameManager.Instance.OnBoardChanged += CreateMarker;
    }
    private void CreateMarker(int y, int x, SquareState state)
    {
        Vector3 markPosition = GetWorldPositionFromCoordinate(y, x);
        if(state == SquareState.Cross)
        {
            Instantiate(crossPrefab, markPosition, Quaternion.identity);
        }
        else if(state == SquareState.Circle)
        {
            Instantiate(circlePrefab, markPosition, Quaternion.identity);
        }
        else
        {
            Logger.Error($"잘못된 값이 입력되었습니다. {state}");
        }
    }

    private Vector3 GetWorldPositionFromCoordinate(int y, int x)
    {
        return new Vector3((x - 1) * cellSize, (y - 1) * cellSize, 0);
    }
}
