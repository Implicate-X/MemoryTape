using System;
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Spi;
using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Drivers.Sitronix.ST7735;

namespace MemoryTape
{
	/// <summary>
	/// The Memory Tape board.
	/// </summary>
	internal partial class MemoryTape
	{
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

		private GpioController gpioController;
		private SpiController spiController;
		private ST7735Controller displayController;

		private const int SCREEN_WIDTH = 128;
		private const int SCREEN_HEIGHT = 128;
		private Graphics screen;

		public void Initialize()
		{
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

			gpioController = GpioController.GetDefault();

			dispCmdPin = gpioController.OpenPin( FEZFeather.GpioPin.PD6 );
			dispResPin = gpioController.OpenPin( FEZFeather.GpioPin.PD5 );
			dispSelPin = gpioController.OpenPin( FEZFeather.GpioPin.PD4 );

			dispCmdPin.SetDriveMode( GpioPinDriveMode.Output );
			dispResPin.SetDriveMode( GpioPinDriveMode.Output );
			dispSelPin.SetDriveMode( GpioPinDriveMode.Output );

			spiController = SpiController.FromName( FEZFeather.SpiBus.Spi4 );

			SpiConnectionSettings displayConnectionSettings =
				ST7735Controller.GetConnectionSettings( SpiChipSelectType.Gpio, dispSelPin );

			SpiDevice displayDevice = spiController.GetDevice( displayConnectionSettings );

			displayController = new ST7735Controller( displayDevice, dispCmdPin, dispResPin );

			displayController.SetDataAccessControl( false, false, false, true );
			displayController.SetDrawWindow( 0, 0, SCREEN_WIDTH - 1, SCREEN_HEIGHT - 1 );
			displayController.Enable();

			Graphics.OnFlushEvent += Graphics_OnFlushEvent;

			SolidBrush whiteBrush = new SolidBrush( Color.White );
			SolidBrush greenBrush = new SolidBrush( Color.Blue );
			SolidBrush blackBrush = new SolidBrush( Color.Black );

			screen = Graphics.FromImage( new Bitmap( SCREEN_WIDTH, SCREEN_HEIGHT ) );

			screen.Clear();

			screen.FillRectangle( greenBrush, 0, 0, SCREEN_WIDTH - 1, SCREEN_HEIGHT - 1 );
			screen.Flush();

			//ledRedPin = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PB13 );
			//ledGrnPin = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PB14 );
			//ledBluPin = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PB15 );

			startButtonPin = GpioController.GetDefault().OpenPin( SC13048.GpioPin.PA4 );

			//ledRedPin.SetDriveMode( GpioPinDriveMode.Output );
			//ledGrnPin.SetDriveMode( GpioPinDriveMode.Output );
			//ledBluPin.SetDriveMode( GpioPinDriveMode.Output );

			startButtonPin.SetDriveMode( GpioPinDriveMode.InputPullUp );

			//ledRedPin.Write( GpioPinValue.Low );
			//ledGrnPin.Write( GpioPinValue.Low );
			//ledBluPin.Write( GpioPinValue.Low );

			startButtonPin.ValueChanged += StartButton_ValueChanged;


		}

		private void Graphics_OnFlushEvent( Graphics sender, byte[] data, int x, int y, int width, int height, int originalWidth )
		{
			displayController.DrawBuffer( data );
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

			//ledRedPin.Write( ( ledRedPin.Read() == GpioPinValue.Low ) ? GpioPinValue.High : GpioPinValue.Low );
			//ledGrnPin.Write( ( ledGrnPin.Read() == GpioPinValue.Low ) ? GpioPinValue.High : GpioPinValue.Low );
		}
	}
}
