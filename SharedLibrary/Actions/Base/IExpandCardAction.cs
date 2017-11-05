namespace SharedLibrary.Actions {
	public interface IExpandCardAction {
		string ExpandUser  { get; }
		bool   ExpandHand  { get; }
		bool   ExpandTable { get; }
	}
}
