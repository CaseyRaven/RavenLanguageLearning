using Godot;
using System;

/*
This class is used to store the pages contained in a book.
The BookScript will use this resource to build the book at runtime.
*/
[GlobalClass]
public partial class BookData : Resource
{
    [Export]
    public PageData[] pages;
}
