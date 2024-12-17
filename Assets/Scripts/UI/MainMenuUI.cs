using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void GoToWorldSelect()
    {
        SceneManager.LoadScene("WorldSelect");
    }

    public void GoToHighScores()
    {
        SceneManager.LoadScene("HighScores");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayUIClickSound()
    {
        AudioManager.Instance.PlayUIClick();
    }
}
