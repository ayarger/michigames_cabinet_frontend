using System;
using System.Runtime.InteropServices;
using UnityEngine;


namespace InControl
{
	using DeviceHandle = UInt32;

	// @cond nodoc
	internal static class Native
	{
		const string LibraryName = "InControlNative";

		public static ThreadSafeQueue<DeviceHandle> AttachedDeviceQueue = new ThreadSafeQueue<DeviceHandle>();
		public static ThreadSafeQueue<DeviceHandle> DetachedDeviceQueue = new ThreadSafeQueue<DeviceHandle>();


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void DeviceAttachedCallback( UInt32 handle );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void DeviceDetachedCallback( UInt32 handle );


		[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
		public delegate void DebugPrintCallback( string text );


		[DllImport( LibraryName, EntryPoint = "InControl_SetDebugPrintFunc" )]
		static extern void SetDebugPrintFunc( DebugPrintCallback debugPrintFunc );


		[DllImport( LibraryName, EntryPoint = "InControl_Init" )]
		static extern void Init( DeviceAttachedCallback deviceAttached, DeviceDetachedCallback deviceDetached, NativeInputOptions options );
		public static void Init( NativeInputOptions options )
		{
			SetDebugPrintFunc( OnDebugPrint );
			Init( OnDeviceAttached, OnDeviceDetached, options );
		}


		[DllImport( LibraryName, EntryPoint = "InControl_Stop" )]
		public static extern void Stop();


		[DllImport( LibraryName, EntryPoint = "InControl_GetVersionInfo" )]
		public static extern void GetVersionInfo( out NativeVersionInfo versionInfo );


		[DllImport( LibraryName, EntryPoint = "InControl_GetDeviceInfo" )]
		public static extern bool GetDeviceInfo( UInt32 handle, out NativeDeviceInfo deviceInfo );


		[DllImport( LibraryName, EntryPoint = "InControl_GetDeviceState" )]
		public static extern bool GetDeviceState( UInt32 handle, out IntPtr deviceState );


		[DllImport( LibraryName, EntryPoint = "InControl_SetHapticState" )]
		public static extern void SetHapticState( UInt32 handle, Byte motor0, Byte motor1 );


		[DllImport( LibraryName, EntryPoint = "InControl_SetLightColor" )]
		public static extern void SetLightColor( UInt32 handle, Byte red, Byte green, Byte blue );


		[DllImport( LibraryName, EntryPoint = "InControl_SetLightFlash" )]
		public static extern void SetLightFlash( UInt32 handle, Byte flashOnDuration, Byte flashOffDuration );


		static void OnDeviceAttached( DeviceHandle handle )
		{
			AttachedDeviceQueue.Enqueue( handle );
		}


		static void OnDeviceDetached( DeviceHandle handle )
		{
			DetachedDeviceQueue.Enqueue( handle );
		}


		static void OnDebugPrint( string text )
		{
			Debug.Log( "[InControl.Native] " + text );
		}
	}
	//@endcond
}

