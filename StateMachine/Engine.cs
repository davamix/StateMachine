using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using StateMachine.StatesSource;

namespace StateMachine
{
	public class Engine : IEngine
	{
		private readonly IStatesSource _statesSource;
		private XDocument document;

		public string CurrentState { get; set; }

		public Engine(IStatesSource statesSource)
		{
			_statesSource = statesSource;

			LoadStates();
		}

		public void LoadStates()
		{
			document = _statesSource.LoadStates();
		}
		
		public bool Next(string nextStep)
		{
			var state = document.Descendants("state")
			                    .Elements("transition")
			                    .FirstOrDefault(x =>
			                                    x.Parent.Attribute("name").Value.Equals(CurrentState) &&
			                                    x.Attribute("input").Value.Equals(nextStep));

			if (state != null)
			{
				CurrentState = state.Attribute("next").Value;
				return true;
			}

			return false;
		}

		public Dictionary<string, string> GetAvailableCommands()
		{
			var commands = document.Descendants("state")
			                       .Elements("transition");

			return commands.ToDictionary(xElement => xElement.Attribute("next").Value,
			                             xElement => xElement.Attribute("input").Value);
		}

		/// <summary>
		/// Get available transition for the status
		/// </summary>
		/// <param name="status"></param>
		/// <returns>State, Input</returns>
		public Dictionary<string, string> GetAvailableCommands(string status)
		{
			var retVal = new Dictionary<string, string>();

			var commands = document.Descendants("state")
								   .Elements("transition");


			foreach (var xElement in commands)
			{
				if (xElement.Parent.Attribute("name").Value.Equals(status)) 
					retVal.Add(xElement.Attribute("next").Value, xElement.Attribute("input").Value);
			}

			return retVal;
		}

		public IEnumerable<string> GetAvailableStates()
		{
			var states = document.Descendants("state").Attributes("name");

			foreach (var state in states)
			{
				yield return state.Value;
			}
		}
    }
}
