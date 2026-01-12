using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EntityWorldUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    public void UpdateText( string value )
    {
        text.text = value;
    }

}
