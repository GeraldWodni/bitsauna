\ profile - heat curve management
\ (c)copyright 2019 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

\ profiles: [<n-seconds> <n-temp>]+ ( last temp is zero ) <n-name> [<char-name]+
create leadfree
      0 ,  31 , \ start
     30 , 151 , \ ramp-up 1
    120 , 181 , \ flux-activate
    140 , 231 , \ ramp-up 2:ta
    200 , 231 , \ melting
    300 ,   0 , \ ramp-down
    4   ,
    CHAR R c, 
    CHAR o c, 
    CHAR H c, 
    CHAR S c, 

create p100
      0 ,  31 , \ start
     30 , 100 , \ ramp-up
     90 , 100 , \ keep 100
    150 ,   0 , \ cool down
    4   ,
    CHAR P c, 
    CHAR 1 c, 
    CHAR 0 c, 
    CHAR 0 c, 

create p222
      0 ,  31 , \ start
     30 , 222 , \ ramp-up
     90 , 222 , \ keep 100
    150 ,   0 , \ cool down
    4   ,
    CHAR P c,
    CHAR 2 c,
    CHAR 2 c,
    CHAR 2 c,


\ leadfree 12 cells dump

leadfree variable profile

\ profile switching
create profiles
    leadfree ,
    p100 ,
    p222 ,
3 constant profiles#

: get-profile-index ( -- n-index )
    0 profiles
    begin
        profile @ over @ <>
    while
        swap 1+ swap \ increment index
        cell +
    repeat drop ;

\ set current profile by index
: profile-by-index ( n-index -- )
    profiles swap cells + \ get profile address
    @ profile ! ; \ set as active

: next-profile ( -- )
    get-profile-index 1+ dup
    profiles# >= if drop 0 then   \ next or wrap
    profile-by-index ;

: prev-profile ( -- )
    get-profile-index dup
    0= if drop profiles# then \ previous or wrap
    1- \ prev index
    profile-by-index ;

\ profile data
: profile-total-time ( -- n-time )
    profile @
    begin
        dup @           \ get time
        over cell + @   \ within bounds
    while
        drop            \ drop time
        2 cells +
    repeat nip ;

: profile-name ( -- c-addr n )
    profile @
    begin
        dup cell + @
    while
        2 cells +
    repeat
    2 cells + dup @ \ get length
    swap cell + swap ;   \ string start

: get-match ( n-time -- addr-profile )
    profile @   \ time addr
    begin
        2dup @ >=       \ within time
        over cell + @   \ within bounds
        and
    while
        2 cells +
    repeat nip ;

: get-temp-range ( addr-profile -- n-start n-end )
\ temperature range of current section
    >r r@ cell - @
       r> cell + @ ;

: range>offset-diff ( n-start n-end -- n-off n-diff )
    over - ;

: get-temp-offset-diff ( addr-profile -- n-off n-diff )
    get-temp-range range>offset-diff ;

: get-time-range ( addr-profile -- n-start n-end )
\ time range of current section
    cell - get-temp-range ; \ just offset by one cell to get corresponding times

: range>fraction ( n-current n-start n-end -- n-numerator n-denominator )
    >r >r r@ -      \ nominator = current - start
    r> r> swap - ;  \ denominator = end-start

: get-time-fraction ( n-time addr-profile -- n-denominator n-numerator )
    get-time-range range>fraction swap ;


: get-temp ( n-time -- n-temp )
    >r r@ get-match dup @ r@ <= if
        r> drop
        drop 0
    else
        >r r@ get-temp-offset-diff  \ n-off n-diff
        r> r> swap get-time-fraction
        rot * swap / \ scale with fraction
        + \ add offset
    then ;

: profile. ( n-end n-start -- )
    do
        cr i . ." :  " i get-temp .
    10 +loop ;

\ 320 0 profile.

: stop-profile ( -- )
    0 SYST_CSR ! ;

: profile-active? ( -- f )
    SYST_CSR @ $7 and 0<> ;

\ systick handler
0 variable profile-time \ seconds since profile start
0 variable setpoint     \ profile setpoint (updated once / second by systick if ative)
: profile-systick ( -- )
    profile-time @ 1+ dup profile-time !    \ increment and get time
    get-temp dup setpoint !                 \ set current target temperature
    0= if
        stop-profile    \ disable systick once target (target-temp=0) is reached
    then ;

: start-profile ( -- )
    \ reset time
    -1 profile-time !
    profile-systick \ execute for initial value

    \ setup systick
    ['] profile-systick irq-systick !
    72000000 10 / SYST_RVR !
    7 SYST_CSR ! ;

cornerstone procold
