namespace Abstractions
{
    public interface IPasswordService
    {
        string Hash(string plainTextPassword);
    }
}
