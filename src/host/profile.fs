\ profiles: [<n-seconds> <n-temp>]+ last temp is zero
create leadfree
      0 ,  31 , \ start
     30 , 151 , \ ramp-up 1
    120 , 181 , \ flux-activate
    140 , 231 , \ ramp-up 2
    200 , 231 , \ melting
    300 ,   0 , \ ramp-down

leadfree 12 cells dump

leadfree variable profile
profile !

: get-low-point ( n-time addr-profile -- addr )
    
    ;
: get-high-point ( n-time addr-profile -- addr )
    ;

: get-match ( n-time -- addr-profile )
    profile @   \ time addr
    begin
        2dup @ >=       \ within time
        over cell + @   \ within bounds
        and
    while
        2 cells +
    repeat nip ;


: .t ( n -- )
    3 .r space ;

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
    then
    ;

: test ( n-end n-end -- )
    do
        cr i .t ." :  " i get-temp .t
    10 +loop ;

320 0 test

