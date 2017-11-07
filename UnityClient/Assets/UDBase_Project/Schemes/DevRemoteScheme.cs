#if Scheme_DevRemoteScheme
using UnityEngine;
using System.Collections;
using UDBase.Common;

public class ProjectScheme : DevScheme {

	public ProjectScheme():base() {
		AddController<Game>(new NetworkGameController(1.0f));
	}
}
#endif
