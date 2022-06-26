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
	/// Pinout of FEZ Feather (Device Name: FEZfeather2).
	/// 
	/// @@img:"D:\Code\IoT\MemoryTape\Docs\fez-feather-pinout.png"
	/// 
	/// </summary>
	internal partial class MemoryTape
	{
		/// <summary>
		/// The green LED pin.<br/>PA6 (J4.38 - Green channel)
		/// </summary>
		private GpioPin ledGrnPin;

		/// <summary>
		/// The red LED pin.<br/>PA3 (J4.39 - Red channel)
		/// </summary>
		private GpioPin ledRedPin;

		/// <summary>
		/// The blue LED pin.<br/>PA7 (J4.37 - Blue channel)
		/// </summary>
		private GpioPin ledBluPin;

		/// <summary>
		/// Display data / Command selection pin, DC=1: Display data, DC=0: Command data.<br/>PD6 (J4.31 - LCD, DC)
		/// </summary>
		private GpioPin dispCmdPin;

		/// <summary>
		/// Reset Pin. Initialize the chip with a low input.<br/>PD5 (J2.17 - LCD, RESET)
		/// </summary>
		private GpioPin dispResPin;

		/// <summary>
		/// Chip select input pin (0: Enable).<br/>PD4 (J2.13 - LCD, SPI CS)
		/// </summary>
		private GpioPin dispSelPin;

		/// <summary>
		/// The tape write pin.<br/>PB1 (J4.40 - Buzzer)
		/// </summary>
		private GpioPin tapeWritePin;

		/// <summary>
		/// The tape write pin.<br/>PA0
		/// </summary>
		private GpioPin tapeReadPin;

		/// <summary>
		/// The start button pin.<br/>PE1 (J4.32 - Button2)
		/// </summary>
		private GpioPin startButtonPin;

	}
}
