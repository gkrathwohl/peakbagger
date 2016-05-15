using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace peaker.Helpers
{
    public class QueueHelper
    {

        private static CloudQueue parseActivitiesQueue;

        public static void ProccessActivitiesForAthlete(int athleteId)
        {
            InitializeStorage();

            // Kick off the background work
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(athleteId));
            parseActivitiesQueue.AddMessage(message);
        }

        private static void InitializeStorage()
        {
            // Open storage account using credentials from .cscfg file.
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ToString());

            // Get context object for working with queues, and 
            // set a default retry policy appropriate for a web user interface.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            //queueClient.DefaultRequestOptions.RetryPolicy = new LinearRetry(TimeSpan.FromSeconds(3), 3);

            // Get a reference to the queue.
            parseActivitiesQueue = queueClient.GetQueueReference("peakqueue");

            // Create the queue if it doesn't already exist
            parseActivitiesQueue.CreateIfNotExists();
        }
    }
}