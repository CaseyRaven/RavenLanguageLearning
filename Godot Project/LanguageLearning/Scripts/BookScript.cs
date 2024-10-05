using Godot;
using System;

public partial class BookScript : Node3D
{
	
	[Export]
	public BookData pageSource;

	public PageData[] pages;

	public int currentPageNumber;
	public PageScript currentPage;
	public PackedScene basePage = (PackedScene)GD.Load("res://page.tscn");
	//.Instantiate();

	public PageScript nextPage;

	const float pageDistance = 15.0f;
	const float pageCooldown = .5f;
	const float swipeThreshold = 100f;


	float swipeStart = -1.0f;
	float swipeVerticalStart = -1.0f;

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


		if (eventName is InputEventScreenTouch)
		{
			float distance;
			if (((InputEventScreenTouch)eventName).Pressed)
				{
					swipeStart = ((InputEventScreenTouch)eventName).Position.X;
					swipeVerticalStart = ((InputEventScreenTouch)eventName).Position.Y;
				}
			else
			{
				if(swipeStart >= 0)
				{
					distance = ((InputEventScreenTouch)eventName).Position.X -swipeStart;
					if(Math.Abs(distance) > swipeThreshold)
					{
						if(distance < 0)
						{
							turnRight = true;
						}
						else
						{
							turnLeft = true;
						}
					}
					distance = ((InputEventScreenTouch)eventName).Position.Y -swipeVerticalStart;
					if(Math.Abs(distance) > swipeThreshold)
					{
						if(distance > 0)
						{
							spinUp = true;
						}
						else
						{
							spinDown = true;
						}
					}
					if(((InputEventScreenTouch)eventName).Position.Y -swipeVerticalStart <= swipeThreshold && 
						((InputEventScreenTouch)eventName).Position.X -swipeStart <= swipeThreshold)
					{
						Vector3 cameraOrigin = ((Camera3D)GetNode("Camera3D")).ProjectRayOrigin (((InputEventScreenTouch)eventName).Position);
						Vector3 targetRay = ((Camera3D)GetNode("Camera3D")).ProjectLocalRayNormal (((InputEventScreenTouch)eventName).Position);
						Vector3 fromPoint = cameraOrigin + targetRay;
						Vector3 toPoint = targetRay * 100.0f;
						GD.Print("Raycast: " + fromPoint + ", " + toPoint);
						var spaceState = GetWorld3D().DirectSpaceState;
						// use global coordinates, not local to node
						var query = PhysicsRayQueryParameters3D.Create(fromPoint, toPoint);
						var result = spaceState.IntersectRay(query);
						foreach( var (key, collider)/*var (collider, collider_id, normal, position, face_index, rid, shape)*/ in result)
						{
							if((((Node)result["collider"]).GetParent()) is CardScript)
							{
								((CardScript)((Node)(result["collider"])).GetParent()).ToggleLock();
								break;
							}
							//GD.Print(((CardScript)((Node)(result["collider"])).GetParent()).Name);
						}
						/*
						if(!((Godot.Collections.Dictionary)result).size()==0)
						{
							GD.Print("Raycast empty");
						}
						else
						{
							GD.Print(((Node)((Node)result["collider"]).GetParent()).Name);
						}*/
					}
				}


			}
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


		if(eventName.IsAction("exit"))
		{
			GetTree().Quit();
		}

	}


}
