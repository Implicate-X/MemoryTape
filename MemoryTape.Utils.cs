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
		/// <param name="value"> 8-bit value. </param>
		/// <returns>
		/// 8-bit value with reversed bits.
		/// </returns>
		private byte ReverseBitOrder( byte value )
		{
			byte result = 0;

			for( byte bit = 0; bit < 8; bit++ )
			{
				result <<= 1;
				result |= ( byte )( value & 1 );

				value >>= 1;
			}

			return result;
		}

		/// <summary>
		/// Reverse bit order.
		/// </summary>
		/// <param name="value"> 8-bit value. </param>
		/// <returns>
		/// 8-bit value with reversed bits.
		/// </returns>
		private byte ReverseBitOrder2( byte value )
		{
			byte result = 0;

			for( byte bit = 0; bit < 8; bit++ )
				if( ( value & ( 1 << bit ) ) != 0 )
					result |= ( byte )( 1 << ( 7 - bit ) );

			return result;
		}

		/// <summary>
		/// Reverse bit order.
		/// </summary>
		/// <param name="value"> 8-bit value. </param>
		/// <returns>
		/// 8-bit value with reversed bits.
		/// </returns>
		private byte ReverseBitOrder3( byte value )
		{
			byte result = 0;

			for( byte bit = 0; bit < 8; ++bit, value >>= 1 )
				result = ( byte )( ( result << 1 ) | ( value & 1 ) );

			return result;
		}

		private static byte[] bitPatternTable = new byte[]
		{
			0b00000000,	0b00000001,	0b00000010,	0b00000011,
			0b00000100, 0b00000101, 0b00000110, 0b00000111,
			0b00001000,	0b00001001,	0b00001010,	0b00001011,
			0b00001100,	0b00001101,	0b00001110,	0b00001111
		};

		/// <summary>
		/// Reverse bit order.
		/// </summary>
		/// <param name="value"> 8-bit value. </param>
		/// <returns>
		/// 8-bit value with reversed bits.
		/// </returns>
		private byte ReverseBitOrder4( byte value )
		{
			return ( byte )
				( bitPatternTable[ value & 0b1111 ] << 4 | 
				( bitPatternTable[ value >> 4 ] ) );
		}

		/// <summary>
		/// Converts a byte to binary string.
		/// </summary>
		/// <param name="value"> 8-bit value. </param>
		/// <returns>
		/// 8-bit binary string.
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
