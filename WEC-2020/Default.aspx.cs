﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace WEC_2020
{
    public partial class _Default : Page
    {

        public int page = 0;
        public string currentPage;
        List<Result> results;
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        
        protected void Search(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchQuery.Text))
            {

                ResultList.InnerHtml = string.Empty;

                string apiID = "006472699420085524376:lsvl8mcfohs";
                string key = "AIzaSyCYKYo5Knbq9FoMTELupi6iVXSaXcVLZ0g";
                string api = "https://www.googleapis.com/customsearch/v1?key=" + key + "&cx=" + apiID + "&q=";
                string q = SearchQuery.Text.Replace(' ', '+');
                api += q;

                this.currentPage = api;
                string json = string.Empty;
                try
                {
                     json = new WebClient().DownloadString(api);
                }
                catch
                {

                }
                SearchObject.Rootobject searchObj = JsonConvert.DeserializeObject<SearchObject.Rootobject>(json);

                results = Get_Results(searchObj);
                populateHtml(results);
                //string link = searchObj.items[1].link;
                // Perform Search method on SearchQuery.Text
            }
        }
        public void populateHtml(List<Result> results)
        {
            ResultList.InnerHtml = "";
                // Perform Search method on SearchQuery.Text
                foreach (Result item in results)
                {
                    string body = "<li  class=\"list-group-item\">";
                    body += "<div class=\"row \">";
                    body += "<a href=\"link\">";
                    body += $"<h2 class=\"font-weight-bold text-info \">{item.text}</h2>";
                    body += "</a>";
                    body += "</div>";
                    body += "<div class=\"row \">";
                    body += $"<a href = \"{item.link}\">{item.displayLink}</a>";
                    body += "</div>";
                    body += "<div class=\"row \">";
                    body += $"<h6 class=\"text-secondary\">{item.snippet}</h6>";
                    body += "</div>";
                    body += "</li>";
                    ResultList.InnerHtml += body;
                }
        }
        
        protected void Page_Previous(object sender, EventArgs e)
        {


        }
        protected void Page_Next(object sender, EventArgs e)
        {
        }


        protected List<Result> Get_Results(SearchObject.Rootobject searchObj)
        {
            List<Result> value = new List<Result>();
            try {
                for (int i = 0; i < searchObj.items.Length; i++)
                {
                    Result retrieved = new Result(searchObj.items[i].title, searchObj.items[i].link, searchObj.items[i].htmlSnippet
                        , searchObj.items[i].displayLink);
                    value.Add(retrieved);
                }
            }
            catch
            {
                return new List<Result>();
            }

            return value;
        }
        public void Tabify(bool isprevious)
        {
            if(isprevious && this.page != 0)
            {
                this.page --;
            }
            else if (!isprevious)
            {
                this.page ++;
            }
            string api = currentPage += "&start=" + (page * 10 + 1).ToString();
            string json = "Error: try failed.";
            try
            {
                json = new WebClient().DownloadString(api);
            }
            catch
            {
                
            }
            SearchObject.Rootobject searchObj = JsonConvert.DeserializeObject<SearchObject.Rootobject>(json);
            var results = Get_Results(searchObj);
            populateHtml(results);
            
        }
    }
}