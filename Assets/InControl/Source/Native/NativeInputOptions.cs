using System;
using System.Runtime.InteropServices;


namespace InControl
{
	[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Ansi )]
	public struct NativeInputOptions
	{
		public UInt16 updateRate;
		public Boolean enableXInput;
		public Boolean preventSleep;
	}
}

