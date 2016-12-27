namespace InControl.NativeProfile
{
	using System;


	// @cond nodoc
	public class MadCatzSSF4FightStickTEMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzSSF4FightStickTEMacProfile()
		{
			Name = "Mad Catz SSF4 FightStick TE";
			Meta = "Mad Catz SSF4 FightStick TE on Mac";

			Matchers = new[] {
				new NativeInputDeviceMatcher {
					VendorID = 0x0738,
					ProductID = 0xf738,
				},
			};
		}
	}
	// @endcond
}


