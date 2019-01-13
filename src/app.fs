\ sauna main app
\ (c)copyright 2019 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

: app ( -- )
    $40 lcd-ddram lcd" T1: "
    $48 lcd-ddram lcd" Ti: "
    begin
        $0F lcd-ddram
        buttons@ $1 and if
            [CHAR] 1
        else
            [CHAR] 0
        then lcd-emit

        spi-ro32
        cr dup hex. \ debug the stuff
        $44 lcd-ddram
        dup th-temp lcd.

        $4C lcd-ddram
            th-itemp lcd.

    key? until ;

: init init app ;

cornerstone acold
