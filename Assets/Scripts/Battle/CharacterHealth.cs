using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Triwoinmag
{
    public class CharacterHealth : NetworkBehaviour, IDamageable
    {
        [SerializeField] private bool Debugging;

        [SerializeField] private float _health = 100;
        public float Health => _health;
        [field: SerializeField] public float MaxHealth { get; set; } = 100;

        [SerializeField] private GameObject ExplosionPrefab;

        private void Start()
        {
        }


        public void ReceiveDamage(float damageAmount, Vector3 hitPosition, GameAgent sender)
        {
            _health -= damageAmount;
            if (Debugging)
            {
                Debug.Log(
                    $"CharacterHealth.ReceiveDamage. New Health: {_health}. Attacker: {sender.gameObject.name}. Attacker faction: {sender.ShipFaction}");
            }

            if (IsServer)
            {
                ManagerScore.I.AddScore(sender.OwnerClientId, 5);
            }

            if (_health <= 0)
            {
                Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
                DeathEffectClientRpc();
                if (IsServer)
                {
                    Destroy(gameObject);
                    ManagerScore.I.AddScore(sender.OwnerClientId, 100);

                }
            }
        }

        [ClientRpc]
        public void DeathEffectClientRpc()
        {
            Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
        }

        public void ReceiveHeal(float healAmount, Vector3 hitPosition, GameAgent sender)
        {
            _health += healAmount;
        }
    }
}