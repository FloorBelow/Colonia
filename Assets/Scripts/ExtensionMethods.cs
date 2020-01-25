using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods
{
    public static bool HasKeyword(this GameObject o, Keyword k) {
		return o.GetComponent<KeywordScript>().HasKeyword(k);
	}

	public static bool HasKeyword(this GameObject o, Keyword[] ks) {
		return o.GetComponent<KeywordScript>().HasKeyword(ks);
	}

	public static bool HasComponent<T>(this GameObject o) where T:Component {
		return o.GetComponent<T>() != null;
	}
}
