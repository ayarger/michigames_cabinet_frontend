namespace InControl.NativeProfile
{
	using System;


	// @cond nodoc
	public class MadCatzSF4FightStickTEMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzSF4FightStickTEMacProfile()
		{
			Name = "Mad Catz SF4 FightStick TE";
			Meta = "Mad Catz SF4 FightStick TE on Mac";

			Matchers = new[] {
				new NativeInputDeviceMatcher {
					VendorID = 0x0738,
					ProductID = 0x4738,
				},
			};
		}
	}
	// @endcond
}


