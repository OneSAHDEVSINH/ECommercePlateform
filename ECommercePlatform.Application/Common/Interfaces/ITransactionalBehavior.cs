using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommercePlatform.Application.Common.Interfaces
{
    /// <summary>
    /// Marker interface for requests that require transaction handling.
    /// Implementing this interface will cause the TransactionBehavior to 
    /// automatically commit the transaction after handler execution.
    /// </summary>
    public interface ITransactionalBehavior
    {
    }
}
