using System.Collections.Generic;

namespace StateMachine
{
	public interface IEngine
	{
		string CurrentState { get; set; }
		
		void LoadStates();
		
		bool Next(string nextStep);
		
		Dictionary<string, string> GetAvailableInputs();
		
		Dictionary<string, string> GetAvailableInputs(string status);

		IEnumerable<string> GetAvailableStates();
	}
}