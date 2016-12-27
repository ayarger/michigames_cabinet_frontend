namespace InControl.NativeProfile
{
	using System;


	// @cond nodoc
	public class MadCatzFightPadNeoMacProfile : Xbox360DriverMacProfile
	{
		public MadCatzFightPadNeoMacProfile()
		{
			Name = "Mad Catz FightPad Neo";
			Meta = "Mad Catz FightPad Neo on Mac";

			Matchers = new[] {
				new NativeInputDeviceMatcher {
					VendorID = 0x1bad,
					ProductID = 0xf03a,
				},
			};
		}
	}
	// @endcond
}


