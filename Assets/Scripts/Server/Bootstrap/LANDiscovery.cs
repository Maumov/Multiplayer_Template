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

        [Header( "Discovery Settings" )]
        [SerializeField] int broadcastPort = 47777;
        [SerializeField] float broadcastInterval = 1.5f;
        [SerializeField] string broadcastMessage = "UNITY_LAN_SERVER";

        public event Action<string> OnServerFound;

        UdpClient udp;
        float timer;

        readonly HashSet<string> discoveredServers = new HashSet<string>();

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

        void Start()
        {
            //StartUDP();
        }

        public void StartUDP()
        {
            try
            {
                udp = new UdpClient( broadcastPort );
                udp.EnableBroadcast = true;

                // ⚠️ MUY IMPORTANTE EN WINDOWS
                udp.Client.SetSocketOption(
                    SocketOptionLevel.Socket,
                    SocketOptionName.ReuseAddress,
                    true );

                BeginReceive();

                Debug.Log( $"[LANDiscovery] UDP listening on {broadcastPort}" );
            }
            catch ( Exception e )
            {
                Debug.LogError( "[LANDiscovery] UDP init failed: " + e );
            }
        }

        void BeginReceive()
        {
            try
            {
                udp.BeginReceive( OnReceive, null );
            }
            catch
            {
                // Socket cerrado
            }
        }

        void OnReceive( IAsyncResult ar )
        {
            IPEndPoint ep = new IPEndPoint( IPAddress.Any, 0 );
            byte[] data;

            try
            {
                data = udp.EndReceive( ar, ref ep );
            }
            catch
            {
                return;
            }

            string msg = Encoding.UTF8.GetString( data );

            if ( msg == broadcastMessage )
            {
                string serverIP = ep.Address.ToString();

                if ( discoveredServers.Add( serverIP ) )
                {
                    Debug.Log( "[LANDiscovery] Server found: " + serverIP );
                    OnServerFound?.Invoke( serverIP );
                }
            }

            BeginReceive(); // 🔁 seguir escuchando
        }

        void Update()
        {
            // Servidor (host o dedicado) → broadcast
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
        }

        void BroadcastServer()
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes( broadcastMessage );
                IPEndPoint ep = new IPEndPoint( IPAddress.Broadcast, broadcastPort );
                udp.Send( data, data.Length, ep );
            }
            catch
            {
                // Ignorar errores UDP normales
            }
        }

        void OnDestroy()
        {
            try
            {
                udp?.Close();
            }
            catch { }
        }
    }
}