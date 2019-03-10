\ profiles: <n-steps> [<n-seconds> <n-temp>]+
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

: get-time-range ( addr-profile -- n-start n-end )
\ time range of current section
    cell - get-temp-range ; \ just offset by one cell to get corresponding times

: range>fraction ( n-current n-start n-end -- n-numerator n-denominator )
    2dup swap - >r \ denominator = end-start
    drop - r> ;    \ nominator = current-start

: get-time-fraction ( n-time addr-profile -- n-numerator n-denominator )
    get-time-range range>fraction ;

: get-temp ( n-time -- n-temp )
    >r r@ get-match dup @ r@ <= if
        r> drop
        drop 0
    else
        ." FR "
        \ cell+ @
        r@ over get-time-fraction swap .t .t
        ." Temp "
        dup get-time-range swap .t .t
        ." Time "
            get-temp-range swap .t .t

        13
        r> drop
    then
    ;

: test ( n-time -- )
    cr
    dup .t ." s: "
    dup get-match dup @ .t cell + @ .t
    get-temp .t ;

  0 test
 15 test
 30 test
250 test
300 test
400 test

bye
