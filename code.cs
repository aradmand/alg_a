using System;
using System.Drawing;

using System.Collections.Generic;

using OpenQuant.API;
using OpenQuant.API.Indicators;


public class DecisionUnit
{
    Scenario currentScenario;

    public DecisionUnit()
    {
        currentScenario = Scenario.none;

    }

    public void run(DecisionData data)
    {
        if (currentScenario == Scenario.none)
        {
            if (triggerScenario1(data))
            {
                //Trigger buy action for scenario1
                currentScenario = Scenario.scenario1;
            }
            else if (triggerScenario2(data))
            {
                //Trigger buy action for scenario2
                currentScenario = Scenario.scenario2;
            }
            else if (triggerScenario3(data))
            {
                //Trigger buy action for scenario3
                currentScenario = Scenario.scenario3;
            }

        }
        else
        {
            switch (currentScenario)
            {
                case Scenario.scenario1:
                    handleScenario1(data);
                    break;

                case Scenario.scenario2:
                    break;

            }
        }
    }

    //Returns true if current data point is within s standard deviations from the regression line
    // false otherwise.
    Boolean dataDropsFromRegression(List<double> coords, int stDev = 2)
    {

        return false;

    }

    class RegressionData
    {
        public double a;
        public double b;
        public double standardDev;
        public List<double> yVals;
    }

    RegressionData calculateRegression(List<double> coords)
    {
        double xySum = 0;
        double xSquaredSum = 0;
        double ySquaredSum = 0;
        double ySum = 0;
        double xSum = 0;
        double mean;
        for (int i = 1; i < coords.Count; i++)
        {
            double x = i;
            double xSquared = x * x;
            double y = coords[i - 1];
            double ySquared = y * y;
            double xy = x * y;
            xSum += x;
            ySum += y;
            xySum += xy;
            xSquaredSum += xSquared;
            ySquaredSum += ySquared;
        }

        double a = ( (ySum * xSquaredSum) - (xSum * xySum) ) / ( (coords.Count * xSquaredSum) - (xSum * xSum) );
        double b = ((coords.Count * xySum) - (xSum * ySum) ) / ( ( coords.Count * xSquaredSum) - (xSum * xSum) );

        //Determine regression line
        // y = a + bx
        List<double> yData = new List<double>();
        for (int i = 1; i < coords.Count; i++)
        {
            double x = i;
            double y = a + (b * x);
            yData.Add(y);
        }

        RegressionData regData = new RegressionData();
        regData.yVals = yData;

        //Calculate standard deviation of regression line
        ySum = 0;
        //First calculate sum
        for (int i = 1; i < yData.Count; i++)
        {
            ySum += yData[i - 1];
        }

        mean = ySum / yData.Count;

        //Next calculate deviations
        List<double> deviations = new List<double>();
        double devSquaredSum = 0;
        for (int i = 1; i < yData.Count; i++)
        {
            double dev = yData[i - 1] - mean;
            double devSquared = dev * dev;
            devSquaredSum += devSquared;
        }

        double standardDev = System.Math.Sqrt(devSquaredSum / (yData.Count - 1));
        regData.a = a;
        regData.b = b;
        regData.standardDev = standardDev;
        regData.yVals = yData;
        return regData;
    }

    void handleScenario1(DecisionData data)
    {
        //Sell Trigger1 data
        double thirdEma45 = data.ema_45.getLastNEMA(2);
        double previousEma45 = data.ema_45.getLastNEMA(1);
        double currentEma45 = data.ema_45.getLastNEMA(0);

        //Sell Trigger2 data
        double thirdSo180BPercentk = data.so_180_b.getLastNPercentK(2);
        double previousSo180BPercentk = data.so_180_b.getLastNPercentK(1);
        double currentSo180BPercentk = data.so_180_b.getLastNPercentK(0);
        double currentSo180BUpperThreshold = data.so_180_b.getHighThreshold();

        //Sell Trigger3 data



        Boolean sellTrigger1 = false; // [45] EMA is negative for 2 candles
        Boolean sellTrigger2 = false; // [180]21,4,5 is Above the 80 threshold and %K slope goes neg.
                                      // && [45] EMA goes negative
       
        Boolean sellTrigger3 = false; //[45] sell if after 10 candles, 40% of profit is lost
        Boolean sellTrigger4 = false; // sell if price drops 3% below buy price
        Boolean sellTrigger5 = false; // sell half of position at 4% profit
        Boolean sellTrigger6 = false; // create regression line after 10 candles and sell if next candle drops out of regression



        if (thirdEma45 > previousEma45 &&
            previousEma45 > currentEma45)
        {
            sellTrigger1 = true;
            //SELL!
        }


        if ((currentSo180BPercentk > currentSo180BUpperThreshold &&
            previousSo180BPercentk > currentSo180BPercentk)
            
            &&
       
            (previousEma45 > currentEma45))
        {
            sellTrigger2 = true;
            //Sell!
        }


                    
    }


