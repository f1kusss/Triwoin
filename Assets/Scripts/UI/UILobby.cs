using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Triwoinmag.ConnectionManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Triwoinmag {
    public class UILobby : MonoBehaviour {
	    [SerializeField] private GameObject _lobbyPanel;

		[SerializeField] private TMP_InputField _nameInputField;
	    [SerializeField] private TMP_InputField _passwordInputField;

	    [SerializeField] private Button _exitButton;

	    [Header("Links")]
	    [SerializeField] private ConnectionManager _connectionManager;

		public String PlayerName => _nameInputField.text;
	    public String PlayerPassword => _passwordInputField.text;


		private void Awake() {
			if(_connectionManager == null)
				_connectionManager = FindObjectOfType<ConnectionManager>();
		}


		public void ButtonConnectAsHost() {
			_connectionManager.ConnectAsHost(PlayerName);

			SwitchLobbyPanel();
		}

		public void ButtonConnectAsClient() {
			_connectionManager.ConnectAsClient(PlayerName);

			SwitchLobbyPanel();
		}

		private void SwitchLobbyPanel() {
			_lobbyPanel.SetActive(!_lobbyPanel.activeSelf);
		}
	}
}
