
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RFPWebApplication.DTOs;
using RFPWebApplication.General;
using RFPWebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace RFPWebApplication.Controllers
{
    public class EventController : Controller
    {
        private const string FolderName = "EventArtworks";
        static string apiUriEvent = "EventManagement";
        private readonly IConfiguration _configuration;
        public EventController(IConfiguration configuration)
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
        public async Task<IActionResult> ManageEvents(bool isDeleteFailed, string error)
        {
            var user = await StaticDataHandler.GetSessionDetails();
            var requestUrl = $"{BaseUrl}{apiUriEvent}/GetEvents";
            EventManagementVM eventManagementVM = new EventManagementVM(_configuration);
            using (var client = new HttpClient())
            {
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
                    eventManagementVM.OutputHandler = outputHandler;
                    eventManagementVM.EventDTO = await response.Content.ReadAsAsync<IEnumerable<EventDTO>>();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    eventManagementVM.OutputHandler.IsErrorOccured = true;
                    eventManagementVM.OutputHandler.Message = "You're not Authorized to perfom this task";
                }
                else
                {
                    outputHandler.IsErrorOccured = false;
                };


                return View(eventManagementVM);
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddEvent()
        {
            EventDTO eventDTO = new EventDTO
            {
                MinistryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl),
                OutputHandler = new OutputHandler { IsErrorOccured = false}
            };
            eventDTO.DateOfEvent = DateTime.UtcNow.AddHours(2);
            eventDTO.EventEndDate = DateTime.UtcNow.AddHours(2);
            return View(eventDTO);
        }

       [HttpPost]
        public async Task<IActionResult> AddEvent(EventDTO eventDTO, IFormFile artwork)
        {
          

            try
            {



                var user = await StaticDataHandler.GetSessionDetails();
                eventDTO.CreatedBy = user.Username;
                eventDTO.CreatedDate = DateTime.Now.AddHours(2);
                if (ModelState.IsValid)
                {
                    var fileUploadResult = await StaticDataHandler.fileUpload(artwork, FolderName,eventDTO.IsTimeActive);
                    if (fileUploadResult.IsErrorOccured)
                    {
                        return View();
                    }
                    else
                    {
                       eventDTO.ImgBytes = (byte[])fileUploadResult.Result; //return the byte data
                        eventDTO.FileName = artwork.FileName;
                    }

                    var requestUrl = $"{BaseUrl}{apiUriEvent}/CreateSingleEvent";
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(requestUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                        eventDTO.FileName = artwork.FileName;
                        var result = await client.PostAsJsonAsync(client.BaseAddress, eventDTO);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ManageEvents");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            eventDTO.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {
                            
                           eventDTO.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
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
                    ModelState.AddModelError("", $"ERROR: {ex.InnerException.Message}");
                }
                else
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
            eventDTO.MinistryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl);
            return View(eventDTO);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateEvent(int eventId)
        {
            var requestUrl = $"{BaseUrl}{apiUriEvent}/GetEvent?eventId={eventId}";
            var eventDTO = new EventDTO
            {
                OutputHandler = new OutputHandler { IsErrorOccured = false },
                MinistryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl)
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage result = await client.GetAsync(requestUrl);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    eventDTO = await result.Content.ReadAsAsync<EventDTO>();
                    var outputHandler = new OutputHandler
                    {
                        IsErrorOccured = true
                    };
                    //eventDTO.OutputHandler = outputHandler;
                }
                else
                {

                    //eventDTO.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                }

            };
            eventDTO.MinistryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl);
            eventDTO.OldImageUrl = eventDTO.ImageUrl;
            return View(eventDTO);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> UpdateEvent(EventDTO eventDTO, IFormFile artwork)
        {

            try
            {
                var user = await StaticDataHandler.GetSessionDetails();
                eventDTO.ModifiedBy = user.Username;
                eventDTO.ModifiedDate = DateTime.Now.AddHours(2);
                if (ModelState.IsValid)
                {

                    if (artwork == null)
                    {

                    }
                    else
                    {
                        var fileUploadResult = await StaticDataHandler.fileUpload(artwork,FolderName,eventDTO.IsTimeActive);
                        if (fileUploadResult.IsErrorOccured)
                        {
                            return View();
                        }
                        else
                        {
                            eventDTO.ImgBytes = (byte[])fileUploadResult.Result; //return the byte data
                            eventDTO.FileName = artwork.FileName;
                        }

                    }
                    var requestUrl = $"{BaseUrl}{apiUriEvent}/UpdateEvent";
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(requestUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

                        var result = await client.PutAsJsonAsync(client.BaseAddress, eventDTO);
                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ManageEvents");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            eventDTO.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {
                            eventDTO.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
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
                    ModelState.AddModelError("", $"ERROR: {ex.InnerException.Message}");
                }
                else
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }
           
            return View(eventDTO);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var requestUrl = $"{BaseUrl}{apiUriEvent}/DeleteEvent?eventId={id}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.DefaultRequestHeaders.Accept.Clear();

                var result = await client.DeleteAsync(client.BaseAddress);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("ManageEvents");
                }
                else
                {
                    return RedirectToAction("ManageEvents", new { isDeleteFailed = true });
                };
            };

        }

        public async Task<IActionResult> EventDetails(int eventId)
        {
            var requestUrl = $"{BaseUrl}{apiUriEvent}/GetEvent?eventId={eventId}";
            EventManagementVM resource = new EventManagementVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    resource.EventSingleObject = await response.Content.ReadAsAsync<EventDTO>();
                };

            };
            return View(resource);

        }
    }
}
