using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ServerEntry : MonoBehaviour
{
    public TextMeshProUGUI label;
    string ip;

    public void Init( string serverIp )
    {
        ip = serverIp;
        label.text = serverIp;
    }

    public void Connect()
    {
        Debug.Log( "[ServerEntry] Connecting to server " + ip );
        NetworkBootstrap.Instance.StartClient( ip );

    }
}