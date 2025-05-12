using UnityEngine;

/// <summary>
/// ������ ���¸� ���ø����̼ǿ� ����Ѵ�.
/// </summary>
public class GameVisualManager : MonoBehaviour
{
    [SerializeField]private GameObject circlePrefab;
    [SerializeField]private GameObject crossPrefab;

    // ���� �ν��Ͻ�? => ������ ���°� �ٲ���� �� => GameManager�� ���°� ������� ��

    // A�� B�� ���°� �ٲ������ ��� �ƴ°�
    // 1. ����(Polling): �ֱ������� �˻�
    // - ����: Busy
    // 2. ������ ����(Observer): B�� �ɵ������� �ڽ��� ���¸� ������ �˷������� ���ڴ�.
    // - ����: ������� ����, ���� ������ �� �� ����
    // - ���� ���� �ϴ� �Ǽ�: ���� ���� ���ϴ� ��
    // �̺�Ʈ

    private float cellSize = 3.3f;

    private void Start()
    {
        // �̺�Ʈ�� �����Ѵ�.
        // ��Ŀ�� ������ ���尡 �ٲ���� ���� �̷������.
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
            Logger.Error($"�߸��� ���� �ԷµǾ����ϴ�. {state}");
        }
    }

    private Vector3 GetWorldPositionFromCoordinate(int y, int x)
    {
        return new Vector3((x - 1) * cellSize, (y - 1) * cellSize, 0);
    }
}
