using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
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
    public class TestimoniesController : Controller
    {
        private const string FolderName = "TestimonyImages";
        static string apiUriTestimony = "Testimony";
        private readonly IConfiguration _configuration;
        public TestimoniesController(IConfiguration configuration)
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
            var requestUrl = $"{BaseUrl}{apiUriTestimony}/GetAllTestimonies?isFiltered={true}";
            TestimonyVM testimonyVM = new TestimonyVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                OutputHandler outputHandler = new OutputHandler();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                     
                    testimonyVM.OutputHandler = outputHandler;
                    testimonyVM.Testimonies = await response.Content.ReadAsAsync<IEnumerable<TestimonyDTO>>();
                };

                return View(testimonyVM);
            }
        }
        public async Task<IActionResult> ManageTestimonies(bool isDeleteFailed, string error)
        {
            var user = await StaticDataHandler.GetSessionDetails();
            var requestUrl = $"{BaseUrl}{apiUriTestimony}/GetAllTestimonies?isFiltered={false}";
            TestimonyVM testimonyVM = new TestimonyVM(_configuration);
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
                        outputHandler.Message = "Something went wrong, please contact techarch team";
                    }
                    else
                    {
                        outputHandler.IsErrorOccured = false;
                    }
                    testimonyVM.OutputHandler = outputHandler;
                    testimonyVM.Testimonies = await response.Content.ReadAsAsync<IEnumerable<TestimonyDTO>>();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    testimonyVM.OutputHandler.IsErrorOccured = true;
                    testimonyVM.OutputHandler.Message = "You're not Authorized to perfom this task";
                }
                else
                {
                    outputHandler.IsErrorOccured = false;
                };
                

                return View(testimonyVM);
            }
        }


        [HttpGet]
        public async Task<IActionResult> AddTestimony()
        {
            var testimonyDTO = new TestimonyDTO
            {
                OutputHandler = new OutputHandler
                {
                    IsErrorOccured = false
                }
            };
            testimonyDTO.TestimonyDate = DateTime.Now.AddHours(2);
            return View(testimonyDTO);
        }


        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> AddTestimony(TestimonyDTO testimonyDTO, IFormFile artwork)
        {

            try
            {
                var user = await StaticDataHandler.GetSessionDetails();
                testimonyDTO.CreatedBy = user.Username;
                testimonyDTO.CreatedDate = DateTime.Now.AddHours(2);
             
                if (ModelState.IsValid)
                {
                    var fileUploadResult = await StaticDataHandler.fileUpload(artwork,FolderName);
                    if (fileUploadResult.IsErrorOccured)
                    {
                        return View(testimonyDTO.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = "Something went wrong"
                        });
                    }
                    else
                    {
                        testimonyDTO.ImgBytes = (byte[])fileUploadResult.Result; //return the byte data
                        testimonyDTO.FileName = artwork.FileName;
                    }

                    var requestUrl = $"{BaseUrl}{apiUriTestimony}/CreateTestimony";
                    using (var client = new HttpClient())
                    {
                       
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                        client.BaseAddress = new Uri(requestUrl);
                        testimonyDTO.FileName = artwork.FileName;


                        var result = await client.PostAsJsonAsync(client.BaseAddress, testimonyDTO);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ManageTestimonies");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            testimonyDTO.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {
                            
                            testimonyDTO.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
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
                    var handler = new OutputHandler
                    {
                        IsErrorOccured = true,
                        Message = ex.InnerException.Message
                    };
                    ModelState.AddModelError("", $"ERROR: {ex.InnerException.Message}");
                }
                else
                {
                    var handler = new OutputHandler
                    {
                        IsErrorOccured = true,
                        Message = ex.Message
                    };
                    ModelState.AddModelError("", ex.Message);
                }
            }
            
            return View(testimonyDTO);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateTestimony(int testimonyId)
        {
            var requestUrl = $"{BaseUrl}{apiUriTestimony}/GetTestimony?testimonyId={testimonyId}";
            var testimonyDTO = new TestimonyDTO
            {
                OutputHandler = new OutputHandler
                {
                    IsErrorOccured = false
                }
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage result = await client.GetAsync(requestUrl);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    testimonyDTO = await result.Content.ReadAsAsync<TestimonyDTO>();
                    var outputHandler = new OutputHandler
                    {
                        IsErrorOccured = true
                    };
                    testimonyDTO.OutputHandler = outputHandler;
                }
                else
                {

                    testimonyDTO.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                }

            };


  
            return View(testimonyDTO);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> UpdateTestimony(TestimonyDTO testimonyDTO, IFormFile artwork)
        {

            try
            {

                var user = await StaticDataHandler.GetSessionDetails();
                testimonyDTO.ModifiedBy = user.Username;
                testimonyDTO.ModifiedDate = DateTime.Now.AddHours(2);
                if (ModelState.IsValid)
                {

                    if (artwork == null)
                    {

                    }
                    else
                    {
                        var fileUploadResult = await StaticDataHandler.fileUpload(artwork,FolderName);
                        if (fileUploadResult.IsErrorOccured)
                        {
                            return View(testimonyDTO.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "Something went wrong"
                            });
                        }
                        else
                        {
                            testimonyDTO.ImgBytes = (byte[])fileUploadResult.Result; //return the byte data
                            testimonyDTO.FileName = artwork.FileName;
                        }

                    }
                    var requestUrl = $"{BaseUrl}{apiUriTestimony}/UpdateTestimony";
                    using (var client = new HttpClient())
                    {
                        
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                        client.BaseAddress = new Uri(requestUrl);
                        

                        var result = await client.PutAsJsonAsync(client.BaseAddress, testimonyDTO);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ManageTestimonies");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            testimonyDTO.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {

                            testimonyDTO.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
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
                    testimonyDTO.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.InnerException.Message };
                }
                else
                {
                    testimonyDTO.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.Message };
                }
            }
            //IEnumerable<SermonCategoryDTO> sermonCategories = await StaticDataHandler.GetSermonCategory(BaseUrl);
            //ministryArm.SermonCategories = sermonCategories;
            return View(testimonyDTO);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var user = await StaticDataHandler.GetSessionDetails();
            var requestUrl = $"{BaseUrl}{apiUriTestimony}/DeleteTestimony?TestimonyId={id}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                var result = await client.DeleteAsync(client.BaseAddress);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("ManageTestimonies");
                }
                else if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("ManageTestimonies", new
                    {
                        isDeleteFailed = true,
                        error = "You're not Authorized to perfom this Action"
                    });

                }
                else
                {
                    return RedirectToAction("ManageTestimonies", new { isDeleteFailed = true });
                };
            };

        }

        public async Task<IActionResult> TestimonyDetails (int testimonyId)
        {
            var requestUrl = $"{BaseUrl}{apiUriTestimony}/GetTestimony?testimonyId={testimonyId}";
            var testimonyVM = new TestimonyVM(_configuration)
            {
                OutputHandler = new OutputHandler
                {
                    IsErrorOccured = false
                }
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage result = await client.GetAsync(requestUrl);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    testimonyVM.Testimony= await result.Content.ReadAsAsync<TestimonyDTO>();
                    var outputHandler = new OutputHandler
                    {
                        IsErrorOccured = true
                    };
                    testimonyVM.OutputHandler = outputHandler;
                }
                else
                {

                    testimonyVM.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                }

            };



            return View(testimonyVM);

        }
    }
}
