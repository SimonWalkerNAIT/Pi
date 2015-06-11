#ifndef __PI_Comm_Base_H__
#define __PI_Comm_Base_H__

// adapted from https://www.kernel.org/doc/Documentation/i2c/dev-interface

// don't mangle names for linkage!
#ifdef __cplusplus
extern "C" {
#endif

// attempt to open the device file provided as an I2C device
// usually /dev/i2c-1 for Pi2
extern int I2C_Open (char* DeviceFileName);

// attempt to close an open device file
extern int I2C_Close (int fHandle);

// write data to the device on the specified address
//  return -1 	: bad device
//  return -2 	: I2C transaction failed
//  return 0 	: OK
extern int I2C_Write (int fHandle, int addr, unsigned char* buff, int length);

// read data from the device on the specified address
// buff must be allocated for length
//  return -1 	: bad device
//  return -2  	: transaction failed
//  return 0	: OK
extern int I2C_Read (int fHandle, int addr, unsigned char* buff, int length);

#ifdef __cplusplus
}
#endif

#endif

