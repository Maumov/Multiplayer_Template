using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Client.Player
{
    public class PlayerInputHandler : NetworkBehaviour
    {
        private ClientPlayerActions actions;
        private PlayerController playerController;
        public KeyCode spawnCharacter;
        public KeyCode attack;
        public KeyCode move;
        public KeyCode Upgrade;
        void Awake()
        {
            actions = GetComponent<ClientPlayerActions>();
            playerController = GetComponent<PlayerController>();
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
                playerController.SpawnCharacter();
            }
            if ( Input.GetKeyDown( Upgrade ) )
            {
                playerController.UpgradeCharacter();
            }
        }
    }

}
