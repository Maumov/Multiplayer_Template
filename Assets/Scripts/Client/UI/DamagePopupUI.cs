using Client.Combat;
using UnityEngine;

namespace Client.UI
{
    public class DamagePopupUI : MonoBehaviour
    {
        [SerializeField] private DamagePopup popupPrefab;
        [SerializeField] private Canvas worldCanvas;

        void OnEnable()
        {
            CombatEvents.OnEnemyDamaged += ShowPopup;
        }

        void OnDisable()
        {
            CombatEvents.OnEnemyDamaged -= ShowPopup;
        }

        private void ShowPopup(
            Vector3 position,
            int damage,
            bool critical )
        {
            var popup = Instantiate(
                popupPrefab,
                position,
                Quaternion.identity,
                worldCanvas.transform );

            popup.Setup( damage, critical );
        }
    }
}
