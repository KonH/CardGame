﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour {
	public Button     Button;
	public GameObject Content;
	public Text       Name;
	public Text       Price;
	public Text       Attack;
	public Text       Hp;

	Action _callback;

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
		text.enabled = valid;
		if ( valid ) {
			text.text = content;
		}
	}

	void SetText(Text text, int content) {
		SetText(text, content > 0 ? content.ToString() : "");
	}

	public void Init(bool withContent, bool interactable, string name, int price, int attack, int hp, int maxHp) {
		Button.gameObject.SetActive(interactable);
		Content.SetActive(withContent);
		SetText(Name,   name);
		SetText(Price,  price);
		SetText(Attack, attack);
		if ( maxHp > 0 ) {
			SetText(Hp, string.Format("{0}/{1}", hp, maxHp));
		} else {
			SetText(Hp, "");
		}
	}

	public void InitPlaceholder(bool interactable) {
		Init(false, interactable, string.Empty, 0, 0, 0, 0);
	}

	public void AddCallback(Action callback) {
		_callback = callback;
	}

	void OnClick() {
		if ( _callback != null ) {
			_callback();
		}
	}
}