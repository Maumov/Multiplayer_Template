using System.Collections.Generic;
using System;
using Unity.Netcode;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace Assets.Scripts.Server.Bootstrap
{
    public class LANDiscovery : MonoBehaviour
    {
        public static LANDiscovery Instance;

        // Evento para avisar cuando se encuentra un servidor
        public event Action<string> OnServerFound;

        [Header( "Broadcast Settings" )]
        public ushort broadcastPort = NetworkConstants.broadcastPort; // Puerto UDP para LAN discovery
        public float broadcastInterval = NetworkConstants.broadcastInterval; // Cada cuánto se envía broadcast (seg)
        public string broadcastMessage = NetworkConstants.broadcastMessage;

        private float timer;
        private List<string> discoveredServers = new List<string>();

        UdpClient udp;
        IPEndPoint remoteEP;

        void Awake()
        {
            if ( Instance != null && Instance != this )
            {
                Destroy( gameObject );
                return;
            }
            Instance = this;
            DontDestroyOnLoad( gameObject );
        }

        void Update()
        {
            // Solo el servidor hace broadcast
            if ( NetworkManager.Singleton.IsServer )
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
                // Solo el cliente escucha
                ListenForServers();
            }
        }

        // -----------------------
        // Servidor: enviar broadcast
        // -----------------------
        private void BroadcastServer()
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes( broadcastMessage );
                IPEndPoint endPoint = new IPEndPoint( IPAddress.Broadcast, broadcastPort );
                udp.Send( data, data.Length, endPoint );
                Debug.LogWarning( "[LANDiscovery] Broadcasting server! " );

            }
            catch ( Exception e )
            {
                Debug.LogWarning( "[LANDiscovery] Broadcast failed: " + e.Message );
            }
        }

        // -----------------------
        // Cliente: escuchar broadcast
        // -----------------------
        private void ListenForServers()
        {
            if ( udp == null )
                return;

            try
            {
                while ( udp.Available > 0 )
                {
                    var remoteEP = new System.Net.IPEndPoint( System.Net.IPAddress.Any, 0 );
                    byte[] data = udp.Receive( ref remoteEP );
                    string msg = System.Text.Encoding.UTF8.GetString( data );

                    if ( msg == broadcastMessage )
                    {
                        string serverIP = remoteEP.Address.ToString();
                        if ( !discoveredServers.Contains( serverIP ) )
                        {
                            discoveredServers.Add( serverIP );
                            Debug.Log( "[LANDiscovery] Server found: " + serverIP );
                            OnServerFound?.Invoke( serverIP );
                        }
                    }
                    Debug.LogWarning( "[LANDiscovery] Listening for servers! " );
                }
                
            }
            catch( SocketException e)
            {
                Debug.LogError( e );
                // Ignorar timeout
            }
        }

        public void StartClientDiscovery()
        {
            try
            {
                udp = new UdpClient( broadcastPort );
                udp.EnableBroadcast = true;
                udp.Client.ReceiveTimeout = 1;
                remoteEP = new IPEndPoint( IPAddress.Any, 0 );

                Debug.Log( $"[LANDiscovery] Listening on UDP {broadcastPort}" );
            }
            catch ( Exception e )
            {
                Debug.LogError( "[LANDiscovery] Failed to start UDP listener: " + e.Message );
            }
            /*
            if ( !NetworkManager.Singleton.IsClient )
            {
                Debug.Log( "[LANDiscovery] Client discovery started." );
                // Nothing extra needed; Update() escuchará broadcast
                udp = new UdpClient( broadcastPort );
                udp.EnableBroadcast = true;
                udp.Client.ReceiveTimeout = 1;
                remoteEP = new IPEndPoint( IPAddress.Any, 0 );

                Debug.Log( "[LANDiscovery] Listening on port " + broadcastPort );
            }
            */
        }

        public void StopClientDiscovery()
        {
            discoveredServers.Clear();
            OnServerFound = null;
            udp?.Close();
            udp = null;
        }
    }
}