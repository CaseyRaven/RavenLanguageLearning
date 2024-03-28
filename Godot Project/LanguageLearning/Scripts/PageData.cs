using Godot;
using System;


[GlobalClass]
public partial class PageData : Resource
{

    [Export]
    public CardData[] cards;// Could later be Strings, and the appropriate CardData is run-time determined.

    [Export]
    public Texture2D panel;

}
