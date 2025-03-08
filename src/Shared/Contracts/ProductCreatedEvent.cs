using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public class ProductCreatedEvent : IEvent
    {
        public DateTime OccurredOn { get; private set; }
        public int ProductId { get; set; }
        public string Title { get; set; }

        public ProductCreatedEvent(int productId, string title)
        {
            ProductId = productId;
            Title = title;
            OccurredOn = DateTime.UtcNow;
        }
    }
}
