using System;
using System.Collections.Generic;

namespace SharedLibrary.Utils {
	public static class CollectionUtils {
		public static List<T> Fill<T>(this List<T> collection, Func<T> value, int count) {
			for ( var i = 0; i < count; i++ ) {
				collection.Add(value != null ? value() : default(T));
			}
			return collection;
		}
	}
}
