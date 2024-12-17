using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSelectUI : MonoBehaviour
{
    public void GoToLevel1()
    {
        SceneManager.LoadScene("Game1");
    }

    public void GoToLevel2()
    {
        SceneManager.LoadScene("Game2");
    }

    public void GoBack()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayUIClickSound()
    {
        AudioManager.Instance.PlayUIClick();
    }
}
