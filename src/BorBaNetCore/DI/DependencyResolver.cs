using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BorBaNetCore.DI
{
    public static class DependencyResolver
    {
        private static IServiceProvider _provider;
        public static IServiceProvider ServiceProvider
        {
            get
            {
                return _provider;
            }
            set
            {
                if (_provider == null)
                {
                    _provider = value;
                }
            }
        }
    }
}
