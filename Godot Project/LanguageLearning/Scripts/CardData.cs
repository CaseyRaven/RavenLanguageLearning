using Godot;
using System;

[GlobalClass]
public partial class CardData : Resource
{
    [Export]
    public string word;
    [Export]
    public int type;
    [Export]
    public Texture2D[] storedFaces;
    [Export]
    public Image[] faces;
    [Export]
    public int maxStage;

    [Export]
    public string[] variants;
    [Export]
    public string[] translations;
    [Export]
    public Vector2I faceSize;
    [Export]
    public int imageFormat = 5;// 4 is rgb8, 5 is rgba8
    [Export]
    public bool locked;
    [Export]
    public bool unlocking;
    [Export]
    public int currentDifficulty;
}
