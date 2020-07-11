﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;

namespace WEC_2020
{
    public partial class _Default : Page
    {
        //Api required parameters
        string apiID = "016570590808609382194:dvshtfuzlco";
        string key = "AIzaSyD4-OyRy5BX7JOZOTfefm0Wd8ONP83AeMM";
        string api;

        List<Result> results;
        //on page load we update inputs and set the api string.
        protected void Page_Load(object sender, EventArgs e)
        {
            api = "https://www.googleapis.com/customsearch/v1?key=" + key + "&cx=" + apiID + "&q=";
            UpdateInputs();

        }
        //this function is responsible for updating visibility of next and previous buttons.
        protected void UpdateInputs()
        {
            Btn_Current_Page.Visible = true;
            Btn_Current_Page.Enabled = true;
            Btn_Prev.Enabled = true;
            Btn_Prev.Visible = true;
            Btn_Next.Enabled = true;
            Btn_Next.Visible = true;
            Btn_Current_Page.Text = Convert.ToString(Convert.ToInt32(Session["Page"])+1);
            if (string.IsNullOrWhiteSpace(SearchQuery.Text) && Convert.ToInt32(Session["Page"]) == 0)
            {
                Btn_Prev.Enabled = false;
                Btn_Prev.Visible = false;
                Btn_Next.Enabled = false;
                Btn_Next.Visible = false;
                Btn_Current_Page.Visible = false;
                Btn_Current_Page.Enabled = false;
            }
            else if (Convert.ToInt32(Session["MaxPageCount"]) == 0)
            {

                Btn_Prev.Enabled = false;
                Btn_Prev.Visible = false;
                Btn_Next.Enabled = false;
                Btn_Next.Visible = false;
                Btn_Current_Page.Visible = false;
                Btn_Current_Page.Enabled = false;
            }
            if (Convert.ToInt32(Session["Page"]) <= 0)
            {
                Btn_Prev.Enabled = false;
                Btn_Prev.Visible = false;
                
            }
            if (Convert.ToInt32(Session["Page"]) >= Convert.ToInt32(Session["MaxPageCount"]))
            {

                Btn_Next.Enabled = false;
                Btn_Next.Visible = false;

            }
          
        }
        /*
         * This function gets results from google api and stores it as list of objects 
         * and calls the populateHtml function.
        */
        protected void Search(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(SearchQuery.Text))
            {
                Session["Page"] = 0;
                //ResultList.InnerHtml = string.Empty;
                Session["Query"] = SearchQuery.Text.Replace(' ', '+');

                string query = api+ Session["Query"];

                string json = string.Empty;
                try
                {
                     json = new WebClient().DownloadString(query);
                    SearchObject.Rootobject searchObj = JsonConvert.DeserializeObject<SearchObject.Rootobject>(json);
                    results = Get_Results(searchObj);
                    Session["MaxPageCount"] = Math.Ceiling(Convert.ToDecimal(searchObj.searchInformation.totalResults)/10);
                    if (Convert.ToInt32(searchObj.searchInformation.totalResults) != 0)
                    {
                        totalResults.Visible = true;
                        totalResults.Text = $"Total Results: {searchObj.searchInformation.totalResults}";
                    }
                    else
                    {
                        totalResults.Text = $"No Results Found";
                    }
                    populateHtml(results);
                }
                catch
                {
                }
                UpdateInputs();
            }
        }
        /*This function takes the list of results and populates the 
         * web form.
        */
        public void populateHtml(List<Result> results)
        {
            ResultList.InnerHtml = "";
                // Perform Search method on SearchQuery.Text
                foreach (Result item in results)
                {
                    string body = "<li  class=\"list-group-item\">";
                    body += "<div class=\"row \">";
                    body += $"<a href=\"{item.link}\">";
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
        //This function is called to move back to previous page of results
        protected void Page_Previous(object sender, EventArgs e)
        {
            Tabify(true);

        }
        //This function is called to move the the next page of results
        protected void Page_Next(object sender, EventArgs e)
        {
            Tabify(false);
        }

        /*
         * This function takes the objects generated from the json file and
         * compresses them into new objects with only the required fields.
         */
        protected List<Result> Get_Results(SearchObject.Rootobject searchObj)
        {
            List<Result> value = new List<Result>();
            try {
                if (searchObj != null)
                {
                    if (searchObj.items != null)
                    {
                        for (int i = 0; i < searchObj.items.Length; i++)
                        {
                            Result retrieved = new Result(searchObj.items[i].title, searchObj.items[i].link, searchObj.items[i].htmlSnippet
                                , searchObj.items[i].displayLink);
                            value.Add(retrieved);
                        }
                    }
                }
            }
            catch( NullReferenceException ex)
            {
                return new List<Result>();
            }

            return value;
        }

        /*
         * This function is called when prevous or next is clicked
         * It grabs the results of the page that you are moving to 
         * and calls populateHtml to display the results.
         */
        public void Tabify(bool isprevious)
        {

            if (isprevious && Convert.ToInt32(Session["Page"]) != 0)
            {
                Session["Page"] = Convert.ToInt32(Session["Page"]) - 1;
                
            }
            else if (!isprevious)
            {
                Session["Page"] = Convert.ToInt32(Session["Page"]) + 1;
            }

            
            string query = api + Session["Query"] + "&start=" + (Convert.ToInt32(Session["Page"]) * 10 + 1).ToString();

            string json = string.Empty;
            try
            {
                json = new WebClient().DownloadString(query);
                SearchObject.Rootobject searchObj = JsonConvert.DeserializeObject<SearchObject.Rootobject>(json);
                var results = Get_Results(searchObj);
                
                populateHtml(results);
            }
            catch
            {
                
            }
            UpdateInputs();

        }
    }
}