using UnityEngine;
using UnityEditor;
using System.Collections;

namespace UDBase.EditorTools {
	public static class SchemesMenuItems {
		[MenuItem("UDBase/Schemes/Default")]
		static void SwitchToScheme_Default() {
			SchemesTool.SwitchScheme("Default");
		}		[MenuItem("UDBase/Schemes/DevLocalScheme")]
		static void SwitchToScheme_DevLocalScheme() {
			SchemesTool.SwitchScheme("DevLocalScheme");
		}		[MenuItem("UDBase/Schemes/DevRemoteScheme")]
		static void SwitchToScheme_DevRemoteScheme() {
			SchemesTool.SwitchScheme("DevRemoteScheme");
		}
	}
}
