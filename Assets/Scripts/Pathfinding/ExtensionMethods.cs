using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ExtensionMethods {
    public static class Utils {
		public static T Pop<T>(this List<T> arr) {
			T val = arr[arr.Count - 1];
			arr.RemoveAt(arr.Count - 1);
			return val;
		}

		public static bool ValidIndex<T>(this List<T> arr, int idx) {
			if (arr == null)
				return false;
			if (idx < 0)
				return false;
			if (idx >= arr.Count)
				return false;
			return true;
		}
	}
}