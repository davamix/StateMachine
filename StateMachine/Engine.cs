using System;
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
			try
			{
				document = _statesSource.LoadStates();
			}
			catch (Exception ex)
			{
				throw new Exception("Error loading states", ex);
			}
			
			
		}
		
		/// <summary>
		/// Move the current state to the next one using the input value as a transition reference
		/// </summary>
		/// <param name="input">Value of the transition reference to move to the related state</param>
		/// <returns>True if moved;</returns>
		public bool Next(string input)
		{
			var state = document.Descendants("state")
			                    .Elements("transition")
			                    .FirstOrDefault(x =>
			                                    x.Parent.Attribute("name").Value.Equals(CurrentState) &&
			                                    x.Attribute("input").Value.Equals(input));

			if (state != null)
			{
				CurrentState = state.Attribute("next").Value;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Get all available inputs
		/// </summary>
		/// <returns>Dictionary of State and Input</returns>
		public Dictionary<string, string> GetAvailableInputs()
		{
			var commands = document.Descendants("state")
			                       .Elements("transition");

			return commands.ToDictionary(xElement => xElement.Attribute("next").Value,
			                             xElement => xElement.Attribute("input").Value);
		}

		/// <summary>
		/// Get available inputs for the status
		/// </summary>
		/// <param name="status">Name of the status</param>
		/// <returns>Dictionary of State and Input</returns>
		public Dictionary<string, string> GetAvailableInputs(string status)
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

		/// <summary>
		/// Get all states available
		/// </summary>
		/// <returns>List of name states</returns>
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
