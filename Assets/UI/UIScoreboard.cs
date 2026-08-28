using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class UIScoreboard : MonoBehaviour
{
    public UIDocument uiDoc;
    private VisualElement uiContainer;
    private Label uiTitleLabel;
    private Label uiParagraphLabel;
    private Button restartButton;
    private bool isScoreboardHidden = true;
    private bool isRestarting = false;
    
    void Start()
    {
        uiContainer = uiDoc.rootVisualElement.Q<VisualElement>("scoreboard");
        uiTitleLabel = uiDoc.rootVisualElement.Q<Label>("titleLabel");
        uiParagraphLabel = uiDoc.rootVisualElement.Q<Label>("paragraphLabel");
        restartButton = uiDoc.rootVisualElement.Q<Button>("restartButton");
        restartButton.clicked += OnRestartClicked;
    }

    public void showScoreboard (int score) {
        if (isScoreboardHidden) {
            isScoreboardHidden = false;
            uiTitleLabel.text = $"Congratulations !";
            uiParagraphLabel.text = $"You found {score.ToString()} Treasure chests!";
            uiContainer.AddToClassList("container--success");
            uiContainer.AddToClassList("show-transition");
        }
    }

    public void showGameOver () {
        if (isScoreboardHidden) {
            isScoreboardHidden = false;
            uiTitleLabel.text = $"Unfortunately,";
            uiParagraphLabel.text = $"your pirate ship has fallen victim to the fearsome kraken!";
            uiContainer.AddToClassList("container--fail");
            uiContainer.AddToClassList("show-transition");
        }
    }

    private void OnRestartClicked () {
        if (isRestarting) return;
        isRestarting = true;
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame () {
        var session = FindObjectOfType<ARSession>();
        if (session != null) session.enabled = false;
        yield return null;
        yield return null;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        while (!asyncLoad.isDone)
            yield return null;
    }
}
