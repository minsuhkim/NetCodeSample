using UnityEngine;

// ���콺 ��ư�� Unity ���ø����̼ǿ� �����ϴ� ������Ʈ
public class GridPosition : MonoBehaviour
{
    [SerializeField] private int _x;
    [SerializeField] private int _y;
    private void OnMouseDown()
    {
        GameManager.Instance.PlayMarker(_x, _y);
    }
}
