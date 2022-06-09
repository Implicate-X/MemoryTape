using System;
using System.Collections;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Pins;

namespace MemoryTape
{
	/// <summary>
	/// The Memory Tape board.
	/// </summary>
	internal partial class MemoryTape
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
		/// The pulse sequence for bit value 0.
		/// </summary>
		protected ArrayList loBit;

		/// <summary>
		/// The pulse sequence for bit value 1.
		/// </summary>
		protected ArrayList hiBit;

		public void Initialize()
		{
			//ledRedPin = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PB13 );
			//ledGrnPin = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PB14 );
			//ledBluPin = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PB15 );

			pinStartButton = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PA4 );

			//ledRedPin.SetDriveMode( GpioPinDriveMode.Output );
			//ledGrnPin.SetDriveMode( GpioPinDriveMode.Output );
			//ledBluPin.SetDriveMode( GpioPinDriveMode.Output );

			pinStartButton.SetDriveMode( GpioPinDriveMode.InputPullUp );

			//ledRedPin.Write( GpioPinValue.Low );
			//ledGrnPin.Write( GpioPinValue.Low );
			//ledBluPin.Write( GpioPinValue.Low );

			pinStartButton.ValueChanged += StartButton_ValueChanged;


			double[] time = { 1 / higherTone, 1 / lowerTone };

			ushort[] loSig = { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 };
			ushort[] hiSig = { 0, 0, 0, 0, 1, 1, 1, 1 };

			loBit = new();
			hiBit = new();

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
