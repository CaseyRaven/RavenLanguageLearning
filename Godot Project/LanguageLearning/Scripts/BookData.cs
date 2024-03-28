using Godot;
using System;

[GlobalClass]
public partial class BookData : Resource
{
    [Export]
    public PageData[] pages;
}
