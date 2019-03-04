\ PWM for STM32F103x
\ (c)copyright 2018 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

\ PWM on PB11: TIM2_CH4

: init ( -- )
    init

    $01 RCC_APB1ENR bis!    \ enable timer 2 clock

    $80 TIM2 TIMx_CR1 hbis! \ enable timer 2
    7200 TIM2 TIMx_PSC h!   \ prescaler (72MHz ^= 100us)

    1000 TIM2 TIMx_ARR  h!  \ top register (100ms)
    200  TIM2 TIMx_CCR4 h!  \ duty cycle (20ms)

    \ remap PA3 -> PB11 (partial remap TIM2_REMAP=10)
    $200 AFIO_MAPR !

    $6800 TIM2 TIMx_CCMR2_Output h! \ PWM mode CH4
    $1000 TIM2 TIMx_CCER h! \ OC4 signal = output

    $01 TIM2 TIMx_EGR hbis! \ update shadow registers
    $01 TIM2 TIMx_CR1 hbis! \ enable timer 2
    ;

: pwm! ( n -- )
    100 * TIM2 TIMx_CCR4 h! ;

: pwm@ ( -- n )
    TIM2 TIMx_CCR4 h@ 100 / ;

\ delay for n us
: us ( -- )  TIM2 TIMx_CNT @ + 1000 mod begin TIM2 TIMx_CNT @ over = until drop ;

\ delay for 1ms
: 1ms ( -- ) TIM2 TIMx_CNT @ 10 + 1000 mod begin TIM2 TIMx_CNT @ over = until drop ;

\ delay for n ms
: ms ( n -- ) 0 ?do 1ms loop ;

cornerstone pcold
