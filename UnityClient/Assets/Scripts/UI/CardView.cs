using System;
using UnityEngine;
using UnityEngine.UI;
using UDBase.Utils;
using DG.Tweening;

public class CardView : MonoBehaviour {
	public Transform  Root;
	public Button     Button;
	public GameObject Content;
	public Text       Name;
	public Text       Price;
	public Text       Attack;
	public Text       Hp;
	public Image      Cover;

	Sequence _seq;
	Action   _callback;

	public bool Interactable {
		get {
			return Button.gameObject.activeSelf;
		}
	}

	void Awake() {
		Button.onClick.AddListener(OnClick);
	}

	void SetText(Text text, string content) {
		var valid    = !string.IsNullOrEmpty(content);
		text.gameObject.SetActive(valid);
		if ( valid ) {
			text.text = content;
		}
	}

	void SetText(Text text, int content) {
		SetText(text, content > 0 ? content.ToString() : "");
	}

	public void Init(bool withContent, string name, int price, int attack, int hp, int maxHp, bool withCover) {
		_seq = TweenHelper.Reset(_seq);
		Button.gameObject.SetActive(false);
		Content.SetActive(withContent);
		SetText(Name,   name);
		SetText(Price,  price);
		SetText(Attack, attack);
		if ( maxHp > 0 ) {
			SetText(Hp, string.Format("{0}/{1}", hp, maxHp));
		} else {
			SetText(Hp, "");
		}
		Cover.enabled = withCover;
	}

	public void InitPlaceholder() {
		Init(false, string.Empty, 0, 0, 0, 0, false);
	}

	public void AddCallback(Action callback) {
		_callback = callback;
	}

	public void SetEffect(Tween t, Action callback = null) {
		_seq = TweenHelper.Replace(_seq);
		_seq.Append(t);
		if ( callback != null ) {
			_seq.AppendCallback(() => callback());
		}
	}

	public void SetEffect(Sequence seq, Action callback = null) {
		TweenHelper.Reset(_seq);
		_seq = seq;
		if ( callback != null ) {
			_seq.AppendCallback(() => callback());
		}
	}

	void OnClick() {
		_callback?.Invoke();
	}

	public void UpdateInteractable(bool value) {
		Button.gameObject.SetActive(value);
	}
}
