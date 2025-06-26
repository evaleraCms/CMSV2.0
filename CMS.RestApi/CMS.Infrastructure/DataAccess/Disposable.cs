using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Infrastructure.DataAccess
{
    public class Disposable : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        private void Dispose(bool disposing) 
        {
            if (disposing) 
            {
                DisposeCore();
            }
        }

        protected virtual void DisposeCore() 
        { }
    }
    
}
