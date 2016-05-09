#region Copyright (C) 201 Sascha Simon

//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see http://www.gnu.org/licenses/.
//
//  Visit the official homepage at http://www.sascha-simon.com

#endregion

using Strava.Athletes;
using Newtonsoft.Json;

namespace Strava.Clubs
{
    public class ClubEvent
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("resource_state")]
        public int ResourceState { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("club_id")]
        public int ClubId { get; set; }

        [JsonProperty("organizing_athlete")]
        public AthleteSummary Organizer { get; set; }

        [JsonProperty("activity_type")]
        public string ActivityType { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("route_id")]
        public int? RouteId { get; set; }

        [JsonProperty("woman_only")]
        public bool IsWomanOnly { get; set; }

        [JsonProperty("private")]
        public bool IsPrivate { get; set; }

        [JsonProperty("skill_levels")]
        public int SkillLevels { get; set; }

        [JsonProperty("terrain")]
        public int Terrain { get; set; }

        [JsonProperty("upcoming_occurences")]
        public string[] Occurences { get; set; }

        [JsonProperty("address")]
        public string Address{ get; set; }
    }
}
