using Godot;
using System;

public partial class CardScript : Node3D
{
	const float ROTATION_SPEED = (float)Math.PI / 2.0f;
	Random randomTester;

	[Export]
	public CardData source;

	MeshInstance3D box;
	StandardMaterial3D surface;// = (BaseMaterial3D) box.material;
	Texture2D faces;// = surface.albedo_texture; // Not how this works, but it helps keep track of the structure

	int[][] facePositions = new int[4][];/* = {{5,0,4,2},
							 {0,4,2,5},
							 {4,2,5,0},
							 {2,5,0,4}}; // bottom, front, top, back
							*/
	
	int[] faceNumbers;// = facePositions[0]; 

	public int currentDifficulty;
	public int maxDifficulty;

	public int goalDifficulty;
	public float targetRotation = 0;

	public int cardType; // 0 is standard, 1 is name, 2 is a spacer

	public bool isSpinning;

	//bool unlocking;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		/*
				facePositions =  [[5,0,4,2],
									[0,4,2,5],
							 		[4,2,5,0],
							 		[2,5,0,4]]; // bottom, front, top, back
									// Doing it like this is apparently a preview feature of C#, and not supported yet
									*/
		

		facePositions[0] = new int[4] {5,0,4,2};
		facePositions[1] = new int[4] {0,4,2,5};
		facePositions[2] = new int[4] {4,2,5,0};
		facePositions[3] = new int[4] {2,5,0,4}; // bottom, front, top, back

		//faceNumbers = facePositions[0];
			


		randomTester = new Random();

		maxDifficulty = source.maxStage;
		if(currentDifficulty < 0)
		{
			currentDifficulty = 0;
		}
		if(currentDifficulty > maxDifficulty)
		{
			currentDifficulty = maxDifficulty;
		}

		cardType = source.type;

		GlobalRotation = new Vector3(currentDifficulty * (float)Math.PI * .5f, GlobalRotation[1], GlobalRotation[2]);
		targetRotation = currentDifficulty * (float)Math.PI * .5f;
		faceNumbers = facePositions[currentDifficulty%4];

		box = GetNode<MeshInstance3D>("./Box");
		surface = new StandardMaterial3D();

		if(cardType == 0)
		{
			for (int i = 0; i < 4; i++)
			{
				GetNode<Label3D>("Box/FaceText" + faceNumbers[i]).Hide();
			}
		}
		else if (cardType == 1)
		{
			for (int i = 0; i < 4; i++)
			{
				GetNode<Label3D>("Box/FaceText" + faceNumbers[i]).Text = source.word;
			}
		}
		else
		{
			GetNode<Node3D>("Box").Hide();
		}

		if(!source.locked)
		{
			for (int i = 0; i < 4; i++)
			{
				GetNode<Label3D>("Lock" + i).Hide();
			}
		}

