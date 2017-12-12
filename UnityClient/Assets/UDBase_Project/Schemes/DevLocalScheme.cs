#if Scheme_DevLocalScheme

public class ProjectScheme : DevScheme {

	public ProjectScheme():base() {
		AddController<Game>(new LocalGameController());
	}
}
#endif
