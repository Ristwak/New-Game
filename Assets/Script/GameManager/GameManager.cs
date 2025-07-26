using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject homePanel;
    public GameObject optionPanel;
    public GameObject aboutPanel;
    public GameObject loadingPanel;

    [Header("light")]
    public Light pointLight;

    // Start is called before the first frame update
    void Start()
    {
        pointLight.gameObject.SetActive(true);
        homePanel.SetActive(true);
        aboutPanel.SetActive(false);
        loadingPanel.SetActive(false);
        optionPanel.SetActive(false);
    }

    public void StartButton()
    {
        // optionPanel.SetActive(true);
        loadingPanel.SetActive(true);
        SceneManager.LoadScene("ShapeScape");
        pointLight.gameObject.SetActive(false);
        homePanel.SetActive(false);
        aboutPanel.SetActive(false);
        // Load the first scene or start the game logic
        Debug.Log("Game Started");
    }

    public void OnOpionSelect(string sceneName)
    {
        pointLight.gameObject.SetActive(false);
        loadingPanel.SetActive(true);
        homePanel.SetActive(false);
        optionPanel.SetActive(false);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            yield return null;
        }
    }

    public void AboutButton()
    {
        pointLight.gameObject.SetActive(false);
        homePanel.SetActive(false);
        aboutPanel.SetActive(true);
    }

    public void BackButton()
    {
        pointLight.gameObject.SetActive(true);
        aboutPanel.SetActive(false);
        homePanel.SetActive(true);
    }

    public void QuitButton()
    {
        Debug.Log("Game Quit");
        Application.Quit();
    }
}