    public enum Scenario
    {
        none,
        scenario1,
        scenario2,
        scenario3
    }

    public Boolean triggerScenario1(DecisionData data)
    {
        //Trigger 1 data
        double previousEma45 = data.ema_45.getLastNEMA(1);
        double currentEma45 = data.ema_45.getLastNEMA(0);


        //Trigger 2 data
        double currentSo180BPercentk = data.so_180_b.getLastNPercentK(0);
        double currentSo180BLowerThreshold = data.so_180_b.getLowThreshold();
        int lowerThresholdAllowance = 10;

        //Trigger 3 data
        double previousSo45APercentK = data.so_45_a.getLastNPercentK(1);
        double currentSo45APercentK = data.so_45_a.getLastNPercentK(0);

        double previousSo45APercentD = data.so_45_a.getLastNPercentD(1);
        double currentSo45APercentD = data.so_45_a.getLastNPercentD(0);

        double previousSo45BPercentK = data.so_45_b.getLastNPercentK(1);
        double currentSo45BPercentK = data.so_45_b.getLastNPercentK(0); 
        
        double previousSo45BPercentD = data.so_45_b.getLastNPercentD(1);
        double currentSo45BPercentD = data.so_45_b.getLastNPercentD(0);

        Boolean trigger1 = false; //EMA[45] has positive slope
        Boolean trigger2 = false; // [180] 14,4,5 gives buy signal
        Boolean trigger3 = false; //[45] both stochastics are positive

        if (previousEma45 < currentEma45)
        {
            trigger1 = true;

        }


        if (currentSo180BPercentk > currentSo180BLowerThreshold &&
            currentSo180BPercentk < (currentSo180BLowerThreshold + lowerThresholdAllowance))
        {
            trigger2 = true;
        }


        if ((previousSo45APercentK < currentSo45APercentK) &&
             (previousSo45APercentD < currentSo45APercentD) &&
             (previousSo45BPercentK < currentSo45BPercentK) &&
             (previousSo45BPercentD < currentSo45BPercentD))
        {
            trigger3 = true;
        }


        return (trigger1 && trigger2 && trigger3);
    }

    public Boolean triggerScenario2(DecisionData data)
    {
        //Trigger 1 data
        double previousEma45 = data.ema_45.getLastNEMA(1);
        double currentEma45 = data.ema_45.getLastNEMA(0);

        //Trigger 2 data
        double currentSo180APercentk = data.so_180_a.getLastNPercentK(0);
        double currentSo180ALowerThreshold = data.so_180_a.getLowThreshold();
        int lowerThresholdAllowance = 10;

        Boolean trigger1 = false; //EMA[45] has positive slope
        Boolean trigger2 = false; // [180] 14,3,3 gives buy signal


        if (previousEma45 < currentEma45)
        {
            trigger1 = true;
        }

        if (currentSo180APercentk > currentSo180ALowerThreshold &&
            currentSo180APercentk < (currentSo180ALowerThreshold + lowerThresholdAllowance))
        {
            trigger2 = true;
        }

        return (trigger1 && trigger2);
    }

    public Boolean triggerScenario3(DecisionData data)
    {
        return false;
    }

}

public class DecisionData
{
    public StochasticOscillator so_180_a;
    public StochasticOscillator so_180_b;

    public StochasticOscillator so_45_a;
    public StochasticOscillator so_45_b;

    public ExponentialMovingAverage ema_45;
    public ExponentialMovingAverage ema_180;
}

public class Bulker
{
    //Inputs:
    // Period
    public int bulkPeriod; //Will bulk together 'bulkPeriod' periods and treat it as one Period

