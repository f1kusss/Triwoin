using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {
	[SerializeField] private float _seconds = 1f;

	private void Start() {
        Destroy(gameObject, _seconds);
    }


}