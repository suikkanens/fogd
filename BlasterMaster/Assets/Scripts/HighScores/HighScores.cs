using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class HighScores : MonoBehaviour
{
    public class HighScore
    {
        public string name;
        public int score;

        public HighScore(string n, int s)
        {
            name = n;
            score = s;
        }
    }

    List<HighScore> _scores = new List<HighScore>();
    List<HighScore> _minigameScores = new List<HighScore>();
    [SerializeField]
    TextMeshProUGUI[] _highScoreList;
    [SerializeField]
    TextMeshProUGUI[] _mgHighScoreList;
    // Start is called before the first frame update
    void Start()
    {
        AddScores("hScore", _scores, _highScoreList);
        AddScores("mgHScore", _minigameScores, _mgHighScoreList);
    }

    void AddScores(string scoreName, List<HighScore> scores, TextMeshProUGUI[] scoreTexts)
    {
        for (int i = 0; i < 10; i++)
        {
            var score = PlayerPrefs.GetInt(i.ToString() + scoreName);
            var name = PlayerPrefs.GetString(i.ToString() + scoreName + "Name");
            scores.Add(new HighScore(name, score));
        }
        var sorted = scores.OrderByDescending(obj => obj.score).ToList();

        int j = 0;
        foreach (TextMeshProUGUI highScore in scoreTexts)
        {
            highScore.text = (j+1).ToString() + ". " + sorted[j].name + ": " + sorted[j].score.ToString();
            j++;
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartMenu");
    }
}