    List<StockDataNode> bulkData = new List<StockDataNode>();
    List<StockDataNode> periodData = new List<StockDataNode>();

    public Bulker(int bulk)
    {
        bulkPeriod = bulk;
    }

    public StockDataNode getLastPeriodData()
    {
        if (periodData.Count <= 0)
        {
            return null;
        }
        return periodData[periodData.Count - 1];
    }

    //Returns -1 if the bulker has not yet built up one period
    //Returns 1 otherwise
    public int update(DateTime timestamp,
                                        double openPrice,
                                        double highPrice,
                                        double lowPrice,
                                        double closePrice)
    {
        //Do nothing, we don't have enough data yet.
        bulkData.Add(new StockDataNode(timestamp, 0, openPrice, highPrice, lowPrice, closePrice));

        if (bulkData.Count == 1)
        {
            return -1;
        }
        if (bulkData.Count % bulkPeriod == 0)
        {
            //Calculate the high, low, open, and close
            double high = getHighPrice(bulkPeriod, bulkData);
            double low = getLowPrice(bulkPeriod, bulkData);
            double open = bulkData[bulkData.Count - bulkPeriod].openVal;
            double close = closePrice;
            periodData.Add(new StockDataNode(timestamp, 0, openPrice, highPrice, lowPrice, closePrice));
        }
        return 1;
    }

    public Double getHighPrice(int N, List<StockDataNode> stockData)
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
                high = stockData[size].highVal;
            }
            if (stockData[size].highVal > high)
            {
                high = stockData[size].highVal;
            }
            size--;
        }

        return high;
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

    public Double getLowPrice(int N, List<StockDataNode> stockData)
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
                low = stockData[size].lowVal;
            }
            if (stockData[size].lowVal < low)
            {
                low = stockData[size].lowVal;
            }
            size--;
        }

        return low;
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

}

public class SimpleMovingAverage
{
    //Inputs:
    // Period
    int period;

    Bulker bulker;
    public SimpleMovingAverage(int tickPeriod, int bulkPer)
    {
        period = tickPeriod;
        bulker = new Bulker(bulkPer);
    }

