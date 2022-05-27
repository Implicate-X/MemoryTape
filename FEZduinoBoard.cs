using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Linq;
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
		/// The tape read pin.
		/// </summary>
		protected GpioPin tapeReadPin;

		/// <summary>
		/// The tape write pin.
		/// </summary>
		protected GpioPin tapeWritePin;

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

			tapeReadPin = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PD1 );
			tapeReadPin.SetDriveMode( GpioPinDriveMode.Input );
			// pinTapeRead.ValueChanged += PinTapeRead_ValueChanged;

			tapeWritePin = GpioController.GetDefault().OpenPin( FEZDuino.GpioPin.PD0 );

			tapeWriteSignal = new SignalGenerator( tapeWritePin );
			tapeWriteSignal.DisableInterrupts = false;
			tapeWriteSignal.IdleValue = GpioPinValue.Low;

			double[] freq = { 4000, 2000 };
			double[] time = { 1 / freq[ 0 ], 1 / freq[ 1 ] };

			ushort[] loSig = { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1 };
			ushort[] hiSig = { 0, 0, 0, 0, 1, 1, 1, 1 };

			TimeSpan[] loBit = new TimeSpan[ 20 ];
			TimeSpan[] hiBit = new TimeSpan[ 16 ];

			ushort idx = 0;

			foreach( ushort sig in loSig )
			{
				loBit[ idx++ ] = TimeSpan.FromSeconds( time[ sig ] );
				loBit[ idx++ ] = TimeSpan.FromSeconds( time[ sig ] );
			}

			idx = 0;

			foreach( ushort sig in hiSig )
			{
				hiBit[ idx++ ] = TimeSpan.FromSeconds( time[ sig ] );
				hiBit[ idx++ ] = TimeSpan.FromSeconds( time[ sig ] );
			}


			while( true )
			{
				tapeWriteSignal.Write( buffer: loBit );
				tapeWriteSignal.Write( buffer: hiBit );

				//Thread.Sleep( 5000 );
			}

			//Thread.Sleep( Timeout.Infinite );
		}

		/// <summary>
		/// Pin tape read value changed.
		/// </summary>
		/// <param name="sender">	The sender. </param>
		/// <param name="e">	 	Gpio pin value changed event information. </param>
		private void PinTapeRead_ValueChanged( GpioPin sender, GpioPinValueChangedEventArgs e )
		{
			if( e.Edge == GpioPinEdge.RisingEdge && isStart )
			{
				isStart = false;

				tapeReadSignal = new SignalCapture( tapeReadPin );
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

			ledRedPin.Write( ( ledRedPin.Read() == GpioPinValue.Low ) ? GpioPinValue.High : GpioPinValue.Low );
			ledGrnPin.Write( ( ledGrnPin.Read() == GpioPinValue.Low ) ? GpioPinValue.High : GpioPinValue.Low );
		}
	}
}
