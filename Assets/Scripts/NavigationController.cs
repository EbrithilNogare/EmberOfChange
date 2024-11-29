using UnityEngine;
using UnityEngine.SceneManagement;

public class NavigationController : MonoBehaviour
{

    public void GotoGame()
    {
        SceneManager.LoadScene("Game");
    }
}
