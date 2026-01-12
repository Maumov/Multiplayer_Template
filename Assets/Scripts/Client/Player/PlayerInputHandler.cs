using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Client.Player
{
    public class PlayerInputHandler : NetworkBehaviour
    {
        private ClientPlayerActions actions;
        private PlayerController networkHandler;
        public KeyCode spawnCharacter;
        public KeyCode attack;
        public KeyCode move;

        void Awake()
        {
            actions = GetComponent<ClientPlayerActions>();
            networkHandler = GetComponent<PlayerController>();
        }

        void Update()
        {
            if ( Input.GetKeyDown( attack ) )
            {
                Debug.Log( "RequestAttack" );
                actions.RequestAttack();
            }
            if ( Input.GetKeyDown( move ) )
            {
                actions.RequestMove();
            }
            if ( Input.GetKeyDown( spawnCharacter ) )
            {
                networkHandler.SpawnCharacter();
            }
        }
    }

}
