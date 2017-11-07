using UDBase.Common;
using UDBase.Controllers.LogSystem;
using UDBase.Controllers.SaveSystem;
using UDBase.Controllers.UserSystem;
using UDBase.Controllers.EventSystem;
using UDBase.Controllers.SceneSystem;

public class DevScheme : Scheme {

	public DevScheme() {
		var save = new InMemorySave();
		save.AddNode<UserSaveNode>("user");
		AddController<Save>(save);

		AddController<Log>   (new UnityLog());
		AddController<Events>(new EventController());
		AddController<User>  (new SaveUser());
		AddController<Scene> (new DirectSceneLoader());

		AddController<Auth>    (new AuthController());
		AddController<Sessions>(new SessionController());
	}
}
