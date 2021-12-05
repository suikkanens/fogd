using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class MinigameScoreControl : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    int _score;
    string _name;
    float _timeBonus;
    float _maxBonus;
    float _maxTime;
    bool _scoreIncremented;
    [SerializeField]
    GameObject _nameInput;
    [SerializeField]
    TextMeshProUGUI _multiplierText;
    List<int> points = new List<int>();
    int _multiplier;

    #region Singleton

    private static MinigameScoreControl _instance;

    public static MinigameScoreControl Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            _instance = this;
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _score = 0;
        scoreText.text = "Score: " + _score;
        _nameInput.SetActive(false);
        _multiplier = 1;
    }

    // Update is called once per frame
    void Update()
    {
        _multiplierText.text = "Multiplier: x" + _multiplier.ToString();
        if (points.Count > 0 && !_scoreIncremented)
        {
            StartCoroutine(AddScoreToGui());
        }
    }

    public void IncrementScore(int amount)
    {
        if (amount > 0)
        {
            points.Add(amount * _multiplier);
        }
        else
        {
            points.Add(amount);
        }
        
    }

    public IEnumerator GameOver()
    {
        _nameInput.SetActive(true);
        scoreText.rectTransform.localPosition = _nameInput.transform.localPosition + new Vector3(0f, 50f, 0f);
        scoreText.text = "Final Score: " + _score;
        MinigameCycle.Instance.PauseGame();
        yield return new WaitUntil(() => _name != null);
        _nameInput.SetActive(false);
        AddScore2Rankings();
        MinigameCycle.Instance.ExitGame();
    }

    public void ReadInput(string input)
    {
        _name = input;
    }

    void AddScore2Rankings()
    {
        int newScore = _score;
        string newName = _name;
        int oldScore;
        string oldName;
        for (int i = 0; i < 10; i++)
        {
            Debug.Log(newName + ": " + newScore.ToString());
            if (PlayerPrefs.HasKey(i + "mgHScore"))
            {
                if (PlayerPrefs.GetInt(i + "mgHScore") < newScore)
                {
                    oldScore = PlayerPrefs.GetInt(i + "mgHScore");
                    oldName = PlayerPrefs.GetString(i + "mgHScoreName");
                    PlayerPrefs.SetInt(i + "mgHScore", newScore);
                    PlayerPrefs.SetString(i + "mgHScoreName", newName);
                    newScore = oldScore;
                    newName = oldName;
                }
            }
            else
            {
                PlayerPrefs.SetInt(i + "mgHScore", newScore);
                PlayerPrefs.SetString(i + "mgHScoreName", newName);
                newScore = 0;
                newName = "";
            }
        }
    }

    IEnumerator AddScoreToGui()
    {
        var pointsSum = points.Sum();
        points.Clear();
        _scoreIncremented = true;

        while (pointsSum != 0)
        {
            int increment;
            if (Mathf.Abs(pointsSum) > 1000)
            {
                increment = (pointsSum > 0) ? 1000 : -1000;
            }
            else if (Mathf.Abs(pointsSum) > 100)
            {
                increment = (pointsSum > 0) ? 100 : -100;
            }
            else
            {
                increment = (pointsSum > 0) ? 1 : -1;
            }

            if (_score <= 0 && increment < 0)
            {
                _score = 0;
                pointsSum = 0;
            }
            else
            {
                _score += increment;
                pointsSum -= increment;
            }
            scoreText.text = "Score: " + _score;
            yield return new WaitForFixedUpdate();
        }
        _scoreIncremented = false;
    }

    public void ResetMultiplier()
    {
        _multiplier = 1;
    }

    public void IncrementMultiplier()
    {
        _multiplier = (_multiplier < 32) ? _multiplier * 2 : 32;
    }
}

