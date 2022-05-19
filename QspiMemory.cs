using System;
using GHIElectronics.TinyCLR.Devices.Storage;
using GHIElectronics.TinyCLR.Devices.Storage.Provider;
using GHIElectronics.TinyCLR.Native;
using GHIElectronics.TinyCLR.Pins;

namespace MemoryTape
{
	/// <summary>
	/// The qspi memory.
	/// </summary>
	public sealed class QspiMemory : IStorageControllerProvider
	{
		/// <summary>
		/// Gets the descriptor.
		/// </summary>
		public StorageDescriptor Descriptor => this.descriptor;
		/// <summary>
		/// The sector size.
		/// </summary>
		const int SectorSize = 4 * 1024;

		private readonly StorageDescriptor descriptor = new()
		{
			CanReadDirect = false,
			CanWriteDirect = false,
			CanExecuteDirect = false,
			EraseBeforeWrite = true,
			Removable = true,
			RegionsContiguous = true,
			RegionsEqualSized = true,
			RegionAddresses = new long[] { 0 },
			RegionSizes = new int[] { SectorSize },
			RegionCount = ( 2 * 1024 * 1024 ) / ( SectorSize )
		};

		private IStorageControllerProvider qspiDrive;

		/// <summary>
		/// Initializes a new instance of the <see cref="QspiMemory"/> class.
		/// </summary>
		public QspiMemory() : this( 2 * 1024 * 1024 )
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="QspiMemory"/> class.
		/// </summary>
		/// <param name="size">The size.</param>
		public QspiMemory( uint size )
		{
			var maxSize = Flash.IsEnabledExtendDeployment ? ( 10 * 1024 * 1024 ) : ( 16 * 1024 * 1024 );

			if( size > maxSize )
				throw new ArgumentOutOfRangeException( "size too large." );

			if( size <= SectorSize )
				throw new ArgumentOutOfRangeException( "size too small." );

			if( size != descriptor.RegionCount * SectorSize )
			{
				descriptor.RegionCount = ( int )( size / SectorSize );
			}

			qspiDrive = StorageController.FromName( SC13048.StorageController.QuadSpi ).Provider;

			this.Open();
		}

		/// <summary>
		/// Opens the.
		/// </summary>
		public void Open()
		{
			qspiDrive.Open();
		}

		/// <summary>
		/// Closes the.
		/// </summary>
		public void Close()
		{
			qspiDrive.Close();
		}

		/// <summary>
		/// Disposes the.
		/// </summary>
		public void Dispose()
		{
			qspiDrive.Dispose();
		}

		/// <summary>
		/// Erases the.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="count">The count.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns>An int.</returns>
		public int Erase( long address, int count, TimeSpan timeout )
		{
			return qspiDrive.Erase( address, count, timeout );
		}

		/// <summary>
		/// Are the erased.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="count">The count.</param>
		/// <returns>A bool.</returns>
		public bool IsErased( long address, int count )
		{
			return qspiDrive.IsErased( address, count );
		}

		/// <summary>
		/// Reads the.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="count">The count.</param>
		/// <param name="buffer">The buffer.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns>An int.</returns>
		public int Read( long address, int count, byte[] buffer, int offset, TimeSpan timeout )
		{
			return qspiDrive.Read( address, count, buffer, offset, timeout );
		}

		/// <summary>
		/// Writes the.
		/// </summary>
		/// <param name="address">The address.</param>
		/// <param name="count">The count.</param>
		/// <param name="buffer">The buffer.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="timeout">The timeout.</param>
		/// <returns>An int.</returns>
		public int Write( long address, int count, byte[] buffer, int offset, TimeSpan timeout )
		{
			return qspiDrive.Write( address, count, buffer, offset, timeout );
		}

		/// <summary>
		/// Erases the all.
		/// </summary>
		/// <param name="timeout">The timeout.</param>
		public void EraseAll( TimeSpan timeout )
		{
			for( var sector = 0; sector < this.Descriptor.RegionCount; sector++ )
			{
				qspiDrive.Erase( sector * this.Descriptor.RegionSizes[ 0 ], this.Descriptor.RegionSizes[ 0 ], timeout );
			}
		}
	}
}
