namespace LazyThreads
{
    /// <summary>
    /// Interface of class encapsulating evaluated by need expression. 
    /// </summary>
    /// <typeparam name="T">Type of encapsulated value.</typeparam>
    public interface ILazy<T>
    {
        /// <summary>
        /// Evaluates encapsulated expression if it has not been done before and returns it.
        /// </summary>
        /// <returns>Value of the expression.</returns>
        T Get();
    }
}