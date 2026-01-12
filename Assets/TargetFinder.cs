using Shared.Combat;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class TargetFinder : MonoBehaviour
{

    [SerializeField] float radius = 10f;
    [SerializeField] LayerMask targetMask;

    public List<ulong> GetTarget()
    {
        Collider[] colliders = Physics.OverlapSphere( transform.position, radius, targetMask );

        List<ulong> targets = new List<ulong>();
        for ( int i = 0 ; i < colliders.Length ; i++ )
        {
            IDamageable damageable = colliders[i].GetComponentInParent<IDamageable>();
            if ( damageable != null )
            {
                ulong targetId = colliders[i].GetComponentInParent<NetworkObject>().NetworkObjectId;
                targets.Add( targetId );
            }
        }

        //Remove self;
        ulong self = transform.GetComponentInParent<NetworkObject>().NetworkObjectId;
        targets.Remove( self );

        return targets;

    }

}
