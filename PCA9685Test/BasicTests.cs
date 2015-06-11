using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Linq;

using PI_Comm_Wrapper;

namespace PCA9685Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Starting PCA9685 Tests...");

			// these tests were run on a Adafruit PCA9685 module with BMS631MG servos on channels 7 and 11, and
			//  a totally generic servo on channel 15
			// NOTE: My PCA9685 module was addressed at 0x48; the out-of-the-box address is 0x40
			// the following test code inits the PWM module and runs the servos back and forth
			try
			{
				using (PI_I2C comm = new PI_I2C())
				{
					// init, write to mode 1 (go to sleep)
					comm.Write16 (0x48, 0x00, 0x10);

					// program for ~50Hz modulation
					comm.Write16 (0x48, 0xFE, 0x80);

					// wake up
					comm.Write16 (0x48, 0x00, 0x00);

					// on time (start at beginning of count cycle (no delay))
					comm.Write16 (0x48, 0x42, 0x00);
					comm.Write16 (0x48, 0x43, 0x00);
					comm.Write16 (0x48, 0x32, 0x00);
					comm.Write16 (0x48, 0x33, 0x00);
					comm.Write16 (0x48, 0x22, 0x00);
					comm.Write16 (0x48, 0x23, 0x00);

					// off time (ctr)
					ProgCh15 (comm, 284);
					ProgCh11 (comm, 315);
					ProgCh7 (comm, 315);

					System.Threading.Thread.Sleep (500);

					do
					{
						// move fully to the right
						ProgCh15 (comm, 83);
						ProgCh11 (comm, 460);
						ProgCh7 (comm, 460);

						System.Threading.Thread.Sleep (1000);

						// move to idle (centre position)
						ProgCh15 (comm, 284);
						ProgCh11 (comm, 315);
						ProgCh7 (comm, 315);

						System.Threading.Thread.Sleep (1000);

						// move fully to the left
						ProgCh15 (comm, 485);
						ProgCh11 (comm, 170);
						ProgCh7 (comm, 170);

						System.Threading.Thread.Sleep (1000);
					}
					while (!Console.KeyAvailable);
				}
			}
			catch (Exception err)
			{
				Console.WriteLine ("Error on the bus : " + err.Message);
			}
		}

		// support methods to program the channels
		private static void ProgCh15 (PI_I2C comm, UInt16 period)
		{
			byte low = (byte)(period & 0xFF);
			byte high = (byte)((period >> 8) & 0x0F);

			comm.Write16 (0x48, 0x44, low);
			comm.Write16 (0x48, 0x45, high);
		}

		private static void ProgCh11 (PI_I2C comm, UInt16 period)
		{
			byte low = (byte)(period & 0xFF);
			byte high = (byte)((period >> 8) & 0x0F);

			comm.Write16 (0x48, 0x34, low);
			comm.Write16 (0x48, 0x35, high);
		}

		private static void ProgCh7 (PI_I2C comm, UInt16 period)
		{
			byte low = (byte)(period & 0xFF);
			byte high = (byte)((period >> 8) & 0x0F);

			comm.Write16 (0x48, 0x24, low);
			comm.Write16 (0x48, 0x25, high);
		}
	}
}
