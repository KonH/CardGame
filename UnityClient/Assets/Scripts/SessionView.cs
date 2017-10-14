using UnityEngine;
using UnityEngine.UI;

public class SessionView : MonoBehaviour {
	public Text NameText;
	public Text UsersText;
	public Button ConnectButton;

	SessionSample _sample;
	string _id;

	void Awake() {
		ConnectButton.onClick.AddListener(TryConnect);
	}

	public void Init(string userName, int users, int maxUsers, SessionSample sample, string id) {
		NameText.text = userName;
		UsersText.text = string.Format("{0}/{1}", users, maxUsers);
		_sample = sample;
		_id = id;
	}

	public void TryConnect() {
		_sample.TryConnect(_id);
	}
}
