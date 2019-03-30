\ sauna main app
\ (c)copyright 2019 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

\ lcd layout
\              PWM  \ function
\ 0123456789ABCDEF  \ column
\ PR1 #123/123 #12  \ 1st line
\   #123 #123 #123  \ 2nd line
\  T1   T2   SETP.  \ function

: app-buttons ( -- )
    buttons@ ?dup if
        case
            1 of start-profile endof
            2 of
                stop-profile
                0 setpoint !
            endof
        endcase
    then

    0 lcd-ddram
    profile-active? if
        lcd" RHS"
        5 lcd-ddram profile-time @ lcd3.
        [CHAR] / lcd-emit
        profile-total-time lcd3.
    else
        lcd" OFF"
        5 lcd-ddram lcd" -Start?"
    then ;

: app ( -- )
    lcd-clear
    $04 lcd-ddram 3 lcd-emit \ Time pattern
    $0D lcd-ddram 4 lcd-emit \ PWM pattern
    $42 lcd-ddram 1 lcd-emit \ Temp 1 pattern
    $47 lcd-ddram 2 lcd-emit \ Temp 2 pattern
    $4C lcd-ddram 0 lcd-emit \ Setpoint pattern
    begin
        app-buttons

        spi-read-both
        $48 lcd-ddram
            th-temp lcd3.

        $43 lcd-ddram
            th-temp dup lcd3.

        setpoint @
        $4D lcd-ddram dup lcd3.

        swap -               \ get difference
        2/                   \ scale "P"
        10 min 0 max pwm!    \ limit and set

        $0E lcd-ddram
            pwm@ lcd2.

    key? until ;

\ : init init app ;


cornerstone acold

app
