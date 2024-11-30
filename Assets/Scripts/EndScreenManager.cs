using UnityEngine;

public class EndScreenManager : MonoBehaviour
{
    public TMPro.TextMeshProUGUI finalMessage;
    void Start()
    {
        finalMessage.text = "You saved " + Store.Instance.savedAnimals + " animals\nand left " + Store.Instance.deadAnimals + " animals to die.";
    }
    public void PlayAgain()
    {
        Store.Instance.ResetStore();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }
}
