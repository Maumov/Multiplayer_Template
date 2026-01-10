using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkBootstrap : MonoBehaviour
{
    public static NetworkBootstrap Instance;

    public GameObject serverRulesPrefab;
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
        Debug.Log( "[BOOT] Dedicated Server" );
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Debug.Log( "[BOOT] Host" );
        
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        string escena = "1_Game";
        NetworkManager.Singleton.SceneManager.LoadScene( $"{escena}", UnityEngine.SceneManagement.LoadSceneMode.Single );
    }
    public void StartClient( string ip )
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData( ip, 7777 );

        NetworkManager.Singleton.StartClient();
        Debug.Log( "[BOOT] Client → " + ip );
    }
}