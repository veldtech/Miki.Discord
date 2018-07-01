using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Discord.Common
{
    public interface ISnowflake<T>
    {
		T Id { get; }
    }
}
