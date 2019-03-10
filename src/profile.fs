\ profiles: <n-steps> [<n-seconds> <n-temp>]+
create leadfree
      0 ,  30 , \ start
     30 , 150 , \ ramp-up 1
    120 , 180 , \ flux-activate
    140 , 230 , \ ramp-up 2
    200 , 230 , \ melting
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

: get-temp ( n-time -- n-temp )
    ;

: test ( n-time -- )
    cr
    dup . ." s: "
    get-match dup @ . cell + @ . ;

  0 test
 30 test
250 test
300 test
400 test

bye
