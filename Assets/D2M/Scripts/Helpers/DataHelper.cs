using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataHelper {
	/// <summary>
	/// Gets the int.
	/// </summary>
	/// <returns>The int.</returns>
	/// <param name="key">Key.</param>
	/// <param name="defaultValue">Default value.</param>
	public static int GetInt(string key, int defaultValue = 0) {
		return PlayerPrefs.GetInt (key, defaultValue);
	}

	public static void SetInt(string key, int value) {
		PlayerPrefs.SetInt (key, value);
	}

	/// <summary>
	/// Gets the float.
	/// </summary>
	/// <returns>The float.</returns>
	/// <param name="key">Key.</param>
	/// <param name="defaultValue">Default value.</param>
	public static float GetFloat(string key, float defaultValue = 0f) {
		return PlayerPrefs.GetFloat(key, defaultValue);
	}

	public static void SetFloat(string key, float value) {
		PlayerPrefs.SetFloat (key, value);
	}

	/// <summary>
	/// Gets the string.
	/// </summary>
	/// <returns>The string.</returns>
	/// <param name="key">Key.</param>
	/// <param name="defaultValue">Default value.</param>
	public static string GetString(string key, string defaultValue = "") {
		return PlayerPrefs.GetString (key, defaultValue);
	}

	public static void SetString(string key, string value) {
		PlayerPrefs.SetString (key, value);
	}

	/// <summary>
	/// Gets the bool.
	/// </summary>
	/// <returns>The bool.</returns>
	/// <param name="key">Key.</param>
	/// <param name="defaultValue">If set to <c>true</c> default value.</param>
	public static bool GetBool(string key, bool defaultValue = false) {
		return (PlayerPrefs.GetInt (key, (defaultValue == false ? 0 : 1)) == 0 ? false : true);
	}

	public static void SetBool(string key, bool value) {
		PlayerPrefs.SetInt (key, value == false ? 0 : 1);
	}
}
