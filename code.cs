using System;
using System.Drawing;

using System.Collections.Generic;

using OpenQuant.API;
using OpenQuant.API.Indicators;

public class SimpleMovingAverage
{
    //Inputs:
    // Period
    int period;
    public SimpleMovingAverage(int tickPeriod)
    {
        period = tickPeriod;
    }

    public void updateSimpleMovingAverage(DateTime timestamp,
                                        double openPrice,
                                        double highPrice,
                                        double lowPrice,
                                        double closePrice)
    {
        if (smaData.Count < period)
        {
            //Do nothing, we don't have enough data yet.
            smaData.Add(new StockDataNode(timestamp, 0, openPrice, highPrice, lowPrice, closePrice));
            return;
        }

        //Use the previous periods to calculate the SMA and store it
        int size = smaData.Count - 1;
        double sum = 0;
        for (int i = 0; i < period; i++)
        {
            sum += smaData[size].closeVal;
            size--;
        }
        double d = sum / period;
        smaData.Add(new StockDataNode(timestamp, d, openPrice, highPrice, lowPrice, closePrice));
        //String output = "Simple Moving Average: " + d.ToString();
        //Console.WriteLine(output);
    }

    public double getLastSMA()
    {
        return smaData[smaData.Count - 1].value;
    }

    List<StockDataNode> smaData = new List<StockDataNode>();
}


public class ExponentialMovingAverage
{
    //Inputs:
    // Period

    int period;
 
    public ExponentialMovingAverage(int tickPeriod)
    {
        period = tickPeriod;
        sma = new SimpleMovingAverage(tickPeriod);
    }

    public void updateExpentialMovingAverage(DateTime timestamp,
                                        double openPrice,
                                        double highPrice,
                                        double lowPrice,
                                        double closePrice)
    {
        if (emaData.Count < period)
        {
            //Do nothing, we don't have enough data yet.
            sma.updateSimpleMovingAverage(timestamp, openPrice, highPrice, lowPrice, closePrice);
            //Use previous day's SMA as the starting EMA
            emaData.Add(new StockDataNode(timestamp, sma.getLastSMA(), openPrice, highPrice, lowPrice, closePrice));
            
            return;
        }
        sma.updateSimpleMovingAverage(timestamp, openPrice, highPrice, lowPrice, closePrice);

        double simpleMovingAvg = sma.getLastSMA();

        double multiplier = 2.00 / (period + 1.00);
       
        double ema = (closePrice - this.getLastEMA() ) * multiplier + this.getLastEMA();

        emaData.Add(new StockDataNode(timestamp, ema, openPrice, highPrice, lowPrice, closePrice));

        //String calcStr = "("+closePrice.ToString() + " - " + this.getLastEMA().ToString() + ") * " + multiplier.ToString() + " + " + this.getLastEMA().ToString();
        //Console.WriteLine("CALCSTR ==== " + calcStr);

        //String output = "EMA: " + ema.ToString();
        //Console.WriteLine(output);

        
    }

    public double getLastEMA()
    {
        return emaData[emaData.Count - 1].value;
    }

    SimpleMovingAverage sma;
    List<StockDataNode> emaData = new List<StockDataNode>();
}

public class StockDataNode
{
    public StockDataNode(DateTime time,
                         Double val,
						 Double open,
						 Double high,
						 Double low,
						 Double close)
    {
        timestamp = time;
        value = val;
		openVal = open;
		highVal = high;
		lowVal = low;
		closeVal = close;
    }
    public DateTime timestamp;
    public Double value;
	public Double openVal;
	public Double highVal;
	public Double lowVal;
	public Double closeVal;
}

public class StochasticOscillator
{
    //Inputs:
    // Ticker_period(in minutes) - how often we want the S.O. to be calculated. (e.g. to get a new S.O. datapoint every minute, set this to one)
    // High_Threshold
    // Low_Threshold
    // N_period_for_percent_K
    // Smoothing_value_for_percent_D
    // M_period_for_percent_D

    //Calculations:
    // Fast_Percent_K
    // Fast_Percent_D
    
    private double tickerPeriod;
    private double highThreshold;
    private double lowThreshold;
    private int nPeriodForPercentK;
    private double smoothingForPercentD;
    private int mPeriodForPercentD;

    public StochasticOscillator(double tickPeriod,
                         double highThres,
                         double lowThres,
                         int nPeriod,
                         double smooth,
                         int mPeriod)
    {
        tickerPeriod = tickPeriod;
        highThreshold = highThres;
        lowThreshold = lowThres;
        nPeriodForPercentK = nPeriod;
        smoothingForPercentD = smooth;
        mPeriodForPercentD = mPeriod;
    }


    public Double getHighClosingPrice(int N, List<StockDataNode> stockData)
    {
        if (stockData.Count <= 0)
        {
            return 0;
        }
        int size = stockData.Count - 1;
        if (N >= stockData.Count)
        {
            size = stockData.Count - 1;
        }
        Double high = -1;
        for (int i = 0; i < N; i++)
        {
            if (size < 0)
            {
                break;
            }
            if (high == -1)
            {
                high = stockData[size].closeVal;
            }
            if (stockData[size].closeVal > high)
            {
                high = stockData[size].closeVal;
            }
            size--;
        }

        return high;
    }


