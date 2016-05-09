using peaker.Helpers;
using peaker.Models;
using Strava.Athletes;
using Strava.Authentication;
using Strava.Clients;
using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;
using Newtonsoft.Json;

namespace peaker.Controllers
{
    public class StravaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private CloudQueue parseActivitiesQueue;
        public StravaController()
        {
            InitializeStorage();
        }

        private void InitializeStorage()
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



        // GET: Strava
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> Connect(string code)
        {
            // Connect to Strava
            string clientId = "3764";
            string clientSecret = "e0b897e6bc461b774c73fbff6936f656d2e376f3";
            WebAuthentication auth = new WebAuthentication();
            await auth.GetTokenAsync(clientId, clientSecret, code);
            StravaClient client = new StravaClient(auth);

            //Receive the currently authorized athlete
            Strava.Athletes.Athlete stravaAthlete = await client.Athletes.GetAthleteAsync();

            // Retreive athlete or create new one
            Models.Athlete currAthlete = db.Athletes.Where(m => m.StravaId == stravaAthlete.Id).FirstOrDefault();
            if (currAthlete == null)
            {
                currAthlete = new Models.Athlete();
                currAthlete.Name = stravaAthlete.FirstName;
                currAthlete.StravaId = (int)stravaAthlete.Id;
                currAthlete.AccessToken = auth.AccessToken;
                db.Athletes.Add(currAthlete);
                db.SaveChanges();
            }

            // Kick off the background work
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(currAthlete.Id));
            parseActivitiesQueue.AddMessage(message);

            // Render the view
            return View();
        }


        public async Task<ActionResult> Connect1(string code)
        {
            string clientId = "3764";
            string clientSecret = "e0b897e6bc461b774c73fbff6936f656d2e376f3";
       
            WebAuthentication auth = new WebAuthentication();
            await auth.GetTokenAsync(clientId, clientSecret, code);

            StravaClient client = new StravaClient(auth);

            //Receive the currently authorized athlete
            Strava.Athletes.Athlete athlete = await client.Athletes.GetAthleteAsync();

            var allActivities = client.Activities.GetAllActivities();

            ViewBag.activities = allActivities.Count;

            //for(int i=0; i<10; i++)
            //{
            //    var activity = allActivities[i];

            //    var IndexedActivity = new IndexedActivity();
            //    IndexedActivity.StravaActivityId = (int)activity.Id;
            //    IndexedActivity.StravaAthleteId = (int)athlete.Id;
            //    db.IndexedActivities.Add(IndexedActivity);
            //}
            //db.SaveChanges();

           // var indexedActivities = db.IndexedActivities.Where(a => a.StravaAthleteId == athlete.Id).ToList();
           // var newActivities = allActivities.SkipWhile(a => indexedActivities.Any(i => i.StravaActivityId == a.Id));

          //  ViewBag.newActivities = newActivities.ToList().Count;

            var firstActivity = allActivities.First();
            var firstActivityLocation = GeoUtils.CreatePoint((double)firstActivity.StartLatitude, (double)firstActivity.StartLongitude);

            //var nearestPeak = (from u in db.Peaks
            //                   orderby u.Location.Distance(firstActivityLocation)
            //                   select u).FirstOrDefault();

            //ViewBag.nearestPeak = nearestPeak.Name;

            ////get bounding box of activity. Find center of box. Get all peaks within radius of (center of box to corner of box + 10m)


            //var nearby = (from peak in db.Peaks
            //                   where peak.Location.Distance(firstActivityLocation) < 1000
            //                   select peak).ToList();


            //client.Streams.GetActivityStream


            //ViewBag.distance = nearestPeak.Location.Distance(firstActivityLocation);
            //ViewBag.height = nearestPeak.ELEV_METER;


            //db.Peaks.Where(p => p.Location.Distance(firstActivityLocation) < 100);

            var activities = await client.Activities.GetActivitiesAsync(1, 20);

            var stream = client.Streams.GetActivityStream(activities[0].Id.ToString(), Strava.Streams.StreamType.LatLng, Strava.Streams.StreamResolution.High);

            return View("Peaks");
        }
    }
}