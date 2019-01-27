\ sauna main app
\ (c)copyright 2019 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

: app ( -- )
    $40 lcd-ddram lcd" T1: "
    $48 lcd-ddram lcd" T2: "
    begin
        $0F lcd-ddram
        buttons@ $1 and if
            [CHAR] 1
        else
            [CHAR] 0
        then lcd-emit

        spi-read-th1
        cr dup hex. \ debug the stuff
        $44 lcd-ddram
        th-temp lcd.

        spi-read-th2
        $4C lcd-ddram
        th-temp lcd.

    key? until ;

: init init app ;

cornerstone acold
