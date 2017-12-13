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

	public void Add(CardView view, bool attach = true) {
		_cards.Add(view);
		var index = _cards.Count - 1;
		ProcessNewCard(view, index, attach);
	}

	public void Insert(CardView view, int index, bool attach = true) {
		_cards.Insert(index, view);
		ProcessNewCard(view, index, attach);
	}

	void ProcessNewCard(CardView view, int index, bool attach) {
		view.transform.SetParent(transform, !attach);
		view.transform.SetSiblingIndex(index);
		view.transform.localScale = Vector3.one;
		view.AddCallback(OnCardClick);
	}

	public void Remove(CardView view, bool recycle) {
		_cards.Remove(view);
		view.transform.SetParent(null);
		if ( recycle ) {
			ObjectPool.Recycle(view);
		}
	}

	void OnCardClick(CardView view) {
		_clickCallback(Cards.IndexOf(view));
	}
}
