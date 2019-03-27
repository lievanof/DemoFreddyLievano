using Demo.DataAccessLayer.Factory;
using Demo.Model;

namespace Demo.DataServices
{
    public class SessionService: ServiceBase<Session>, ISessionService
    {
        public SessionService(IDatabaseFactory databaseFactory)
            : base(databaseFactory)
        { }
    }
}
