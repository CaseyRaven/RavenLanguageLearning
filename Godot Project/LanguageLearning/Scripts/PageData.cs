using Godot;
using System;


[GlobalClass]
public partial class PageData : Resource
{

    [Export]
    public CardData[] cards;// Could later be Strings, and the appropriate CardData is run-time determined.

    [Export]
    public Texture2D panel;

    [Export]
    public string[] names;// These are assigned in order to name cards in the page, to get around shared resources

}
