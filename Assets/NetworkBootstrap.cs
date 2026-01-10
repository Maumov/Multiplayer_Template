using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkBootstrap : MonoBehaviour
{
    public static NetworkBootstrap Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if ( CommandLineArgs.Has( "-server" ) )
            StartDedicatedServer();
    }

    public void StartDedicatedServer()
    {
        NetworkManager.Singleton.StartServer();
        SceneManager.LoadScene( "1_Game" );
        Debug.Log( "[BOOT] Dedicated Server" );
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        SceneManager.LoadScene( "1_Game" );
        Debug.Log( "[BOOT] Host" );
    }

    public void StartClient( string ip )
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData( ip, 7777 );

        NetworkManager.Singleton.StartClient();
        SceneManager.LoadScene( "1_Game" );
        Debug.Log( "[BOOT] Client → " + ip );
    }
}