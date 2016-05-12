using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using peaker.Models;
using peaker.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
using System.Configuration;

namespace peaker.Controllers
{
    public class AthletesController : Controller
    {
        private PeakDbContext db = new PeakDbContext();
        private CloudQueue parseActivitiesQueue;

        // GET: Athletes
        public ActionResult Index()
        {
            // get logged in user
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

            //get athlete
            var athlete = db.Athletes.FirstOrDefault(m => m.UserId == user.Id);

            if (athlete == null)
            {
                return View("Connect", null);
            }

            AthleteDetailViewModel ViewModel = new AthleteDetailViewModel();
            ViewModel.AthleteId = athlete.Id;
            ViewModel.AthleteName = athlete.Name;
            ViewModel.IndexedActivityCount = db.IndexedActivities.Where(m => m.Athlete.Id == athlete.Id).Count();
            ViewModel.AthleteSummitCompletions = db.SummitCompletions.Where(m => m.Athlete.Id == athlete.Id).Include(m => m.Peak).ToList();

            return View(ViewModel);
        }

        public JsonResult GetAthleteSummits(int? id)
        {
            if (id == null)
            {
                return Json("Id cannot be null", JsonRequestBehavior.AllowGet);
            }
            Athlete athlete = db.Athletes.Find(id);
            if (athlete == null)
            {
                return Json("Id not found", JsonRequestBehavior.AllowGet);
            }

            var summits = db.SummitCompletions.Where(m => m.Athlete.Id == athlete.Id).Include(m => m.Peak).ToList();

            return Json(summits, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProccessActivities(int id)
        {
            ProccessActivitiesForAthlete(id);

            return Json("proccessing", JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ProccessActivitiesForAthlete(int athleteId)
        {
            InitializeStorage();

            // Kick off the background work
            var message = new CloudQueueMessage(JsonConvert.SerializeObject(athleteId));
            parseActivitiesQueue.AddMessage(message);
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
    }
}
