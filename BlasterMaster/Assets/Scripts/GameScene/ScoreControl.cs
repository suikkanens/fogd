using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ScoreControl : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    int _score;
    int _scoreToAdd;
    string _name;
    float _timeBonus;
    float _maxBonus;
    float _maxTime;
    bool _scoreIncremented;
    [SerializeField]
    GameObject _nameInput;
    [SerializeField]
    PlayerMovement _playerScript;
    [SerializeField]
    TextMeshProUGUI _timeBonusText;
    [SerializeField]
    TextMeshProUGUI _multiplierText;
    int _multiplier;
    List<int> _points = new List<int>();

    #region Singleton

    private static ScoreControl _instance;

    public static ScoreControl Instance
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
        _scoreToAdd = 0;
        _maxBonus = 10000;
        _maxTime = 3600f;
        scoreText.text = "Score: " + _score;
        _nameInput.SetActive(false);
        _timeBonusText.enabled = false;
        _multiplier = 1;
        IncrementMultiplier(true);
    }

    // Update is called once per frame
    void Update()
    {
        _maxTime -= Time.deltaTime;
        _timeBonus = _maxBonus * (_maxTime/3600f);

        if (_points.Count > 0 && !_scoreIncremented)
        {
            StartCoroutine(AddScoreToGui());
        }

    }

    public void IncrementScore(int amount)
    {
        if (amount > 0)
        {
            _points.Add(_multiplier*amount);
        }
        else
        {
            _points.Add(amount);
        }
        
    }

    public IEnumerator GameOver()
    {
        _playerScript.StopPlayer();
        _playerScript.enabled = false;
        _timeBonusText.enabled = true;
        var finalBonus = (int)_timeBonus;
        _timeBonusText.text = "Time bonus: +" + finalBonus.ToString();
        scoreText.rectTransform.localPosition = _timeBonusText.rectTransform.localPosition - Vector3.right *141.5f - Vector3.up * 25;
        scoreText.text = "Final Score: " + _score;
        yield return new WaitForSeconds(2);
        _scoreIncremented = true;
        IncrementScore((int)finalBonus);
        StartCoroutine(AddScoreToGui(true));
        yield return new WaitUntil(() => !_scoreIncremented);
        //GameCycle.Instance.PauseGame();
        _nameInput.SetActive(true);
        yield return new WaitUntil(() => _name != null);
        _nameInput.SetActive(false);
        AddScore2Rankings();
        GameCycle.Instance.ExitGame();
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
            if (PlayerPrefs.HasKey(i+"hScore"))
            {
                if (PlayerPrefs.GetInt(i + "hScore")<newScore)
                {
                    oldScore = PlayerPrefs.GetInt(i + "hScore");
                    oldName = PlayerPrefs.GetString(i + "hScoreName");
                    PlayerPrefs.SetInt(i + "hScore", newScore);
                    PlayerPrefs.SetString(i + "hScoreName", newName);
                    newScore = oldScore;
                    newName = oldName;
                }
            }
            else
            {
                PlayerPrefs.SetInt(i + "hScore", newScore);
                PlayerPrefs.SetString(i + "hScoreName", newName);
                newScore = 0;
                newName = "";
            }
        }
    }

    IEnumerator AddScoreToGui(bool finalScore=false)
    {
        _scoreIncremented = true;
        var pointsSum = _points.Sum();
        _points.Clear();

        while (pointsSum != 0)
        {
            int increment;
            if (Mathf.Abs(pointsSum) > 1000)
            {
                increment = (pointsSum > 0) ? 1000 : -1000;
            }
            else if(Mathf.Abs(pointsSum) > 100)
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
            if (finalScore)
            {
                scoreText.text = "Final Score: " + _score;
            }
            else
            {
                scoreText.text = "Score: " + _score;
            }
            yield return new WaitForFixedUpdate();
        }
        _scoreIncremented = false;
    }

    public void IncrementMultiplier(bool reset)
    {
        if (reset)
        {
            _multiplier = 1;
        }
        else
        {
            _multiplier = (_multiplier < 32) ? _multiplier * 2 : 32;
        }

        _multiplierText.text = "Multiplier: x" + _multiplier.ToString();
    }
}
