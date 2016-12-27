namespace InControl.NativeProfile
{
	using System;


	// @cond nodoc
	public class MortalKombatXbox360ControllerMacProfile : Xbox360DriverMacProfile
	{
		public MortalKombatXbox360ControllerMacProfile()
		{
			Name = "Mortal Kombat Xbox 360 Controller";
			Meta = "Mortal Kombat Xbox 360 Controller on Mac";

			Matchers = new[] {
				new NativeInputDeviceMatcher {
					VendorID = 0x1bad,
					ProductID = 0xf906,
				},
			};
		}
	}
	// @endcond
}


