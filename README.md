# Pi - Mono Library for GPIO and I2C
This repository contains a C++ project for running GPIO and I2C on the Raspberry Pi, a C# wrapper for the C++ project, and a test project to validate that it all works.
This material is in the early stages of development, but it has been tested with the Raspberry Pi B+ and Raspberry Pi 2 models, using GPIO and I2C on the TCS34725 colour sensor module and PCA9685 PWM module.
This project requires that the i2c-dev kernel module be enabled on the Pi for I2C functionality.

NOTE: The .so and .dll files in the Lib folder *MUST* be copied to the same folder as the excutables built by your projects or the sample projects. This is not automatically done!

This solution was created and is maintained in MonoDevelop 3.0.3.2 (as of now).

Any questions or comments are always welcome.
