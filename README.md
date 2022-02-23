# CoinLegsSignalBacktester
This application is for backtesting the recorded data of the coinlegs signals.
</br>
The data can be downloaded here
</br>
https://github.com/Lu05/CoinLegsSignalBacktesterData
</br>
and are also included as submodule inside this repository.
The backtest data will be updated regularly.
The data contains the signal, the last price at the signal time and the ticker changes for 24H. I think 24H should be enough for most signals. 
</br>
</br>
The trading bot can be found here
</br>
https://github.com/Lu05/CoinLegsSignalTrader

## Usage
At the moment there are two commands u can use.
For all commands there is a -h option to show all available parameters and options. So to get the help for backtesting run backtest -h.
### backtest
This command will backtest all data at the folder defined at the config file.
Each result will be printed at the console with the specific profit and also the max loss for this trade.
The max loss is the difference between the entry price and the lowest price(in case of long) between entry and exit.
</br>
</br>
There is also a -p option for plotting the results. This option will save an html file for each signal which will be saved at a plot folder. This folder will be created at the current directory.
</br>
Here is an example of a plot:
---placeholder---

The colors are

| Color|      Description| 
|:----------|:-------------|
red | stop loss
green | take profit
blue | entry price
grey | all the targets and SL from the signal
yellow | entry and exit

This is also really helpfull for visual debugging a strategy.
### optimize
This command will try to find the best parameters for the current strategy to maximize the profit. It will also test all the data from the folder at the config.</br>
It will run parallel and so it can be a bit CPU consuming and it will not stop at any point. To cancel it you can press Ctrl+C or simply exit the program. But don't forget to write down the parameters before exiting.
### config
The config file is the most important thing for both of the parameters.
There is a file called config.example.json at the root of this repository wich all available parameters. The parameters will be explained in detail below.
The parameters for the config file itself are
| Parameter|      Description| 
|:----------|:-------------|
DataPath | absolute path to the data folder
StrategyToUse | name of the strategy that should be used for backtesting or optimization
## Strategies
### base parameters
These are parameters which should be set for every strategy. If a parameter is not available the default value of the type will be used. Parameter types could be 
* int
* bool
* double

>Please note that all double parameters are percentage values. So 1.0 mens 100% and 0.01 means 1%.

| Parameter|      Type| Description
|:----------|:-------------|:-------------|
UseStopLoss | boolean | True if the trade should exit at SL. If false SL will be ignored
StopLoss | double | SL value that should be used for backtests
TakeProfit | double | TP value that should be used for backtests
MarketBuy | boolean | If true the trade will begin at the first price update. If false it will start at the first update where the price is higher that entry(in case of long)

All these parameters can be set for each strategy

### MarketPlaceFixedTakeProfitStrategy
This strategy will use fixed take stop loss and take profit values.</br>
Parameters are
| Parameter|      Type| Description
|:----------|:-------------|:-------------|
TakeProfitIndex | int | 1 will set TP1 from the signal, 2 TP2 and so on. Will override TakeProfit base parameter if set
UseStopLossFromSignal | boolean | true will use SL from the signal, false will use SL percentage value from the base parameter

### BlackFishMoveTakeProfitM2Strategy
The strategy is described here:
</br>
https://github.com/Lu05/CoinLegsSignalTrader

There are no additional parameters for this strategy. SL is signal SL, start TP is TP 5 and the SL will move based on the price.

### MarketPlaceTrailingStopLossStrategy
This strategy will use trailing stop loss.
Parameters are
| Parameter|      Type| Description
|:----------|:-------------|:-------------|
TrailingOffset | double | Offset from the current price to SL
TrailingStartOffset | double | min profit which needs to be reached before the trailing will start
UseStopLossFromSignal | boolean | true will use SL from the signal, false will use SL percentage value from the base parameter

## Optimize parameters
Each parameter can be optimized by the optimize algorithm. There is no magic inside. It only tests random value and if the profit is better than the last one it will be printed.
Examples can be found at the config.example.json file at the OptimizationtConfigs part.
Each optimization config hast 3 parameters
| Parameter|      Type| Description
|:----------|:-------------|:-------------|
StrategyName | string | Name of the strategy which the config applies to
BacktestParameters | json array | List of fixed parameters. All these parameters will not be optimized
OptimizationParameters | json array | All these parameters will be tested randomly.

Each OptimizationParameter needs to have 3 properties.
</br> 
| Property|      Description
|:----------|:-------------
Key | Name of the parameter from the strategy
Min | Minimal value of the parameter
Max | maximal value of the parameter

In general you can optimize any parameter of the strategy but I advice you to use values which you are willing to use at live trading.
</br> 
You could use a SL between 0 and 100% to get awesome results but do you use 100% SL for your trades?
## Implementing your own strategy
Therefore you have to inherit from StrategyBase.
</br> 
On SetParameters you can set your parameters from the config file.
</br> 
Update will be called for each price inside the data set.

## Support

If you need technical support, want to talk about this project or discuss new ideas you can find it here:
</br>
<img src="https://user-images.githubusercontent.com/3795343/154133549-215dd069-4ca3-4bc6-b3b5-d715b40689c9.png" width="100" />
</br>
https://t.me/CoinLegsSignalTrader


As described above the app is free of use.
If you are making some good money with it you have multiple possibilities to support my work.
### Ref links
You can create an account with my ref link.
#### ByBit
https://www.bybit.com/en-US/invite?ref=MOPVGP%230
<br>
Or ref code MOPVGP#0
#### Binance
https://accounts.binance.com/en/register?ref=38895065
<br>
Or ref code 38895065

### Send some crypto
#### BTC
bc1qtf3fg05xp5zau7k20sewpd0qtswfevept9v09v
#### ETH
0x419dB75736Ce12C6100fB3059485E4eBae366f05 
#### BSC
0x419dB75736Ce12C6100fB3059485E4eBae366f05