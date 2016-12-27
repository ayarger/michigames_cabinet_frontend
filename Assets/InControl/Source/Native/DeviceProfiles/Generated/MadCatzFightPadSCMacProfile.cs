namespace InControl.NativeProfile
{
	using System;


	// @cond nodoc
	public class MadCatzFightPadSCMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzFightPadSCMacProfile()
		{
			Name = "Mad Catz FightPad SC";
			Meta = "Mad Catz FightPad SC on Mac";

			Matchers = new[] {
				new NativeInputDeviceMatcher {
					VendorID = 0x1bad,
					ProductID = 0xf03f,
				},
			};
		}
	}
	// @endcond
}


