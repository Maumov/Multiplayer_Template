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
        public int port = 47777;
        public float interval = 1f;
        public string broadcastMsg = "UNITY_LAN_SERVER";

        public Action<string> OnServerFound;

        UdpClient sender;
        UdpClient listener;
        float timer;

        void Start()
        {
            listener = new UdpClient( port );
            listener.BeginReceive( OnReceive, null );
        }

        void Update()
        {
            if ( !NetworkManager.Singleton.IsServer )
                return;

            if ( sender == null )
            {
                sender = new UdpClient();
                sender.EnableBroadcast = true;
            }

            timer += Time.deltaTime;
            if ( timer >= interval )
            {
                Broadcast();
                timer = 0;
            }
        }

        void Broadcast()
        {
            byte[] data = Encoding.UTF8.GetBytes( broadcastMsg );

            foreach ( var ip in GetBroadcastIPs() )
                sender.Send( data, data.Length, new IPEndPoint( ip, port ) );
        }

        void OnReceive( IAsyncResult ar )
        {
            IPEndPoint ep = new IPEndPoint( IPAddress.Any, 0 );
            byte[] data = listener.EndReceive( ar, ref ep );

            if ( Encoding.UTF8.GetString( data ) == broadcastMsg )
                OnServerFound?.Invoke( ep.Address.ToString() );

            listener.BeginReceive( OnReceive, null );
        }

        IPAddress[] GetBroadcastIPs()
        {
            var list = new System.Collections.Generic.List<IPAddress>();
            foreach ( var ip in Dns.GetHostEntry( Dns.GetHostName() ).AddressList )
            {
                if ( ip.AddressFamily == AddressFamily.InterNetwork )
                {
                    var b = ip.GetAddressBytes();
                    b[ 3 ] = 255;
                    list.Add( new IPAddress( b ) );
                }
            }
            return list.ToArray();
        }

        void OnDestroy()
        {
            sender?.Close();
            listener?.Close();
        }
    }
}