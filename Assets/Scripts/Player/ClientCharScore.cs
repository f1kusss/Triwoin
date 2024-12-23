using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Triwoinmag
{
    public class ClientCharScore : NetworkBehaviour
    {
        [SerializeField] private UIPlayerCharScore uiPlayerCharScore;
        [SerializeField] private NetworkVariable<int> charScore = new NetworkVariable<int>();

        public override void OnNetworkSpawn()
        {
            if (!IsOwner) return;

            uiPlayerCharScore = FindObjectOfType<UIPlayerCharScore>();

            charScore.OnValueChanged += OnCharScoreChanged;
        }

        public override void OnNetworkDespawn()
        {
            charScore.OnValueChanged -= OnCharScoreChanged;
        }

        private void OnCharScoreChanged(int previousValue, int newValue)
        {
            UpdateScore(newValue);
        }

        public void UpdateScore(int newScore)
        {
            uiPlayerCharScore.CharScoreChanged(newScore);
        }

        public void ReceiveNewScore(int newValue)
        {
            charScore.Value = newValue;
        }
    }
}