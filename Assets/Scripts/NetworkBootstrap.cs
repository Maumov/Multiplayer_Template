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
        var nm = NetworkManager.Singleton;

        var utp = ( UnityTransport ) nm.NetworkConfig.NetworkTransport;

        // FORZAR bind LAN
        utp.SetConnectionData(
            "0.0.0.0",   // escuchar en todas las interfaces
            7778
        );

        // OPCIONAL pero recomendado
        //utp.Protocol = UnityTransport.ProtocolType.UDP;

        hostStartedSuccesfully = NetworkManager.Singleton.StartHost();
        Debug.Log( "[BOOT] Host" );
        
    }

    private void OnServerStarted()
    {
        ToastMessage.instance.ShowMessage( $"Host started {hostStartedSuccesfully}" );
        NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
        
        //string escena = "1_Game";
        //NetworkManager.Singleton.SceneManager.LoadScene( $"{escena}", UnityEngine.SceneManagement.LoadSceneMode.Single );
        
    }
    void EnsureNetworkStopped()
    {
        if ( NetworkManager.Singleton.IsListening )
        {
            Debug.Log( "[NET] Shutting down existing session" );
            //NetworkManager.Singleton.Shutdown();
        }
    }

    bool clientStartedSuccesfully;
    string ipHost;
    public void StartClient( string ip )
    {
        EnsureNetworkStopped();

        NetworkManager ins = NetworkManager.Singleton;
        var transport = (UnityTransport)ins.NetworkConfig.NetworkTransport;
        transport.SetConnectionData( ip, 7778 );
        ipHost = ip.ToLower();
        var nm = NetworkManager.Singleton;
        Debug.Log( $"IsClient={nm.IsClient}" );
        Debug.Log( $"IsServer={nm.IsServer}" );
        Debug.Log( $"IsListening={nm.IsListening}" );
        Debug.Log( $"NM enabled={nm.isActiveAndEnabled}" );
        Debug.Log( $"NM count={FindObjectsOfType<NetworkManager>().Length}" );
        Debug.Log( $"NM comparing UnityTransports={transport == GetComponent<UnityTransport>()}" );
        
        NetworkManager.Singleton.OnClientStarted += OnClientStarted;
        clientStartedSuccesfully = NetworkManager.Singleton.StartClient();
        
    }

    private void OnClientStarted()
    {
        ToastMessage.instance.ShowMessage( $"Client started {clientStartedSuccesfully} + {ipHost}" );
        Debug.Log( "[BOOT] Client → asd");
    }

}