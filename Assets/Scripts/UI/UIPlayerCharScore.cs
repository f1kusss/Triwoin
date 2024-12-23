using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Triwoinmag {
    public class UIPlayerCharScore : MonoBehaviour {

	    [SerializeField] private TextMeshProUGUI _scoreText;

        private void Start() {
	        if (_scoreText == null)
		        _scoreText = GetComponent<TextMeshProUGUI>();
	        _scoreText.text = "0";
        }

        public void CharScoreChanged(int newValue) {
	        _scoreText.text = newValue.ToString();

        }
    }
}
