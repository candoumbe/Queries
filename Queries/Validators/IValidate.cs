namespace Queries.Validators
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValidate<in T>
    {
        bool IsValid(T element);
    }
}
