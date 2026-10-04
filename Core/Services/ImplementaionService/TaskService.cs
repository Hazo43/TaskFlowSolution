using AutoMapper;
using Domain.Entites;
using Domain.Entites.Enums;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Services.Abstraction.Interfaces;
using Services.Specifications;
using Shared.DTOs;
using Shared.DTOs.Enums;
using Shared.DTOs.TaskModule;

namespace Services.ImplementaionService
{
    public class TaskService : ITaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProjectMemberRepository _projectMemberRepository;
        private readonly IMapper _mapper;

        public TaskService(IUnitOfWork unitOfWork, UserManager<User> userManager,
                          ICurrentUserService currentUserService, IProjectMemberRepository projectMemberRepository,
                          IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _projectMemberRepository = projectMemberRepository;
            _mapper = mapper;
        }

        // GET ALL
        public async Task<IEnumerable<TaskResultDto>> GetAllAsync(TaskSpecificationParameter parameter)
        {

            // اللي باعت الطلب بتجيبو من التوكن User بتاع ال Id بتجيب ال
            var currentUserId = _currentUserService.UserId;

            // نفسو وتعمل فحص عليه User بتجيب ال
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new UserNotFoundException(currentUserId);

            // عشان نعمل اتشك isAdmin بنجيب ال
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            //  Tasks رجعلو كل ال isAdmin لو هو 
            // بتاعتها project اللي هو عضو في ال Tasks و هاتلو ال currentUserId و شوف ال TaskWithCategoryAndProjectAndAssignedSpecifications روح علي ال isAdmin لو هو مش
            var spec = isAdmin
                ? new TaskWithCategoryAndProjectAndAssignedSpecifications(parameter)
                : new TaskWithCategoryAndProjectAndAssignedSpecifications(parameter, currentUserId);


            var allTasks = await _unitOfWork.GetRepository<Tasks, int>().GetAllAsync(spec);

            return _mapper.Map<IEnumerable<TaskResultDto>>(allTasks);
        }

        // GET BY ID
        public async Task<TaskResultDto> GetByIdAsync(int id)
        {
            var spec = new TaskWithCategoryAndProjectAndAssignedSpecifications(id);
            var task = await _unitOfWork.GetRepository<Tasks, int>().GetByIdAsync(spec);
            if (task is null)
                 throw new TaskNotFoundException(id);

            var currentUserId = _currentUserService.UserId;
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new UserNotFoundException(currentUserId);

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            var isOwner = currentUserId == task.Project.OwnerId;
            var isMember = await _projectMemberRepository.GetProjectMemberAsync(task.ProjectId, currentUserId);

            if (!isAdmin && !isOwner && isMember is null)
                throw new ForbiddenException("Only Admin, Project Owner Or Project Member Can View This Task");

            return _mapper.Map<TaskResultDto>(task);
        }

