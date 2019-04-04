\ sauna main app
\ (c)copyright 2019 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

\ lcd layout
\              PWM  \ function
\ 0123456789ABCDEF  \ column
\ PR1 #123/123 #12  \ 1st line
\   #123 #123 #123  \ 2nd line
\  T1   T2   SETP.  \ function

: lcd-time. ( -- )
    3 lcd-emit \ Time pattern

    profile-total-time dup
    999 > if \ if total time > 999, display in minutes
        [CHAR] m swap \ signal minutes
        60 /
        profile-time @ 60 /
    else
        [CHAR] s swap \ signal seconds
        profile-time @
    then
    lcd3.   \ show fraction
    [CHAR] / lcd-emit
    lcd3.
    lcd-emit ; \ show seconds/minutes

: app-buttons ( -- )
    buttons-once@ case
        1 of
            profile-active? if
                stop-profile
            else
                start-profile
            then
        endof
        2 of prev-profile endof
        4 of next-profile endof
    endcase

    0 lcd-ddram
    profile-active? if
        profile-name lcd-type
        lcd-time.
    else
        \            NAME
        \    0123456789ABC
        lcd" OFF/SEL:"
        profile-name lcd-type
        $20 lcd-emit
    then ;

: lcd-temp. ( n -- )
    dup -1 = if
        drop lcd" ERR"
    else
        lcd3.
    then ;

-1 variable last-time
: app-log ( n-current n-setpoint -- )
    \ only report in second intervals
    profile-time @ last-time @ over <> if
        last-time !
        cr swap . .
    else
        drop 2drop
    then ;

\ compute average (ignore errornous sensor)
: th-avg ( n1 n2 -- n3 )
    dup -1 = if \ 2nd sensor broken
        drop exit
    then
    over -1 = if \ 1st sensor broken
        nip exit
    then
    + 2/ ;  \ both working, avg it

: app ( -- )
    lcd-clear

    begin
        app-buttons

        \ Info: display is updated every cycle to avoid errors
        $0D lcd-ddram 4 lcd-emit \ PWM pattern

        spi-read-both
        th-temp swap th-temp

        $40 lcd-ddram
        lcd"   "   \ 2nd line

        1 lcd-emit \ Temp 1 pattern

        dup lcd-temp.

        $20 lcd-emit
          2 lcd-emit \ Temp 2 pattern

        over lcd-temp.

        $20 lcd-emit
          0 lcd-emit \ Setpoint pattern

        th-avg

        setpoint @
            dup lcd3.

        2dup app-log

        swap -               \ get difference
        2/                   \ scale "P"
        10 min 0 max pwm!    \ limit and set

        $0E lcd-ddram
            pwm@ lcd2.

    key? until ;

: init init app ;

cornerstone acold

start-profile
app
