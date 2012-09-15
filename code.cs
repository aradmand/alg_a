using System;
using System.Drawing;

using OpenQuant.API;
using OpenQuant.API.Indicators;

public class test{
	public test(){}
	 public int A;	
}

public class MyStrategy : Strategy
{

	test t;
	
	public override void OnStrategyStart()
	{
		t = new test();
		t.A = 10;
		System.Console.WriteLine("On strategy start");
		System.Console.WriteLine(t.A);
		
	}

	public override void OnBar(Bar bar)
	{
		t.A++;
		System.Console.WriteLine(t.A);
		System.Console.WriteLine(bar);
	}

	public override void OnQuote(Quote quote)
	{
		System.Console.WriteLine("ON QUOTE!");
		System.Console.WriteLine(quote);	
	}

	public override void OnBarOpen(Bar bar)
	{
	//	System.Console.WriteLine(bar);
	}
}






