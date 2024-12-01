using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationController : MonoBehaviour
{

    public void StartEasy()
    {
        Store.Instance.difficulty = 0;
        SceneManager.LoadScene("Game");
    }
    public void StartNormal()
    {
        Store.Instance.difficulty = 1;
        SceneManager.LoadScene("Game");
    }
    public void StartHard()
    {
        Store.Instance.difficulty = 2;
        SceneManager.LoadScene("Game");
    }
}
