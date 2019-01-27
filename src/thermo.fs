\ MAX31855 thermocouple amplifier driver
\ (c)copyright 2019 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

: th-read ( -- x )
    ;

\ get extneral temperature
: th-temp ( x -- n )
    dup $10007 and if
        drop -1 \ error (OC, Short or general Fault)
    else
        20 rshift
    then ;

\ get internal temperature
: th-itemp ( x -- n )
    8 rshift
    $3F and ;

\ $18819E0 th-temp .   \ 24°C
\ $18819E0 th-itemp .  \ 25°C
