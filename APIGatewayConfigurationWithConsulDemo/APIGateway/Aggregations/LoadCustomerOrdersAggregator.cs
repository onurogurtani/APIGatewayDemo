using Ocelot.Middleware;
using Ocelot.Multiplexer;

namespace APIGateway.Aggregations
{
    public class CustomAggregator : IDefinedAggregator
    {
        #region Methods

        public Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
