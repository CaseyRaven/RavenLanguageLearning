using Godot;
using System;



public partial class FaceBuilder : Node
{

	//These will have to be much larger in the future.
	const int FACE_WIDTH = 128;
	const int FACE_HEIGHT = 128;
	const int BORDER_WIDTH = 4;

	const int FULL_HINT_Y = 32;
	const int FULL_HINT_WIDTH = 96;
	const int FULL_HINT_HEIGHT = 32;

	const int SMALL_HINT_Y = 64;
	const int SMALL_HINT_WIDTH = 16;
	const int SMALL_HINT_HEIGHT = 16;

	const int TEXT_BOX_Y = 80;
	const int TEXT_BOX_WIDTH = 96;
	const int TEXT_BOX_HEIGHT = 16;

	const int SIGN_Y = 96;
	const int SIGN_WIDTH = 96;
	const int SIGN_HEIGHT = 32;



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}


	//Build Face
	public static Image Build(Color background, Color border, Color inner, String native, String other, int difficulty)
	{
		Image result = Image.Create(FACE_WIDTH, FACE_HEIGHT, false, (Image.Format) 5 /*FormatRGBA8*/);
		result.Fill(border);

		Rect2I region = new Rect2I(BORDER_WIDTH, BORDER_WIDTH, FACE_WIDTH - BORDER_WIDTH * 2, FACE_HEIGHT - BORDER_WIDTH * 2);
		result.FillRect(region, background);

		//The Background and border are now appropriately colored.
		// The next step is to establish the boxes for content.
		// These boxes may vary in size, position, and number based on the difficulty value.

		// This is for the Hint at the top of the card.
		if (difficulty == 0) // Change to any difficulty that would include this element
		{
			region = new Rect2I( (FACE_WIDTH/2) - (FULL_HINT_WIDTH/2) - BORDER_WIDTH, FULL_HINT_Y - (FULL_HINT_HEIGHT / 2) - BORDER_WIDTH, 
				FULL_HINT_WIDTH + BORDER_WIDTH * 2, FULL_HINT_HEIGHT + BORDER_WIDTH * 2);	
			result.FillRect(region, border);

			region = new Rect2I( (FACE_WIDTH/2) - (FULL_HINT_WIDTH/2), FULL_HINT_Y - (FULL_HINT_HEIGHT / 2), 
				FULL_HINT_WIDTH, FULL_HINT_HEIGHT);
			result.FillRect(region, inner);
		}

		// This is for the Sign Language section at the bottom of the card
		if (difficulty == 0) // Change to any difficulty that would include this element
		{
			region = new Rect2I( (FACE_WIDTH/2) - (SIGN_WIDTH/2) - BORDER_WIDTH, SIGN_Y - (SIGN_HEIGHT / 2) - BORDER_WIDTH, 
				SIGN_WIDTH + BORDER_WIDTH * 2, SIGN_HEIGHT + BORDER_WIDTH * 2);	
			result.FillRect(region, border);

			region = new Rect2I( (FACE_WIDTH/2) - (SIGN_WIDTH/2), SIGN_Y - (SIGN_HEIGHT / 2), 
				SIGN_WIDTH, SIGN_HEIGHT);
			result.FillRect(region, inner);
		}



		return result;

	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
