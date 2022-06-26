using System;
using System.Collections;
using System.Diagnostics;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Signals;
using GHIElectronics.TinyCLR.Pins;

namespace MemoryTape
{
    internal partial class MemoryTape
    {
		/// <summary>
		/// The tape write pin.<br/>PB1 (J4.40 - Buzzer)
		/// </summary>
		protected GpioPin tapeWritePin;

		/// <summary>
		/// The tape write signal.
		/// </summary>
		protected SignalGenerator tapeWriteSignal;

		/// <summary>
		/// The tape data byte.
		/// </summary>
		protected ArrayList tapeDataByte;

		/// <summary>
		/// Begins file write.
		/// </summary>
		public void BeginFileWrite()
		{
			tapeDataByte = new();

			Debug.WriteLine( ByteToBinaryString( ReverseBitOrder4( 0xF0 ) ) );
		}



		/// <summary>
		/// Writes the file header.
		/// </summary>
		public void WriteFileHeader()
		{
			tapeDataByte.AddRange( loBit );     // Start bit

			byte dataByte = 0xBC;               // Test byte

			for( byte i = 0; i < 8; i++ )
			{
				byte flag = ( byte )( ( dataByte & ( byte )Math.Pow( 2, i ) ) >> i );

				tapeDataByte.AddRange( ( flag == 1 ) ? hiBit : loBit );
			}

			tapeDataByte.AddRange( hiBit );     // Stop bit
		}

		/// <summary>
		/// Writes the file content.
		/// </summary>
		public void WriteFileContent()
		{

		}

		/// <summary>
		/// Ends file write.
		/// </summary>
		public void EndFileWrite()
		{
			TimeSpan[] tapeDataBuffer = new TimeSpan[ tapeDataByte.Count ];

			ushort idx = 0;

			foreach( TimeSpan timeSpan in tapeDataByte )
				tapeDataBuffer[ idx++ ] = timeSpan;


			tapeWritePin = GpioController.GetDefault().OpenPin( FEZFeather.GpioPin.PB1 );

			tapeWriteSignal = new SignalGenerator( tapeWritePin )
			{
				DisableInterrupts = false,
				IdleValue = GpioPinValue.Low
			};

			tapeWriteSignal.Write( tapeDataBuffer );
		}
	}
}
