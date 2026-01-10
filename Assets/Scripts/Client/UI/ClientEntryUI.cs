using TMPro;
using UnityEngine;

public class ClientEntryUI : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    public void Init( ulong clientId, bool isHost )
    {
        label.text = isHost
            ? $"Client {clientId} (HOST)"
            : $"Client {clientId}";
    }
}