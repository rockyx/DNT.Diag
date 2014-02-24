/* -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */

/* serial port functions
 *
 * Author: CHris Toshok <toshok@ximian.com>
 */

#include <termios.h>
#include <unistd.h>
#include <fcntl.h>
#include <string.h>
#include <errno.h>
#if defined(__APPLE__)
#include "fakepoll.h"
#else
#include <sys/poll.h>
#endif
#include <sys/ioctl.h>

/* This is for FIONREAD on solaris */
#if defined(sun)
#include <sys/filio.h>
#endif

/* sys/time.h (for timeval) is required when using osx 10.3 (but not 10.4) */
#ifdef __APPLE__
#include <sys/time.h>
#endif

#include <cstdint>
#include <cstdio>
#include <cstdlib>

typedef int boolean;

#ifndef FALSE
#define FALSE 0
#endif

#ifndef TRUE
#define TRUE 1
#endif

/* This is a copy of System.IO.Ports.Handshake */
typedef enum {
	NoneHandshake = 0, XOnXOff = 1, RequestToSend = 2, RequestToSendXOnXOff = 3
} MonoHandshake;

/* This is a copy of System.IO.POrts.Parity */
typedef enum {
	NoneParity = 0, Odd = 1, Even = 2, Mark = 3, Space = 4
} MonoParity;

/* THis is a copy of System.IO.Ports.StopBits */
typedef enum {
	NoneStopBits = 0, One = 1, Two = 2, OnePointFive = 3
} MonoStopBits;

/* This is a copy of System.IO.Ports.SerialSignal */
typedef enum {
	NoneSignal, Cd = 1, /* Carrier detect */
	Cts = 2, /* Clear to send */
	Dsr = 4, /* Data set ready */
	Dtr = 8, /* Data terminal ready */
	Rts = 16 /* Request to send */
} MonoSerialSignal;

/*
 * Silence the compiler, we do not need these prototypes to be public, since these are only
 * used by P/Invoke
 */

int open_serial(char *devfile);
int close_serial(int unix_fd);
uint32_t read_serial(int fd, uint8_t *buffer, int offset, int count);
int write_serial(int fd, uint8_t *buffer, int offset, int count, int timeout);
int discard_buffer(int fd, boolean input);
int32_t get_bytes_in_buffer(int fd, boolean input);
boolean is_baud_rate_legal(int baud_rate);
int setup_baud_rate(int baud_rate);
boolean set_attributes(int fd, int baud_rate, MonoParity parity, int dataBits,
		MonoStopBits stopBits, MonoHandshake handshake);
MonoSerialSignal get_signal(int fd, int32_t *error);
int32_t set_signal(int fd, MonoSerialSignal signal, boolean value);
int breakprop(int fd);
boolean poll_serial(int fd, int32_t *error, int timeout);
void *list_serial_devices(void);

int open_serial(char *devfile) {
	int fd;
	fd = open(devfile, O_RDWR | O_NOCTTY | O_NONBLOCK);

	return fd;
}

int close_serial(int unix_fd) {
	// linus writes: do not retry close after EINTR
	return close(unix_fd);
}

uint32_t read_serial(int fd, uint8_t *buffer, int offset, int count) {
	uint32_t n;

	n = read(fd, buffer + offset, count);

	return (uint32_t) n;
}

int write_serial(int fd, uint8_t *buffer, int offset, int count, int timeout) {
#ifdef TOMIC_DEVICE_V1
	if (write(fd, buffer + offset, count) == count)
		return 0;
	return -1;
#else
	struct pollfd pinfo;
	uint32_t n;

	pinfo.fd = fd;
	pinfo.events = POLLOUT;
	pinfo.revents = POLLOUT;

	n = count;

	while (n > 0)
	{
		ssize_t t;
		if (timeout != 0)
		{
			int c;
			while ((c = poll(&pinfo, 1, timeout)) == -1 && errno == EINTR)
			;
			if (c == -1)
			return -1;
		}

		do
		{
			t = write(fd, buffer + offset, n);
		}while (t == -1 && errno == EINTR);

		if (t < 0)
		return -1;

		offset += t;
		n -= t;
	}

	return 0;
#endif
}

