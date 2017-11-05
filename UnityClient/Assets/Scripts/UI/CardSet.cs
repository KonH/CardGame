using System;
using System.Collections.Generic;
using UnityEngine;

public class CardSet : MonoBehaviour {
	List<CardView> _cards = new List<CardView>();
	Action<int>    _clickCallback;

	public List<CardView> Cards {
		get {
			return _cards;
		}
	}

	public void Init(Action<int> cardClick) {
		_clickCallback = cardClick;
	}

	public void Clear() {
		foreach ( var card in _cards ) {
			ObjectPool.Recycle(card);
		}
		_cards.Clear();
	}

	public void Add(CardView view) {
		_cards.Add(view);
		view.transform.SetParent(transform, false);
		view.transform.localScale = Vector3.one;
		var index = _cards.Count - 1;
		Action callback = null;
		if ( view.Interactable ) {
			callback = () => OnCardClick(index);
		}
		view.AddCallback(callback);
	}

	void OnCardClick(int index) {
		_clickCallback(index);
	}
}
