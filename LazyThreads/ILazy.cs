namespace LazyThreads
{
    public interface ILazy<T>
    {
        T Get();
    }
}