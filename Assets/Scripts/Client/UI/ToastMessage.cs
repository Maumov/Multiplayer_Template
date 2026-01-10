using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToastMessage : MonoBehaviour
{
    public static ToastMessage instance;
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        text = GetComponent<TextMeshProUGUI>();
    }

    public void ShowMessage( string message )
    {
        text.text = message;
    }
}
