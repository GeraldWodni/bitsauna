\ custom lcd chars
\ (c)copyright 2019 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

create pattern-setpoint \ stored in char 0
    %10011 c,
    %11001 c,
    %11101 c,
    %11001 c,
    %10011 c,
    %00001 c,
    %00001 c,
    %00011 c,

create pattern-t1   \ stored in char 1
    %00001 c,
    %01011 c,
    %11001 c,
    %01001 c,
    %11000 c,
    %01000 c,
    %10100 c,
    %01000 c,

create pattern-t2   \ stored in char 2
    %00011 c,
    %01001 c,
    %11010 c,
    %01011 c,
    %11000 c,
    %01000 c,
    %10100 c,
    %01000 c,

create pattern-time \ stored in char 3
    %00000 c,
    %01110 c,
    %10101 c,
    %10111 c,
    %10001 c,
    %01110 c,
    %00000 c,
    %00000 c,

create pattern-pwm \ stored in char 4
    %00000 c,
    %01001 c,
    %10010 c,
    %01001 c,
    %10010 c,
    %01001 c,
    %00000 c,
    %11111 c,

\ some utility functions
\ display 2 digit number with optional leading 0
: lcd2. ( n -- )
    dup  10 < if [CHAR] 0 lcd-emit then
    lcd. ;

\ display 3 digit number with optional leading 0s
: lcd3. ( n -- )
    dup 100 < if [CHAR] 0 lcd-emit then
    lcd2. ;

: init ( -- )
    init
    pattern-setpoint    0 lcd-char
    pattern-t1          1 lcd-char
    pattern-t2          2 lcd-char
    pattern-time        3 lcd-char
    pattern-pwm         4 lcd-char ;
