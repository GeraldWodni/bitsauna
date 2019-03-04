\ sauna main app
\ (c)copyright 2019 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

30 variable setpoint
: app ( -- )
    $40 lcd-ddram lcd" T1: "
    $48 lcd-ddram lcd" T2: "
    begin
        $0F lcd-ddram
        buttons@ lcd.

        buttons@ ?dup if
            buttons@ 230 * 4 / setpoint !
            $00 lcd-ddram lcd" SET:    "
            $04 lcd-ddram setpoint @ lcd.
        then

        spi-read-both
        $4C lcd-ddram
            th-temp lcd.

        $44 lcd-ddram
            th-temp dup lcd.

        setpoint @ swap -    \ get difference
        2/                   \ scale "P"
        10 min 0 max pwm!    \ limit and set

        $0D lcd-ddram
            pwm@ lcd.

    key? until ;

: init init app ;

cornerstone acold
