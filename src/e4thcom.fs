\ e4thcom main include file to load everything at once
\ (c)copyright 2018 by Gerald Wodni <gerald.wodni@gmail.com>

\ HINT: load usb.fs first (once, it will provide a new eraseflash)!

#include registers.fs
#include standard.fs
#include gpio.fs
#include pwm.fs
#include spi.fs
#include thermo.fs
#include lcd.fs
#include lcd-patterns.fs
#include profile.fs

#include app.fs
