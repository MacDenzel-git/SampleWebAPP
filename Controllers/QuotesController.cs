using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using projectWebApplication.DTOs;
using projectWebApplication.General;
using projectWebApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace projectWebApplication.Controllers
{
    public class QuotesController : Controller
    {
        private const string FolderName = "QoutesArtworks";

        static string apiUriQoute = "Qoute";
        private readonly IConfiguration _configuration;
        public QuotesController(IConfiguration configuration)
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
        public async Task<IActionResult> ManageQoutes(bool isDeleteFailed,string error)
        {
            var user = await StaticDataHandler.GetSessionDetails();
            var requestUrl = $"{BaseUrl}{apiUriQoute}/GetAllQoutesForAdmin";
            QouteVM qouteVM = new QouteVM(_configuration);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);
               

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (isDeleteFailed) //This is coming from delete function if anything wrong happens there, throw error
                    {
                        qouteVM.OutputHandler.IsErrorOccured = true;
                        if (string.IsNullOrEmpty(error))
                        {
                            qouteVM.OutputHandler.Message = "Something went wrong, Delete failed. Check if related records exist e.g events or Media";

                        }
                        else
                        { qouteVM.OutputHandler.Message = error; }
                    }
                    else
                    {
                        qouteVM.OutputHandler.IsErrorOccured = false;
                    }
                   
                    qouteVM.QouteDTO = await response.Content.ReadAsAsync<IEnumerable<QouteDTO>>();
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    qouteVM.OutputHandler.IsErrorOccured = true;
                    qouteVM.OutputHandler.Message = "You're not Authorized to perfom this task";
                }
                else
                {
                    qouteVM.OutputHandler =  await response.Content.ReadAsAsync<OutputHandler>();
                };


                return View(qouteVM);
            }
        }
        
        public async Task<IActionResult> Index(int? page)
        {
            var requestUrl = $"{BaseUrl}{apiUriQoute}/GetAllQoutes?isFiltered={true}";
            QouteVM qouteVM = new QouteVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);
                OutputHandler outputHandler = new OutputHandler();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    qouteVM.QouteDTO = await response.Content.ReadAsAsync<IEnumerable<QouteDTO>>();
                };
                qouteVM.PaginatedQoutes = await PaginatedList<QouteDTO>.CreateAsync(qouteVM.QouteDTO, page ?? 1, 50);


               // qouteVM.List = Extensions.Batch(qouteVM.QouteDTO.OrderByDescending(x => x.QouteId), 7);
                qouteVM.PageSetup.PageTitle = "Quotes";
                return View(qouteVM);

            }
        }

        
        [HttpGet]
        public async Task<IActionResult> AddQoute()
        {
            var qouteDTO = new QouteDTO
            {
                OutputHandler = new OutputHandler
                {
                    IsErrorOccured = false
                }
            };
         
            return View(qouteDTO);
        }
       [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> AddQoute(QouteDTO qouteDTO, IFormFile artwork)
        {

            try
            {
                var user = await StaticDataHandler.GetSessionDetails();
                qouteDTO.CreatedBy = user.Username;
                qouteDTO.CreatedDate = DateTime.Now.AddHours(2);
                if (ModelState.IsValid)
                {
                    var fileUploadResult = await StaticDataHandler.fileUpload(artwork,FolderName, qouteDTO.IsFeaturedOnHomePage);
                    if (fileUploadResult.IsErrorOccured)
                    {
                        return View(qouteDTO.OutputHandler = new OutputHandler
                        {
                            IsErrorOccured = true,
                            Message = "Something went wrong"
                        });
                    }
                    else
                    {
                        qouteDTO.ImgBytes = (byte[])fileUploadResult.Result; //return the byte data
                        qouteDTO.FileName = artwork.FileName;
                    }

                    var requestUrl = $"{BaseUrl}{apiUriQoute}/CreateQoute";
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(requestUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

                        qouteDTO.FileName = artwork.FileName;
                        var result = await client.PostAsJsonAsync(client.BaseAddress, qouteDTO);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ManageQoutes");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            qouteDTO.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {
                           qouteDTO.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
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
           
            return View(qouteDTO);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateQoute(int qouteId)
        {
            var requestUrl = $"{BaseUrl}{apiUriQoute}/GetQoute?qouteId={qouteId}";
            var qouteDTO = new QouteDTO
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
                    qouteDTO = await result.Content.ReadAsAsync<QouteDTO>();
                    var outputHandler = new OutputHandler
                    {
                        IsErrorOccured = true
                    };
                    qouteDTO.OutputHandler = outputHandler;
                }
                else
                {

                    qouteDTO.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                }

            };


            //qoute.QouteImg = qoute.Artwork;
            return View(qouteDTO);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> UpdateQoute(QouteDTO qouteDTO, IFormFile artwork)
        {

            try
            {

                var user = await StaticDataHandler.GetSessionDetails();
                qouteDTO.ModifiedBy = user.Username;
                qouteDTO.ModifiedDate = DateTime.Now.AddHours(2);
                if (ModelState.IsValid)
                {

                    if (artwork == null)
                    {

                    }
                    else
                    {
                        var fileUploadResult = await StaticDataHandler.fileUpload(artwork,FolderName,qouteDTO.IsFeaturedOnHomePage);
                        if (fileUploadResult.IsErrorOccured)
                        {
                            return View(qouteDTO.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "Something went wrong"
                            });
                        }
                        else
                        {
                            qouteDTO.ImgBytes = (byte[])fileUploadResult.Result; //return the byte data
                            qouteDTO.FileName = artwork.FileName;
                        }

                    }
                    var requestUrl = $"{BaseUrl}{apiUriQoute}/UpdateQoute";
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(requestUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

                        var result = await client.PutAsJsonAsync(client.BaseAddress, qouteDTO);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("ManageQoutes");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            qouteDTO.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {

                            qouteDTO.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
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
           
            return View(qouteDTO);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var user = await StaticDataHandler.GetSessionDetails();

            var requestUrl = $"{BaseUrl}{apiUriQoute}/DeleteQoute?qouteId={id}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                client.DefaultRequestHeaders.Accept.Clear();

                var result = await client.DeleteAsync(client.BaseAddress);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("ManageQoutes");
                }
                else if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("ManageQoutes", new
                    {
                        isDeleteFailed = true,
                        error = "You're not Authorized to perfom this Action"
                    });

                }
                else
                {
                    return RedirectToAction("ManageQoutes", new { isDeleteFailed = true });
                };
            };

        }

        public async Task<FileResult> DownloadFile(int qouteId)
        {

            var requestUrl = $"{BaseUrl}{apiUriQoute}/GetQoute?qouteId={qouteId}";
            var qouteDTO = new QouteDTO
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
                    qouteDTO = await result.Content.ReadAsAsync<QouteDTO>();
                    var outputHandler = new OutputHandler
                    {
                        IsErrorOccured = true
                    };
                    qouteDTO.OutputHandler = outputHandler;
                }
                else
                {

                    qouteDTO.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                }

            };


            //qoute.QouteImg = qoute.Artwork;
            //Send the File to Download.
            var filename = Path.GetFileName(qouteDTO.QouteImg);
            return File(qouteDTO.ImgBytes, "application/octet-stream", filename);
        }

    }
}
