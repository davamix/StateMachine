using System.Collections.Generic;

namespace StateMachine
{
	public interface IEngine
	{
		string CurrentState { get; set; }
		
		void ReloadConfigurationFile(string filePath);
		
		bool Next(string nextStep);
		
		Dictionary<string, string> GetAvailableCommands();
		
		Dictionary<string, string> GetAvailableCommands(string status);

		IEnumerable<string> GetAvailableStates();
	}
}