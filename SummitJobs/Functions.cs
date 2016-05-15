using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Configuration;
using Strava.Authentication;
using Strava.Clients;
using peaker.Helpers;
using Strava.Activities;
using Polylines;
using Geo;
using System.Spatial;
using GeoAPI.Geometries;
using System.Device.Location;
using peaker.Models;

namespace SummitJobs
{
    public class Functions
    {

        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static void ParseActivities([QueueTrigger("peakqueue")] int code, TextWriter log)
        {
            try
            {
                Task.WaitAll(parseActivitiesAsync(code));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                throw;
            }
        }


        private static void indexActivity(PeakDbContext db, int id, Athlete athlete)
        {
            var indexActivity = new IndexedActivity();
            indexActivity.StravaActivityId = id;
            indexActivity.Athlete = athlete;
            db.IndexedActivities.Add(indexActivity);

            // don't really have to save changes now, but it might help the duplicates while testing
            db.SaveChanges();
        }

        private static async Task parseActivitiesAsync(int athleteId)
        {
            using (var db = new PeakDbContext())
            {
                string clientId = "3764";
                string clientSecret = "e0b897e6bc461b774c73fbff6936f656d2e376f3";

                var athlete = db.Athletes.Find(athleteId);
                var accessToken = athlete.AccessToken;

                // Get strava token
                WebAuthentication auth = new WebAuthentication();
                auth.AccessToken = accessToken;

                StravaClient client = new StravaClient(auth);

                //Receive the currently authorized athlete
                Strava.Athletes.Athlete stravaAthlete = await client.Athletes.GetAthleteAsync();

                // Retreive athlete or create new one
                Athlete currAthlete = db.Athletes.Where(m => m.StravaId == stravaAthlete.Id).FirstOrDefault();

                var allActivities = client.Activities.GetAllActivities();

                var count = 0;
                foreach(var activity in allActivities)
                {
                    count++;

                    Console.WriteLine("activity " + count.ToString());

                    // continue if activity has already been indexed
                    if (db.IndexedActivities.Any(m => m.StravaActivityId == activity.Id))
                    {
                        continue;
                    }

                    // index the activity so we know we searched it
                    indexActivity(db, (int)activity.Id, currAthlete);

                    // continue if there's no polyline
                    if (activity.Map.SummaryPolyline == null)
                    {
                        continue;
                    }

                    ////var stream = client.Streams.GetActivityStream(activity.Id.ToString(), Strava.Streams.StreamType.LatLng, Strava.Streams.StreamResolution.Medium).First();

                    //var fullActivity = client.Activities.GetActivity(activity.Id.ToString(), false);
                    //fullActivity.Map.Polyline;


                    var fullActivity = client.Activities.GetActivity((activity.Id).ToString(), false);
                    var activityPolyline = Polyline.DecodePolyline(fullActivity.Map.Polyline).ToList();

                    //var boundingBox = GetBoundingBox(activityPolyline);
                    var boundingBox = GetBufferedBoundingBox(activityPolyline);


                    var peaksInBBox = (from peak in db.Peaks
                                       where peak.Latitude > boundingBox.MinLat && peak.Latitude < boundingBox.MaxLat
                                       && peak.Longitude > boundingBox.MinLon && peak.Longitude < boundingBox.MaxLon
                                       select peak).ToList();

                    foreach (var peak in peaksInBBox)
                    {
                        bool reachedSummit = polylineReachedSummit(activityPolyline, peak, 40);

                        if (reachedSummit)
                        {
                            var summitCompletion = new SummitCompletion();
                            summitCompletion.Peak = peak;
                            summitCompletion.Athlete = currAthlete;
                            summitCompletion.Date = fullActivity.DateTimeStart;
                            summitCompletion.StravaActivityId = (int)fullActivity.Id;

                            db.SummitCompletions.Add(summitCompletion);
                        }

                    }

                    if(count % 10 == 0)
                    {
                        db.SaveChanges();
                    }
                    

                }

                db.SaveChanges();

                Console.WriteLine("done");
            }
        }

        private static bool polylineReachedSummit(List<PolylineCoordinate> polyLine, Peak peak, int completionDistance)
        {
            var summit = new GeoCoordinate(peak.Latitude, peak.Longitude);

            var closest = double.MaxValue;

            foreach (var point in polyLine)
            {
                var currPoint = new GeoCoordinate(point.Latitude, point.Longitude);

                var distToSummit = currPoint.GetDistanceTo(summit);
                if (distToSummit < closest)
                    closest = distToSummit;

            }

            if(closest < completionDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }

        private static BoundingBox GetBoundingBox(List<Polylines.PolylineCoordinate> polyLine)
        {
            var boundingBox = new BoundingBox();

            double minLat = int.MaxValue;
            double maxLat = int.MinValue;
            double minLon = int.MaxValue;
            double maxLon = int.MinValue;

            foreach (var point in polyLine)
            {
                if (point.Latitude < minLat){ minLat = point.Latitude; }
                else if (point.Latitude > maxLat){ maxLat = point.Latitude; }

                if (point.Longitude < minLon){ minLon = point.Longitude; }
                else if (point.Longitude > maxLon){ maxLon = point.Longitude; }
            }

            boundingBox.MaxLat = maxLat;
            boundingBox.MinLat = minLat;
            boundingBox.MaxLon = maxLon;
            boundingBox.MinLon = minLon;


            return boundingBox;
        }



        private static BoundingBox GetBufferedBoundingBox(List<Polylines.PolylineCoordinate> polyLine)
        {
            var boundingBox = new BoundingBox();

            double minLat = int.MaxValue;
            double maxLat = int.MinValue;
            double minLon = int.MaxValue;
            double maxLon = int.MinValue;

            foreach (var point in polyLine)
            {
                if (point.Latitude < minLat) { minLat = point.Latitude; }
                else if (point.Latitude > maxLat) { maxLat = point.Latitude; }

                if (point.Longitude < minLon) { minLon = point.Longitude; }
                else if (point.Longitude > maxLon) { maxLon = point.Longitude; }
            }

            boundingBox.MaxLat = maxLat + 0.1;
            boundingBox.MinLat = minLat - 0.1;
            boundingBox.MaxLon = maxLon + 0.1;
            boundingBox.MinLon = minLon - 0.1;


            return boundingBox;
        }

    }
}