int discard_buffer(int fd, boolean input) {
	return tcflush(fd, input ? TCIFLUSH : TCOFLUSH);
}

int32_t get_bytes_in_buffer(int fd, boolean input) {
	int32_t retval;

	if (ioctl(fd, input ? FIONREAD : TIOCOUTQ, &retval) == -1) {
		return -1;
	}

	return retval;
}

boolean is_baud_rate_legal(int baud_rate) {
	return setup_baud_rate(baud_rate) != -1;
}

int setup_baud_rate(int baud_rate) {
	switch (baud_rate) {
	/*Some values are not defined on OSX and *BSD */
#if defined(B921600)
	case 921600:
		baud_rate = B921600;
		break;
#endif
#if defined(B460800)
	case 460800:
		baud_rate = B460800;
		break;
#endif
	case 230400:
		baud_rate = B230400;
		break;
	case 115200:
		baud_rate = B115200;
		break;
	case 57600:
		baud_rate = B57600;
		break;
	case 38400:
		baud_rate = B38400;
		break;
	case 19200:
		baud_rate = B19200;
		break;
	case 9600:
		baud_rate = B9600;
		break;
	case 4800:
		baud_rate = B4800;
		break;
	case 2400:
		baud_rate = B2400;
		break;
	case 1800:
		baud_rate = B1800;
		break;
	case 1200:
		baud_rate = B1200;
		break;
	case 600:
		baud_rate = B600;
		break;
	case 300:
		baud_rate = B300;
		break;
	case 200:
		baud_rate = B200;
		break;
	case 150:
		baud_rate = B150;
		break;
	case 134:
		baud_rate = B134;
		break;
	case 110:
		baud_rate = B110;
		break;
	case 75:
		baud_rate = B75;
		break;
	case 50:
	case 0:
	default:
		baud_rate = -1;
		break;
	}
	return baud_rate;
}

boolean set_attributes(int fd, int baud_rate, MonoParity parity, int dataBits,
		MonoStopBits stopBits, MonoHandshake handshake) {
	struct termios newtio;

	if (tcgetattr(fd, &newtio) == -1)
		return FALSE;

	newtio.c_cflag |= (CLOCAL | CREAD);
	newtio.c_lflag &= ~(ICANON | ECHO | ECHOE | ECHOK | ECHONL | ISIG | IEXTEN);
	newtio.c_oflag &= ~(OPOST);
	newtio.c_iflag = IGNBRK;

	/* setup baudrate */
	baud_rate = setup_baud_rate(baud_rate);

	/* char lenght */
	newtio.c_cflag &= ~CSIZE;
	switch (dataBits) {
	case 5:
		newtio.c_cflag |= CS5;
		break;
	case 6:
		newtio.c_cflag |= CS6;
		break;
	case 7:
		newtio.c_cflag |= CS7;
		break;
	case 8:
	default:
		newtio.c_cflag |= CS8;
		break;
	}

	/* stopbits */
	switch (stopBits) {
	case NoneStopBits:
		/* Unhandled */
		break;
	case One: /* One */
		/* do nothing, the default is one stop bit */
		newtio.c_cflag &= ~CSTOPB;
		break;
	case Two: /* Two */
		newtio.c_cflag |= CSTOPB;
		break;
	case OnePointFive: /* OnePointFive */
		/* XXX unhandled */
		break;
	}

	/* parity */
	newtio.c_iflag &= ~(INPCK | ISTRIP);

	switch (parity) {
	case NoneParity: /* None */
		newtio.c_cflag &= ~(PARENB | PARODD);
		break;

	case Odd: /* Odd */
		newtio.c_cflag |= PARENB | PARODD;
		break;

	case Even: /* Even */
		newtio.c_cflag &= ~(PARODD);
		newtio.c_cflag |= (PARENB);
		break;

	case Mark: /* Mark */
		/* XXX unhandled */
		break;
	case Space: /* Space */
		/* XXX unhandled */
		break;
	}

	newtio.c_iflag &= ~(IXOFF | IXON);
#ifdef CRTSCTS
	newtio.c_cflag &= ~CRTSCTS;
#endif /* def CRTSCTS */

	switch (handshake) {
	case NoneHandshake: /* None */
		/* do nothing */
		break;
	case RequestToSend: /* RequestToSend (RTS) */
#ifdef CRTSCTS
		newtio.c_cflag |= CRTSCTS;
#endif /* def CRTSCTS */
		break;
	case RequestToSendXOnXOff: /* RequestToSendXOnXOff (RTS + XON/XOFF) */
#ifdef CRTSCTS
		newtio.c_cflag |= CRTSCTS;
#endif /* def CRTSCTS */
		/* fall through */
	case XOnXOff: /* XOnXOff */
		newtio.c_iflag |= IXOFF | IXON;
		break;
	}

	if (cfsetospeed(&newtio, baud_rate) < 0
			|| cfsetispeed(&newtio, baud_rate) < 0
			|| tcsetattr(fd, TCSANOW, &newtio) < 0) {
		return FALSE;
	} else {
		return TRUE;
	}
}

