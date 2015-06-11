using System;

using PI_Comm_Wrapper;

namespace GPIO_Test
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Starting GPIO Tests...");

			// create the GPIO object to use (make sure it is disposed of when done...)
			using (PI_GPIO gpio = new PI_GPIO())
			{
				// setup GPIO pin 17 as an output (physical pin 11)
				if (!gpio.UsePin (17, PI_GPIO.PinDirection.Output))
					Console.WriteLine ("Error using pin...");

				// set the pin to go high for 1 second
				if (!gpio.WritePin (17, true))
					Console.WriteLine ("Error writing pin...");

				// block for a second
				System.Threading.Thread.Sleep (1000);

				// set the pin to go low
				if (!gpio.WritePin (17, false))
					Console.WriteLine ("Error writing pin...");

				// set GPIO pin 21 to be an input (physical pin 40)
				gpio.UsePin (21, PI_GPIO.PinDirection.Input);

				do
				{
					// read the pin value, check for errors...
					bool bValue;
					if (!gpio.ReadPin (21, out bValue))
						break;
						
					// show the pin value...
					Console.WriteLine ("Pin 21 is " + bValue.ToString());
				}
				while (!Console.KeyAvailable);
			}
		}
	}
}
