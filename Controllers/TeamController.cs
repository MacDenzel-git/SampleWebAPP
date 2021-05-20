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
    public class TeamController : Controller
    {
        private readonly IConfiguration _configuration;
        private const string FolderName = "TeamMembersArtworks";
        static string apiUriTeamMembers = "TeamMembers";
        static string apiUriResources = "resource";
        public TeamController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> PastorAubrey()
        {
            PageDTO pastorAubreyPage = new PageDTO(_configuration);
            var requestUrl = $"{BaseUrl}{apiUriResources}/GetFeaturedResources";
            ResourcePageDTO resources = new ResourcePageDTO();
            ResourceVM resourceVM = new ResourceVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                  pastorAubreyPage.Resources = await response.Content.ReadAsAsync<IEnumerable<ResourceDTO>>();

                }
                else
                {
                    resourceVM.OutputHandler = await response.Content.ReadAsAsync<OutputHandler>();
                }

            };
            
            return View(pastorAubreyPage);
        }
         
       
        public string BaseUrl
        {
            get
            {
                return _configuration["EndpointURL"];
            }
        }


        public async Task<IActionResult> TeamMembersForAdmin(bool isDeleteFailed, string error)
        {
            var user = await StaticDataHandler.GetSessionDetails();

            var requestUrl = $"{BaseUrl}{apiUriTeamMembers}/GetTeamMembersForAdmin";
            TeamMembersVM teamMembers = new TeamMembersVM(_configuration);
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
                        outputHandler.Message = "Delete failed! Something went wrong, please contact techarch team";
                    }
                    else
                    {
                        outputHandler.IsErrorOccured = false;
                    }
                    teamMembers.OutputHandler = outputHandler;
                    teamMembers.TeamMembersDTO = await response.Content.ReadAsAsync<IEnumerable<TeamMembersDTO>>();

                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    teamMembers.OutputHandler.IsErrorOccured = true;
                    teamMembers.OutputHandler.Message = "You're not Authorized to perfom this task";
                }
                else
                {
                    outputHandler.IsErrorOccured = false;
                };
                

            };
            //teamMembers.Branches = await StaticDataHandler.GetTeamMemberCategory(BaseUrl);
            return View(teamMembers);
        }
        public async Task<IActionResult> TeamMembers()
        {
            var requestUrl = $"{BaseUrl}{apiUriTeamMembers}/GetTeamMembers";
            TeamMembersVM teamMembers = new TeamMembersVM(_configuration);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    teamMembers.TeamMembersDTO = await response.Content.ReadAsAsync<IEnumerable<TeamMembersDTO>>();

                };

            };
            teamMembers.BranchDTO = await StaticDataHandler.GetBranches(BaseUrl);
            return View(teamMembers);
        }

        //public async Task<IActionResult> TeamMembersByCategory(string name, int id)
        //{
        //    var requestUrl = $"{BaseUrl}{apiUriTeamMembers}/TeamMembersByCategory?categoryId={id}";
        //    TeamMembersVM teamMembers = new TeamMembersVM();
        //    using (var client = new HttpClient())
        //    {
        //        client.BaseAddress = new Uri(requestUrl);
        //        HttpResponseMessage response = await client.GetAsync(requestUrl);

        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            teamMembers.TeamMembersDTO = await response.Content.ReadAsAsync<IEnumerable<TeamMembersDTO>>();

        //        };

        //    };
        //    teamMembers.BranchDTO = await StaticDataHandler.GetBranches(BaseUrl);
        //   // teamMembers.PageHeader = name; //This is the category name carried over from the teamMember details page
        //    return View(teamMembers);
        //}

        public async Task<IActionResult> TeamMemberDetails(int teamMemberId)
        {
            var requestUrl = $"{BaseUrl}{apiUriTeamMembers}/GetTeamMemberDetails?teamMemberId={teamMemberId}";
            TeamMembersDTO teamMember = new TeamMembersDTO();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    teamMember = await response.Content.ReadAsAsync<TeamMembersDTO>();
                };
                teamMember.Branches = await StaticDataHandler.GetBranches(BaseUrl);
                var filteredbranches = teamMember.Branches.Where(x => x.BranchId == teamMember.BranchId);
                teamMember.BranchName = filteredbranches.Select(p => p.BranchName).ToString() ;
            };
            return View(teamMember);

        }

        [HttpGet]
        public async Task<IActionResult> AddTeamMember()
        {

            IEnumerable<BranchDTO> branches = await StaticDataHandler.GetBranches(BaseUrl);
            IEnumerable<PositionDTO> positions = await StaticDataHandler.GetPositions(BaseUrl);
            IEnumerable<MinistryArmDTO> ministryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl);
            var teamMemberDTO = new TeamMembersDTO
            {
              Branches   = branches,
              Positions = positions,
              MinistryArms = ministryArms

            };
            return View(teamMemberDTO);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> AddTeamMember(TeamMembersDTO teamMember, IFormFile artwork)
        {

            try
            {
                var user = await StaticDataHandler.GetSessionDetails();
                teamMember.CreatedBy = user.Username;
                teamMember.DateCreated = DateTime.Now.AddHours(2);
             
                if (ModelState.IsValid)
                {
                    if (artwork != null)
                    {
                        var fileUploadResult = await StaticDataHandler.fileUpload(artwork, FolderName);
                        if (fileUploadResult.IsErrorOccured)
                        {
                            return View(teamMember.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "Something went wrong"
                            });
                        }
                        else
                        {
                            teamMember.Artwork = (byte[])fileUploadResult.Result; //return the byte data
                            teamMember.Filename = artwork.FileName;
                        }
                    }

                    var requestUrl = $"{BaseUrl}{apiUriTeamMembers}/AddTeamMember";
                    using (var client = new HttpClient())
                    {
                        string token = "";
                        client.BaseAddress = new Uri(requestUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                        teamMember.Filename = artwork.FileName;
                        var result = await client.PostAsJsonAsync(client.BaseAddress, teamMember);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("TeamMembersForAdmin");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            teamMember.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {
                            teamMember.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                            teamMember.Branches = await StaticDataHandler.GetBranches(BaseUrl);
                            teamMember.Positions = await StaticDataHandler.GetPositions(BaseUrl);
                            teamMember.MinistryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl);
                        }

                    };
                }
                else
                {
                    teamMember.Branches = await StaticDataHandler.GetBranches(BaseUrl);
                    teamMember.Positions = await StaticDataHandler.GetPositions(BaseUrl);
                    teamMember.MinistryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl);
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
            IEnumerable<BranchDTO> branches = await StaticDataHandler.GetBranches(BaseUrl);
            IEnumerable<PositionDTO> positions = await StaticDataHandler.GetPositions(BaseUrl);
            teamMember.Positions = positions;
            return View(teamMember);
        }

        [HttpGet]
        public async Task<IActionResult> EditTeamMember(int teamMemberId)
        {
            var requestUrl = $"{BaseUrl}{apiUriTeamMembers}/GetTeamMemberDetails?teamMemberId={teamMemberId}";
            TeamMembersDTO teamMember = new TeamMembersDTO();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                HttpResponseMessage response = await client.GetAsync(requestUrl);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    teamMember = await response.Content.ReadAsAsync<TeamMembersDTO>();
                }
                else
                {

                    teamMember.OutputHandler = await response.Content.ReadAsAsync<OutputHandler>();
                }
                teamMember.Branches = await StaticDataHandler.GetBranches(BaseUrl);
                teamMember.Positions = await StaticDataHandler.GetPositions(BaseUrl);
                teamMember.MinistryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl);
            };
            
            teamMember.Branches = await StaticDataHandler.GetBranches(BaseUrl);
            teamMember.Positions = await StaticDataHandler.GetPositions(BaseUrl);
            teamMember.MinistryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl);
            teamMember.OldImageUrl = teamMember.ImageUrl;
            teamMember.CurrentImageName = Path.GetFileName(teamMember.ImageUrl);
            return View(teamMember);
        }

        [HttpPost]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> EditTeamMember(TeamMembersDTO teamMember, IFormFile artwork)
        {

            try
            {
                var user = await StaticDataHandler.GetSessionDetails();
                teamMember.ModifiedBy = user.Username;
                teamMember.ModifiedDate = DateTime.Now.AddHours(2);
                
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
                            return View(teamMember.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "Something went wrong"
                            });
                        }
                        else
                        {
                            teamMember.Artwork = (byte[])fileUploadResult.Result; //return the byte data
                            teamMember.Filename = artwork.FileName;
                        }

                    }
                    var requestUrl = $"{BaseUrl}{apiUriTeamMembers}/EditTeamMember";
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(requestUrl);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                        var result = await client.PostAsJsonAsync(client.BaseAddress, teamMember);

                        if (result.StatusCode == HttpStatusCode.OK)
                        {
                            return RedirectToAction("TeamMembersForAdmin");
                        }
                        else if (result.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            teamMember.OutputHandler = new OutputHandler
                            {
                                IsErrorOccured = true,
                                Message = "You're not Authorized to perfom this task"
                            };
                        }
                        else
                        {
                            teamMember.OutputHandler = await result.Content.ReadAsAsync<OutputHandler>();
                        };

                    };
                }
                else
                {


                    teamMember.Branches = await StaticDataHandler.GetBranches(BaseUrl);
                    teamMember.Positions = await StaticDataHandler.GetPositions(BaseUrl);
                    teamMember.MinistryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl);

                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message != null)
                {
                    teamMember.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.InnerException.Message };
                }
                else
                {
                    teamMember.OutputHandler = new OutputHandler { IsErrorOccured = true, Message = ex.Message };
                }
            }
            IEnumerable<BranchDTO> branches = await StaticDataHandler.GetBranches(BaseUrl);
            teamMember.MinistryArms = await StaticDataHandler.GetMinistryArmsAsync(BaseUrl);
            teamMember.Positions = await StaticDataHandler.GetPositions(BaseUrl);
            teamMember.CurrentImageName = Path.GetFileName(teamMember.ImageUrl);

            teamMember.Branches = branches;
            return View(teamMember);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var user = await StaticDataHandler.GetSessionDetails();

            var requestUrl = $"{BaseUrl}{apiUriTeamMembers}/DeleteTeamMember?TeammemberId={id}";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(requestUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

                var result = await client.DeleteAsync(client.BaseAddress);

                if (result.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction("TeamMembersForAdmin");
                }
                else if (result.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("TeamMembersForAdmin", new
                    {
                        isDeleteFailed = true,
                        error = "You're not Authorized to perfom this Action"
                    });

                }
                else
                {
                    return RedirectToAction("TeamMembersForAdmin", new { isDeleteFailed = true });
                };
            };

        }

    }
}
