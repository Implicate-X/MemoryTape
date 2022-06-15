using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Adc;
using GHIElectronics.TinyCLR.Drivers.Sitronix.ST7735;

namespace MemoryTape
{
	/// <summary>
	/// An Analog/Digital Converter.
	/// </summary>
	internal class ADC
	{
		protected AdcController adcController;
		protected AdcChannel adcChannel;

		/// <summary>
		/// Initializes this object.
		/// </summary>
		public void Initialize()
		{
			adcController = AdcController.FromName( FEZFeather.Adc.Controller1.Id );
			adcChannel = adcController.OpenChannel( FEZFeather.Adc.Controller1.PA0 );

			while( true )
			{
				double d = adcChannel.ReadRatio() * 3;
				Debug.WriteLine( "An-> " + d.ToString( "N2" ) + " " + adcChannel.ReadValue() / 256 );
				Thread.Sleep( 100 );
			}
		}
	}
}
