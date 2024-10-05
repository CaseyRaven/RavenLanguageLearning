using Godot;
using System;
using System.Collections.Generic;

public partial class PageScript : Node3D
{
	/*
		A page is a single panel of a book.
		A different script will be used to read user input to control navigation between pages.
		Because a page may be loaded while a diferent page is active, the function MakeActive will be called when a page becomes active.
	*/

	[Export]
	public PageData data;


	bool muted = false;

	public int stage;
	int maxStage;
	int audioLanguageSwitchStage;

	//English Audio file
	public String audioFileNative;
	//Other Language Audio File
	public String audioFileForeign;

	public CardScript[] cards;

	public const float spinCooldown = .5f;
	public float spinTimer = 0;

	//Image File
	public PackedScene baseCard;

	public Texture2D pageScene;

	[Export]
	public AspectRatioContainer scaleController;

	public AudioStream stream;
	public AudioStreamPlayer sound;

	public bool verticalLayout;

	float movementTarget;
	const float movementSpeed = 10.0f;

	public void MakeActive()
	{
		/*
		if(!muted)
		{
			sound.Play();
		}
		*/
		GetNode<CanvasItem>("./AspectRatioContainer").Visible = true; // CanvasItem.Visible

	}

	public void MakeInactive()
	{
		/*

		sound.Stop();

		*/
		GetNode<AspectRatioContainer>("./AspectRatioContainer").Visible = false; // CanvasItem.Visible
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int nameCount = 0;

		pageScene = data.panel;
		
		GetNode<TextureRect>("./AspectRatioContainer/TextureRect").Texture = pageScene;

		baseCard = (PackedScene)GD.Load("res://card.tscn");

		cards = new CardScript[data.cards.Length];// Needs to be filled with appropriate cards still

		for(int i = 0; i < data.cards.Length; i++)
		{
			cards[i] = (CardScript)baseCard.Instantiate();
			cards[i].source = data.cards[i];
			if(!data.cards[i].locked)
				cards[i].currentDifficulty = stage; // Some weirdness with max difficulty defined below
			else
				cards[i].currentDifficulty = data.cards[i].currentDifficulty;
			if(cards[i].source.type == 1 && data.names != null && nameCount < data.names.Length)
			{
				cards[i].source.word = data.names[nameCount++];
			}
			AddChild(cards[i]);
			maxStage = Math.Max(maxStage, cards[i].maxDifficulty);

		}

		//maxStage = cards[0].maxDifficulty;

		AdjustSize();

		EvenlySpaceCards();

		// Move this to a function
		String activeAudioFile;
		if (stage >= audioLanguageSwitchStage)
		{
			activeAudioFile = audioFileForeign;
		}
		else
		{
			activeAudioFile = audioFileNative;
		}

		/*
		if(activeAudioFile != "")
		{
		var file = FileAccess.Open(activeAudioFile, FileAccess.ModeFlags.Read);

		if(activeAudioFile.EndsWith(".wav"))
		{
			stream = new AudioStreamWav();
			((AudioStreamWav)stream).Data = file.GetBuffer((long)file.GetLength());
		}
		else if(activeAudioFile.EndsWith(".mp3"))
		{
			stream = new AudioStreamMP3();
			((AudioStreamMP3)stream).Data = file.GetBuffer((long)file.GetLength());
		}
		// Add any additional formats
		else
		{
			stream = new AudioStream(); // This probably errors out.
		}
		//stream.Data = file.GetBuffer(file.GetLength());

		sound.Stream = stream;
		}
		/*
		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		var sound = new AudioStreamMP3();
		sound.Data = file.GetBuffer(file.GetLength());
		return sound;
		*/

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (Math.Abs(movementTarget - Position[0]) > movementSpeed * (float)delta)
		{
			if(Position[0] > movementTarget)
			{
				Translate(new Vector3(-(float)delta*movementSpeed,0,0));
			}
			else{
				Translate(new Vector3((float)delta*movementSpeed,0,0));
			}
		}


		if(spinTimer > 0)
		{
			spinTimer -= (float)delta;
		}




	}


	public override void _Input(InputEvent @event)
	{
		if ( spinTimer <= 0)
		{

			InputEventScreenDrag drag = @event as InputEventScreenDrag;
			if (drag != null)
			{
				if(Math.Abs(drag.Velocity[0]) < .5f)
				{
					if(drag.Velocity[0] > .5f)
					{
						SpinPage(1);
					}
					else if(drag.Velocity[0] < -.5f)
					{
						SpinPage(-1);
					}
				}
			}

		}

	}

	public void EvenlySpaceCards()
	{
		int halfRow = 6;
		Vector3 center = new Vector3(0,-2.0f,0);
		float maxRange = 10.0f;
		float minSpacing = 1.25f;

		if( verticalLayout == false || cards.Length <= halfRow)
		{
			float leftEdge = center[0] - ((cards.Length-1)/2.0f * minSpacing);

			for(int i = 0; i < cards.Length; i++)
			{
				cards[i].Position = new Vector3(leftEdge, center[1], center[2]);
				leftEdge += minSpacing;
			}
		}
		else
		{
			center[1] = 0.0f;
			float leftEdge = center[0] - ((cards.Length-1)/4.0f * minSpacing);

			for(int i = 0; i < cards.Length/2; i++)
			{
				cards[i].Position = new Vector3(leftEdge, center[1], center[2]);
				leftEdge += minSpacing;
			}

			leftEdge = center[0] - ((cards.Length-1)/4.0f * minSpacing);
			for(int i = 0; i < cards.Length/2 + cards.Length%2; i++)
			{
				cards[i + cards.Length/2].Position = new Vector3(leftEdge, center[1]- 3.0f, center[2]);
				leftEdge += minSpacing;
			}

		}
	}


	public void SpinPage(int direction)
	{
		if(((stage < maxStage && direction > 0)|| (stage >=0 && direction < 0))&& spinTimer <= 0)
		{
			foreach(CardScript target in cards)
			{
				target.RequestSpin(direction);
			}
			spinTimer = spinCooldown;

			stage+=direction;
		}
	}


	public void AdjustSize()
	{
		float hFormatHeight = .5f;
		float vFormatHeight = .33f;


		Vector2 bounds = GetViewport().GetVisibleRect().Size;
		bool verticalPreference = bounds[0] < bounds[1];
		verticalLayout = verticalPreference;
		// Calculate the initial aspecet ratio so it can be kept
		// the same when the image is rescaled.
		Vector2 imageScale = pageScene.GetSize();
		float ratio = imageScale[0]/imageScale[1];
		scaleController.Ratio = ratio;
			

		if (verticalPreference) // The screen has more y than x space, like phone screens
		{
			scaleController.SetSize ( new Vector2(bounds[0],vFormatHeight* bounds[1]));


		}
		else
		{
			scaleController.SetSize(new Vector2(bounds[0],hFormatHeight* bounds[1]));


		}

		
	}

	public void SlideTo(float destination)
	{
		movementTarget = destination;
	}
}