    public void updateSimpleMovingAverage(DateTime timestamp,
                                        double openPrice,
                                        double highPrice,
                                        double lowPrice,
                                        double closePrice)
    {
        int evalPeriod = bulker.update(timestamp,
                                         openPrice,
                                         highPrice,
                                         lowPrice,
                                         closePrice);
        if (evalPeriod == -1)
        {
            //We haven't built up a period yet
            return;
        }

        StockDataNode node = bulker.getLastPeriodData();
        openPrice = node.openVal;
        highPrice = node.highVal;
        lowPrice = node.lowVal;
        closePrice = node.closeVal;

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

    Bulker bulker;
 
    public ExponentialMovingAverage(int tickPeriod, int bulkPer)
    {
        period = tickPeriod;
        bulker = new Bulker(bulkPer);
        sma = new SimpleMovingAverage(tickPeriod, bulkPer);
    }

    public void updateExpentialMovingAverage(DateTime timestamp,
                                        double openPrice,
                                        double highPrice,
                                        double lowPrice,
                                        double closePrice)
    {
        int evalPeriod = bulker.update(timestamp,
                                         openPrice,
                                         highPrice,
                                         lowPrice,
                                         closePrice);
        if (evalPeriod == -1)
        {
            //We haven't built up a period yet
            return;
        }

        StockDataNode node = bulker.getLastPeriodData();
        openPrice = node.openVal;
        highPrice = node.highVal;
        lowPrice = node.lowVal;
        closePrice = node.closeVal;

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

    //Grabs the EMA value from N periods ago
    //i.e. getLastNEMA(2) returns the EMA Stock Data for 2 periods ago:
    //   per0 per1 per2 per3 [Current Per]
    // getLastNEMA(2) would return per2
    public double getLastNEMA(int n)
    {
        period = n;
        if( (period <= 0) || (emaData.Count - period) <= 0)
        {
            period = 0;
        }

        return emaData[emaData.Count - 1 - period].value;
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

    public StockDataNode(DateTime time,
                     Double val,
                     Double open,
                     Double high,
                     Double low,
                     Double close,
                     Double val2)
    {
        timestamp = time;
        value = val;
        openVal = open;
        highVal = high;
        lowVal = low;
        closeVal = close;
        value2 = val2;
    }

    public DateTime timestamp;
    public Double value;
	public Double openVal;
	public Double highVal;
	public Double lowVal;
	public Double closeVal;
    public Double value2;
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
    
    private int tickerPeriod;
    private double highThreshold;
    private double lowThreshold;
    private int nPeriodForPercentK;
    private double smoothingForPercentD;
    private int mPeriodForPercentD;

    Bulker bulker;
    public StochasticOscillator(int tickPeriod,
                           int bulkPer,
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
        bulker = new Bulker(bulkPer);
    }

    public double getLowThreshold()
    {
        return lowThreshold;
    }

    public double getHighThreshold()
    {
        return highThreshold;
    }


    public Double getHighPrice(int N, List<StockDataNode> stockData)
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
                high = stockData[size].highVal;
            }
            if (stockData[size].highVal > high)
            {
                high = stockData[size].highVal;
            }
            size--;
        }

        return high;
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

    public Double getLowPrice(int N, List<StockDataNode> stockData)
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
                low = stockData[size].lowVal;
            }
            if (stockData[size].lowVal < low)
            {
                low = stockData[size].lowVal;
            }
            size--;
        }

        return low;
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
        int evalPeriod = bulker.update(timestamp,
                                         openPrice,
                                         highPrice,
                                         lowPrice,
                                         closePrice);
        if (evalPeriod == -1)
        {
            //We haven't built up a period yet
            return;
        }

        StockDataNode node = bulker.getLastPeriodData();
        openPrice = node.openVal;
        highPrice = node.highVal;
        lowPrice = node.lowVal;
        closePrice = node.closeVal;

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
            sum += percentKData[size].value2;
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
            double low = getLowPrice(nPeriodForPercentK, percentKData);
            double high = getHighPrice(nPeriodForPercentK, percentKData);
            double k = (currentClosingPrice - low) / (high - low);

            //Find the average of %k over the last SMOOTHING number of periods
            double kSum = k;
            int index = percentKData.Count - 2; //-2 to account for the above added node
            for (int i = 0; i < smoothingForPercentD - 1; i++)
            {
                kSum += percentKData[index].value;
                index--;
            }
            double avgK = kSum / smoothingForPercentD;

            percentKData[percentKData.Count - 1].value = k;
            percentKData[percentKData.Count - 1].value2 = avgK;
            String calc ="current: " + currentClosingPrice.ToString() + "\n";
            calc += "low: " + low.ToString() + "\n";
            calc += "high: " + high.ToString();
            //Console.WriteLine(calc);
            //String output = "PercentK: " + k.ToString();
            //Console.WriteLine(output);
        }
    }

    public double getLastNPercentK(int n)
    {
        int period = n;
        if ((period <= 0) || (percentKData.Count - period) <= 0)
        {
            period = 0;
        }
        return percentKData[percentKData.Count - 1 - period].value;
    }

    public double getLastPercentK()
    {
        if (percentKData.Count == 0) {
           return 0;
        }
        return percentKData[percentKData.Count - 1].value;
    }

    public double getLastSmoothedPercentK()
    {
        if (percentKData.Count == 0)
        {
            return 0;
        }
        return percentKData[percentKData.Count - 1].value2;
    }


    public double getLastNPercentD(int n)
    {
        int period = n;
        if ((period <= 0) || (percentDData.Count - period) <= 0)
        {
            period = 0;
        }
        return percentDData[percentDData.Count - 1 - period].value;
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
    DecisionUnit decisionUnit;

    StochasticOscillator so_180_a;
    StochasticOscillator so_180_b;
    
    StochasticOscillator so_45_a;
    StochasticOscillator so_45_b;
   
    ExponentialMovingAverage ema_45;
    ExponentialMovingAverage ema_180;

	public override void OnStrategyStart()
	{
        decisionUnit = new DecisionUnit()
        //so_180_a Stochastic Variables
        int so_180_a_tickPeriod = 10;
        int so_180_a_bulkPeriod = 180;
        double so_180_a_highThres = 80;
        double so_180_a_lowThres = 20;
        int so_180_a_nPeriod = 14;
        double so_180_a_smooth = 3;
        int so_180_a_mPeriod = 3;

        //so_180_b Stochastic Variables
        int so_180_b_tickPeriod = 10;
        int so_180_b_bulkPeriod = 180;
        double so_180_b_highThres = 80;
        double so_180_b_lowThres = 20;
        int so_180_b_nPeriod = 21;
        double so_180_b_smooth = 4;
        int so_180_b_mPeriod = 5;


        //so_45_a Stochastic Variables
        int so_45_a_tickPeriod = 10;
        int so_45_a_bulkPeriod = 45;
        double so_45_a_highThres = 80;
        double so_45_a_lowThres = 20;
        int so_45_a_nPeriod = 14;
        double so_45_a_smooth = 3;
        int so_45_a_mPeriod = 3;

        //so_45_b Stochastic Variables
        int so_45_b_tickPeriod = 10;
        int so_45_b_bulkPeriod = 45;
        double so_45_b_highThres = 80;
        double so_45_b_lowThres = 20;
        int so_45_b_nPeriod = 21;
        double so_45_b_smooth = 4;
        int so_45_b_mPeriod = 5;

        //ema_45 Exponential Moving Avg. Variables
        int ema_45_period = 10;
        int ema_45_bulkPeriod = 45;

        //ema_180 Exponential Moving Avg. Variables
        int ema_180_period = 10;
        int ema_180_bulkPeriod = 180;

		System.Console.WriteLine("On strategy start");
        so_180_a = new StochasticOscillator(so_180_a_tickPeriod, 
                                            so_180_a_bulkPeriod,
                                            so_180_a_highThres,
                                            so_180_a_lowThres,
                                            so_180_a_nPeriod,
                                            so_180_a_smooth,
                                            so_180_a_mPeriod);


        so_180_b = new StochasticOscillator(so_180_b_tickPeriod,
                                            so_180_b_bulkPeriod,
                                            so_180_b_highThres,
                                            so_180_b_lowThres,
                                            so_180_b_nPeriod,
                                            so_180_b_smooth,
                                            so_180_b_mPeriod);

        so_45_a = new StochasticOscillator(so_45_a_tickPeriod,
                                    so_45_a_bulkPeriod,
                                    so_45_a_highThres,
                                    so_45_a_lowThres,
                                    so_45_a_nPeriod,
                                    so_45_a_smooth,
                                    so_45_a_mPeriod);

        so_45_b = new StochasticOscillator(so_45_b_tickPeriod,
                                    so_45_b_bulkPeriod,
                                    so_45_b_highThres,
                                    so_45_b_lowThres,
                                    so_45_b_nPeriod,
                                    so_45_b_smooth,
                                    so_45_b_mPeriod);

        ema_180 = new ExponentialMovingAverage(ema_180_period, ema_180_bulkPeriod);
        ema_45 = new ExponentialMovingAverage(ema_45_period, ema_45_bulkPeriod);

	}

	public override void OnBar(Bar bar)
	{
        so_180_a.updateStochasticOscillator(bar.DateTime,
                                           bar.Open,
                                           bar.High,
                                           bar.Low,
                                           bar.Close);

        so_180_b.updateStochasticOscillator(bar.DateTime,
                                           bar.Open,
                                           bar.High,
                                           bar.Low,
                                           bar.Close);

        so_45_a.updateStochasticOscillator(bar.DateTime,
                                           bar.Open,
                                           bar.High,
                                           bar.Low,
                                           bar.Close);

        so_45_b.updateStochasticOscillator(bar.DateTime,
                                           bar.Open,
                                           bar.High,
                                           bar.Low,
                                           bar.Close);

        ema_180.updateExpentialMovingAverage(bar.DateTime, bar.Open, bar.High, bar.Low, bar.Close);
        ema_45.updateExpentialMovingAverage(bar.DateTime, bar.Open, bar.High, bar.Low, bar.Close);

        decisionUnit.run();

        //Prints Timestamp, %k, %D, EMA, ClosePrice
        //String output = bar.DateTime.ToString() + "\t" + so_180.getLastPercentK() + "\t" + so_180.getLastSmoothedPercentK() + "\t" + so_180.getLastPercentD() + "\t" + ema.getLastEMA().ToString() + "\t" + bar.Close;
        //Console.WriteLine(output);
	}

	public override void OnQuote(Quote quote)
	{	
	}

	public override void OnBarOpen(Bar bar)
	{
	
	}
}










