using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs.Resources
{
    public interface IDestinationProvider : IProvider
    {
        public Task<HttpResponseMessage?> PostAsync(Post post);
    }
}
