namespace Module4TopShelfService
{
    public enum FilePart
    {
        prefix = 0,
        number = 1,
        extension = 2,
        firstExtension = 0,
        secondExtension = 1,
    }

    public enum BarCodeResult
    {
        Equals = 0,
        NotEqual = 1,
        BrkokenFormat = 2
    }
}
