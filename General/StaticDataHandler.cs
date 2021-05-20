using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using projectWebApplication.DTOs;
using projectWebApplication.DTOs.Authentication;
using projectWebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace projectWebApplication.General
{
    public class StaticDataHandler
    {
        public static async Task<IEnumerable<ResourceCategoryDTO>> GetResourceCategory(string baseUrl)
        {
            var requestUrl = $"{baseUrl}ResourceCategory/GetResourceCategories";
            IEnumerable<ResourceCategoryDTO> resourceCategories = new List<ResourceCategoryDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    resourceCategories = await response.Content.ReadAsAsync<IEnumerable<ResourceCategoryDTO>>();
                };

            };
            return resourceCategories;
        }
        public static async Task<PopulateDropdownDTO> PopulateResourceDropdown(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Utils/GetDropdownItems";
            PopulateDropdownDTO items = new PopulateDropdownDTO();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    items = await response.Content.ReadAsAsync<PopulateDropdownDTO>();
                };

            };
            return items;
        }

        public static async Task<IEnumerable<MinistryArmDTO>> GetMinistryArmsAsync(string baseUrl)
        {

            var requestUrl = $"{baseUrl}MinistryArm/GetMinistryArms?isAdminRequest={true}";
            IEnumerable<MinistryArmDTO> ministrArms = new List<MinistryArmDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ministrArms = await response.Content.ReadAsAsync<IEnumerable<MinistryArmDTO>>();

                };

            };
            return ministrArms;
        }

        public static async Task<IEnumerable<BranchDTO>> GetBranches(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Branch/GetBranches";
            IEnumerable<BranchDTO> branches = new List<BranchDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    branches = await response.Content.ReadAsAsync<IEnumerable<BranchDTO>>();
                };

            };

            return branches;
        }

        public static async Task<IEnumerable<PositionDTO>> GetPositions(string baseUrl)
        {
            var requestUrl = $"{baseUrl}Position/GetAllPositions";
            IEnumerable<PositionDTO> positions = new List<PositionDTO>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    positions = await response.Content.ReadAsAsync<IEnumerable<PositionDTO>>();
                };

            };

            return positions;
        }

        public static async Task<OutputHandler> fileUpload(IFormFile artwork,string foldername, bool IsFeaturedOnHomePage=false)
        {
            try
            {
                var imageFolderName = Path.Combine("wwwroot", "images");
                var imagePathToSave = Path.Combine(Directory.GetCurrentDirectory(), imageFolderName,foldername);
                //DirectoryInfo di = Directory.CreateDirectory(pathToSave);
                var artworkFileName = ""; var imageFullPath = "";
                if (artwork.Length > 0)
                {
                    artworkFileName = ContentDispositionHeaderValue.Parse(artwork.ContentDisposition).FileName.Trim('"');
                    imageFullPath = Path.Combine(imagePathToSave, artworkFileName);

                    using (var stream = new FileStream(imageFullPath, FileMode.Create))
                    {
                        artwork.CopyTo(stream);
                    }
                }

                //Convert the file to transfer it over to the API

                using (FileStream fs = new FileStream(imageFullPath, FileMode.Open, FileAccess.Read))
                {
                    // Create a byte array of file stream length
                    byte[] bytes = System.IO.File.ReadAllBytes(imageFullPath);
                    //Read block of bytes from stream into the byte array
                    fs.Read(bytes, 0, System.Convert.ToInt32(fs.Length));
                    //Close the File Stream
                    fs.Close();
                    // teamMember.Artwork = bytes; //return the byte data


                    if (!IsFeaturedOnHomePage) //if file is for the home page keep a copy on the application for faster loading to avoid file to byte API convertion when loading the homepage
                    {
                        ///Delete file from folder 
                        if (System.IO.File.Exists(imageFullPath))
                        {
                            System.IO.File.Delete(imageFullPath);
                        }
                    }

                    return new OutputHandler
                    {
                        IsErrorOccured = false,
                        Result = bytes
                    };
                }
            }
            catch (Exception ex)
            {
                return new OutputHandler
                {
                    IsErrorOccured = true,

                };
            }
        }
        public static string GetDynamicMenuItems(string baseUrl)
        {
            try
            {

                string response = "";
                var requestUrl = $"{baseUrl}Utils/GetMenuItems";
                using (var client = new WebClient())
                {
                    response = client.DownloadString(requestUrl);
                    if (!string.IsNullOrEmpty(response))
                    {
                        //  baseViewModel = await response.Content.ReadAsAsync<BaseViewModel>();

                    };

                };
                return response;
            }
            catch (Exception ex)
            {

                throw;
            }



        }

        public static async Task<SessionDetailsDTO> GetSessionDetails()
        {
            SessionDetailsDTO sessionDetails =  new SessionDetailsDTO
            {
                               
                Username = SessionHandlerAppContext.Current.Session.GetString("username"),
                Token = SessionHandlerAppContext.Current.Session.GetString("Token"),
                 
            };

            if (string.IsNullOrEmpty(sessionDetails.Token))
            {
                sessionDetails.IsSet = false;
            }
            else 
            { sessionDetails.IsSet = true; }
            return sessionDetails;
        }






        //public static BaseViewModel GetDynamicMenuItems(string baseUrl)
        //{

        //    var requestUrl = $"{baseUrl}Utils/GetMenuItems";
        //    BaseViewModel baseViewModel = new BaseViewModel(_);
        //    using (var client = new WebClient())
        //    {
        //        //client.BaseAddress = new Uri(requestUrl);
        //       string response = client.DownloadString(requestUrl);

        //        if (!string.IsNullOrEmpty(response))
        //        {
        //          //  baseViewModel = await response.Content.ReadAsAsync<BaseViewModel>();

        //        };

        //    };
        //    return baseViewModel;

        //}
    }
}
