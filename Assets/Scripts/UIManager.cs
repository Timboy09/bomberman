using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI enemiesText;
    public TextMeshProUGUI heartText;
    public TextMeshProUGUI levelInfoText;
    public GameObject levelInfoPanel;
    public GameObject gameOverPanel;
    public Button restartBtn;

    public string startLevelText = "Kill All Enemies!";
    public string endLevelText = "Level Complete! \n Find the Door to exit";

    private void Awake()
    {        
        restartBtn.onClick.AddListener(RestartLevel);
    }

    private void Start()
    {
        StartCoroutine(DisplayLevelInfo(startLevelText));
    }

    public IEnumerator DisplayLevelInfo(string text)
    {
        levelInfoText.text = text;
        levelInfoPanel.SetActive(true);
        yield return new WaitForSeconds(2f);
        levelInfoPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
