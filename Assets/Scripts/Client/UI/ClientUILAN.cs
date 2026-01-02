using Assets.Scripts.Server.Bootstrap;
using System.Collections;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Client.UI
{
    public class ClientUILAN : MonoBehaviour
    {

        [Header( "UI Elements" )]
        public Button hostButton;
        public Button joinButton;
        public Transform serverListContent; // Content de ScrollView
        public GameObject serverEntryPrefab; // Prefab de botón o item para cada servidor

        private string selectedServerIP = "";

        void Start()
        {
            hostButton.onClick.AddListener( StartHost );
            joinButton.onClick.AddListener( JoinServer );

            // Inicia discovery
            LANDiscovery.Instance.OnServerFound += OnServerFound;
            LANDiscovery.Instance.StartClientDiscovery();
        }

        void OnDestroy()
        {
            LANDiscovery.Instance.OnServerFound -= OnServerFound;
        }

        private void OnServerFound( string serverIP )
        {
            // Crear botón para cada servidor detectado
            GameObject entry = Instantiate( serverEntryPrefab, serverListContent );
            Button b = entry.GetComponent<Button>();
            b.GetComponentInChildren<Text>().text = serverIP;

            b.onClick.AddListener( () =>
            {
                selectedServerIP = serverIP;
            } );
        }

        private void StartHost()
        {
            Debug.Log( "[UI] Starting Host" );
            NetworkManager.Singleton.StartHost();
        }

        private void JoinServer()
        {
            if ( string.IsNullOrEmpty( selectedServerIP ) )
            {
                Debug.LogWarning( "No server selected!" );
                return;
            }

            Debug.Log( $"[UI] Connecting to server {selectedServerIP}" );
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.ConnectionData.Address = selectedServerIP;
            NetworkManager.Singleton.StartClient();
        }
    }
}