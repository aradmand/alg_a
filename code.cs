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
                         Double val)
    {
        timestamp = time;
        value = val;
    }
    public DateTime timestamp;
    public Double value;
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
    private double nPeriodForPercentK;
    private double smoothingForPercentD;
    private double mPeriodForPercentD;

    public StochasticOscillator(double tickPeriod,
                         double highThres,
                         double lowThres,
                         double nPeriod,
                         double smooth,
                         double mPeriod)
    {
        tickerPeriod = tickPeriod;
        highThreshold = highThres;
        lowThreshold = lowThres;
        nPeriodForPercentK = nPeriod;
        smoothingForPercentD = smooth;
        mPeriodForPercentD = mPeriod;
    }

    public void updateStochasticOscillator(DateTime timestamp,
                                           double openPrice,
                                           double highPrice,
                                           double lowPrice,
                                           double closePrice)
    {
        String output = "UpdatingStochasticOscillator: " + " " + timestamp.ToString() + " " + openPrice.ToString() + " " + highPrice + " " + lowPrice;
        Console.WriteLine(output);
        updateFastPercentK(timestamp,
                           openPrice,
                           highPrice,
                           lowPrice,
                           closePrice);
    }


    private void updateFastPercentK(DateTime timestamp, 
                            double openPrice,
                            double highPrice,
                            double lowPrice,
                            double closePrice)
                            
    {
        if (percentKData.Count < tickerPeriod)
        {
            percentKData.Add(new StockDataNode(timestamp, openPrice));
        }
        else
        {
            percentKData.Add(new StockDataNode(timestamp, openPrice));
            Console.WriteLine("OPEN pRICE");

            double currentClosePrice = closePrice;
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









