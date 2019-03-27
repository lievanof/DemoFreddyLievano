using Demo.DataAccessLayer.Factory;
using Demo.Model;

namespace Demo.DataServices
{
    public class UserService: ServiceBase<User>, IUserService
    {
        public UserService(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        { }
    }
}
