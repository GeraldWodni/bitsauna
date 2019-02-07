\ GPIO helpers for STM32F103x
\ (c)copyright 2018 by Gerald Wodni <gerald.wodni@gmail.com>

\ configure output pin as output/open-drain/input(pull-down/up)/alternate
compiletoflash
: gpio-configure ( x-mode n-pin addr-port -- )
    GPIOx_CRL           \ add low offset
    over 8 / if
        cell +          \ pin >= 8: go to CRH (high register)
        swap 8 - swap   \ lower pin count
    then >r ( x-mode n-pin/mod8 )
    \ clear nibble
    $F over 4 * lshift
    r@ bic!
    \ set configured bits
    4 * lshift
    r> bis! ;

: gpio-output       ( n-pin addr-port -- )
    %0010 -rot gpio-configure ;

: gpio-open-drain   ( n-pin addr-port -- )
    %0110 -rot gpio-configure ;

: gpio-alternate    ( n-pin addr-port -- )
    \ alternate function
    %1011 -rot gpio-configure ;

: gpio-alternate-open-drain ( n-pin addr-port -- )
    %1111 -rot gpio-configure ;

: gpio-input        ( n-pin addr-port -- )
    %0100 -rot gpio-configure ;

: gpio-input-pullup ( n-pin addr-port -- )
    over bit
    over GPIOx_BSRR !   \ set output data for pullup
    %1000 -rot gpio-configure ;

: gpio-input-pulldown ( n-pin addr-port -- )
    over bit
    over GPIOx_BRR !   \ reset output data for pulldown
    %1000 -rot gpio-configure ;

\ write and read output bit
: gpio@ ( n-pin addr-port -- f )
    swap bit swap   \ convert bit to mask
    GPIOx_IDR bit@ ;

: gpio! ( f n-pin addr-port -- )
    swap bit swap   \ convert bit to mask
    rot if
        GPIOx_BSRR !
    else
        GPIOx_BRR !
    then ;

\ named ports
 0 GPIOA 2constant lcd-en
 1 GPIOA 2constant lcd-rs
 2 GPIOA 2constant lcd-rw
 4 GPIOA 2constant lcd-d4
 5 GPIOA 2constant lcd-d5
 6 GPIOA 2constant lcd-d6
 7 GPIOA 2constant lcd-d7

 0 GPIOB 2constant sw1
 1 GPIOB 2constant sw2
10 GPIOB 2constant sw3

13 GPIOB 2constant th-sck
15 GPIOB 2constant th-cs1
12 GPIOB 2constant th-cs2

\ fetch button mask
: buttons@ ( -- x )
    0
    sw1 gpio@ 0= if $1 or then
    sw2 gpio@ 0= if $2 or then
    sw3 gpio@ 0= if $4 or then ;

\ only fetch on press down
0 variable last-buttons
: buttons-once@ ( -- x )
    last-buttons @ 0= if
        buttons@ dup last-buttons !
    else
        buttons@ 0= if
            0 last-buttons !
        then
        0 \ report no button press
    then ;

: buttons@. ( -- )
    begin
        cr buttons@ hex.
    key? until ;

: buttons-once@. ( -- )
    begin
        buttons-once@ ?dup if
            cr ." Pressed: " hex.
        then
    key? until ;

: init ( -- )
    init
    sw1 gpio-input-pullup
    sw2 gpio-input-pullup
    sw3 gpio-input-pullup

    lcd-en  gpio-output
    lcd-rs  gpio-output
    lcd-rw  gpio-output
    lcd-d4  gpio-output
    lcd-d5  gpio-output
    lcd-d6  gpio-output
    lcd-d7  gpio-output

    th-sck  gpio-output
    th-cs1  gpio-output
    -1 th-cs1 gpio!
    th-cs2  gpio-output
    -1 th-cs2 gpio!

    0 lcd-rw gpio! ;


cornerstone gcold