        // CREATE TASK
        public async Task<TaskResultDto> CreateAsync(CreateTaskDto createTaskDto)
        {
            var currentUserId = _currentUserService.UserId;

            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new UserNotFoundException(currentUserId);

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(createTaskDto.ProjectId);
            if (project is null)
                throw new ProjectNotFoundException(createTaskDto.ProjectId);

            // هيخش جوا isAdmin لو هو مش
            if (!isAdmin)
            {
                // ولا لا isMember هيشوف هو 
                var isMember = await _projectMemberRepository.GetProjectMemberAsync
                                                       (createTaskDto.ProjectId, currentUserId);
                // رجعلو Owner ولا isMember لو هو مش
                // Exception رجعلو null لو ب
                if (isMember is null && project.OwnerId != currentUserId)
                    throw new ForbiddenException($"You are Not a Member or Owner Of This Project");
            }

            // AssignedTo
            // Include عشان بنعمل Navigation Properties عملت دي عشان ال
            User? assignedUser = null;

            if (createTaskDto.AssignedToId.HasValue)
            {
                // دا المستخدم اللي انا هخصص ليه التاك
                assignedUser = await _userManager.FindByIdAsync(createTaskDto.AssignedToId.Value.ToString());

                // هل المستخدم اللي انا عاوز اخصص ليه التاسك دا موجود ولا لا
                if (assignedUser is null)
                    throw new UserNotFoundException(createTaskDto.AssignedToId.Value);

                // في المشروع او ليه علاقه بالمشروع  Member هل المستخدم دا 
                var isAssignUserMember = await _projectMemberRepository.GetProjectMemberAsync
                                                   (createTaskDto.ProjectId, createTaskDto.AssignedToId.Value);

                //Exception رجع Owner ولا Member اخر حاجه بقولو لو المستخدم اللي انا عاوز اخصص ليه التاسك دا مش 
                if (isAssignUserMember is null && project.OwnerId != createTaskDto.AssignedToId.Value)
                    throw new ForbiddenException("Assigned User Is Not A Member Of This Project");

            }

            // Include عشان بنعمل Navigation Properties عملت دي عشان ال
            Category? category = null;

            //  "categoryId" لو المستخدم بعت
            if (createTaskDto.CategoryId.HasValue)
            {
                // اللي هو بعتو "categoryId" ب ال category ندخل نجيب ال
                category = await _unitOfWork.GetRepository<Category, int>()
                                            .GetByIdAsync(createTaskDto.CategoryId.Value);

                // Exception رجعلو null هنشوف لو ب 
                if (category is null)
                    throw new Exception($"Category with Id:{createTaskDto.CategoryId} Not Found");
            }

            // Invalid Priority Value اللي مبعوته بيشوف هيه موجود ولا لا لو موجود هيعدي عادي لو مش موجوده Enum دي بتعمل فحص ل قيمه ال
            if (!Enum.IsDefined(typeof(TaskPriorityDTO), createTaskDto.Priority))
                throw new Exception($"Invalid Priority Value {createTaskDto.Priority}");

            var task = new Tasks()
            {
                // Entity و نحطها ف ال Dto بنبدا بقا ناخد الداتا اللي هة باعتها في ال

                CreatedAt = DateTime.UtcNow,
                Status = Domain.Entites.Enums.TaskStatus.Pending,
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                DueDate = createTaskDto.DueDate,
                ProjectId = createTaskDto.ProjectId,
                CategoryId = createTaskDto.CategoryId,
                AssignedToId = createTaskDto.AssignedToId,

                Priority = (Domain.Entites.Enums.TaskPriority)createTaskDto.Priority,

                // Navigation Properties
                Project = project,
                AssignedTo = assignedUser,
                Category = category
            };

            await _unitOfWork.GetRepository<Tasks, int>().AddAsync(task);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TaskResultDto>(task);

        }

