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
		/// Reverse bit order.
		/// </summary>
		/// <param name="value">	The value. </param>
		/// <returns>
		/// A byte.
		/// </returns>
		private byte ReverseBitOrder( byte value )
		{
			byte result = 0;

			for( byte idx = 0; idx < 8; idx++ )
				if( ( value & ( 1 << idx ) ) != 0 )
					result |= ( byte )( 1 << ( 7 - idx ) );

			return result;
		}

		/// <summary>
		/// Byte to binary string.
		/// </summary>
		/// <param name="value">	The value. </param>
		/// <returns>
		/// A string.
		/// </returns>
		private string ByteToBinaryString( byte value )
		{
			byte mask = 1;
			string result = string.Empty;

			for( byte idx = 0; idx < 8; idx++ )
			{
				result = ( value & mask ) + result;
				value = ( byte )( value >> 1 );
			}

			return result;
		}

	}
}
