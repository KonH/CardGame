using System;
using UnityEngine;
using UnityEngine.UI;
using UDBase.UI.Common;

[RequireComponent(typeof(UIElement))]
public class NoticeWindow : MonoBehaviour {
	static NoticeWindow Instance { get; set; }

	public static void Show(string header, string message, string actionText, Action callback = null) {
		Instance?.ShowWindow(header, message, actionText, callback);
	}

	public static void ShowWithOkButton(string header, string message, Action callback = null) {
		Show(header, message, "OK", callback);
	}

	public Text   Header;
	public Text   Message;
	public Button HideButton;
	public Text   ButtonText;

	UIElement _element;
	Action    _callback;

	void Awake() {
		_element = GetComponent<UIElement>();
		HideButton.onClick.AddListener(OnClick);
		Instance = this;
	}

	void Start() {
		_element.Hide();
	}

	void OnDestroy() {
		Instance = null;
	}

	void ShowWindow(string header, string message, string actionText, Action callback) {
		Header.text = header;
		Message.text = message;
		ButtonText.text = actionText;
		_callback = callback;
		_element.Show();
	}

	void OnClick() {
		_element.Hide();
		_callback?.Invoke();
		_callback = null;
	}
}
