using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using Newtonsoft.Json;
using peaker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Spatial;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeakETL
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlServerTypes.Utilities.LoadNativeAssemblies(AppDomain.CurrentDomain.BaseDirectory);

            using (var db = new PeakDbContext())
            {
                Console.WriteLine(String.Format("{0} existing peaks found in database", db.Peaks.Count()));


                var geoJsonPath = "C:/dev/peaker/PeakETL/Data/peaks.geojson";

                Console.WriteLine("Reading geoJsonText from " + geoJsonPath);

                string geoJsonText = System.IO.File.ReadAllText(geoJsonPath);

                Console.WriteLine("Parsing GeoJSON for features");

                var features = JsonConvert.DeserializeObject<FeatureCollection>(geoJsonText);

                Console.WriteLine(String.Format("Found {0} features", features.Features.Count));



                Peak peak;
                int count = 0;
                int onePercent = features.Features.Count / 100;
                var lastTime = DateTime.Now;
                foreach (Feature feature in features.Features)
                {
                    if(count % onePercent == 0)
                    {
                        decimal percentComplete = count / (decimal)features.Features.Count;
                        Console.WriteLine(string.Format("{0}% complete", percentComplete * 100));
                        var timeSinceLastPercent = DateTime.Now - lastTime;
                        lastTime = DateTime.Now;
                        Console.WriteLine(string.Format("Time since: {0}", timeSinceLastPercent));
                    }


                    peak = new Peak();

                    try
                    {
                        peak.Name = feature.Properties.First(f => f.Key == "NAME").Value.ToString();
                    }
                    catch
                    {
                        peak.Name = "Unknown";
                    }

                    try
                    {
                        peak.ELEV_METER = int.Parse(feature.Properties.First(f => f.Key == "ELEV_METER").Value.ToString());
                    }
                    catch
                    {
                        peak.ELEV_METER = 0;
                    }

                    try
                    {
                        peak.STCTYFIPS = int.Parse(feature.Properties.First(f => f.Key == "STCTYFIPS").Value.ToString());
                    }
                    catch
                    {
                        peak.STCTYFIPS = 0;
                    }

                    Point location = feature.Geometry as Point;
                    GeographicPosition point = location.Coordinates as GeographicPosition;

                    //var pointString = string.Format("POINT({0} {1})", point.Longitude, point.Latitude);
                    //peak.Location = DbGeography.FromText(pointString);

                    peak.Latitude = point.Latitude;
                    peak.Longitude = point.Longitude;

                    var res = db.Database.ExecuteSqlCommand("INSERT INTO[dbo].[Peaks] ([Name],[STCTYFIPS],[ELEV_METER],[Latitude],[Longitude])VALUES({0},{1},{2},{3},{4})",
                        peak.Name,
                        peak.STCTYFIPS,
                        peak.ELEV_METER,
                        peak.Latitude,
                        peak.Longitude);

                    //db.Peaks.Add(peak);

                    count++;
                    //if (count % 100 == 0)
                    //{
                        
                    //    db.SaveChanges();
                    //}
                }  // for each
                //db.SaveChanges();

                Console.ReadKey();

            } //using dbcontext
        }  // main()
    } // class
}  // namespace


