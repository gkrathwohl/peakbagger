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
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace peaker.Controllers
{
    public class StravaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public StravaController()
        {
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
                ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());

                currAthlete = new Models.Athlete();
                currAthlete.UserId = user.Id;
                currAthlete.Name = stravaAthlete.FirstName;
                currAthlete.StravaId = (int)stravaAthlete.Id;
                currAthlete.AccessToken = auth.AccessToken;
                db.Athletes.Add(currAthlete);
                db.SaveChanges();
            }

            // Render the view
            return View();
        }

    }
}