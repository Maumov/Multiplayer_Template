using UnityEngine;
using UnityEngine.UI;
public class ServerEntry : MonoBehaviour
{
    public Text label;
    string ip;

    public void Init( string serverIp )
    {
        ip = serverIp;
        label.text = serverIp;

        GetComponent<Button>().onClick.AddListener( Connect );
    }

    void Connect()
    {
        NetworkBootstrap.Instance.StartClient( ip );
    }
}