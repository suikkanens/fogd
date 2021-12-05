using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnding : MonoBehaviour
{
    public GameObject victoryText;
    bool won;
    
    void Start()
    {
        victoryText.SetActive(false);
        won = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PlayerMovement>().GetCollectibles() == 3 && !won)
            {
                won = true;
                victoryText.SetActive(true);
                StartCoroutine(ScoreControl.Instance.GameOver());
            }
        }
    }
}
