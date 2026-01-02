using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Server.Bootstrap
{
    public class ServerBootstrap : MonoBehaviour
    {
        void Start()
        {
            Debug.Log( "[SERVER] ServerBootstrap started" );
            Debug.Log( "Application.isBatchMode = " + Application.isBatchMode );
            NetworkManager.Singleton.StartServer();
            Debug.Log( "[SERVER] Servidor dedicado iniciado" );
        }
    }
}