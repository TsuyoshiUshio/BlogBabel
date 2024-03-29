﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BabelLibs.Resources
{
    public interface ISourceProvider : IProvider
    {
        public Task<Post> GetPostAsync(string itemId);
    }
}
