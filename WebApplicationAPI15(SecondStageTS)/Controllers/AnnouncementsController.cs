using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebApplicationAPI15_SecondStageTS_.Context;
using WebApplicationAPI15_SecondStageTS_.dto;
using WebApplicationAPI15_SecondStageTS_.Models;
using WebApplicationAPI15_SecondStageTS_.Options;
using WebApplicationAPI15_SecondStageTS_.Services.RecaptchaService;
using WebApplicationAPI15_SecondStageTS_.utils.Paging;
using WebApplicationAPI15_SecondStageTS_.utils.Sort;

namespace WebApplicationAPI15_SecondStageTS_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementsController : ControllerBase
    {
        private readonly ApplicationContext _context;
        private readonly IMapper _mapper;     
        private readonly IRecaptchaService _recaptcha;      
        private readonly IOptions<AnnCountOptions> _annCountOptions;
       
        public AnnouncementsController(ApplicationContext context, IMapper mapper, IRecaptchaService recaptcha, IOptions<AnnCountOptions> annCountOptions)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));           
            _recaptcha = recaptcha ?? throw new ArgumentNullException(nameof(recaptcha));          
            _annCountOptions = annCountOptions ?? throw new ArgumentNullException(nameof(annCountOptions));
        }

        private int SetMaxAnnouncementCount()
        {            
            int MaxAnnouncementCount = _annCountOptions.Value.MaxAnnouncementCount;
            if (MaxAnnouncementCount <1)
              {
                 throw new Exception("Недопустимое значение поля MaxAnnouncementCount в файле настроек! ");     
              }
            return MaxAnnouncementCount;
        }
       
        //GET api/announcements/1/5   
        [HttpGet("{page}/{pageSize}")]
        public async Task<ActionResult<GetResult<AnnouncementDTO>>> GetAnnouncements(int page, int pageSize, [FromQuery]string? searchString, 
            [FromQuery]Guid? userId, [FromQuery]SortName? sortName = SortName.CreationDate, [FromQuery] SortMode? sortMode = SortMode.Asc)
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
                }
               
                query = SortAndPaging(announcements, sortName, sortMode, page, pageSize);
                GetResult<AnnouncementDTO> result = new GetResult<AnnouncementDTO>();
                result.CountRowsFound = query.RowCount;
                result.Rows = await _mapper.ProjectTo<AnnouncementDTO>(query.Results).ToListAsync();

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        private PagedResult<Announcement> SortAndPaging(IQueryable<Announcement> announcements, 
            SortName? sortName, SortMode? sortMode, int page, int pageSize) 
        {    
            if (sortMode == SortMode.Desc)
            {
                announcements = sortName switch
                {                    
                    SortName.OrderNumber => announcements.OrderByDescending(o => o.OrderNumber),
                    SortName.Rating => announcements.OrderByDescending(o => o.Rating),
                    _=> announcements.OrderByDescending(o => o.CreationDate)
                };
            }
            else
            {
                announcements = sortName switch
                {                    
                    SortName.OrderNumber => announcements.OrderBy(o => o.OrderNumber),
                    SortName.Rating => announcements.OrderBy(o => o.Rating),
                    _ => announcements.OrderBy(o => o.CreationDate)
                };
            }

            return(announcements.GetPaged(page, pageSize));
        }
               
       
        //GET api/announcements/5     
        [HttpGet("{id}")]
        public async Task<ActionResult<AnnouncementDTO>> GetAnnouncement(Guid id)
        {            
            try
            {
                var query = _context.Announcements.Where(an => an.Id == id);
                var announcementDTO = await _mapper.ProjectTo<AnnouncementDTO>(query).SingleOrDefaultAsync();

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
        [HttpPost()]
        public async Task<ActionResult<Guid>> PostAnnouncement(AnnouncementDTO announcementDTO, [FromQuery]Guid? userId)
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
                announcement = SetAnnouncementFilds(announcement, userId);
                _context.Announcements.Add(announcement);
                await _context.SaveChangesAsync();             

                return Ok(announcement.Id);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }
        private Announcement SetAnnouncementFilds(Announcement announcement, Guid? userId)
        {
            int MaxAnnouncementCount = SetMaxAnnouncementCount();
            if (userId != null)
            {
                User user = _context.Users.Include(an => an.Announcements).SingleOrDefault(u => u.Id == userId);

                if (user != null)
                {
                    announcement.user = user;
                    if (user.Announcements.Count >= MaxAnnouncementCount)
                    {
                        throw new Exception($"Один пользователь может публиковаит не более {MaxAnnouncementCount} объяалений!");                        
                    }
                }
                else
                {
                    throw new Exception($"Пользователя с таким Id {userId} не существует!");                  
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

            return announcement;
        }
        //PUT api/announcements
        [HttpPut()]
        public async Task<ActionResult<Guid>> PutAnnouncement(AnnouncementDTO announcementDTO)
        {
            try
            {
                if (announcementDTO == null)
                {
                    return BadRequest();
                }

               // Announcement announcement = _mapper.Map<AnnouncementDTO, Announcement>(announcementDTO);
                Announcement announcement = await _context.Announcements.Include(u =>u.user).SingleOrDefaultAsync(an => an.Id == announcementDTO.Id);

                if (announcement == null)//приверить в работе
                {
                    return NotFound();
                }

                announcement.OrderNumber = announcementDTO.OrderNumber;
                announcement.Text = announcementDTO.Text;
                announcement.Rating = announcementDTO.Rating;
                announcement.Image = announcementDTO.Image;
                announcement.user.Name = announcementDTO.userDTO.Name;

                _context.Update(announcement);
                await _context.SaveChangesAsync();
                           
                return Ok(announcement.Id);
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

        //DELETE api/announcements/5
        [HttpDelete()]
        public async Task<ActionResult<AnnouncementDTO>> DeleteAnnouncement([FromQuery]Guid id)
        {
            try
            {
                Announcement announcement = await _context.Announcements.SingleOrDefaultAsync(an => an.Id == id);

                if (announcement == null)
                {
                    return NotFound();
                }

                _context.Announcements.Remove(announcement);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new { e.Message, e.StackTrace });
            }
        }

   
    }
}