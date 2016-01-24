using System.Collections.Generic;
using StateMachine.StatesSource;
using Xunit;

namespace StateMachine.Test
{
	public class Engine
	{
		private readonly IStatesSource _statesSource;
		private readonly StateMachine.Engine _engine;
		
		public Engine()
		{
			_statesSource = new StatesSourceXml(@"Data\workflow.xml");

			_engine = new StateMachine.Engine(_statesSource)
			          {
				          CurrentState = "start"
			          };
		}

		[Fact]
		public void InitialStateIsStart()
		{
			Assert.Equal("start", _engine.CurrentState);
		}

		[Fact]
		public void ThereAreThreeCommandsForPendingApprovalStatus()
		{
			var commands = new Dictionary<string, string>
			               {
				               {"changeRequested", "toChangeRequested"},
				               {"approved", "toApproved"},
				               {"denied", "toDenied"}
			               };
			var actual = _engine.GetAvailableCommands("pendingApproval");
			Assert.Equal(commands, actual);
		}

		#region Test of state changes

		[Fact]
		public void FromStartToNotSubmitted()
		{
			FromStateToState("start", "notSubmitted", "toNotSubmitted");
		}

		[Fact]
		public void FromNotSubmittedToSubmitted()
		{
			FromStateToState("notSubmitted", "submitted", "toSubmitted");
		}

		[Fact]
		public void FromSubmittedToPendingApproval()
		{
			FromStateToState("submitted", "pendingApproval", "toPendingApproval");
		}

		[Fact]
		public void FromPendingApprovalToChangeRequested()
		{
			FromStateToState("pendingApproval", "changeRequested", "toChangeRequested");
		}

		[Fact]
		public void FromPendingApprovalToDenied()
		{
			FromStateToState("pendingApproval", "denied", "toDenied");
		}

		[Fact]
		public void FromPendingApprovalToApproved()
		{
			FromStateToState("pendingApproval", "approved", "toApproved");
		}

		[Fact]
		public void FromChangeRequestedToNotSubmitted()
		{
			FromStateToState("changeRequested", "notSubmitted", "toNotSubmitted");
		}

		[Fact]
		public void FromDeniedToCompleted()
		{
			FromStateToState("denied", "completed", "toCompleted");
		}

		[Fact]
		public void FromApprovedToCompleted()
		{
			FromStateToState("approved", "completed", "toCompleted");
		}

		[Fact]
		public void CannotMoveFromCompletedToAnyOther()
		{
			FromStateToStateRestricted("complted", "toStart");
			FromStateToStateRestricted("complted", "toNotSubmitted");
			FromStateToStateRestricted("complted", "toSubmitted");
			FromStateToStateRestricted("complted", "toPendingApproval");
			FromStateToStateRestricted("complted", "toChangeRequested");
			FromStateToStateRestricted("complted", "toDenied");
			FromStateToStateRestricted("complted", "toApproved");
		}

		#endregion

		/// <summary>
		/// Test that cannot move from 'initialState' to the state indicated by 'inputState'
		/// </summary>
		/// <param name="initialState">Initial state from where it's try to move it</param>
		/// <param name="inputState">Input value indicating to which state should be moved</param>
		private void FromStateToStateRestricted(string initialState, string inputState)
		{
			_engine.CurrentState = initialState;
			var moved = _engine.Next(inputState);

			Assert.False(moved);
			Assert.Equal(initialState, _engine.CurrentState);
		}


		private void FromStateToState(string initialState, string finalState, string inputState)
		{
			_engine.CurrentState = initialState;
			var moved = _engine.Next(inputState);

			Assert.True(moved);
			Assert.Equal(finalState, _engine.CurrentState);
		}
	}
}
