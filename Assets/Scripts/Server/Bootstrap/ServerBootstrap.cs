using Unity.Netcode;
using UnityEngine;

namespace Server.Bootstrap
{
    public class ServerBootstrap : MonoBehaviour
    {
        public GameObject ServerPrefab;
        void Start()
        {
            Debug.Log( "[SERVER] ServerBootstrap started" );
            Debug.Log( "Application.isBatchMode = " + Application.isBatchMode );
            NetworkManager.Singleton.StartServer();
            Debug.Log( "[SERVER] Servidor dedicado iniciado" );
            Instantiate( ServerPrefab );
        }
    }
}