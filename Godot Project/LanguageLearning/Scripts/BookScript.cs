using Godot;
using System;

public partial class BookScript : Node
{
	
	[Export]
	public BookData pageSource;

	public PageData[] pages;

	public int currentPageNumber;
	public PageScript currentPage;
	public PackedScene basePage = (PackedScene)GD.Load("res://page.tscn");
	//.Instantiate();

	public PageScript nextPage;

	const float pageDistance = 10.0f;

	
	Random randomTester;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		randomTester = new Random();


		pages = new PageData[pageSource.pages.Length];
		for (int i = 0; i < pages.Length; i++)
		{
			pages[i] = pageSource.pages[i];
		}
		currentPageNumber = 0;
		currentPage = (PageScript)basePage.Instantiate();
		currentPage.data = pages[currentPageNumber];


		AddChild(currentPage);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if(randomTester.Next(0,1000) > 998)
		{
			currentPage.SpinPage(1);
		}
		else if(randomTester.Next(0,1000) > 998) // technically not equal odds. I don't care.
		{
			currentPage.SpinPage(-1);
		}
		else if(randomTester.Next(0,1000) > 998)
		{
			TurnPage(1);
		}
		else if(randomTester.Next(0,1000) > 998)
		{
			TurnPage(-1);
		}

	}

	public void TurnPage(int direction)
	{
		if(direction != 0 && currentPageNumber+ direction >= 0 && currentPageNumber+ direction < pages.Length)
		{
			if(nextPage!=null)
			{
				nextPage.QueueFree();
			}
			currentPageNumber = currentPageNumber + direction;

			currentPage.MakeInactive();
			currentPage.SlideTo( -direction * pageDistance);
			nextPage = (PageScript)basePage.Instantiate();
			nextPage.data = pages[currentPageNumber];
			nextPage.stage = currentPage.stage;
			nextPage.Position = new Vector3(direction *pageDistance, nextPage.Position[1], nextPage.Position[2]);
			AddChild(nextPage);
			nextPage.SlideTo(0);
			nextPage.MakeActive();
			PageScript temp = currentPage;
			currentPage = nextPage;
			nextPage = temp;

		}
		
	}

}
