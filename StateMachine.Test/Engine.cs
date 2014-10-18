using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;


namespace StateMachine.Test
{
	public class Engine
	{
		private StateMachine.Engine _engine;

		[SetUp]
		public void SetUp()
		{
			_engine = new StateMachine.Engine(@"Data\workflow.xml")
			          {
				          CurrentState = "start"
			          };
		}

		[Test]
		public void InitialStateIsStart()
		{
			Assert.That(_engine.CurrentState, Is.EqualTo("start"));
		}

		[Test]
		public void ThereAreThreeCommandsForPendingApprovalStatus()
		{
			var commands = new Dictionary<string, string>
			               {
				               {"changeRequested", "toChangeRequested"},
				               {"approved", "toApproved"},
				               {"denied", "toDenied"}
			               };
			
			CollectionAssert.AreEquivalent(_engine.GetAvailableCommands("pendingApproval"), commands);
		}

		#region Test of state changes

		[Test]
		public void FromStartToNotSubmitted()
		{
			FromStateToState("start", "notSubmitted", "toNotSubmitted");
		}

		[Test]
		public void FromNotSubmittedToSubmitted()
		{
			FromStateToState("notSubmitted", "submitted", "toSubmitted");
		}

		[Test]
		public void FromSubmittedToPendingApproval()
		{
			FromStateToState("submitted", "pendingApproval", "toPendingApproval");
		}

		[Test]
		public void FromPendingApprovalToChangeRequested()
		{
			FromStateToState("pendingApproval", "changeRequested", "toChangeRequested");
		}

		[Test]
		public void FromPendingApprovalToDenied()
		{
			FromStateToState("pendingApproval", "denied", "toDenied");
		}

		[Test]
		public void FromPendingApprovalToApproved()
		{
			FromStateToState("pendingApproval", "approved", "toApproved");
		}

		[Test]
		public void FromChangeRequestedToNotSubmitted()
		{
			FromStateToState("changeRequested", "notSubmitted", "toNotSubmitted");
		}

		[Test]
		public void FromDeniedToCompleted()
		{
			FromStateToState("denied", "completed", "toCompleted");
		}

		[Test]
		public void FromApprovedToCompleted()
		{
			FromStateToState("approved", "completed", "toCompleted");
		}

		[Test]
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

			Assert.That(moved, Is.False);
			Assert.That(_engine.CurrentState, Is.EqualTo(initialState));
		}


		private void FromStateToState(string initialState, string finalState, string inputState)
		{
			_engine.CurrentState = initialState;
			var moved = _engine.Next(inputState);

			Assert.That(moved, Is.True);
			Assert.That(_engine.CurrentState, Is.EqualTo(finalState));
		}
	}
}
