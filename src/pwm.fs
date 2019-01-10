\ PWM for STM32F103x
\ (c)copyright 2018 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

\ PWM1 on PA0: TIM2_CH1
\ PWM2 on PA2: TIM2_CH3

: init ( -- )
    init

    $01 RCC_APB1ENR bis!    \ enable timer 2 clock

    $80 TIM2 TIMx_CR1 hbis! \ enable timer 2
    72 TIM2 TIMx_PSC h!     \ prescaler (72MHz ^= 1us)

    20000 TIM2 TIMx_ARR  h! \ top register (20ms)
    1000  TIM2 TIMx_CCR1 h! \ duty cycle (1ms)
    1000  TIM2 TIMx_CCR3 h! \ duty cycle (1ms)

    $01 TIM2 TIMx_EGR hbis! \ update shadow registers
    $01 TIM2 TIMx_CR1 hbis! \ enable timer 2
    ;

\ delay for n us
: us ( -- )  TIM2 TIMx_CNT @ + 20000 mod begin TIM2 TIMx_CNT @ over = until drop ;

\ delay for 1ms
: 1ms ( -- ) TIM2 TIMx_CNT @ 1000 + 20000 mod begin TIM2 TIMx_CNT @ over = until drop ;

\ delay for n ms
: ms ( n -- ) 0 ?do 1ms loop ;

cornerstone pcold
