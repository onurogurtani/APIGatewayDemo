using Newtonsoft.Json.Linq;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Net.Http.Headers;

namespace APIGateway.Aggregations
{
    public class UserBasketOrderAggregator : IDefinedAggregator
    {
        #region Methods

        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            var user = await responses[0].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var basket = await responses[1].Items.DownstreamResponse().Content.ReadAsStringAsync();
            var order = await responses[2].Items.DownstreamResponse().Content.ReadAsStringAsync();

            var basketJson = JObject.Parse(basket);

            ////////////////////////////////////////////////////////////////////
            ///
            var userJson = JObject.Parse(user);

            userJson.Property("header").Remove();

            basketJson.Add("buyer", userJson);

            ////////////////////////////////////////////////////////////////
            ///
            var orderJson = JObject.Parse(order);

            orderJson.Property("header").Remove();

            basketJson.Add("order", orderJson);

            ////////////////////////////////////////////////////////////

            var stringContent = new StringContent(basketJson.ToString())
            {
                Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
            };

            return new DownstreamResponse(
                stringContent,
                HttpStatusCode.OK,
                new List<KeyValuePair<string, IEnumerable<string>>>(),
                "OK");
        }

        #endregion

    }
}
