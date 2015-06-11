using System;
using System.Runtime.InteropServices;

namespace PI_Comm_Wrapper
{
	public class PI_I2C : IDisposable
	{
		// DLL Mapping to private local class
		private static class _PI_Comm
		{
			[DllImport("libpi_comm_base.so", EntryPoint = "I2C_Open", SetLastError = true)]
			public static extern int I2C_Open (string DeviceFileName);

			[DllImport("libpi_comm_base.so", EntryPoint = "I2C_Close", SetLastError = true)]
			public static extern int I2C_Close (int fHandle);

			[DllImport("libpi_comm_base.so", EntryPoint = "I2C_Write", SetLastError = true)]
			public static extern int I2C_Write (int fHandle, int addr, byte[] buff, int length);

			[DllImport("libpi_comm_base.so", EntryPoint = "I2C_Read", SetLastError = true)]
			public static extern int I2C_Read (int fHandle, int addr, byte[] buff, int length);
		}

		// device name as provided by the caller
		public string I2CDeviceName { get; private set; }

		// device handle as provided by the OS
		private int _DeviceHandle = -1;

		// one and only CTOR (using model B+/2 file name)
		public PI_I2C (string DeviceName = @"/dev/i2c-1")
		{
			// save the device name
			I2CDeviceName = DeviceName;

			// save the device handle
			_DeviceHandle = _PI_Comm.I2C_Open (I2CDeviceName);

			// ensure that the handle is valid
			if (_DeviceHandle < 0)
				throw new ArgumentException ("Unable to open device");
		}

		// free the handle we have open
		public void Dispose ()
		{
			_PI_Comm.I2C_Close (_DeviceHandle);
		}

		// for read/write methods: 
		//  return -1 	: bad device
		//  return -2 	: I2C transaction failed
		//  return -3   : bad buffer lenght

		// write n number of bytes to the target device
		public int Write (byte address, byte[] values)
		{
			if (values.Length < 1)
				return -3;

			return _PI_Comm.I2C_Write (_DeviceHandle, address, values, values.Length);
		}

		// write out 8 bits of data to the target device
		public int Write8 (byte address, byte value)
		{
			byte[] buff = new byte[1];
			buff[0] = value;
			return _PI_Comm.I2C_Write (_DeviceHandle, address, buff, 1);
		}

		// write out 16 bits of data to the target device
		public int Write16 (byte address, byte value0, byte value1)
		{
			byte[] buff = new byte[2];
			buff[0] = value0;
			buff[1] = value1;
			return _PI_Comm.I2C_Write (_DeviceHandle, address, buff, 2);
		}

		// read n bytes of data from the target device
		public int Read (byte address, byte[] buff, int iCount)
		{
			if (buff.Length < iCount)
				return -3;
			return _PI_Comm.I2C_Read (_DeviceHandle, address, buff, iCount);
		}

		// read 8 bytes of data from the target device
		public int Read8 (byte address, byte[] buff)
		{
			if (buff.Length < 1)
				return -3;
			return _PI_Comm.I2C_Read (_DeviceHandle, address, buff, 1);
		}

		// read 16 bytes of data from the target device
		public int Read16 (byte address, byte[] buff)
		{
			if (buff.Length < 2)
				return -3;

			return _PI_Comm.I2C_Read (_DeviceHandle, address, buff, 2);
		}
	}
}