        // UBDATE TASK
        public async Task<TaskResultDto> UpdateAsync(int taskid, UpdateTaskDto updateTaskDto)
        {
            //1] request اللي باعت ال User  بتاع ال Id دا 
            var currentUserId = _currentUserService.UserId;

            //2] currentUserId بتاعو دا Id نفسو من ال User بنجيب ال
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new UserNotFoundException(currentUserId);

            //3] دا ولا لا taskid عشان نتاكد هيه موجود ب ال task بنجيب ال
            var task = await _unitOfWork.GetRepository<Tasks, int>().GetByIdAsync(taskid);
            if (task is null)
                throw new TaskNotFoundException(taskid);

            //4] عليها Check و بنعمل project بنجيب ال
            var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(task.ProjectId);
            if (project is null)
                throw new ProjectNotFoundException(task.ProjectId);

            // Authorization
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            var isOwner = currentUserId == project.OwnerId;

            // اللي متخصصلو التاسك ولا لا AssignedUser دي بنتاكد ان دا نفس ال
            var isAssignedUser = currentUserId == task.AssignedToId;

            if (!isAdmin && !isOwner && !isAssignedUser)
                throw new ForbiddenException("Only Admin, Project Owner Or Assigned User Can Update Task ");

            //5] AssignedTo Validation
            if (updateTaskDto.AssignedToId.HasValue)
            {
                // وبشوف هو موجود ولا لا Id دا المستخدم اللي انا هخصص ليه التاسك بجبو ب ال
                var assignedUser = await _userManager.FindByIdAsync(updateTaskDto.AssignedToId.Value.ToString());
                if (assignedUser is null)
                    throw new UserNotFoundException(updateTaskDto.AssignedToId.Value);

                // ولا لا project بشوف الشخص اللي انا هخصص ليه التاسك هو موجود اصلا في ال
                var isAssignedUserMember = await _projectMemberRepository.GetProjectMemberAsync
                                                     (project.Id, updateTaskDto.AssignedToId.Value);

                // Exception رجعلو  owner or member و لا هو project لو مش موجود ف ال
                if (isAssignedUserMember is null && project.OwnerId != updateTaskDto.AssignedToId.Value)
                    throw new ForbiddenException(" Assigned User Is Not A Member Of This Project ");
            }

            // 5] Check CategoryId
            if (updateTaskDto.CategoryId.HasValue)
            {
                // اللي هو باعتو و بشوف هو موجود ولا لا الاول CategoryId باخد ال
                var category = await _unitOfWork.GetRepository<Category, int>().GetByIdAsync(updateTaskDto.CategoryId.Value);
                if (category is null)
                    throw new CategoryNotFoundException(updateTaskDto.CategoryId.Value);
            }


            // Invalid Priority Value اللي مبعوته بيشوف هيه موجود ولا لا لو موجود هيعدي عادي لو مش موجوده Enum دي بتعمل فحص ل قيمه ال
            if (!Enum.IsDefined(typeof(TaskPriorityDTO), updateTaskDto.Priority))
                throw new Exception($"Invalid Priority Value:{updateTaskDto.Priority}");

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.DueDate = updateTaskDto.DueDate;
            task.AssignedToId = updateTaskDto.AssignedToId;
            task.CategoryId = updateTaskDto.CategoryId;
            task.UpdatedAt = DateTime.UtcNow;

            task.Priority = (Domain.Entites.Enums.TaskPriority)updateTaskDto.Priority;

            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<TaskResultDto>(task);
        }

        // UPDATE TASK STATUS
        public async Task<TaskResultDto> UpdateTaskStatus(UpdateTaskStatusDto updateTaskStatusDto, int id)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new UserNotFoundException(currentUserId);

            var task = await _unitOfWork.GetRepository<Tasks, int>().GetByIdAsync(id);
            if (task is null)
                throw new TaskNotFoundException(id);

            var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(task.ProjectId);
            if (project is null)
                throw new ProjectNotFoundException(task.ProjectId);

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isOwner = currentUserId == project.OwnerId;
            var isAssignedUser = currentUserId == task.AssignedToId;

            if (!isAdmin && !isAssignedUser && !isOwner)
                throw new ForbiddenException("Only Admin, Project Owner Or Assigned User Can Update Task Status");

            // Invalid Status Value اللي مبعوته بيشوف هيه موجود ولا لا لو موجود هيعدي عادي لو مش موجوده Enum دي بتعمل فحص ل قيمه ال
            if (!Enum.IsDefined(typeof(TaskStatusDTO), updateTaskStatusDto.Status))
                throw new Exception($"Invalid Status Value {updateTaskStatusDto.Status}");
           
            task.Status = (Domain.Entites.Enums.TaskStatus)updateTaskStatusDto.Status;
            task.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<TaskResultDto>(task);
        }

        // DELETE TASK
        public async Task Delete(int id)
        {

            var currentUserId = _currentUserService.UserId;
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new UserNotFoundException(currentUserId);

            var task = await _unitOfWork.GetRepository<Tasks, int>().GetByIdAsync(id);
            if (task is null)
                throw new TaskNotFoundException(id);

            var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(task.ProjectId);
            if (project is null)
                throw new ProjectNotFoundException(task.ProjectId);

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
            var isOwner = currentUserId == project.OwnerId;

            if (!isAdmin && !isOwner)
                throw new ForbiddenException(" Only Admin Or Owner Can Delete Tasks");

            _unitOfWork.GetRepository<Tasks, int>().Remove(task);

            await _unitOfWork.SaveChangesAsync();



        }

    }
}
