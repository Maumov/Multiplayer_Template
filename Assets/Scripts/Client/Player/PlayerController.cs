using Shared.Combat;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Client.Player
{
    public class PlayerController : NetworkBehaviour
    {
        [Header( "Player Stats" )]
        public NetworkVariable<int> health = new NetworkVariable<int>( 100,NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server );

        [Header( "Character" )]
        public GameObject characterPrefab; // prefab visual / jugable
        private GameObject characterInstance;
        private bool canControlCharacter;
        private TargetFinder targetFinder;

        [Header( "UI" )]
        public GameObject UIPrefab;
        private PlayerUIController uiController;

        public NetworkVariable<ulong> CharacterNetId = new( 0 );
        public Transform spawnPoint; // opcional, referencia al CharacterSpawnPoint

        public override void OnNetworkSpawn()
        {
            if ( IsOwner )
            {
                DontDestroyOnLoad( gameObject ); // mantiene este player al cambiar de escena
            }

            // Se ejecuta en host y clientes
            CharacterNetId.OnValueChanged += OnCharacterAssigned;
            
            // Si ya hay Character (late joiner)
            if ( CharacterNetId.Value != 0 )
                OnCharacterAssigned( 0, CharacterNetId.Value );
        }

        void Start()
        {
            if ( !IsOwner )
                return; // solo el dueño instancia su personaje

            if ( spawnPoint == null )
                spawnPoint = transform; // fallback

        }

        #region SPAWN CHARACTER
        public void SpawnCharacter()
        {
            if ( !IsOwner )
                return;

            if ( characterPrefab == null )
                return;

            if ( CharacterNetId.Value != 0 ) 
                return;

            Vector3 spawnPosition = new Vector3( Random.Range( -3f, 3f ), 1f, Random.Range( -3f, 3f ) );
            RequestSpawnCharacterServerRpc( spawnPosition );
        }

        [ServerRpc]
        void RequestSpawnCharacterServerRpc( Vector3 spawnPosition )
        {
            if ( !IsServer )
                return; // Extra seguridad

            // Si ya tiene Character, no spawnea otro
            if ( CharacterNetId.Value != 0 )
                return;

            // Instanciamos el personaje
            GameObject go = Instantiate( characterPrefab, spawnPosition, Quaternion.identity );
            NetworkObject netObj = go.GetComponent<NetworkObject>();

            // Spawn con ownership del cliente
            netObj.SpawnWithOwnership( OwnerClientId );
            EntityController entity = netObj.GetComponent<EntityController>();
            entity.SetInitToCharacter( health.Value );

            // Guardamos NetworkObjectId para que los clientes lo resuelvan
            CharacterNetId.Value = netObj.NetworkObjectId;
        }

        private void OnCharacterAssigned( ulong oldId, ulong newId )
        {
            if ( NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue( newId, out var netObj ) )
            {
                characterInstance = netObj.gameObject;
                
                if ( IsOwner )
                {
                    //Si es personaje de este jugador entonces instanciamos el resto de la UI Local

                    //Setup local stuff...
                    targetFinder = characterInstance.GetComponent<TargetFinder>();
                    //setup UI
                    GameObject ui = Instantiate( UIPrefab );
                    uiController = ui.GetComponent<PlayerUIController>();
                    uiController.Init( characterInstance );
                }
            }
        }
        #endregion

        #region ATTACK
        public void RequestAttack()
        {
            List<ulong> targets = targetFinder.GetTarget();
            if ( targets.Count == 0 )
                return;
            
            ulong targetId = targets[ 0 ];
            Debug.Log( $"Start Request Attack" );
            RequestAttackServerRpc( targetId );
        }

        [ServerRpc]
        private void RequestAttackServerRpc( ulong target )
        {
            if ( !IsServer )
                return; // Extra seguridad
            // NO lógica aquí
            Server.Player.ServerPlayerActions.HandleAttack( OwnerClientId, target );
        }
        #endregion

        #region MOVE
        public void Move()
        {
            if ( !IsOwner || characterInstance == null )
            {
                return;
            }
            Vector3 newPosition = new Vector3( Random.Range( -3f, 3f ), 1f, Random.Range( -3f, 3f ) );
            RequestMoveServerRPC( newPosition );
        }

        [ServerRpc]
        public void RequestMoveServerRPC( Vector3 position )
        {
            if ( !IsServer )
                return; // Extra seguridad

            characterInstance.transform.position = position;
        }

        #endregion
        #region UpgradeCharacter
        public void UpgradeCharacter()
        {
            if ( !IsOwner )
            {
                return;
            }
            
            RequestUpgradeCharacterServerRPC();
        }

        [ServerRpc]
        public void RequestUpgradeCharacterServerRPC( )
        {
            if ( !IsServer )
                return; // Extra seguridad

            health.Value += 100;
        }
        #endregion
    }
}