    public Double getLowClosingPrice(int N, List<StockDataNode> stockData)
    {
        if (stockData.Count <= 0)
        {
            return 0;
        }
        int size = stockData.Count - 1;
        if (N >= stockData.Count)
        {
            size = stockData.Count - 1;
        }
        Double low = 9999;
        for (int i = 0; i < N; i++)
        {
            if (size < 0)
            {
                break;
            }
            if (low == 9999)
            {
                low = stockData[size].closeVal;
            }
            if (stockData[size].closeVal < low)
            {
                low = stockData[size].closeVal;
            }
            size--;
        }

        return low;
    }

    public void updateStochasticOscillator(DateTime timestamp,
                                           double openPrice,
                                           double highPrice,
                                           double lowPrice,
                                           double closePrice)
    {
        //String output = "UpdatingStochasticOscillator: " + " " + timestamp.ToString() + " Open=" + openPrice.ToString() + " High=" + highPrice + " Low=" + lowPrice + " Close=" + closePrice;
        //Console.WriteLine(output);
        updateFastPercentK(timestamp,
                           openPrice,
                           highPrice,
                           lowPrice,
                           closePrice);

        updateFastPercentD(timestamp,
                           openPrice,
                           highPrice,
                           lowPrice,
                           closePrice);
    }



    private void updateFastPercentD(DateTime timestamp,
                            double openPrice,
                            double highPrice,
                            double lowPrice,
                            double closePrice)
    {
        if (percentKData.Count < mPeriodForPercentD)
        {
            //Do nothing, we don't have enough data yet.
            return;
        }
        
        //Use the previous M periods to calculate the current fast %D, and store it
        int size = percentKData.Count - 1;
        double sum = 0;
        for (int i = 0; i < mPeriodForPercentD; i++)
        {
            sum += percentKData[size].value;
            size--;
        }
        double d = sum / mPeriodForPercentD;
        percentDData.Add(new StockDataNode(timestamp, d, openPrice, highPrice, lowPrice, closePrice));
        //String output = "PercentD: " + d.ToString();
        //Console.WriteLine(output);
        
    }


    private void updateFastPercentK(DateTime timestamp, 
                            double openPrice,
                            double highPrice,
                            double lowPrice,
                            double closePrice)
                            
    {
        if (percentKData.Count < tickerPeriod)
        {
            percentKData.Add(new StockDataNode(timestamp, 0, openPrice, highPrice, lowPrice, closePrice));
        }
        else
        {
            //Use the previous N periods to calculate the current fast %k, and store it
            percentKData.Add(new StockDataNode(timestamp, 0, openPrice, highPrice, lowPrice, closePrice));
            double currentClosingPrice = closePrice;
            double low = getLowClosingPrice(nPeriodForPercentK, percentKData);
            double high = getHighClosingPrice(nPeriodForPercentK, percentKData);
            double k = (currentClosingPrice - low) / (high - low);
            percentKData[percentKData.Count - 1].value = k;
            String calc ="current: " + currentClosingPrice.ToString() + "\n";
            calc += "low: " + low.ToString() + "\n";
            calc += "high: " + high.ToString();
            //Console.WriteLine(calc);
            //String output = "PercentK: " + k.ToString();
            //Console.WriteLine(output);
        }
    }

    public double getLastPercentK()
    {
        if (percentKData.Count == 0) {
           return 0;
        }
        return percentKData[percentKData.Count - 1].value;
    }


    public double getLastPercentD()
    {
        if (percentDData.Count == 0)
        {
            return 0;
        }
        return percentDData[percentDData.Count - 1].value;
    }

    List<StockDataNode> percentKData = new List<StockDataNode>();
    List<StockDataNode> percentDData = new List<StockDataNode>();
}




public class MyStrategy : Strategy
{
    StochasticOscillator so;
    ExponentialMovingAverage ema;

	public override void OnStrategyStart()
	{
        //Stochastic Variables
        double tickPeriod = 10;
        double highThres = 80;
        double lowThres = 20;
        int nPeriod = 14;
        double smooth = 3;
        int mPeriod = 3;

        //Exponential Moving Avg. Variables
        int emaPeriod = 10;

		System.Console.WriteLine("On strategy start");
        so = new StochasticOscillator(tickPeriod, 
                                      highThres,
                                      lowThres,
                                      nPeriod,
                                      smooth,
                                      mPeriod);

        ema = new ExponentialMovingAverage(emaPeriod);
	}

	public override void OnBar(Bar bar)
	{
        so.updateStochasticOscillator(bar.DateTime,
                                           bar.Open,
                                           bar.High,
                                           bar.Low,
                                           bar.Close);

        ema.updateExpentialMovingAverage(bar.DateTime, bar.Open, bar.High, bar.Low, bar.Close);

        //Prints Timestamp, %k, %D, EMA, ClosePrice
        String output = bar.DateTime.ToString() + "\t" + so.getLastPercentK() + "\t" + so.getLastPercentD() + "\t" + ema.getLastEMA().ToString() + "\t" + bar.Close;
        Console.WriteLine(output);
	}

	public override void OnQuote(Quote quote)
	{	
	}

	public override void OnBarOpen(Bar bar)
	{
	
	}
}










