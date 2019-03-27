using Demo.DataAccessLayer.Context;

namespace Demo.DataAccessLayer.Factory
{
    public class DbFactory : Disposable, IDatabaseFactory
    {
        private DemoContext demoContext;

        public DemoContext Get()
        {
            return demoContext ?? (demoContext = new DemoContext());
        }
        protected override void DisposeCore()
        {
            if (demoContext != null)
                demoContext.Dispose();
        }
    }
}
