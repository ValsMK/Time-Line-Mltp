using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public TextMeshProUGUI pointsText;

    public void Setup(int score)
    {
        gameObject.SetActive(true);
        pointsText.text = "Результат: " + score.ToString() + " очков";
    }

    public void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
