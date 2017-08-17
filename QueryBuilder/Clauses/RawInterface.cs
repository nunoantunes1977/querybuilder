namespace SqlKata
{
    public interface RawInterface
    {
        string Expression { get; set; }
        object[] Bindings { set; }
    }
}