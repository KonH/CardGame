#if Scheme_DevRemoteScheme

public class ProjectScheme : DevScheme {

	public ProjectScheme():base() {
		AddController<Game>(new NetworkGameController(1.0f, 5));
	}
}
#endif
