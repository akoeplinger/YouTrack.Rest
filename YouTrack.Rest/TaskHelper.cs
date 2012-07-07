using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YouTrack.Rest
{
    internal static class TaskHelper
    {
        internal static Task EmptyTask
        {
            get { return Task.Factory.StartNew(() => { }); }
        }

        internal static void ThrowIfExceptionOccured(Task r)
        {
            if (r.Exception != null)
                throw r.Exception;
        }

        internal static void ThrowIfExceptionOccured<T>(Task<T> r)
        {
            if (r.Exception != null)
                throw r.Exception;
        }
    }
}
