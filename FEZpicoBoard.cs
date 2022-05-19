using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Signals;
using GHIElectronics.TinyCLR.IO.TinyFileSystem;
using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.Pins;

namespace MemoryTape
{
	/// <summary>
	/// The FEZpico board.
	/// </summary>
	internal class FEZpicoBoard
	{
		/// <summary>
		/// The green LED pin.
		/// </summary>
		protected GpioPin pinLedGrn;

		/// <summary>
		/// The red LED pin.
		/// </summary>
		protected GpioPin pinLedRed;

		/// <summary>
		/// The blue LED pin.
		/// </summary>
		protected GpioPin pinLedBlu;

		/// <summary>
		/// Gets the start button.
		/// </summary>
		protected GpioPin pinStartButton;

		/// <summary>
		/// Gets the tape read signal.
		/// </summary>
		protected GpioPin pinTapeRead;

		protected SignalCapture tapeReadSignal;

		/// <summary>
		/// Initializes the FEZpico board.
		/// </summary>
		public void Initialize()
		{
			//const int CLUSTER_SIZE = 1024;

			//DeviceInformation.SetDebugInterface(DebugInterface.Disable);

			try
			{
				//var tfs = new TinyFileSystem( new QspiMemory(), CLUSTER_SIZE );
			}
			catch( Exception ex )
			{
				Debug.WriteLine( ex.Message );
			}

			//if( !tfs.CheckIfFormatted() )
			//{
			//	//Do Format if necessary 
			//	tfs.Format();
			//}
			//else
			//{
			//	// Mount tiny file system
			//	tfs.Mount();
			//}
			pinLedRed = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PB5 );
			pinLedGrn = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PB4 );
			pinLedBlu = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PB3 );

			pinStartButton = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PH1 );

			pinLedRed.SetDriveMode( GpioPinDriveMode.Output );
			pinLedGrn.SetDriveMode( GpioPinDriveMode.Output );
			pinLedBlu.SetDriveMode( GpioPinDriveMode.Output );

			pinStartButton.SetDriveMode( GpioPinDriveMode.InputPullUp );

			pinLedRed.Write( GpioPinValue.Low );
			pinLedGrn.Write( GpioPinValue.High );
			pinLedBlu.Write( GpioPinValue.High );

			pinStartButton.ValueChanged += StartButton_ValueChanged;

			pinTapeRead = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PA1 );

			tapeReadSignal = new SignalCapture( pinTapeRead );
			tapeReadSignal.DisableInterrupts = false;

			// 0 1000 0000 1
			// 0 0000 0000 1
			// 0 0000 0000 1
			// 0 0010 0100 1
			// 0 1000 0000 1
			// 0 0010 0100 1
			// 0 0011 0100 1
			// 0 0001 0000 1
			// 0 0010 0100 1

			//bool waitForEdge = false;
			//while( !tapeReadSignal.CanReadPulse );
			//Debug.WriteLine( "CanReadPulse" );

			//try
			//{
			//	while( true )
			//	{
			//		if( tapeReadSignal.CanReadPulse )
			//		{
			//			tapeReadSignal.ReadPulse( 2, GpioPinEdge.RisingEdge, waitForEdge );
			//		}
			//	}
			//}
			//catch( Exception ex )
			//{
			//	Debug.WriteLine( ex.Message );
			//}
			Thread.Sleep( Timeout.Infinite );
		}

		/// <summary>
		/// Starts the button_ value changed.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		private void StartButton_ValueChanged( GpioPin sender, GpioPinValueChangedEventArgs e )
		{
			if( e.Edge == GpioPinEdge.FallingEdge )
			{
				return;
			}

			//tapeReadSignal.Capture( 1000, GpioPinEdge.RisingEdge, false );

			pinLedRed.Write( ( pinLedRed.Read() == GpioPinValue.Low ) ? GpioPinValue.High : GpioPinValue.Low );
			pinLedGrn.Write( ( pinLedGrn.Read() == GpioPinValue.Low ) ? GpioPinValue.High : GpioPinValue.Low );
		}
	}
}
