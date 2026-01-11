using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    public NetworkVariable<float> MatchTime = new(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        if ( Instance != null )
        {
            Destroy( gameObject );
            return;
        }

        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if ( !IsServer )
            return;

        InitGame();
    }

    private void Update()
    {
        if ( !IsServer )
            return;

        MatchTime.Value += Time.deltaTime; 
    }

    private void InitGame()
    {
        Debug.Log( "Inicializando reglas del juego (solo servidor)" );
        // reglas, timers, spawn de enemigos, etc
    }
}