#ifdef TOMIC_DEVICE_V1

#include <android/log.h>

int32_t serial_command(const char *name) {
	char buff[256];
	sprintf(buff, "%s/%s", "/system/bin", name);
	return system(buff);
}

#endif

static int32_t get_signal_code(MonoSerialSignal signal) {
	switch (signal) {
	case Cd:
		return TIOCM_CAR;
	case Cts:
		return TIOCM_CTS;
	case Dsr:
		return TIOCM_DSR;
	case Dtr:
		return TIOCM_DTR;
	case Rts:
		return TIOCM_RTS;
	default:
		return 0;
	}

	/* Not reached */
	return 0;
}

extern "C" {

static MonoSerialSignal get_mono_signal_codes(int signals) {
	int retval = NoneSignal;

	if ((signals & TIOCM_CAR) != 0)
		retval |= Cd;
	if ((signals & TIOCM_CTS) != 0)
		retval |= Cts;
	if ((signals & TIOCM_DSR) != 0)
		retval |= Dsr;
	if ((signals & TIOCM_DTR) != 0)
		retval |= Dtr;
	if ((signals & TIOCM_RTS) != 0)
		retval |= Rts;

	return (MonoSerialSignal) retval;
}

}

MonoSerialSignal get_signals(int fd, int32_t *error) {
	int signals;

	*error = 0;

	if (ioctl(fd, TIOCMGET, &signals) == -1) {
		*error = -1;
		return NoneSignal;
	}

	return get_mono_signal_codes(signals);
}

int32_t set_signal(int fd, MonoSerialSignal signal, boolean value) {
#ifdef TOMIC_DEVICE_V1

	if (signal == Dtr)
	{
		if (!value)
			return serial_command("pullup");
		else
			return serial_command("pulldown");
	}
#endif

	int signals, expected, activated;

	expected = get_signal_code(signal);
	if (ioctl(fd, TIOCMGET, &signals) == -1)
		return -1;

	activated = (signals & expected) != 0;
	if (activated == value) /* Already set */
		return 1;

	if (value)
		signals |= expected;
	else
		signals &= ~expected;

	if (ioctl(fd, TIOCMSET, &signals) == -1)
		return -1;

	return 1;
}

int breakprop(int fd) {
	return tcsendbreak(fd, 0);
}

boolean poll_serial(int fd, int32_t *error, int timeout) {
	struct pollfd pinfo;

	*error = 0;

	pinfo.fd = fd;
	pinfo.events = POLLIN;
	pinfo.revents = 0;

	while (poll(&pinfo, 1, timeout) == -1 && errno == EINTR) {
		/* EINTR is an OK condition, we should not throw in the upper layer an IOException */
		if (errno != EINTR) {
			*error = -1;
			return FALSE;
		}
	}

	return (pinfo.revents & POLLIN) != 0 ? 1 : 0;
}

/*
 * mono internals should not be used here.
 * this serial stuff needs to be implemented with icalls.
 * make this at least compile until the code is moved elsewhere
 * defined(linux) is wrong, too
 */
void*
list_serial_devices(void) {
	return NULL;
}

