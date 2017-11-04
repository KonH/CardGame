using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour {
	public Text Name;
	public Text Price;
	public Text Attack;
	public Text Hp;

	void SetText(Text text, string content) {
		var valid = !string.IsNullOrEmpty(content);
		text.enabled = valid;
		if ( valid ) {
			text.text = content;
		}
	}

	void SetText(Text text, int content) {
		SetText(text, content > 0 ? content.ToString() : "");
	}

	public void Init(string name, int price, int attack, int hp, int maxHp) {
		SetText(Name, name);
		SetText(Price, price);
		SetText(Attack, attack);
		if ( maxHp > 0 ) {
			SetText(Hp, string.Format("{0}/{1}", hp, maxHp));
		} else {
			SetText(Hp, "");
		}
	}
}
