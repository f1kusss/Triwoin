using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleDebugLog : MonoBehaviour {
	[Header("Controls")]
	[SerializeField] private bool WriteTo_myLog = true;
	[SerializeField] private bool ShowLogPanel = false;
	[SerializeField] private bool SaveToFile = false;

	[Header("Settings")]
	[SerializeField] private KeyCode KeyCodeSwitchOnOff = KeyCode.L;

	[Header("Refs UI")]
	public GameObject Panel;
	public TextMeshProUGUI Text;

	private string _myLog = "*";
	private string _filename = "";

	//private int kChars = 700;

	private void OnEnable() {
		Debug.Log("SimpleDebugLog.OnEnable. Subscribing to Application.logMessageReceived");
		Application.logMessageReceived += Log;
		//Panel.SetActive(ShowLogPanel);
	}

	private void OnDisable() {
		Application.logMessageReceived -= Log;
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCodeSwitchOnOff)) {
			ShowLogPanel = !ShowLogPanel;
			Panel.SetActive(ShowLogPanel);
		}

		Text.text = _myLog;
	}


	public void Log(string logString, string stackTrace, LogType type) {
		// Updating log
		_myLog = _myLog + " \n" + logString;
		//if (myLog.Length > kChars) { myLog = myLog.Substring(myLog.Length - kChars); }

		// If we want to save log to a file
		if (!SaveToFile) return;
		if (_filename == "") {
			string d = System.Environment.GetFolderPath(
				System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
			System.IO.Directory.CreateDirectory(d);
			string r = Random.Range(1000, 9999).ToString();
			_filename = d + "/log-" + r + ".txt";
		}
		try {
			System.IO.File.AppendAllText(_filename, logString + " \n"); }
		catch { }
		}


	//void OnGUI() {
	//	if (!doShow) { return; }
	//	GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
	//		new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
	//	GUI.TextArea(new Rect(10, 10, 540, 370), myLog);
	//}
}
