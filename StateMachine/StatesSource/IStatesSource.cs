using System.Xml.Linq;

namespace StateMachine.StatesSource
{
	public interface IStatesSource
	{
		XDocument LoadStates();
	}
}
