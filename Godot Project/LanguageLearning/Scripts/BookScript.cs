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
	const float pageCooldown = .5f;

	public float pageCooldownTimer;

	
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

		pageCooldownTimer -= 0f;

		AddChild(currentPage);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (pageCooldownTimer > 0)
		{
			pageCooldownTimer -= (float)delta;
		}

		/*
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
		*/

	}

	public void TurnPage(int direction)
	{
		if(pageCooldownTimer > 0)
		{
			return;
		}

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
			pageCooldownTimer = pageCooldown;

		}
		
	}

	public override void _UnhandledInput(InputEvent eventName)
	{
		
		bool spinUp = false;
		bool spinDown = false;
		bool turnLeft = false;
		bool turnRight = false;

		if(eventName.IsAction("up"))
		{
			spinUp = true;
		}
		else if (eventName.IsAction("down"))
		{
			spinDown = true;
		}
		else if (eventName.IsAction("left"))
		{
			turnLeft = true;
		}
		else if(eventName.IsAction("right"))
		{
			turnRight = true;
		}




		if(spinUp)
		{
			currentPage.SpinPage(1);
		}
		else if(spinDown)
		{
			currentPage.SpinPage(-1);
		}
		else if(turnLeft)
		{
			TurnPage(-1);
		}
		else if(turnRight)
		{
			TurnPage(1);
		}

	}

}
