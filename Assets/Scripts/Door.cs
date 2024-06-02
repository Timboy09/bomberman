using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class Door : MonoBehaviour
{
    private GameManager GManager;
    private Animator doorAnim;
    private bool flag = false;

    [SerializeField] private bool levelLocked = true;
    [SerializeField] List<GameObject> enemiesInLevel;

    public static object door { get; internal set; }

    private void Start()
    {
        GManager = GameManager.GManager;
        doorAnim = this.GetComponent<Animator>();

        FindAllEnemiesInLevel();
        StartCoroutine(StartLevelTimer());
    }

    private IEnumerator StartLevelTimer()
    {
        yield return new WaitForSeconds(2f);
        doorAnim.SetBool("triggerDoor", true);
    }

    private void FindAllEnemiesInLevel()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();

        for (int i = 0; i < enemies.Length; i++)
        {
            enemiesInLevel.Add(enemies[i].gameObject);
        }

        EnemiesText(enemiesInLevel.Count);
    }

    public void CheckLevelComplete(GameObject enemyDestroyed)
    {
        for (int i = 0; i < enemiesInLevel.Count; i++)
        {
            if(enemiesInLevel[i] == enemyDestroyed)
            {
                enemiesInLevel.Remove(enemiesInLevel[i]);
                EnemiesText(enemiesInLevel.Count);
            }
        }

        if(enemiesInLevel.Count == 0 || enemiesInLevel == null)
        {
            EnemiesText(0);
            StartCoroutine(GManager.uiCanvas.DisplayLevelInfo(GManager.uiCanvas.endLevelText));
            levelLocked = false;
        }
    }

    public void LoadNextLevel()
    {
        if (!levelLocked)
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            if(DoesSceneExistAtIndex(currentSceneIndex +1))
            {
                StartCoroutine(EndLevelAnimation(currentSceneIndex));
            }
            else
            {
                GManager.uiCanvas.gameOverPanel.SetActive(true);
                GManager.playerController.canMove = false;
                GManager.playerController.canJump = false;
            }
        }
    }

    private IEnumerator EndLevelAnimation(int currentSceneIndex)
    {
        GManager.playerController.canMove = false;
        GManager.playerController.canJump = false;
        GManager.playerController.playerAnim.SetBool("exit", true);
        yield return new WaitForSeconds(0.8f);
        doorAnim.SetBool("triggerDoor", true);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(currentSceneIndex + 1);
    }

    private bool DoesSceneExistAtIndex(int index)
    {
        if(EditorBuildSettings.scenes.Length >= index)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void EnemiesText(int enemyCount)
    {
        if (enemyCount < 10)
        {
            GManager.uiCanvas.enemiesText.text = "x0" + enemyCount.ToString();
        }
        else
        {
            GManager.uiCanvas.enemiesText.text = "x" + enemyCount.ToString();
        }
    }

    private void DoorOpened()
    {
        if (!flag)
        {
            flag = true;
            doorAnim.SetBool("triggerDoor", false);
            GManager.playerController.canMove = true;
        }
    }
}
