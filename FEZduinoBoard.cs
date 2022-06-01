using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Signals;
using GHIElectronics.TinyCLR.Pins;

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
		protected GpioPin ledGrnPin;

		/// <summary>
		/// The red LED pin.
		/// </summary>
		protected GpioPin ledRedPin;

		/// <summary>
		/// The blue LED pin.
		/// </summary>
		protected GpioPin ledBluPin;

		/// <summary>
		/// Gets the start button.
		/// </summary>
		protected GpioPin pinStartButton;

		/// <summary>
		/// The lower Tone in Hz.
		/// </summary>
		protected const double lowerTone = 2000.0;

		/// <summary>
		/// The higher tone in Hz.
		/// </summary>
		protected const double higherTone = 4000.0;

		/// <summary>
		/// The tape write pin.
		/// </summary>
		protected GpioPin tapeWritePin;

		/// <summary>
		/// The tape write signal.
		/// </summary>
		protected SignalGenerator tapeWriteSignal;

		public void Initialize()
		{
			ledRedPin = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PE8 );
			ledGrnPin = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PE9 );
			ledBluPin = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PE10 );

			pinStartButton = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PD9 );

			ledRedPin.SetDriveMode( GpioPinDriveMode.Output );
			ledGrnPin.SetDriveMode( GpioPinDriveMode.Output );
			ledBluPin.SetDriveMode( GpioPinDriveMode.Output );

			pinStartButton.SetDriveMode( GpioPinDriveMode.InputPullUp );

			ledRedPin.Write( GpioPinValue.High );
			ledGrnPin.Write( GpioPinValue.Low );
			ledBluPin.Write( GpioPinValue.Low );

			pinStartButton.ValueChanged += StartButton_ValueChanged;


			double[] time = { 1 / higherTone, 1 / lowerTone };

			ushort[] loSig = { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 };
			ushort[] hiSig = { 0, 0, 0, 0, 1, 1, 1, 1 };

			ArrayList loBit = new();
			ArrayList hiBit = new();

			foreach( ushort sig in loSig )
			{
				loBit.Add( TimeSpan.FromSeconds( time[ sig ] ) );
				loBit.Add( TimeSpan.FromSeconds( time[ sig ] ) );
			}

			foreach( ushort sig in hiSig )
			{
				hiBit.Add( TimeSpan.FromSeconds( time[ sig ] ) );
				hiBit.Add( TimeSpan.FromSeconds( time[ sig ] ) );
			}

			ArrayList tapeDataByte = new();

			tapeDataByte.AddRange( loBit );		// Start bit

			byte dataByte = 0xAA;				// Test byte

			for( byte i = 0; i < 8; i++ )
			{
				byte flag = ( byte )( ( dataByte & ( byte )Math.Pow( 2, i ) ) >> i );

				tapeDataByte.AddRange( ( flag == 1 ) ? hiBit : loBit );
			}

			tapeDataByte.AddRange( hiBit );		// Stop bit


			TimeSpan[] bufferFailed = new TimeSpan[ tapeDataByte.Count ];


			/// FAILED with SignalGenerator.Write
			/// 

			tapeDataByte.CopyTo( bufferFailed );


			TimeSpan[] bufferSucceeded = new TimeSpan[ tapeDataByte.Count ];

			/// SUCCEEDED with SignalGenerator.Write
			///
			ushort idx = 0;

			foreach( TimeSpan timeSpan in tapeDataByte )
				bufferSucceeded[ idx++ ] = timeSpan;


			tapeWritePin = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PD0 );

			tapeWriteSignal = new SignalGenerator( tapeWritePin )
			{
				DisableInterrupts = false,
				IdleValue = GpioPinValue.Low
			};

			tapeWriteSignal.Write( bufferSucceeded );

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

			ledRedPin.Write( ( ledRedPin.Read() == GpioPinValue.Low ) ? GpioPinValue.High : GpioPinValue.Low );
			ledGrnPin.Write( ( ledGrnPin.Read() == GpioPinValue.Low ) ? GpioPinValue.High : GpioPinValue.Low );
		}
	}
}
