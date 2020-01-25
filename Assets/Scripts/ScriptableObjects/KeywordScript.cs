using UnityEngine;

public class KeywordScript : MonoBehaviour
{
	[SerializeField]
	Keyword[] keywords;

	public bool HasKeyword(Keyword k) {
		foreach (Keyword keyword in keywords) if (keyword == k) return true;
		return false;
	}

	public bool HasKeyword(Keyword[] ks) {
		foreach (Keyword k in ks) foreach (Keyword keyword in keywords) if (keyword == k) { return true; }
		return false;
	}

}
