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

            // Add Owner as ProjectMember    ProjectMember في كلاس ال member هيتضاف ك Owner يعني ال
            var projectMember = new ProjectMember()
            {
                UserId = ownerId,
                ProjectId = project.Id,
                JoinedAt = DateTime.UtcNow
            };

            await _projectMemberRepository.AddProjectMemberAsync(projectMember);

            // Save ProjectMember
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


            //  ولا لاisAdmin او isOwner دي الميثود اللي بنعرف منها هو
            await EnsureUsAdminOrIsOwner(projectWithId);


            // if null Remove
            _unitOfWork.GetRepository<Project, int>().Remove(projectWithId);
            // save Changes
            await _unitOfWork.SaveChangesAsync();
        }

        // Get All
        public async Task<IEnumerable<ProjectResultDto>> GetAllProjectAsync()
        {

            // token بنجيبو من ال request اللي باعت ال  currentUserId بنجيب ال 
            var currentUserId = _currentUserService.UserId;

            // token اللي جبناه من ال currentUserId من ال currentUser بنجيب ال
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new Exception(" Current User Is Null");

            // check is Admin or no
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            IEnumerable<Project> projects;
            // if is admin return GetAllProduct
            if(isAdmin )
            {
                projects = await _unitOfWork.GetRepository<Project, int>().GetAllAsync();
            }
            // if not admin return Get Project By User Id   اللي هة عاملو Project رجعلو ال admin يعني لو مش 
            else
            {
                projects = await _projectMemberRepository.GetProjectsByUserIdAsync(currentUserId);
            }

            return _mapper.Map<IEnumerable<ProjectResultDto>>(projects);
        }

        // Get By Id
        public async Task<ProjectResultDto> GetByIdAsync(int id)
        {
            var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(id);
            if (project is null)
                throw new Exception("Project is null");


            // Get Current User Id from Token
            var currentUserId = _currentUserService.UserId;

            // Get Current User
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());

            if (currentUser is null)
                throw new Exception("Current User Is Null");

            // Check if Current User is Admin
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
           
            // Check if Current User is Project Owner
            var isOwner =  currentUserId == project.OwnerId;
           
            // Check if Current User is Project Member
            var isMember = await _projectMemberRepository.GetProjectMemberAsync(id, currentUserId);

            // UnauthorizedAccessException لو هو مش اي حاجه من الثلاثه دول هيرجعلو ال
            if (!isAdmin && !isOwner && isMember is null)
                throw new UnauthorizedAccessException("Only Admin, Project Owner Or Project Member Can View This Project");

            return _mapper.Map<ProjectResultDto>(project);
        }

        // Add Member
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

            //  ولا لاisAdmin او isOwner دي الميثود اللي بنعرف منها هو
            await EnsureUsAdminOrIsOwner(projectWithId);

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

        // Remove Member
        public async Task RemoveMemberAsync(int projectId, int userId)
        {
            var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(projectId);
            if (project is null)
                throw new Exception($" project with id:{projectId} not found");

            var userIsExist = await _userManager.FindByIdAsync(userId.ToString());
            if (userIsExist is null)
                throw new Exception($" user with id:{userId} not found");

            //  ولا لاisAdmin او isOwner دي الميثود اللي بنعرف منها هو
            await EnsureUsAdminOrIsOwner(project);

            // Members مينفعش امسح من ال Owner هو userId دي بتوضح ان لو ال
            if (userId == project.OwnerId)
                throw new Exception("Cannot remove the Project Owner from members");

            var memberIsExist = await _projectMemberRepository.GetProjectMemberAsync(projectId, userId);
            if (memberIsExist is null)
                throw new Exception(" User is not a member of this project");

            _projectMemberRepository.RemoveProjectMember(memberIsExist);

            await _unitOfWork.SaveChangesAsync();

        }

        // Update Project
        public async Task<ProjectResultDto> UpdateProjectAsync(int id, UpdateProjectDto updateDto)
        {
            var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(id);

            if (project is null)
                throw new Exception($" Project with id:{id} not Found");


            //  ولا لاisAdmin او isOwner دي الميثود اللي بنعرف منها هو
            await EnsureUsAdminOrIsOwner(project);

            // update data
            project.Name = updateDto.Name;
            project.Description = updateDto.Description;
            project.UpdatedAt = DateTime.UtcNow;



            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ProjectResultDto>(project);


        }
     
        
        
        
        //  ولا لا Admin او  Owner او الشخص اللي عاوز يعدل او يضيف او يمسح هو request الميثود بتشوف الشخص اللي بعت ال
        private async Task EnsureUsAdminOrIsOwner(Project project)
        {
            //  token بتجيبو من ال request  بتجيب  الشخص اللي باعت ال 
            var currentUserId = _currentUserService.UserId;

            //  ولا لا Null بتجيب الشخص و تشوف هو 
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new Exception($"Current User Is Null");
            
            // Check if current user is Admin
            // ولا لا Admin بتشوف اللي عامل الطلب ال
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");


            //7- Check if current user is Project Owner
            // ولا لا Owner بتشوف اللي عامل الطلب ال
            var isOwner = currentUserId == project.OwnerId;

            // UnauthorizedAccessException هيعدي غير كدا هيرجعلو isAdmin او isOwner لو
            if (!isAdmin && !isOwner)
                throw new UnauthorizedAccessException("Only Admin Or Project Owner Can Perform This Action");
        }
    }
}
