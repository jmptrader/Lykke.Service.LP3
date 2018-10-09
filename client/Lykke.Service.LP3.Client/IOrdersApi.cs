using System.Collections.Generic;
using Lykke.Service.LP3.Client.Models.Orders;
using Refit;

namespace Lykke.Service.LP3.Client
{
    public interface IOrdersApi
    {
        [Get("/api/orders")]
        IReadOnlyList<LimitOrderModel> GetAll();
    }
}