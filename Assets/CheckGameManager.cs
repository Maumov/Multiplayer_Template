using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CheckGameManager : MonoBehaviour
{
    GameManager gameManager;
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        string time = string.Format( "0:000.0", gameManager.MatchTime.Value );
        text.text = $"{time}";
    }
}
