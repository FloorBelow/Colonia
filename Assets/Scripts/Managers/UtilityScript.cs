using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilityScript
{
	public static GameData data;

	public static ResourceData GetResource(string name) {
		for(int i = 0; i < data.resources.Length; i++) {
			if (name == data.resources[i].resourceName) return data.resources[i];
		}
		return null;
	}

	public static string IntToRoman(int i) {
		int[] dec = new int[] { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };
		string[] roman = new string[] { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
		string result = "";
		for (int x = 0; x < dec.Length; x++) {
			while (i % dec[x] < i) {
				result += roman[x];
				i -= dec[x];
			}
		}
		return result;
	}
}
