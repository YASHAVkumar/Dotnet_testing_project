namespace testing_web;

public interface IUserRepository
{
    public bool SaveUserDetail(string uname, string pass);

    public Task<User> GetUserByid(int i);
}
