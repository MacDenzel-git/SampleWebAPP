﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
using projectWebApplication.DTOs.Authentication;
using projectWebApplication.General;
using projectWebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace projectWebApplication.Controllers
{
    public class MinistryArmsController : Controller
    {
       
        private const string FolderName = "MinistryArmsArtworks";
        static string apiUriMinistryArms = "MinistryArm";
        private readonly IConfiguration _configuration;
        public MinistryArmsController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointURL"];
            }
        }
        public async Task<IActionResult> Index()
        {
            var requestUrl = $"{BaseUrl}{apiUriMinistryArms}/GetMinistryArms";
            MinistryArmVM ministrArmVM = new MinistryArmVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ministrArmVM.MinistryArmsList = await response.Content.ReadAsAsync<IEnumerable<MinistryArmDTO>>();

                };

            };
            return View(ministrArmVM);
        }

        public async Task<IActionResult> ManageMinistryArms(bool isDeleteFailed, string error)
        {
            var user = await StaticDataHandler.GetSessionDetails();
            //send admin = true to avoid looping which converts files to bytes, we don't need it here. 
            //True means this request has been generated by an individual with admin privileges from the Admin Portal, so the API handles the request as an admin request
            var requestUrl = $"{BaseUrl}{apiUriMinistryArms}/GetMinistryArms?isAdminRequest={true}";
            MinistryArmVM ministryArmVM = new MinistryArmVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                OutputHandler outputHandler = new OutputHandler();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (isDeleteFailed) //This is coming from delete function if anything wrong happens there, throw error
                    {
                        outputHandler.IsErrorOccured = true;
                        if (string.IsNullOrEmpty(error))
                        {
                            outputHandler.Message = "Something went wrong, Delete failed. Check if related records exist e.g events or Media";

                        }
                        else
                        { outputHandler.Message = error; }
                    }
                   
                    ministryArmVM.OutputHandler = outputHandler;
                    ministryArmVM.MinistryArmsList = await response.Content.ReadAsAsync<IEnumerable<MinistryArmDTO>>();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ministryArmVM.OutputHandler.IsErrorOccured = true;
                    ministryArmVM.OutputHandler.Message = "You're not Authorized to perfom this task";
                }
                else
                {
                    outputHandler.IsErrorOccured = false;
                };

            };
            //sermons.SermonCategories = await StaticDataHandler.GetSermonCategory(BaseUrl);
            return View(ministryArmVM);
        }
        public async Task<IActionResult> MinistryArms()
        {
            var requestUrl = $"{BaseUrl}{apiUriMinistryArms}/GetMinistryArms";
            MinistryArmVM ministrArmVM = new MinistryArmVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ministrArmVM.MinistryArmsList = await response.Content.ReadAsAsync<IEnumerable<MinistryArmDTO>>();

                };

            };
            return View(ministrArmVM);
        }

        public async Task<IActionResult> ResourcesByCategory(string name, int id)
        {
            var requestUrl = $"{BaseUrl}{apiUriMinistryArms}/SermonsByCategory?categoryId={id}";
            ResourceVM sermons = new ResourceVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sermons.Resources = await response.Content.ReadAsAsync<IEnumerable<ResourceDTO>>();

                };

            };
            sermons.ResourceCategories = await StaticDataHandler.GetResourceCategory(BaseUrl);
            sermons.PageHeader = name; //This is the category name carried over from the sermon details page
            return View(sermons);
        }

        public async Task<IActionResult> ResourceDetails(int resourceId)
        {
            var requestUrl = $"{BaseUrl}{apiUriMinistryArms}/GetResourcesDetails?resourceId={resourceId}";
            ResourceDTO sermon = new ResourceDTO();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    sermon = await response.Content.ReadAsAsync<ResourceDTO>();
                };
                sermon.ResourceCategories = await StaticDataHandler.GetResourceCategory(BaseUrl);
            };
            return View(sermon);

        }

        [HttpGet]
        public async Task<IActionResult> AddMinistryArm()
        {
            var ministry = new MinistryArmDTO
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false }
            };

            return View(ministry);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> AddMinistryArm(MinistryArmDTO ministryArm, IFormFile artwork)
        {

            try
            {
                var user = await StaticDataHandler.GetSessionDetails();
                ministryArm.CreatedBy = user.Username;
                ministryArm.CreatedDate = DateTime.Now.AddHours(2);
               
                if (ModelState.IsValid)
                {
                    var fileUploadResult = await StaticDataHandler.fileUpload(artwork, FolderName, ministryArm.IsFeaturedOnHomePage);
                    if (fileUploadResult.IsErrorOccured)
                    {
                        return View(ministryArm.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = "Something went wrong"
                        });
                    }
                    else
                    {
                        ministryArm.ImgBytes = (byte[])fileUploadResult.Result; //return the byte data
                        ministryArm.Filename = artwork.FileName;
                    }

                    var requestUrl = $"{BaseUrl}{apiUriMinistryArms}/CreateMinistryArm";
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                        client.BaseAddress = new Uri(requestUrl);
                        ministryArm.Filename = artwork.FileName;


                        var result = await client.PostAsJsonAsync(client.BaseAddress, ministryArm);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ManageMinistryArms");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            ministryArm.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {
                            ministryArm.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                        }
                    };
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message != null)
                {
                    ministryArm.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.InnerException.Message };
                }
                else
                {
                    ministryArm.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.Message };
                }
            }
            IEnumerable<ResourceCategoryDTO> sermonCategories = await StaticDataHandler.GetResourceCategory(BaseUrl);

            return View(ministryArm);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateMinistryArm(int ministryArmId)
        {
            var requestUrl = $"{BaseUrl}{apiUriMinistryArms}/GetMinistryArm?ministryArmId={ministryArmId}";
            MinistryArmVM ministryArmVm = new MinistryArmVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage result = await client.GetAsync(requestUrl);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    ministryArmVm.MinistryArmDTO = await result.Content.ReadAsAsync<MinistryArmDTO>();
                }
                else
                {

                    ministryArmVm.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                }

            };


            ministryArmVm.MinistryArmDTO.OldImageUrl = ministryArmVm.MinistryArmDTO.Artwork;
            return View(ministryArmVm.MinistryArmDTO);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> UpdateMinistryArm(MinistryArmDTO ministryArm, IFormFile artwork)
        {

            try
            {
                var user = await StaticDataHandler.GetSessionDetails();
                ministryArm.ModifiedBy = user.Username ;
                ministryArm.ModifiedDate = DateTime.Now.AddHours(2);
                if (ModelState.IsValid)
                {

                    if (artwork == null)
                    {

                    }
                    else
                    {
                        var fileUploadResult = await StaticDataHandler.fileUpload(artwork, FolderName, ministryArm.IsFeaturedOnHomePage);
                        if (fileUploadResult.IsErrorOccured)
                        {
                            return View(ministryArm.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "Something went wrong"
                            });
                        }
                        else
                        {
                            ministryArm.ImgBytes = (byte[])fileUploadResult.Result; //return the byte data
                            ministryArm.Filename = artwork.FileName;
                        }

                    }
                    var requestUrl = $"{BaseUrl}{apiUriMinistryArms}/UpdateMinistryArm";
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(requestUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

                        var result = await client.PutAsJsonAsync(client.BaseAddress, ministryArm);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ManageMinistryArms");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            ministryArm.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {

                            ministryArm.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                        }
                    };
                }
                else
                {
 
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message != null)
                {
                    ministryArm.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.InnerException.Message };
                }
                else
                {
                    ministryArm.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.Message };
                }
            }
            return View(ministryArm);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await StaticDataHandler.GetSessionDetails();
            var requestUrl = $"{BaseUrl}{apiUriMinistryArms}/DeleteMinistryArm?ministryArmId={id}";
            using (var client = new HttpClient())
            {

                client.BaseAddress = new Uri(requestUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                var result = await client.DeleteAsync(client.BaseAddress);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("ManageMinistryArms");
                }
                else if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("ManageMinistryArms", new 
                    { 
                        isDeleteFailed = true, 
                        error = "You're not Authorized to perfom this Action"
                    });

                }
                else
                {
                    return RedirectToAction("ManageMinistryArms", new { isDeleteFailed = true });
                };
            };

        }

        public async Task<IActionResult> MinistryArmDetails(int ministryArmId)
        {
            var requestUrl = $"{BaseUrl}{apiUriMinistryArms}/GetMinistryArm?ministryArmId={ministryArmId}";
            MinistryArmVM ministryArmVm = new MinistryArmVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage result = await client.GetAsync(requestUrl);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    ministryArmVm.MinistryArmDTO = await result.Content.ReadAsAsync<MinistryArmDTO>();
                }
                else
                {

                    ministryArmVm.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                }

            };

            if (ministryArmVm.MinistryArmDTO.MinistryArmAbbreviation == null)
            {
                ministryArmVm.PageSetup.PageTitle = ministryArmVm.MinistryArmDTO.MinistryArmName;
            }
            else
            {
                ministryArmVm.PageSetup.PageTitle = $"{ministryArmVm.MinistryArmDTO.MinistryArmName}({ministryArmVm.MinistryArmDTO.MinistryArmAbbreviation})";

            }
            ministryArmVm.MinistryArmDTO.OldImageUrl = ministryArmVm.MinistryArmDTO.Artwork;
            return View(ministryArmVm);
        }

        
    }
}
