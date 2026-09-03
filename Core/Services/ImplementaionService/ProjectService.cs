using AutoMapper;
using Domain.Entites;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Services.Abstraction.Interfaces;
using Shared.DTOs.Project;

namespace Services.ImplementaionService
{
    public class ProjectService : IProjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;
        private readonly IProjectMemberRepository _projectMemberRepository;

        public ProjectService
            (
            IUnitOfWork unitOfWork, IMapper mapper,
            ICurrentUserService currentUserService,
            UserManager<User> userManager , 
            IProjectMemberRepository projectMemberRepository
            )
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _projectMemberRepository = projectMemberRepository;
        }

        // اللي باعت الطلب token او من ال request من ال userid دا كلاس انا عاملو بيجيب ال
        // مين المستخدم الحالي services بيعرف ال class ال
        // ICurrentUserService currentUserService 

        // Important
        // Create Project 
        public async Task<ProjectResultDto> CreateProjectAsync(CreateProjectDto createDto)
        {
            // اللي باعت الطلب token او من ال request من ال userid دا كلاس انا عاملو بيجيب ال
            // مين المستخدم الحالي services بيعرف ال class ال
            // _currentUserService

            var ownerId = _currentUserService.UserId;

            // Project Entity الي CreateProjectDto نحول الـ  
            var project = _mapper.Map<Project>(createDto);

            // عشان مش اي حد يبعتو token بنجيب صاحب البروجيكت اللي جاي من ال
            project.OwnerId = ownerId;

            // الجديد Project بنضيف ال
            await _unitOfWork.GetRepository<Project, int>().AddAsync(project);

            // بنحفظ التغيرات
            await _unitOfWork.SaveChangesAsync();

            //  Project الخاص بId بتكون ولدت Database بعد الحفظ، الـ  
            // وبعد كده نحول الـ Project Entity إلى ProjectResultDto
            return _mapper.Map<ProjectResultDto>(project);
        }

        // Delete
        public async Task DeleteAsync(int id)
        {
            // get id
            var projectWithId = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(id);
            // Check is null or no
            if (projectWithId is null)
                throw new Exception($"Project with id:{id} not found");
            // if null Remove
            _unitOfWork.GetRepository<Project, int>().Remove(projectWithId);
            // save Changes
            await _unitOfWork.SaveChangesAsync();
        }

        // Get All
        public async Task<IEnumerable<ProjectResultDto>> GetAllProjectAsync()
        {
            var allProjects = await _unitOfWork.GetRepository<Project, int>().GetAllAsync();

            return _mapper.Map<IEnumerable<ProjectResultDto>>(allProjects);
        }

        // Get By Id
        public async Task<ProjectResultDto> GetByIdAsync(int id)
        {
            var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(id);
            if (project is null)
                throw new Exception("Project is null");

            return _mapper.Map<ProjectResultDto>(project);
        }

        public async Task AddMemberAsync(int projectId, int userId)
        {
            // 1-  ؟projec هل ال
            var projectWithId = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(projectId);
            if (projectWithId is null)
                throw new Exception($"Project With Id:{projectId} not found");

            //2 -  ؟user هل ال
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                throw new Exception($"User With Id:{userId} not found");

            //3- Exception واحد رجعلو ProjectMember موجودين مع بعض في  userId او projectId لو ال
            var memberIsExist = await _projectMemberRepository.GetProjectMemberAsync(projectId, userId);
            if (memberIsExist is not null)
                throw new Exception($"User with id:{userId} is already a member of Project with id:{projectId}");

            //4-  JWT من ال CurrentUser جبت ال
            var currentUserId = _currentUserService.UserId;

            //5- اللي انا جبتو موجود currentUser بتاكد ان ال
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new Exception(" Current User Is Null");

            //6- Check if current user is Admin
            // ولا لا Admin بتشوف اللي عامل الطلب ال
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            //7- Check if current user is Project Owner
            // ولا لا Owner بتشوف اللي عامل الطلب ال
            var isOwner = currentUserId == projectWithId.OwnerId;

            //8- UnauthorizedAccessException رجعلو isOwner ولا isAdmin لو مش  
            if (!isAdmin && !isOwner)
                throw new UnauthorizedAccessException("Only Admin Or Project Owner Add Members");

            //9-  projectmember انشاء
            var projectmember = new ProjectMember()
            {
                UserId = userId,
                ProjectId = projectId,
                JoinedAt = DateTime.UtcNow
            };

            //10- projectmember ضيف ال
            await _projectMemberRepository.AddProjectMemberAsync(projectmember);
            //11- احفظ التغيرات 
            await _unitOfWork.SaveChangesAsync();


        }

        public async Task RemoveMemberAsync(int projectId, int userId)
        {
            var projectisExist = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(projectId);
            if (projectisExist is null)
                throw new Exception($" project with id:{projectId} not found");

            var userIsExist = await _userManager.FindByIdAsync(userId.ToString());
            if (userIsExist is null)
                throw new Exception($" user with id:{userId} not found");

            // request اللي بعت ال user ال
            var currentUserId = _currentUserService.UserId;

            //
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new Exception($"Current User Is Null");

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            var isOwner = currentUserId == projectisExist.OwnerId;

            if(!isAdmin && !isOwner)
               throw new UnauthorizedAccessException("Only Admin Or Project Owner Remove Members");

            var memberIsExist = await _projectMemberRepository.GetProjectMemberAsync(projectId, userId);
            if (memberIsExist is null)
                throw new Exception(" User Is null");

            _projectMemberRepository.RemoveProjectMember(memberIsExist);

            await _unitOfWork.SaveChangesAsync();

        }

        // Update Project
        public async Task<ProjectResultDto> UpdateProjectAsync(int id, UpdateProjectDto updateDto)
        {
            var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(id);

            if (project is null)
                throw new Exception($" Project with id:{id} not Found");

            // update data
            project.Name = updateDto.Name;
            project.Description = updateDto.Description;
            

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProjectResultDto>(project);


        }
    }
}
