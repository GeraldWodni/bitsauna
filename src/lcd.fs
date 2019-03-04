\ Driver for the KS0066 LCD Controller
\ Also compatible with HD44780
\ (c)copyright 2016 by Gerald Wodni<gerald.wodni@gmail.com>

\ the following words need to be defined by the user, as they depend on the pin configuration and the controller
\ nibble>lcd ( x -- ) sends a nibble to the LCD (set D7-D4 and strobe E)
\ lcd-mode-data ( -- ) data mode (clear RS)
\ lcd-mode-cmd  ( -- ) data mode (set RS)

compiletoflash

: lcd-mode-data ( -- )
    -1 lcd-rs gpio! ;

: lcd-mode-cmd ( -- )
     0 lcd-rs gpio! ;

: lcd-nibble ( x -- )
    dup $01 and lcd-d4 gpio!
    dup $02 and lcd-d5 gpio!
    dup $04 and lcd-d6 gpio!
        $08 and lcd-d7 gpio! 
    10 us
    -1 lcd-en gpio!
    10 us
     0 lcd-en gpio!
    10 us ;

: lcd-read-status ( -- x )
    lcd-mode-cmd
    lcd-d4 gpio-input
    lcd-d5 gpio-input
    lcd-d6 gpio-input
    lcd-d7 gpio-input

    -1 lcd-rw gpio!
    10 us

    0 \ read upper nibble
    -1 lcd-en gpio!
    10 us
    $10 lcd-d4 gpio@ and or
    $20 lcd-d5 gpio@ and or
    $40 lcd-d6 gpio@ and or
    $80 lcd-d7 gpio@ and or
    10 us
     0 lcd-en gpio!
    10 us

    \ read lower nibble
    -1 lcd-en gpio!
    10 us
    $01 lcd-d4 gpio@ and or
    $02 lcd-d5 gpio@ and or
    $04 lcd-d6 gpio@ and or
    $08 lcd-d7 gpio@ and or
    10 us
     0 lcd-en gpio!

     0 lcd-rw gpio!
    10 us
    lcd-d4 gpio-output
    lcd-d5 gpio-output
    lcd-d6 gpio-output
    lcd-d7 gpio-output
    10 us ;

\ --- low level interface ---
\ send byte to display
: lcd-emit ( x -- )
    \ ." 8bit: " hex. cr ;
    dup 4 rshift lcd-nibble \ higher nibble
    $F and lcd-nibble ;     \ lower  nibble

\ send command to display
: lcd-cmd ( x -- )
    lcd-mode-cmd
    lcd-emit
    lcd-mode-data ;

\ clear display content
: lcd-clear ( -- )
    $01 lcd-cmd 2 ms ;

\ move cursor to home position
: lcd-home ( -- )
    $02 lcd-cmd 2 ms ;

\ I/D=1 increment or I/D=0 decrement address after write,
\ S=1 shift entire display to I/D direction
: lcd-entry-mode ( f-I/D f-S -- )
    0<> $01 and swap \ S
    0<> $02 and or   \ I/D
    $04 or lcd-cmd ;

\ D=1 display on, D=0 off
\ C=1 cursor  on, C=0 off
\ B=1 cursor blink on, B=0 off
: lcd-display-control ( f-D f-C f-B -- )
    0<> $01 and swap    \ B
    0<> $02 and or swap \ C
    0<> $04 and or      \ D
    $08 or lcd-cmd ;

\ S/C=1 display shift, S/C=0 cursor shift
\ R/L=1 to the right, R/L=0 to the left
: lcd-c/d-shift ( f-S/C f-R/L -- )
    0<> $04 and swap \ R/L
    0<> $08 and or   \ S/C
    $10 or lcd-cmd ;

\ DL=1 8 bit, DL=0 4 bit data length
\ N=1 2 lines, N=0 1 line
\ F=1 5x10 dots, F=0 5x8 fots
: lcd-function-set ( f-DL f-N f-F -- )
    0<> $04 and swap    \ F
    0<> $08 and or swap \ N
    0<> $10 and or      \ DL
    $20 or lcd-cmd ;

\ set cgram-address
: lcd-cgram ( u-addr -- )
    $3F and $40 or lcd-cmd ;

: lcd-ddram ( u-addr -- )
    $7F and $80 or lcd-cmd ;

\ initialize 4 bit interface ( according to datasheet )
: lcd-init ( f-lines -- )
    lcd-mode-cmd
    40 ms           \ powerup wait
    $02 lcd-nibble  \ function set
    1 ms

    0 swap 0 lcd-function-set  \ 5x7, 4bits, f-lines
    -1 0 0 lcd-display-control  \ display on
    lcd-clear                   \ clear display
    -1 0 lcd-entry-mode ;       \ increment on store, no shift

\ initialize by instruction
: lcd-reset ( f-lines -- )
    lcd-mode-cmd
    40 ms
    $03 lcd-nibble
    5 ms
    $03 lcd-nibble
    100 us
    $03 lcd-nibble
    lcd-init ;

\ --- high level interface ---
\ send string to display
: lcd-type ( c-addr n -- )
    bounds do
        i c@ lcd-emit
    loop ;

\ like .( but send to lcd
: lcd( ( -- ) immediate
    [char] ) parse lcd-type ;

\ like ." but send to lcd
: lcd" ( -- ) immediate
    postpone s" postpone lcd-type ;

\ number output
: lcd. ( -- )
    0 <# #S #> lcd-type ;

\ define a custom character, point to address containing 8 consecutive pattern codes
: lcd-char ( c-addr u-char -- )
    8 * lcd-cgram
    8 bounds do
        i c@ lcd-emit
    loop lcd-home ;

: init ( -- )
    init -1 lcd-reset
    lcd" bitsauna PWM:"
    $40 lcd-ddram
    lcd" init" ;

cornerstone lcold

\ -1 lcd-init
