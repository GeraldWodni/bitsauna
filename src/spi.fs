\ SPI driver for STM32F103
\ (c)copyright 2018, 2019 by Gerald Wodni <gerald.wodni@gmail.com>

compiletoflash

: spi-wait-ready-tx ( -- )
    begin
        SPI2 SPIx_SR h@ $02 and \ wait for TXE (Transmit buffer empty)
    until ;

: spi-wait-ready-rx ( -- )
    begin
        SPI2 SPIx_SR h@ $01 and \ wait for RXNE Receive buffer not empty)
    until ;

: spi ( x -- x )
    spi-wait-ready-tx
    SPI2 SPIx_DR h! 
    spi-wait-ready-rx
    SPI2 SPIx_DR h@ ;

\ read 32 bits
: spi-ro32 ( -- x )
    $FF spi 16 lshift
    $FF spi or ;
    
: spi-read-th1 ( -- x )
    0 th-cs1 gpio!
    spi-ro32
    -1 th-cs1 gpio! ;

: spi-read-th2 ( -- x )
    0 th-cs2 gpio!
    spi-ro32
    -1 th-cs2 gpio! ;


: init-spi
    $4000 RCC_APB1ENR bis!  \ enable SPI2 Clock

    \ set pin to SPI
    th-sck gpio-alternate
    -1 th-cs1 gpio!
    -1 th-cs2 gpio!

    \ Master Mode: Set SSM and SSI in CR1

    \     DFF Data frame format 0:8bit, 1:16bit
    \     | SSM: Software slave management
    \     | |SSI: Internal slave select
    \     | ||LSB First
    \     | |||SPIE SPI Enable
    \     | ||||BR Baud rate control
    \     | |||||  Master selection
    \     | |||||  |CPOL
    \     | |||||\ ||Cphase
    \ 111111|||||\\|||
    \ 5432109876543210
     %0000101101111100 SPI2 SPIx_CR1 h!

    \ TXEIE: TX Buffer empty interrupt enable
    \ |RXNEIE: RX Buffer not empty interrupt enable
    \ ||ERRIE: Error interrupt enable
    \ |||Reserved
    \ |||| SSOE: SS output enable
    \ |||\\|TXDMAEN: TX buffer DMA enable
    \ |||\\||RXDMAEN: RX buffer DMA enable
    \ |||\\|||
    \ 76543210
     %00000100 SPI2 SPIx_CR2 h!
     ;

: spi.. ( -- )
    begin
        cr spi-read-th1 hex.
        spi-read-th2 hex.
    key? until ;

: init init init-spi ;

