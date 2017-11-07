#if Scheme_DevLocalScheme
using UnityEngine;
using System.Collections;
using UDBase.Common;

public class ProjectScheme : DevScheme {

	public ProjectScheme():base() {
		AddController<Game>(new LocalGameController());
	}
}
#endif
