using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace PI_Comm_Wrapper
{
	public class PI_GPIO : IDisposable
	{
		// possible pin directions
		public enum PinDirection
		{
			Input,			// pin used as an input
			Output			// pin used as an output
		}

		// known pin states (assume we are the only ones manipulating GPIO pins)
		private Dictionary<int, PinDirection> _PinStates = new Dictionary<int, PinDirection>();

		public PI_GPIO ()
		{
		}

		public void Dispose ()
		{
			// free all pins, release all resource
			while (_PinStates.Count > 0)
				UnusePin (_PinStates.Keys.First ());
		}

		// call to assign a pin for use
		public bool UsePin (int GPIO_Pin_ID, PinDirection dir)
		{
			// user wants to use the specified pin for the specified direction

			// first, ensure that we don't already have the pin in use
			if (_PinStates.ContainsKey (GPIO_Pin_ID))
				throw new InvalidOperationException ("Pin " + GPIO_Pin_ID.ToString() + " already in use.");

			// attempt to open the pin for use (export it)
			try {
				File.WriteAllText (@"/sys/class/gpio/export", GPIO_Pin_ID.ToString ());								
			} catch (Exception err) {
				// error with file operation
				Console.WriteLine (err.Message);
				return false;
			}

			// attempt to set the direction of the pin (requires use of OS created file)
			// practical use has found that this can fail for an arbitrary amount of times
			//  keep trying until it works, or bail after too many attempts
			int iFailCount = 0;
			while (iFailCount++ < 1000)
			{
				try
				{
					File.WriteAllText (@"/sys/class/gpio/gpio" + GPIO_Pin_ID.ToString () + @"/direction", dir == PinDirection.Input ? "in" : "out");

					// if no exception, then get out, otherwise exception will show access error
					break;
				}
				catch (Exception)
				{
					// error with file operation

					// try again...
					System.Threading.Thread.Sleep (0);
					continue;
				}
			}

			// did we hit the fail count limit?
			if (iFailCount == 1000)
			{
				Console.WriteLine ("Unable to set pin direction...");
				return false;
			}

			//Console.WriteLine ("It took " + iFailCount.ToString() + " attempts to set direction...");

			// if all is well, add this pin to the in use table
			if (dir == PinDirection.Input)
				_PinStates.Add (GPIO_Pin_ID, PinDirection.Input);
			else
				_PinStates.Add (GPIO_Pin_ID, PinDirection.Output);				

			return true;
		}

		// call to release a pin from use
		public bool UnusePin (int GPIO_Pin_ID)
		{
			// user wants to stop using the specified pin

			// first, ensure that we have the pin in use
			if (!_PinStates.ContainsKey (GPIO_Pin_ID))
				throw new InvalidOperationException ("Pin " + GPIO_Pin_ID.ToString() + " not in use.");

			// attempt to unexport the pin
			int iFailCount = 0;
			while (iFailCount++ < 1000)
			{
				try
				{
					File.WriteAllText (@"/sys/class/gpio/unexport", GPIO_Pin_ID.ToString ());

					// if no exception, then get out, otherwise exception will show access error
					break;
				}
				catch (Exception)
				{
					// error with file operation

					// try again...
					System.Threading.Thread.Sleep (0);
					continue;
				}
			}

			// did we hit the fail count limit?
			if (iFailCount == 1000)
			{
				Console.WriteLine ("Unable to unexport pin " + GPIO_Pin_ID.ToString() + "...");
				return false;
			}

			//Console.WriteLine ("It took " + iFailCount.ToString() + " attempts to unexport the pin...");

			// if all is well, remove this pin for the known list
			_PinStates.Remove (GPIO_Pin_ID);

			return true;
		}	

		// call to set the state of a pin that is in use (for writing)
		public bool WritePin (int GPIO_Pin_ID, bool value)
		{
			if (!_PinStates.ContainsKey (GPIO_Pin_ID) || _PinStates [GPIO_Pin_ID] != PinDirection.Output)
				throw new InvalidOperationException ("Pin " + GPIO_Pin_ID.ToString() + " not set for writing.");

			int iFailCount = 0;
			while (iFailCount++ < 1000)
			{
				try
				{
					File.WriteAllText (@"/sys/class/gpio/gpio" + GPIO_Pin_ID.ToString () + @"/value", value? "1":"0");

					// if no exception, then get out, otherwise exception will show access error
					break;
				}
				catch (Exception)
				{
					// error with file operation

					// try again...
					System.Threading.Thread.Sleep (0);
					continue;
				}
			}

			// did we hit the fail count limit?
			if (iFailCount == 1000)
			{
				Console.WriteLine ("Unable to set pin value...");
				return false;
			}

			//Console.WriteLine ("It took " + iFailCount.ToString() + " attempts to set pin value...");

			return true;				
		}

		// call to read the state of a pin that is in use (for reading)
		public bool ReadPin (int GPIO_Pin_ID, out bool value)
		{
			// assume low unless otherwise set...
			value = false;

			// ensure the pin is in use for reading
			if (!_PinStates.ContainsKey (GPIO_Pin_ID) || _PinStates [GPIO_Pin_ID] != PinDirection.Input)
				throw new InvalidOperationException ("Pin " + GPIO_Pin_ID.ToString() + " not set for reading.");

			int iFailCount = 0;
			while (iFailCount++ < 1000)
			{
				try
				{
					string result = File.ReadAllText (@"/sys/class/gpio/gpio" + GPIO_Pin_ID.ToString () + @"/value");
					result = result.Trim ();

					// set the return argument to the read state
					value = result == "1";

					// if no exception, then get out, otherwise exception will show access error
					break;
				}
				catch (Exception)
				{
					// error with file operation

					// try again...
					System.Threading.Thread.Sleep (0);
					continue;
				}
			}

			// did we hit the fail count limit?
			if (iFailCount == 1000)
			{
				Console.WriteLine ("Unable to set pin value...");
				return false;
			}

			//Console.WriteLine ("It took " + iFailCount.ToString() + " attempts to set pin value...");

			return true;		
		}
	}
}

