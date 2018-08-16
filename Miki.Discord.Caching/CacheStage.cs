using Miki.Cache;
using Miki.Discord.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Discord.Caching
{
    public interface ICacheStage
    {
		void Initialize(CacheClient client);
	}
}
