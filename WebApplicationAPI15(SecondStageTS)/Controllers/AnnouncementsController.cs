using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationAPI15_SecondStageTS_.dto;
using WebApplicationAPI15_SecondStageTS_.Models;
using WebApplicationAPI15_SecondStageTS_.Services;

namespace WebApplicationAPI15_SecondStageTS_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        ApplicationContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _appEnwironment;
        private readonly IRecaptchaService _recaptcha;
        private IConfiguration Configuration;      
       
        public AnnouncementsController(ApplicationContext context, IMapper mapper, IWebHostEnvironment appEnwironment, IRecaptchaService recaptcha, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _appEnwironment = appEnwironment;
            _recaptcha = recaptcha;
            Configuration = configuration;                 
        }

        private int SetMaxAnnouncementCount()
        {           
            int MaxAnnouncementCount = int.Parse(Configuration["MaxAnnouncementCount"]);
                
            if (MaxAnnouncementCount <1)
              {
                 throw new Exception("Недопустимое значение поля MaxAnnouncementCount в файле настроек! ");     
              }
            return MaxAnnouncementCount;
        }
        //GET api/announcements/1/5   
        [HttpGet("{page}/{pageSize}/{searchString}/{userId?}/{sortOrder?}")]
        public async Task<ActionResult<IEnumerable<AnnouncementDTO>>> GetAnnouncements(int page, int pageSize, string searchString, Guid? userId, SortState? sortOrder = SortState.CreationDateAsc)

        {
            try
            {
                PagedResult<Announcement> query;
                IQueryable<Announcement> announcements = _context.Announcements;
                if (!string.IsNullOrEmpty(searchString))
                {                   
                    announcements = _context.Announcements.Where(s => (s.Text.Contains(searchString)) || (s.user.Name.Contains(searchString))
                    || (EF.Functions.Like(s.OrderNumber.ToString(), searchString)) || (EF.Functions.Like(s.Rating.ToString(), searchString))
                    || (EF.Functions.Like(s.CreationDate.ToString(), searchString)));
                }

                if (userId != null)//если передан userId - проводим фильтрацию по userId
                {
                    announcements = announcements.Where(w => w.UserId == userId);
                    query = SortMethod(announcements, sortOrder, page, pageSize);
                }
                else  // иначе только сортируем
                {
                    query = SortMethod(announcements, sortOrder, page, pageSize);
                }

                var announcementsDTO = await _mapper.ProjectTo<AnnouncementDTO>(query.Results).ToListAsync();
                return announcementsDTO;
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        private PagedResult<Announcement> SortMethod(IQueryable<Announcement> announcements, SortState? sortOrder, int page, int pageSize) 
        {
            announcements = sortOrder switch
            {
                SortState.CreationDateDesc => announcements.OrderByDescending(o => o.CreationDate),
                SortState.OrderNumberAsc => announcements.OrderBy(o => o.OrderNumber),
                SortState.OrderNumberDesc => announcements.OrderByDescending(o => o.OrderNumber),
                SortState.RatingAsc => announcements.OrderBy(o => o.Rating),
                SortState.RatingDesc => announcements.OrderByDescending(o => o.Rating),
                _ => announcements.OrderBy(o => o.CreationDate),
            };

            return(announcements.GetPaged(page, pageSize));
        }

        //GET api/announcements/ImageName  
        [HttpGet("GetImage/{ImageName}")]
        public ActionResult GetImage(string ImageName)
        {            
            try
            {
                string file_path = Path.Combine(_appEnwironment.ContentRootPath, "Images/" + ImageName);
                string file_type = "application/png";

                return PhysicalFile(file_path, file_type, ImageName);
            }
            catch (Exception e)
            {

                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        //GET api/announcements/5     
        [HttpGet("GetAnnouncement/{id}")]
        public async Task<ActionResult<AnnouncementDTO>> GetAnnouncement(Guid id)
        {            
            try
            {
                var query = _context.Announcements.Where(an => an.Id == id);
                var announcementDTO = await _mapper.ProjectTo<AnnouncementDTO>(query).FirstOrDefaultAsync();

                if (announcementDTO == null)
                {
                    return NotFound();
                }

                return Ok(announcementDTO);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        //POST api/announcements
        [HttpPost("PostAnnouncement/{userId?}")]
        public async Task<ActionResult<Announcement>> PostAnnouncement(AnnouncementDTO announcementDTO, Guid? userId)
        {
            try
            {
                var captchaResponse = await _recaptcha.Validate(Request.Form);
                if (!captchaResponse.Success)
                {
                    ModelState.AddModelError("reCaptchaError", "reCAPTCHA error occured. Please try again.");
                    return RedirectToAction(nameof(Index));
                }
                if (announcementDTO == null)
                {
                    return BadRequest();
                }

                Announcement announcement = _mapper.Map<AnnouncementDTO, Announcement>(announcementDTO);
                int MaxAnnouncementCount = SetMaxAnnouncementCount();
                if (userId!=null)
                {

                    User user = _context.Users.Include(an => an.Announcements).FirstOrDefault(u => u.Id == userId);// && u.Announcements.Count< MaxAnnouncementCount);

                    if (user != null)
                    {
                        announcement.user = user;
                        if (user.Announcements.Count >= MaxAnnouncementCount)
                        {
                            ModelState.AddModelError("Announcement", $"Один пользователь может публиковаит не более {MaxAnnouncementCount} объяалений!");
                            return BadRequest(ModelState);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("Announcement", $"Пользователя с таким Id не существует!");
                        return NotFound(ModelState);
                    }
                }

                if (!_context.Announcements.Any())
                {
                    announcement.OrderNumber = 1;
                }
                else
                {
                    announcement.OrderNumber = _context.Announcements.Max(e => e.OrderNumber) + 1;
                }               
                announcement.UserId = announcement.user.Id;
                announcement.CreationDate = DateTime.Now;
                _context.Announcements.Add(announcement);
                await _context.SaveChangesAsync();

                var query = _context.Announcements.Where(an => an.Id == announcement.Id);
                var announcementDTO2 = await _mapper.ProjectTo<AnnouncementDTO>(query).FirstOrDefaultAsync();

                return Ok(announcementDTO2);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        //PUT api/announcements
        [HttpPut("PostAnnouncement")]
        public async Task<ActionResult<Announcement>> PutAnnouncement(AnnouncementDTO announcementDTO)
        {
            try
            {
                if (announcementDTO == null)
                {
                    return BadRequest();
                }

                Announcement announcement = _mapper.Map<AnnouncementDTO, Announcement>(announcementDTO);

                if (!_context.Announcements.Any(an => an.Id == announcement.Id))
                {
                    return NotFound();
                }

                _context.Update(announcement);
                await _context.SaveChangesAsync();

                var query = _context.Announcements.Where(an => an.Id == announcement.Id);
                var announcementDTO2 = await _mapper.ProjectTo<AnnouncementDTO>(query).FirstOrDefaultAsync();

                return Ok(announcementDTO2);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        //DELETE api/announcements/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<AnnouncementDTO>> DeleteAnnouncement(Guid id)
        {
            try
            {
                Announcement announcement = _context.Announcements.Include(an => an.user).FirstOrDefault(an => an.Id == id);

                if (announcement == null)
                {
                    return NotFound();
                }

                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();
                return Ok(announcement);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

   
    }
}