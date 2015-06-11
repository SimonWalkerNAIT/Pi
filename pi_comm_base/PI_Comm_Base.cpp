#include <linux/i2c-dev.h>
#include <fcntl.h>
#include <sys/ioctl.h>
#include <unistd.h>

#include "PI_Comm_Base.h"

// adapted from https://www.kernel.org/doc/Documentation/i2c/dev-interface

// attempt to open the device file provided as an I2C device
// usually /dev/i2c-1 for Pi2
int I2C_Open (char* DeviceFileName)
{
	// attempt to open the device file, as provided
	int fHandle = open (DeviceFileName, O_RDWR);
	
	// error, return error code of -1
	if (fHandle < 0)
		return -1;
		
	// return handle of device file
	return fHandle;
}

// attempt to close an open device file
int I2C_Close (int fHandle)
{
	return close (fHandle);
}

// write data to the device on the specified address
//  return -1 	: bad device
//  return -2 	: I2C transaction failed
//  return 0 	: OK
int I2C_Write (int fHandle, int addr, unsigned char* buff, int length)
{
	// set the device we wish to talk to
	if (ioctl (fHandle, I2C_SLAVE, addr) < 0)
		return -1;	// bad device

	// attempt the write
	if (write (fHandle, buff, length) != length)
		return -2;	// transaction failed

	// all OK
	return 0;
}

// read data from the device on the specified address
// buff must be allocated for length
//  return -1 	: bad device
//  return -2  	: transaction failed
//  return 0	: OK
int I2C_Read (int fHandle, int addr, unsigned char* buff, int length)
{
	// set the device we wish to talk to
	if (ioctl (fHandle, I2C_SLAVE, addr) < 0)
		return -1;	// bad device

	// attempt the read...
	int numRead = read (fHandle, buff, length);
	
	if (numRead != length)
		return -2;	// transaction failed

	return 0;
}
