using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Threading;
using GHIElectronics.TinyCLR.Pins;
using GHIElectronics.TinyCLR.Devices.Gpio;
using GHIElectronics.TinyCLR.Devices.Signals;

namespace MemoryTape
{
	/// <summary>
	/// The program.
	/// </summary>
	internal class Program
	{
		/// <summary>
		/// Mains the.
		/// </summary>
		static void Main()
		{

			FEZduinoBoard board = new();

			board.Initialize();
		}
	}
}
