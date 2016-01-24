using System.Xml.Linq;

namespace StateMachine.StatesSource
{
	public class StatesSourceXml : IStatesSource
	{
		private readonly string _filePath;

		public StatesSourceXml(string filePath)
		{
			_filePath = filePath;
		}

		public XDocument LoadStates()
		{
			return XDocument.Load(_filePath);
		}
	}
}
