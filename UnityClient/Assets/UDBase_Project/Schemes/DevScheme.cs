#if Scheme_DevScheme
using UDBase.Common;
using UDBase.Controllers.SceneSystem;

public class ProjectScheme : Scheme {

	public ProjectScheme() {
		AddController<Scene>(new DirectSceneLoader());
	}
}
#endif
