using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using projectWebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace projectWebApplication.Controllers
{
    public class ResourcesController : Controller
    {
        private const string FolderName = "ResourceArtworks";

        private readonly IConfiguration _configuration;
        public ResourcesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        static string apiUriResources = "resource";
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointURL"];
            }
        }


        public async Task<IActionResult> ResourcesForAdmin(bool isDeleteFailed, string error)
        {
            var user = await StaticDataHandler.GetSessionDetails();

            var requestUrl = $"{BaseUrl}{apiUriResources}/GetResourcesForAdmin";
            ResourceVM resources = new ResourceVM(_configuration);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                client.BaseAddress = new Uri(requestUrl);
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
                    else
                    {
                        outputHandler.IsErrorOccured = false;
                    }

                    resources.OutputHandler = outputHandler; //tired
                    resources.Resources = await response.Content.ReadAsAsync<IEnumerable<ResourceDTO>>();

                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    resources.OutputHandler.IsErrorOccured = true;
                    resources.OutputHandler.Message = "You're not Authorized to perfom this task";
                }
                else
                {
                    outputHandler.IsErrorOccured = false;
                };


            };
             return View(resources);
        }
        public async Task<IActionResult> Resources(string resourceType, int? page)
        {
            var requestUrl = $"{BaseUrl}{apiUriResources}/GetResources?resourcetype={resourceType}";
            ResourcePageDTO resources = new ResourcePageDTO();
            ResourceVM resourceVM = new ResourceVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    resources = await response.Content.ReadAsAsync<ResourcePageDTO>();
                    
                }
                else
                {
                    resourceVM.OutputHandler = await response.Content.ReadAsAsync<OutputHandler>();
                }

            };

            //resources.ResourceCategories = await StaticDataHandler.GetResourceCategory(BaseUrl);


            resourceVM.PageSetup.PageTitle = resourceType;
            resourceVM.ResourceCategories = resources.ResourceCategories;
            resourceVM.PaginatedResources = await PaginatedList<ResourceDTO>.CreateAsync(resources.Resources, page ?? 1, 6);
            resourceVM.Resources = resources.Resources;
            return View(resourceVM);
        }

        public async Task<IActionResult> ResourcesByCategory(string name, int id)
        {
            var requestUrl = $"{BaseUrl}{apiUriResources}/ResourcesByCategory?categoryId={id}";
            ResourceVM resources = new ResourceVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    resources.Resources = await response.Content.ReadAsAsync<IEnumerable<ResourceDTO>>();

                };

            };
            resources.ResourceCategories = await StaticDataHandler.GetResourceCategory(BaseUrl);
            resources.PageHeader = name; //This is the category name carried over from the resource details page
            return View(resources);
        }

        public async Task<IActionResult> ResourceDetails(int resourceId)
        {
            var requestUrl = $"{BaseUrl}{apiUriResources}/GetResourceDetails?resourceId={resourceId}";
            ResourceVM resource = new ResourceVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    resource.Resource = await response.Content.ReadAsAsync<ResourceDTO>();
                };
                resource.ResourceCategories = await StaticDataHandler.GetResourceCategory(BaseUrl);
            };
            return View(resource);

        }

        [HttpGet]
        public async Task<IActionResult> AddResource()
        {
            PopulateDropdownDTO populateDropdownDTO = new PopulateDropdownDTO();
            populateDropdownDTO = await StaticDataHandler.PopulateResourceDropdown(BaseUrl);
            var resourceDTO = new ResourceDTO
            {
                ResourceCategories = populateDropdownDTO.ResourceCategories,
                ResourceTypes = populateDropdownDTO.ResourceTypes

            };
            return View(resourceDTO);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> AddResource(ResourceDTO resource, IFormFile artwork)
        {
            var user = await StaticDataHandler.GetSessionDetails();

            PopulateDropdownDTO populateDropdownDTO = new PopulateDropdownDTO();
            populateDropdownDTO = await StaticDataHandler.PopulateResourceDropdown(BaseUrl);
            try
            {
                resource.CreatedBy = user.Username;
                resource.DateCreated = DateTime.Now.AddHours(2);
                if (ModelState.IsValid)
                {
                    var fileUploadResult = await StaticDataHandler.fileUpload(artwork,FolderName,resource.IsFeatured);
                    if (fileUploadResult.IsErrorOccured)
                    {
                        return View();
                    }
                    else
                    {
                        resource.Artwork = (byte[])fileUploadResult.Result; //return the byte data
                        resource.Filename = artwork.FileName;
                    }

                    var requestUrl = $"{BaseUrl}{apiUriResources}/AddResource";
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(requestUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        resource.Filename = artwork.FileName;
                        var result = await client.PostAsJsonAsync(client.BaseAddress, resource);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ResourcesForAdmin");
                        }
                        else
                        {
                           resource.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                        }


                    };
                }
                else
                {
                   

                    resource.ResourceCategories = populateDropdownDTO.ResourceCategories;
                    resource.ResourceTypes = populateDropdownDTO.ResourceTypes;


                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message != null)
                {
                    resource.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.InnerException.Message };
                }
                else
                {
                    resource.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.Message };
                }
            }
            resource.ResourceCategories = populateDropdownDTO.ResourceCategories;
            resource.ResourceTypes = populateDropdownDTO.ResourceTypes;
            return View(resource);
        }

        [HttpGet]
        public async Task<IActionResult> EditResource(int resourceId)
        {
            
            var requestUrl = $"{BaseUrl}{apiUriResources}/GetResourceDetails?resourceId={resourceId}";
            ResourceDTO resource = new ResourceDTO();
            PopulateDropdownDTO populateDropdownDTO = new PopulateDropdownDTO();
            populateDropdownDTO = await StaticDataHandler.PopulateResourceDropdown(BaseUrl);
            using (var client = new HttpClient())
            {
                 client.BaseAddress = new Uri(requestUrl);

                HttpResponseMessage result = await client.GetAsync(requestUrl);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    resource = await result.Content.ReadAsAsync<ResourceDTO>();
                }
                
                else
                {

                    resource.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                }
                resource.ResourceCategories = populateDropdownDTO.ResourceCategories;
                resource.ResourceTypes = populateDropdownDTO.ResourceTypes;
            };

           
            resource.ResourceCategories = populateDropdownDTO.ResourceCategories;
            resource.ResourceTypes = populateDropdownDTO.ResourceTypes;
            resource.OldImageUrl = resource.ImageUrl;
            return View(resource);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> EditResource(ResourceDTO resource, IFormFile artwork)
        {
            var user = await StaticDataHandler.GetSessionDetails();
            PopulateDropdownDTO populateDropdownDTO = new PopulateDropdownDTO();
            populateDropdownDTO = await StaticDataHandler.PopulateResourceDropdown(BaseUrl);
            try
            {
          
                resource.CreatedBy = "Denzel";
                resource.DateCreated = DateTime.Now.AddHours(2);
                if (ModelState.IsValid)
                {

                    if (artwork == null)
                    {
                    }
                    else
                    {
                        var fileUploadResult = await StaticDataHandler.fileUpload(artwork,FolderName,resource.IsFeatured);
                        if (fileUploadResult.IsErrorOccured)
                        {
                            return View(resource.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "Something went wrong"
                            });
                        }
                        else
                        {
                            resource.Artwork = (byte[])fileUploadResult.Result; //return the byte data
                            resource.Filename = artwork.FileName;
                        }

                    }
                    var requestUrl = $"{BaseUrl}{apiUriResources}/EditResource";
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(requestUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

                        var result = await client.PostAsJsonAsync(client.BaseAddress, resource);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ResourcesForAdmin");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            resource.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {
                            populateDropdownDTO = await StaticDataHandler.PopulateResourceDropdown(BaseUrl);
                            resource.ResourceCategories = populateDropdownDTO.ResourceCategories;
                            resource.ResourceTypes = populateDropdownDTO.ResourceTypes;
                            resource.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                        }
                    };
                }
                else
                {
                    resource.ResourceCategories = await StaticDataHandler.GetResourceCategory(BaseUrl);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message != null)
                {
                    resource.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.InnerException.Message };
                }
                else
                {
                    resource.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.Message };
                }
            }

           
            resource.ResourceCategories = populateDropdownDTO.ResourceCategories;
            resource.ResourceTypes = populateDropdownDTO.ResourceTypes;
            return View(resource);
        }



        public async Task<IActionResult> Delete(int id)
        {
            var user = await StaticDataHandler.GetSessionDetails();

            var requestUrl = $"{BaseUrl}{apiUriResources}/DeleteResource?resourceId={id}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

                var result = await client.DeleteAsync(client.BaseAddress);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("ResourcesForAdmin");
                }
                else if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("ResourcesForAdmin", new
                    {
                        isDeleteFailed = true,
                        error = "You're not Authorized to perfom this Action"
                    });

                }
                else
                {
                    return RedirectToAction("ResourcesForAdmin", new { isDeleteFailed = true });
                };
            };

        }

    }
}
