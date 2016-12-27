using System;
using UnityEngine;


namespace InControl
{
	public class PlayerOneAxisAction : OneAxisInputControl
	{
		PlayerAction negativeAction;
		PlayerAction positiveAction;

		/// <summary>
		/// The binding source type that provided input to this action.
		/// </summary>
		public BindingSourceType LastInputType = BindingSourceType.None;


		internal PlayerOneAxisAction( PlayerAction negativeAction, PlayerAction positiveAction )
		{
			this.negativeAction = negativeAction;
			this.positiveAction = positiveAction;
			Raw = true;
		}


		internal void Update( ulong updateTick, float deltaTime )
		{
			ProcessActionUpdate( negativeAction );
			ProcessActionUpdate( positiveAction );

			var value = Utility.ValueFromSides( negativeAction, positiveAction );
			CommitWithValue( value, updateTick, deltaTime );
		}


		void ProcessActionUpdate( PlayerAction action )
		{
			if (action.UpdateTick > UpdateTick)
			{
				UpdateTick = action.UpdateTick;
				LastInputType = action.LastInputType;
			}
		}
	}
}