		if((source.storedFaces == null || source.storedFaces.Length > 0) && source.faces !=null && source.faces.Length > 0 && source.faces[0] == null)
		{
			InitializeDataFromTextures();
		}
		else if(source.faces != null && source.faces.Length > 0 && source.faces[0] == null)
		{
			InitializeDataFromResources();
		}
		else {}
		//FlipFaces();	// This is currently not working correctly. The code has been duplicated elsewhere to fix this.
		LoadFacesFromData();
	}


	public void InitializeDataFromResources()
	{
		if(cardType !=0)
			return;
		string target = source.word;
		for(int i =0; i <= source.maxStage; i++)
		{
			source.faces[i] = GD.Load<Texture2D>("res://Images/"+target+"-" + i +".jpg").GetImage();
			/*
			if(i%4 != 0 && i%4 != 3)
			{
				source.faces[i].FlipY();
				source.faces[i].FlipX();
			}
			*/
		}
		FlipFaces();

	}

	public void InitializeDataFromTextures()
	{
		for(int i = 0; i <= source.maxStage; i++)
		{
			source.faces[i] = source.storedFaces[i].GetImage();
			source.faces[i].Decompress();
			source.faces[i].Convert((Image.Format)5);
			/*
			if(i%4 != 0 && i%4 != 3)
			{
				source.faces[i].FlipY();
				source.faces[i].FlipX();
			}
			*/
		}
		FlipFaces();
	}

	// Due to the UV mapping of Boxes, two faces will be upside down unless they are corrected beforehand.
	public void FlipFaces()
	{
		//if(cardType !=0)
		//	return;
		for(int i =0; i <= source.maxStage; i++)
		{
			if(i%4 != 0 && i%4 != 3)
			{
				source.faces[i].FlipY();
				source.faces[i].FlipX();
			}
		}
	}

	public void LoadFacesFromData()
	{
		// Loads three faces from the data in the CardData object.
		// These are the bottom, front, and top faces.
		if(cardType != 0)
		{
			return;
		}
		
		Image surfaceHolder = Image.Create(source.faceSize[0]*3, source.faceSize[1]*2, false, (Image.Format)5 /*FormatRGBA8*/);
		surfaceHolder.Fill(new Color(1.0f,1.0f,1.0f));

		//Image[] sections = new Image[6];
		Image current;

		Rect2I bounds = new Rect2I(0,0,source.faceSize[0],source.faceSize[1]);

		//sections[0] = FaceBuilder.Build(new Color(.8f,.8f,.2f), new Color(.2f,.8f,.8f), new Color(.8f,.2f,.8f), "", "", 0);
		//surfaceHolder.BlitRect(sections[0], bounds, new Vector2I(0, 0));

		for (int i = 2; i >= 0; i--)
		{
			if(i == 2 && currentDifficulty >= maxDifficulty)
			{
				i--;
			}
			if (i != 0 || currentDifficulty != 0)
			{
				int j = faceNumbers[i];
				current = source.faces[i + currentDifficulty - 1];//Image.LoadFromFile("C:\\Users\\Casey\\Pictures\\" + (i + currentDifficulty)+ ".png");
				//current.Decompress();
				//current.Convert((Image.Format)5);
				//surfaceHolder.FillRect(new Rect2I(source.faceSize[0]*(j%3),source.faceSize[1]*(j/3), source.faceSize[0], source.faceSize[1]), new Color(1.0f,1.0f,1.0f));
				surfaceHolder.BlitRectMask(current, current, bounds, new Vector2I(source.faceSize[0]*(j%3),source.faceSize[1]*(j/3)));
			}
		}

		//blit_rect ( Image src, Rect2i src_rect, Vector2i dst )


		Texture2D newFaces = ImageTexture.CreateFromImage(surfaceHolder);

		//surface.SetTexture( (BaseMaterial3D.TextureParam)0 /*TEXTURE_ALBEDO*/ , newFaces);
		surface.AlbedoTexture =  newFaces;
		box.Mesh.SurfaceSetMaterial(0, surface);
		//This appears to be mutually exclusive with using a shader

	}



	public void RequestSpin(int direction)
	{
		goalDifficulty += direction;
		goalDifficulty = Math.Max(0, Math.Min(maxDifficulty, goalDifficulty));
		if(!source.locked)
		{
			Spin(direction);
		}

	}

	public void Spin(int direction)
	{

		if(cardType != 0)
		{
			return;
		}


		if (direction > 0 && currentDifficulty < maxDifficulty) //increase difficulty
		{


			targetRotation += (float)Math.PI / 2.0f;

			currentDifficulty++;
			source.currentDifficulty = currentDifficulty;

			/*
			int temp = faceNumbers[1];
			faceNumbers[1] = faceNumbers[2];
			faceNumbers[2] = faceNumbers[3];
			faceNumbers[3] = faceNumbers[0];
			faceNumbers[0] = temp;
			*/

			faceNumbers = facePositions[currentDifficulty % 4];

			isSpinning = true;
			LoadFacesFromData();

		}
		else if (direction < 0 && currentDifficulty > 0) // decrease difficulty
		{

			targetRotation -= (float)Math.PI / 2.0f;

			currentDifficulty--;
			source.currentDifficulty = currentDifficulty;
			/*
			int temp = faceNumbers[3];
			faceNumbers[3] = faceNumbers[2];
			faceNumbers[2] = faceNumbers[1];
			faceNumbers[1] = faceNumbers[0];
			faceNumbers[0] = temp;
			*/

			faceNumbers = facePositions[currentDifficulty % 4];

			isSpinning = true;
			LoadFacesFromData();
		}

		if(targetRotation >= (float)Math.PI)
		{
			targetRotation -= 2.0f*(float)Math.PI;
		}
		else if(targetRotation < -(float)Math.PI)
		{
			targetRotation += 2.0f*(float)Math.PI;
		}

		//Additionally load an additional face in the direction of the spin. There should always
		// be one face loaded on either side of the active face.


	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//float distance = Math.Min(targetRotation - GlobalRotation[0], 2.0f * (float)Math.PI - GlobalRotation[0] + targetRotation);
		float distance = targetRotation - GlobalRotation[0];
		if(Math.Abs(distance) > Math.PI)
		{
			distance = -distance; //The magnitude is wrong, but that should not prevent this from working.
		}

		if (GlobalRotation[0] != targetRotation && (Math.Abs(distance) <= ROTATION_SPEED * (float) delta * 2) 
		|| Math.Abs(distance) >= (float)Math.PI * 2.0f)
		{
			//GlobalRotation = new Vector3(targetRotation, 0, 0);
			isSpinning = false;
		}
		else if(distance > 0)
		{
			GlobalRotation = new Vector3(GlobalRotation[0] + (float)delta * ROTATION_SPEED, 0, 0);
		}
		else if(distance < 0)
		{
			GlobalRotation = new Vector3(GlobalRotation[0] - (float)delta * ROTATION_SPEED, 0, 0);
		}
		
		//GlobalRotationDegrees = new Vector3(GlobalRotationDegrees[0] - (float)delta * ROTATION_SPEED, 0, 0);

		if (source.unlocking && !isSpinning)
		{
			if(currentDifficulty == goalDifficulty)
			{
				source.unlocking = false;
				source.locked = false;
			}
			else
			{
				Spin(goalDifficulty - currentDifficulty);
			}
		}
		/*
		if(randomTester.Next(0,1000) > 998)
		{
			Spin(1);
		}
		else if(randomTester.Next(0,1000) > 998) // technically not equal odds. I don't care.
		{
			Spin(-1);
		}
		*/

		//RotateY((float) delta * 0);
		//RotateX((float) delta * 5);

	}

	public void ToggleLock()
	{
		if(source.locked)
		{
			Unlock();
		}
		else
		{
			Lock();
		}
	}

	public void Lock()
	{
		source.locked = true;
		for (int i = 0; i < 4; i++)
			{
				GetNode<Label3D>("Lock" + i).Show();
			}
		
	}

	public void Unlock()
	{
		//source.locked = false;
		//goalDifficulty = goal;
		source.unlocking = true;
		for (int i = 0; i < 4; i++)
		{
			GetNode<Label3D>("Lock" + i).Hide();
		}
	}
}
