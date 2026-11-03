using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    private int currentStage = 0;
    private string nextStage;
    public void Start()
    {
        Debug.Log("Current Stage: " + currentStage);
    }

    public void NextStage()
    {
        if (SceneManager.GetActiveScene().name == "ShopScene")
        {
            currentStage++;
            nextStage = "Stage" + currentStage;
            SceneManager.LoadScene(nextStage);
        }
        else
        {
            SceneManager.LoadScene("ShopScene");
        }
    }
}
