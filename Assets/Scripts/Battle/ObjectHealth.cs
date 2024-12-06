using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Triwoinmag {
	public class ObjectHealth : MonoBehaviour, IDamageable {
		[SerializeField] private bool Debugging;

		[SerializeField] private float _health = 100;
		public float Health => _health;
		[field: SerializeField] public float MaxHealth { get; set; } = 100;

		[SerializeField] private GameObject ExplosionPrefab;

		private void Start() {

		}


		public void ReceiveDamage(float damageAmount, Vector3 hitPosition, GameAgent sender) {
			_health -= damageAmount;
			if (Debugging) {
				Debug.Log(
					$"CharacterHealth.ReceiveDamage. New Health: {_health}. Attacker: {sender.gameObject.name}. Attacker faction: {sender.ShipFaction}");
			}

			if (_health <= 0) {
				Instantiate(ExplosionPrefab, transform.position, Quaternion.identity);
				Destroy(gameObject);
			}
		}

		public void ReceiveHeal(float healAmount, Vector3 hitPosition, GameAgent sender) {
			_health += healAmount;
		}

	}
}