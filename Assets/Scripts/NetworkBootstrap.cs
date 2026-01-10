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

    bool hostStartedSuccesfully;
    public void StartHost()
    {
        EnsureNetworkStopped();

        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        
        hostStartedSuccesfully = NetworkManager.Singleton.StartHost();
        Debug.Log( "[BOOT] Host" );
        
    }

    private void OnServerStarted()
    {
        ToastMessage.instance.ShowMessage( $"Host started {hostStartedSuccesfully}" );
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        string escena = "1_Game";
        NetworkManager.Singleton.SceneManager.LoadScene( $"{escena}", UnityEngine.SceneManagement.LoadSceneMode.Single );
        
    }
    void EnsureNetworkStopped()
    {
        if ( NetworkManager.Singleton.IsListening )
        {
            Debug.Log( "[NET] Shutting down existing session" );
            NetworkManager.Singleton.Shutdown();
        }
    }
    public void StartClient( string ip )
    {
        EnsureNetworkStopped();

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData( ip, 7777 );

        bool clientStarted = NetworkManager.Singleton.StartClient();
        ToastMessage.instance.ShowMessage( $"Client started: {clientStarted} ");
        Debug.Log( "[BOOT] Client → " + ip );
    }
}