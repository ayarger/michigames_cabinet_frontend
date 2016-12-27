namespace InControl.NativeProfile
{
	using System;


	// @cond nodoc
	public class MadCatzFightPadTE2MacProfile : Xbox360DriverMacProfile
	{
		public MadCatzFightPadTE2MacProfile()
		{
			Name = "Mad Catz FightPad TE2";
			Meta = "Mad Catz FightPad TE2 on Mac";

			Matchers = new[] {
				new NativeInputDeviceMatcher {
					VendorID = 0x1bad,
					ProductID = 0xf080,
				},
			};
		}
	}
	// @endcond
}


