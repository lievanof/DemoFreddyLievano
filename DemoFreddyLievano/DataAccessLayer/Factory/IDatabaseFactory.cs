using System;
using Demo.DataAccessLayer.Context;

namespace Demo.DataAccessLayer.Factory
{
    public interface IDatabaseFactory: IDisposable
    {
        DemoContext Get();
    }
}
