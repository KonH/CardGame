using System;
using System.Reflection;
using System.Collections.Generic;
using SharedLibrary.Actions;

namespace SharedLibrary.Utils {
	public static class ReflectionUtils {
		static Dictionary<string, Type> _cache = new Dictionary<string, Type>();

		public static Type GetActionType(string typeName) {
			Type type = null;
			if ( !_cache.TryGetValue(typeName, out type) ) {
				type = Assembly.GetAssembly(typeof(IGameAction)).GetType(typeName);
				_cache.Add(typeName, type);
			}
			return type;
		} 
	}
}
