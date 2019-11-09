using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Host;

namespace ToDoOperations
{
    public static class DocCreateToDoItem
    {
        [FunctionName("DocCreateToDoItem")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "items")]HttpRequest req,
            [CosmosDB(
        databaseName: "ToDoItems",
        collectionName: "Items",
        ConnectionStringSetting = "CosmosDBConnection")]
    IAsyncCollector<object> todos,
            ILogger log)
        {
            log.LogInformation("Creating a new todo list item");
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<CreateToDoItem>(requestBody);

            var todo = new ToDoItem() { Description = input.Description };
            //the object we need to add has to have a lower case id property or we'll
            // end up with a cosmosdb document with two properties - id (autogenerated) and Id
            await todos.AddAsync(new { id = todo.Id, todo.CreatedOn, todo.IsCompleted, todo.Description });
            return new OkObjectResult(todo);
        }
    }
}
