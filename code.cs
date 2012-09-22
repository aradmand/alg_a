using System;
using System.Drawing;

using System.Collections.Generic;

using OpenQuant.API;
using OpenQuant.API.Indicators;

public class ExponentialMovingAverage
{
    //Inputs:
    // Period
   
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
        int size = N;
        if (N >= stockData.Count)
        {
            size = stockData.Count - 1;
        }
        Double high = 9999;
        for (int i = size; i > 0; i--)
        {
            if (i == size)
            {
                high = stockData[i].closeVal;
            }
            if (stockData[i].closeVal > high)
            {
                high = stockData[i].closeVal;
            }
        }
        return high;
    }


    public Double getLowClosingPrice(int N, List<StockDataNode> stockData)
    {
       int size = N;
        if (N >= stockData.Count)
        {
            size = stockData.Count - 1;
        }
        Double low = 9999;
        for (int i = size; i > 0; i--)
        {
            if (i == size)
            {
                low = stockData[i].closeVal;
            }
            if (stockData[i].closeVal < low)
            {
                low = stockData[i].closeVal;
            }
        }
        return low;
    }

    public void updateStochasticOscillator(DateTime timestamp,
                                           double openPrice,
                                           double highPrice,
                                           double lowPrice,
                                           double closePrice)
    {
        String output = "UpdatingStochasticOscillator: " + " " + timestamp.ToString() + " " + openPrice.ToString() + " " + highPrice + " " + lowPrice + " " + closePrice;
        Console.WriteLine(output);
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
        String output = "PercentD: " + d.ToString();
        Console.WriteLine(output);
        
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
            Console.WriteLine(calc);
            String output = "PercentK: " + k.ToString();
            Console.WriteLine(output);
        }
    }


    List<StockDataNode> percentKData = new List<StockDataNode>();
    List<StockDataNode> percentDData = new List<StockDataNode>();
}




public class MyStrategy : Strategy
{
    StochasticOscillator so;

	public override void OnStrategyStart()
	{
		System.Console.WriteLine("On strategy start");
        so = new StochasticOscillator(10, 
                                      80,
                                      20,
                                      14,
                                      3,
                                      3);
	}

	public override void OnBar(Bar bar)
	{
		//System.Console.WriteLine(bar);
        so.updateStochasticOscillator(bar.DateTime,
                                           bar.Open,
                                           bar.High,
                                           bar.Low,
                                           bar.Close);
	}

	public override void OnQuote(Quote quote)
	{	
	}

	public override void OnBarOpen(Bar bar)
	{
	
	}
}










