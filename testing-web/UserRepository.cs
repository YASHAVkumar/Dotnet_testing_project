namespace testing_web;

public class UserRepository:IUserRepository
{
    public async Task<User> GetUserByid(int i)
    {
        if (i <= 0)
            throw new ArgumentException("id cannot be -ve or zero");

        return new User()
        {
            Id = i,
            Name = "Jhon snow",
            Age = 0,
            Date = DateTime.Now,
            IsActive = true,
        };
    }

    public bool SaveUserDetail(string uname,string pass)
    {
        //save to db by sql client
        return true;
    }

}
