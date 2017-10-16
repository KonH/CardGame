using UDBase.Controllers.EventSystem;
using UDBase.Controllers.LogSystem;
using UDBase.Utils;

public class BearerWebClient : WebClient {
	public void Init() {
		Events.Subscribe<Auth_UpdateHeader>(this, OnUpdateAuthHeader);
	}

	public void Reset() {
		Events.Unsubscribe<Auth_UpdateHeader>(OnUpdateAuthHeader);
	}

	void OnUpdateAuthHeader(Auth_UpdateHeader e) {
		if ( !string.IsNullOrEmpty(e.AuthHeader) ) {
			Log.MessageFormat("OnUpdateAuthHeader: new header = '{0}'", LogTags.Common, e.AuthHeader);
			ApplyAuthHeader(e.AuthHeader);
		}
	}
}
