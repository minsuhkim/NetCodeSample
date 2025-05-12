using UnityEngine;

// 마우스 버튼을 Unity 애플리케이션에 전달하는 컴포넌트
public class GridPosition : MonoBehaviour
{
    [SerializeField] private int _x;
    [SerializeField] private int _y;
    private void OnMouseDown()
    {
        GameManager.Instance.PlayMarker(_x, _y);
    }
}
