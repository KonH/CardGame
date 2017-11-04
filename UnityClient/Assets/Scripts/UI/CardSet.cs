using System.Collections.Generic;
using UnityEngine;

public class CardSet : MonoBehaviour {
	List<CardView> _cards = new List<CardView>();

	public List<CardView> Cards {
		get {
			return _cards;
		}
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
	}
}
