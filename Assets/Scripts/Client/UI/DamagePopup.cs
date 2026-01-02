using System;
using UnityEngine;

namespace Client.UI
{
    public class DamagePopup : MonoBehaviour
    {
        public void Setup( int damage, bool critical )
        {
            Debug.Log( $"Damage Popup: Damage: {damage} CRIT: {critical}" );
        }
    }
}