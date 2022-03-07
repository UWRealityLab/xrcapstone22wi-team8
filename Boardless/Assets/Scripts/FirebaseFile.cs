public struct FirebaseFile
{
    public string Name;
    public string RefOrNull;
    public string ContentOrNull;

    public FirebaseFile(string name, string refOrNull, string contentOrNull)
    {
        Name = name;
        RefOrNull = refOrNull;
        ContentOrNull = contentOrNull;
    }
}