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
		/// Display data / Command selection pin, DC=1: Display data, DC=0: Command data.<br/>PA13
		/// </summary>
		private GpioPin dataCmdPin;

		/// <summary>
		/// Reset Pin. Initialize the chip with a low input.<br/>PA14
		/// </summary>
		private GpioPin dispResPin;

		/// <summary>
		/// Chip select input pin (0: Enable).<br/>PA15
		/// </summary>
		private GpioPin chipSelPin;

		/// <summary>
		/// Gets the start button.
		/// </summary>
		private GpioPin pinStartButton;

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
			gpioController = GpioController.GetDefault();

			//dataCmdPin = gpioController.OpenPin( SC13048.GpioPin.PA13 );
			//dispResPin = gpioController.OpenPin( SC13048.GpioPin.PA14 );
			//chipSelPin = gpioController.OpenPin( SC13048.GpioPin.PA15 );

			//dataCmdPin.SetDriveMode( GpioPinDriveMode.Output );
			//dispResPin.SetDriveMode( GpioPinDriveMode.Output );
			//chipSelPin.SetDriveMode( GpioPinDriveMode.Output );

			//spiController = SpiController.FromName( SC13048.SpiBus.Spi1 );

			//SpiConnectionSettings displayConnectionSettings =
			//	ST7735Controller.GetConnectionSettings( SpiChipSelectType.Gpio, chipSelPin );

			//SpiDevice displayDevice = spiController.GetDevice( displayConnectionSettings );

			//displayController = new ST7735Controller( displayDevice, dataCmdPin, dispResPin );

			//displayController = new ST7735Controller(
			//	spiController.GetDevice( ST7735Controller.GetConnectionSettings
			//	( SpiChipSelectType.Gpio, gpioController.OpenPin( SC13048.GpioPin.PA15 ) ) ), //CS pin.
			//	gpioController.OpenPin( SC13048.GpioPin.PA13 ), //RS pin.
			//	gpioController.OpenPin( SC13048.GpioPin.PA14 ) //RESET pin.
			//);

			//displayController.SetDataAccessControl( true, true, false, false ); //Rotate the screen.
			//displayController.SetDrawWindow( 0, 0, SCREEN_WIDTH - 1, SCREEN_HEIGHT - 1 );
			//displayController.Enable();

			//Graphics.OnFlushEvent += Graphics_OnFlushEvent;

			SolidBrush whiteBrush = new SolidBrush( Color.White );
			SolidBrush greenBrush = new SolidBrush( Color.Green );
			SolidBrush blackBrush = new SolidBrush( Color.Black );
			Bitmap bm = new Bitmap( SCREEN_WIDTH, SCREEN_HEIGHT );

			screen = Graphics.FromImage( bm );

			screen.Clear();

			screen.FillRectangle( greenBrush, 0, 0, SCREEN_WIDTH - 1, SCREEN_HEIGHT - 1 );
			screen.Flush();

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

			ledRedPin.Write( ( ledRedPin.Read() == GpioPinValue.Low ) ? GpioPinValue.High : GpioPinValue.Low );
			ledGrnPin.Write( ( ledGrnPin.Read() == GpioPinValue.Low ) ? GpioPinValue.High : GpioPinValue.Low );
		}
	}
}
