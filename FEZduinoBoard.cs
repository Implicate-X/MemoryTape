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
using GHIElectronics.TinyCLR.Devices.Storage;
using GHIElectronics.TinyCLR.Devices.Storage.Provider;

namespace MemoryTape
{
	/// <summary>
	/// The FEZpico board.
	/// </summary>
	internal class FEZduinoBoard
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
		/// The tape read pin.
		/// </summary>
		protected GpioPin pinTapeRead;

		/// <summary>
		/// The tape write pin.
		/// </summary>
		protected GpioPin pinTapeWrite;

		/// <summary>
		/// The tape read signal.
		/// </summary>
		protected SignalCapture tapeReadSignal;

		/// <summary>
		/// The tape write signal.
		/// </summary>
		protected SignalGenerator tapeWriteSignal;

		protected TimeSpan[] timeSpan = new TimeSpan[ 10 ];
		protected bool isStart = true;
		protected int count = 0;


		public void Initialize()
		{
			StorageController qspiDrive = StorageController.FromName( FEZDuino.StorageController.QuadSpi );


			pinLedRed = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PE8 );
			pinLedGrn = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PE9 );
			pinLedBlu = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PE10 );

			pinStartButton = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PD9 );

			pinLedRed.SetDriveMode( GpioPinDriveMode.Output );
			pinLedGrn.SetDriveMode( GpioPinDriveMode.Output );
			pinLedBlu.SetDriveMode( GpioPinDriveMode.Output );

			pinStartButton.SetDriveMode( GpioPinDriveMode.InputPullUp );

			pinLedRed.Write( GpioPinValue.High );
			pinLedGrn.Write( GpioPinValue.Low );
			pinLedBlu.Write( GpioPinValue.Low );

			pinStartButton.ValueChanged += StartButton_ValueChanged;

			pinTapeRead = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PD1 );
			pinTapeRead.SetDriveMode( GpioPinDriveMode.Input );
			pinTapeRead.ValueChanged += PinTapeRead_ValueChanged;

			pinTapeWrite = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PD0 );

			double[] freq = { 2000, 1000 };
			double[] time = { 1/freq[0], 1/freq[1] };

			double[] Bit0 = 
			{
				time[0],
				time[0],
				time[0],
				time[0],
				time[0],
				time[0],
				time[0],
				time[0],
				time[1],
				time[1]
			};

			double[] Bit1 =
			{
				time[0],
				time[0],
				time[0],
				time[0],
				time[1],
				time[1],
				time[1],
				time[1]
			};

			//while( count < 10 ) ;

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
			//Thread.Sleep( Timeout.Infinite );
		}

		#region Documentation
		/// <summary>
		/// Pin tape read value changed.
		/// </summary>
		/// <param name="sender">	The sender. </param>
		/// <param name="e">	 	Gpio pin value changed event information. </param>
		#endregion
		private void PinTapeRead_ValueChanged( GpioPin sender, GpioPinValueChangedEventArgs e )
		{
			if( e.Edge == GpioPinEdge.RisingEdge && isStart )
			{
				isStart = false;

				tapeReadSignal = new SignalCapture( pinTapeRead );
				tapeReadSignal.DisableInterrupts = false;
				tapeReadSignal.Timeout = TimeSpan.FromSeconds( 1 );
				count += tapeReadSignal.Read( out var init, timeSpan );

			}
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
