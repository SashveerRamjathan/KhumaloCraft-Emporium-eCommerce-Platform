using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;
using ST10361554_CLDV6211_Order_Workflow_Function.Models;
using System.Net;

namespace ST10361554_CLDV6211_Order_Workflow_Function
{
    public static class OrderWorkflowFunction
    {
        

        [FunctionName("OrderWorkflowFunction")]
        public static async Task<string> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            OrderRequestData order = context.GetInput<OrderRequestData>();

            var OrderRecordCreated = await context.CallActivityAsync<string>(nameof(CreateOrderRecord), order);

            return OrderRecordCreated;
        }

		// Code Attribution
		// Method written using code from: 
		// cgillum
		// Microsoft Learn
		// https://learn.microsoft.com/en-us/azure/azure-functions/durable/durable-functions-orchestrations?tabs=csharp-inproc

		[FunctionName(nameof(CreateOrderRecord))]
        public static async Task<string> CreateOrderRecord([ActivityTrigger] OrderRequestData order, ILogger log)
        {
            if (order.ShoppingCartItems != null && order.ShoppingCartItems.Count > 0)
            {
                log.LogInformation($"Creating order record for user {order.UserId}");
                //Console.WriteLine($"Creating order record for user {order.UserId}");

                var orderTotal = CalculateOrderTotal(order.ShoppingCartItems);

                using (var dbContext = new KhumaloCraftDatabaseContext())
                {
                    // Create order record in database
                    var orderRecord = new Order
                    {
                        UserId = order.UserId,
                        OrderDate = DateOnly.FromDateTime(DateTime.Now),
                        OrderTotalAmount = orderTotal,
                        OrderStatus = "Pending"
                    };

                    dbContext.Orders.Add(orderRecord);

                    // Create OrderDetails for each item in the shopping cart
                    foreach (var item in order.ShoppingCartItems)
                    {
                        var orderDetail = new OrderDetail
                        {
                            Order = orderRecord,
                            CraftworkId = item.CraftworkId,
                            Quantity = item.Quantity
                        };

                        orderRecord.OrderDetails.Add(orderDetail);

                        // Update stock level of craftwork
                        var craftwork = await dbContext.Craftworks.FindAsync(item.CraftworkId);

                        if (craftwork == null)
                        {
                            throw new Exception($"Craftwork with ID {item.CraftworkId} is not available");
                        }

                        craftwork.CraftworkQuantity -= item.Quantity;
                        dbContext.Craftworks.Update(craftwork);
                    }

                    await dbContext.SaveChangesAsync();
                    log.LogInformation($"Order created and saved successfully for User ID: {order.UserId}");
                    //Console.WriteLine($"Order created and saved successfully for User ID: {order.UserId}");

                }

                return $"Order created and saved successfully for User ID: {order.UserId}";
            }
            else
            {
                Console.WriteLine("No items in the shopping cart to create an order");
                return "No items in the shopping cart to create an order";
            }
        }

        [FunctionName("OrderWorkflowFunction_HttpStart")]
        public static async Task<HttpResponseMessage> HttpStart(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            try
            {
                // Get the order data from the request body
                OrderRequestData order = await req.Content.ReadAsAsync<OrderRequestData>();

                // Start the orchestration with the order data
                string instanceId = await starter.StartNewAsync("OrderWorkflowFunction", order);

                log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

                // Return an HTTP response indicating the orchestration was successfully started
                return starter.CreateCheckStatusResponse(req, instanceId);
            }
            catch (Exception ex)
            {
                log.LogError($"Error starting orchestration: {ex.Message}");

                // Return an HTTP response indicating the error
                return new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent($"Error starting orchestration: {ex.Message}")
                };
            }
        }

        public static decimal CalculateOrderTotal(List<ShoppingCartItem> shoppingCartItems)
        {
            decimal orderTotal = 0;
            foreach (var item in shoppingCartItems)
            {
                orderTotal += item.Quantity * item.craftwork.CraftworkPrice;
            }

            return orderTotal;
        }
    }

    public class OrderRequestData
    {
        public required string UserId { get; set; }
        public required List<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
    }
}