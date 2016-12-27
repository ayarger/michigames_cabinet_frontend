using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;


namespace InControl
{
	public class PlayerTwoAxisAction : TwoAxisInputControl
	{
		PlayerAction negativeXAction;
		PlayerAction positiveXAction;
		PlayerAction negativeYAction;
		PlayerAction positiveYAction;

		/// <summary>
		/// Gets or sets a value indicating whether the X axis should be inverted for
		/// this action. When false (default), the X axis will be positive up,
		/// the same as Unity.
		/// </summary>
		public bool InvertXAxis { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether the Y axis should be inverted for
		/// this action. When false (default), the Y axis will be positive up,
		/// the same as Unity.
		/// </summary>
		public bool InvertYAxis { get; set; }

		/// <summary>
		/// The binding source type that provided input to this action.
		/// </summary>
		public BindingSourceType LastInputType = BindingSourceType.None;


		internal PlayerTwoAxisAction( PlayerAction negativeXAction, PlayerAction positiveXAction, PlayerAction negativeYAction, PlayerAction positiveYAction )
		{
			this.negativeXAction = negativeXAction;
			this.positiveXAction = positiveXAction;
			this.negativeYAction = negativeYAction;
			this.positiveYAction = positiveYAction;

			InvertXAxis = false;
			InvertYAxis = false;
			Raw = true;
		}


		internal void Update( ulong updateTick, float deltaTime )
		{
			ProcessActionUpdate( negativeXAction );
			ProcessActionUpdate( positiveXAction );
			ProcessActionUpdate( negativeYAction );
			ProcessActionUpdate( positiveYAction );

			var x = Utility.ValueFromSides( negativeXAction, positiveXAction, InvertXAxis );
			var y = Utility.ValueFromSides( negativeYAction, positiveYAction, InputManager.InvertYAxis || InvertYAxis );
			UpdateWithAxes( x, y, updateTick, deltaTime );
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
