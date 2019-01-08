\ Standard utilities
\ (c)copyright 2018 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash
: .s ( -- )
    \ display stack contents
    [CHAR] < emit depth 0 <# #s #> type [CHAR] > emit space depth 0 ?do
        depth i - 1- pick .
    loop ;

: cell ( -- n-cell-size )
    1 cells ;

: hex. ( u -- )
    base @ >r hex u. r> base ! ;

: bounds ( c-addr n -- addr-end addr-start )
    over + swap ;

: between ( n-val n-min n-max-1 -- f )
	>r over <=
	swap r> < and ;

\ sign extend 16 to 32 bit
: sign-h ( u1 -- n1 )
    dup $8000 and if
        $FFFF0000 or
    then ;

\ sign extend 8 to 32 bit
: sign-c ( u1 -- n1 )
    dup $80 and if
        $FFFFFF00 or
    then ;

: 0<= ( n -- f )
    dup 0= swap 0< or ;

: between ( n-val n-min n-max-1 -- f )
	>r over <=
	swap r> < and ;

: dump ( c-addr n -- )
    cr
    bounds do
        i hex. ."  "
        i c@ hex. cr
    loop ;

\ helpers for handling 3 items on stack
: 3dup >r 2dup r@ -rot r> ;

\ warning: only works on STM32
: flash! ( x addr -- )
    >r dup
    r@ hflash!
    16 rshift
    r> 2 + hflash! ;

: free-ram
    compiletoram
    flashvar-here here - u. ;

: free-flash
    compiletoflash
    $10000 here - u. ;

\ taken from mecrisp dissambler by Matthias Koch
: words ( -- )
  cr
  dictionarystart
  begin
    dup 6 + ctype space
    dictionarynext
  until
  drop ;

\ Further utils

\ Wait for any bit of mask in register to become true
: wait4true ( x-mask h-addr -- )
    begin
        2dup h@ and
    until 2drop ;

\ Wait for all bits of mask in register to become false
: wait4false ( n-bit h-addr -- )
    begin
        2dup h@ and 0=
    until 2drop ;

cornerstone cold
