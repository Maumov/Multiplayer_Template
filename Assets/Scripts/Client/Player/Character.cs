using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Client.Player
{
    public class Character : NetworkBehaviour
    {
        [Header( "Movement" )]
        public float speed = 5f;

        private Transform ownerTransform;

        void Start()
        {
            if ( !IsOwner )
                return; // solo el dueño mueve este Character
            ownerTransform = transform;
        }

        void Update()
        {
            if ( !IsOwner )
                return;

            float h = Input.GetAxis( "Horizontal" );
            float v = Input.GetAxis( "Vertical" );
            Vector3 move = new Vector3( h, 0, v ) * speed * Time.deltaTime;
            ownerTransform.Translate( move );
        }
    }
}