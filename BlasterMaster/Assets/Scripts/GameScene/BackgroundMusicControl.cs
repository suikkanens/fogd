using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicControl : MonoBehaviour
{
    public AudioClip defaultAudio;
    public AudioClip alertAudio;
    public AudioClip detectedAudio;

    AudioSource _AudioSource;
    int _enemiesAlertedCount;
    int _playerDetectedCount;
    bool _enemiesAlerted;
    bool _playerDetected;

    #region Singleton

    private static BackgroundMusicControl _instance;

    public static BackgroundMusicControl Instance
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
        _AudioSource = GetComponent<AudioSource>();
        _AudioSource.clip = defaultAudio;
        _AudioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        _enemiesAlerted = (_enemiesAlertedCount > 0);
        _playerDetected = (_playerDetectedCount > 0);

        //Debug.Log("Enemiesalerted: " + _enemiesAlertedCount.ToString());
        //Debug.Log("detected: " + _playerDetectedCount.ToString());

        if (_playerDetected && _AudioSource.clip != detectedAudio)
        {
            _AudioSource.clip = detectedAudio;
        }
        else if (!_playerDetected && _enemiesAlerted && _AudioSource.clip != alertAudio)
        {
            _AudioSource.clip = alertAudio;
        }
        else if (!_playerDetected && !_enemiesAlerted && _AudioSource.clip != defaultAudio)
        {
            _AudioSource.clip = defaultAudio;
        }

        if (!_AudioSource.isPlaying)
        {
            _AudioSource.Play();
        }
    }

    public void IncrementEnemiesAlerted(bool value)
    {
        var increment = (value) ? 1 : -1;
        _enemiesAlertedCount += increment;
    }

    public void IncrementPlayerDetected(bool value)
    {
        var increment = (value) ? 1 : -1;
        _playerDetectedCount += increment;
    }
}
