using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Assets.Scripts.Server.Bootstrap
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using Unity.Netcode;
    using UnityEngine;

    public class LANDiscovery : MonoBehaviour
    {
        public static LANDiscovery Instance;

        [Header( "Discovery Settings" )]
        [SerializeField] int broadcastPort = 47777;
        [SerializeField] float broadcastInterval = 1.5f;
        [SerializeField] string broadcastMessage = "UNITY_LAN_SERVER";

        public event Action<string> OnServerFound;

        UdpClient udp;
        IPEndPoint receiveEP;
        float timer;

        HashSet<string> discoveredServers = new HashSet<string>();

        void Awake()
        {
            if ( Instance != null )
            {
                Destroy( gameObject );
                return;
            }

            Instance = this;
            DontDestroyOnLoad( gameObject );
        }


        public void StartUDP()
        {
            try
            {
                udp = new UdpClient( broadcastPort );
                udp.EnableBroadcast = true;

                // Importante: evita bloquear el frame
                udp.Client.ReceiveTimeout = 1;

                receiveEP = new IPEndPoint( IPAddress.Any, 0 );

                Debug.Log( $"[LANDiscovery] UDP ready on port {broadcastPort}" );
            }
            catch ( Exception e )
            {
                Debug.LogError( "[LANDiscovery] Failed to start UDP: " + e );
            }
        }

        void Update()
        {
            // 1️⃣ Servidor (host o dedicado) hace broadcast
            if ( NetworkManager.Singleton != null &&
                NetworkManager.Singleton.IsServer )
            {
                timer += Time.deltaTime;
                if ( timer >= broadcastInterval )
                {
                    BroadcastServer();
                    timer = 0f;
                }
            }
            else
            {
                // 2️⃣ Cliente / Launcher escucha SIEMPRE
                ListenForServers();
            }
        }

        void BroadcastServer()
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes( broadcastMessage );
                IPEndPoint ep = new IPEndPoint( IPAddress.Broadcast, broadcastPort );
                udp.Send( data, data.Length, ep );
            }
            catch ( Exception e )
            {
                Debug.LogWarning( "[LANDiscovery] Broadcast failed: " + e.Message );
            }
        }

        void ListenForServers()
        {
            if ( udp == null )
                return;

            try
            {
                byte[] data = udp.Receive( ref receiveEP ); // NO bloquea (timeout)
                string msg = Encoding.UTF8.GetString( data );

                if ( msg != broadcastMessage )
                    return;

                string serverIP = receiveEP.Address.ToString();

                if ( discoveredServers.Add( serverIP ) )
                {
                    Debug.Log( "[LANDiscovery] Server found: " + serverIP );
                    OnServerFound?.Invoke( serverIP );
                }
            }
            catch ( SocketException e )
            {
                // Timeout esperado cuando no hay paquetes
                if ( e.SocketErrorCode != SocketError.TimedOut )
                {
                    Debug.LogWarning( "[LANDiscovery] Socket error: " + e.Message );
                }
            }
        }

        public void ClearDiscoveredServers()
        {
            discoveredServers.Clear();
        }

        void OnDestroy()
        {
            udp?.Close();
            udp = null;
        }
    }

}