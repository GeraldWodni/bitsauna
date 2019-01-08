\ sauna main app
\ (c)copyright 2019 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

: app ( -- )
    begin
        $4F lcd-ddram
        buttons@ $1 and if
            [CHAR] 1
        else
            [CHAR] 0
        then lcd-emit

    key? until ;
