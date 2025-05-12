using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private Text gameOverText;

    private void Start()
    {
        GameManager.Instance.OnGameOver += UpdateGameOverText;
        gameObject.SetActive(false);
    }

    private void UpdateGameOverText(GameOverState gameOverState)
    {
        gameObject.SetActive(true);
        if(gameOverState == GameOverState.Tie)
        {
            gameOverText.text = "Tie!";
        }
        else
        {
            gameOverText.text = $"{gameOverState} Win!";
        }
    }
}
