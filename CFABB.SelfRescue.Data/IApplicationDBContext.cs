using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CFABB.SelfRescue.Data {
    public interface IApplicationDBContext {
        int SaveChanges(string Username);
        Task<int> SaveChangesAsync(CancellationToken token, string username);
    }
}
