using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

enum Row
{
    Front,
    Middle,
    Back
}

public class MinigameCycle : MonoBehaviour
{
    [SerializeField]
    GameObject _enemyPrefab;
    [SerializeField]
    GameObject _blueGuyPrefab;
    [SerializeField]
    GameObject _expGuyPrefab;
    List<GameObject>[] _enemies;
    [SerializeField]
    ConveyorBeltControl[] _rows;
    [SerializeField]
    GameObject _pauseScreen;
    [SerializeField]
    GameObject _insctructions;
    [SerializeField]
    TextMeshProUGUI _roundText;
    bool _waveOnGoing;
    int _waveCount;
    bool _gameOver;

    #region Singleton

    private static MinigameCycle _instance;

    public static MinigameCycle Instance
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
        _waveOnGoing = false;
        _enemies = new List<GameObject>[3];
        _enemies[(int)Row.Front] = new List<GameObject>();
        _enemies[(int)Row.Middle] = new List<GameObject>();
        _enemies[(int)Row.Back] = new List<GameObject>();
        _waveCount = 0;
        _gameOver = false;
        _pauseScreen.SetActive(false);
        _roundText.text = "Round: " + (_waveCount).ToString();
        PauseGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _insctructions.SetActive(false);
            ContinueGame();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseScreen();
        }
        if (!_waveOnGoing && _waveCount < 5)
        {
            StartCoroutine(StartWave());
            _waveCount++;
            _roundText.text = "Round: " + (_waveCount).ToString();
        }
        else if (!_waveOnGoing && !_gameOver)
        {
            _gameOver = true;
            StartCoroutine(MinigameScoreControl.Instance.GameOver());
        }
    }

    IEnumerator StartWave()
    {
        _waveOnGoing = true;
        var r1SpawnPoint = Random.Range(0f, 1f) > 0.5f;
        var r2SpawnPoint = Random.Range(0f, 1f) > 0.5f;
        var r3SpawnPoint = Random.Range(0f, 1f) > 0.5f;
        for (int i = 0; i<4; i++)
        {
            SpawnEnemy((int)Row.Front, r1SpawnPoint);
            SpawnEnemy((int)Row.Middle, r2SpawnPoint);
            SpawnEnemy((int)Row.Back, r3SpawnPoint);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(18f);

        ResetEnemies((int)Row.Front);
        ResetEnemies((int)Row.Middle);
        ResetEnemies((int)Row.Back);
        _rows[(int)Row.Front].enabled = true;
        _rows[(int)Row.Middle].enabled = true;
        _rows[(int)Row.Back].enabled = true;
        yield return new WaitForSeconds(2f);
        _waveOnGoing = false;
    }

    public void RemoveEnemy(int row, GameObject enemy)
    {
        _enemies[row].Remove(enemy);
    }

    void SpawnEnemy(int row, bool spawnPoint)
    {
        var rand = Random.Range(0f, 1f);
        var prefab = _enemyPrefab;
        if (rand > 0.7f && rand <= 0.9f)
        {
            prefab = _blueGuyPrefab;
        }
        else if (rand > 0.9f)
        {
            prefab = _expGuyPrefab;
        }
        GameObject enemy = Instantiate(prefab, _rows[row].GetSpawn(spawnPoint).position, Quaternion.Euler(0f,180f,0f));
        enemy.GetComponent<MinigameEnemyControl>().SetRow(row);
        _enemies[row].Add(enemy);
    }

    void ResetEnemies(int row)
    {
        _rows[row].enabled = false;
        foreach (GameObject enemy in _enemies[row])
        {
            enemy.GetComponent<MinigameEnemyControl>().SetAsImmortal();
            enemy.GetComponent<Rigidbody>().AddForce(Vector3.forward * 1500f, ForceMode.Impulse);
        }
        _enemies[row].Clear();
    }

    void PauseScreen()
    {
        PauseGame();
        _pauseScreen.SetActive(true);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ContinueGame()
    {
        _pauseScreen.SetActive(false);
        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("StartMenu");
    }
}
