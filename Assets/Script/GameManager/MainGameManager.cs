using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameManager : MonoBehaviour
{
    [Header("UI References")]
    public Canvas homeUI;
    public GameObject loadingScreen;
    public GameObject gameOverPanel;


    public void ShowGameOverScreen()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (loadingScreen != null)
            loadingScreen.SetActive(false);
    }

    public void Retry()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (loadingScreen != null)
            loadingScreen.SetActive(true);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Debug.Log("Quitting the game...");
        Application.Quit();
    }
}
