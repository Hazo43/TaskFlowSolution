using AutoMapper;
using Domain.Entites;
using Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Services.Abstraction.Interfaces;
using Services.Specifications;
using Shared.DTOs.CommentModule;

namespace Services.ImplementaionService
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IProjectMemberRepository _projectMemberRepository;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager,
                           ICurrentUserService currentUserService, IProjectMemberRepository projectMemberRepository)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _projectMemberRepository = projectMemberRepository;
        }

        // CREATE COMMENT WITh TASK ID
        public async Task<CommentResultDTO> CreateComment(int taskId, CreateCommentDTO createCommentDTO)
        {
            // Get User From JWT
            var currentUserId = _currentUserService.UserId;
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new Exception(" Current User Is Null Or Not Found");

            var task = await _unitOfWork.GetRepository<Tasks, int>().GetByIdAsync(taskId);
            if (task is null)
                throw new Exception($" Task With Id:{taskId} Is null ");

            var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(task.ProjectId);
            if (project is null)
                throw new Exception($" Project With Id:{task.ProjectId} Is null");

            // Check if current user is Admin
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Admin can create a comment on any task
            // Non-admin can create a comment if he is a member or owner of the task's project
            if (!isAdmin)
            {
                // Non-admin must be Project Owner or Project Member
                var isMember = await _projectMemberRepository.GetProjectMemberAsync(task.ProjectId, currentUserId);

                if (isMember is null && project.OwnerId != currentUserId)
                    throw new UnauthorizedAccessException($"You are Not a Member Of This Project");
            }

            // بقا create نعمل
            var comment = new Comment()
            {
                Content = createCommentDTO.Content,
                AuthorId = currentUserId,
                TaskId = taskId,
                CreatedAt = DateTime.UtcNow
            };

            // add and save
            await _unitOfWork.GetRepository<Comment, int>().AddAsync(comment);
            await _unitOfWork.SaveChangesAsync();

            // Get Comment again with Author and Task
            //  Task و Author ب ال Comment دا عشان يرجعلي ال
            var spec = new CommentWithTaskAndAuthorSpecifications(comment.Id);

            var createComment = await _unitOfWork.GetRepository<Comment, int>().GetByIdAsync(spec);
            if (createComment is null)
                throw new Exception($" Create Comment With Id:{comment.Id} Not Found");

            return _mapper.Map<CommentResultDTO>(createComment);
        }

        public async Task DeleteComment(int commentId)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new Exception($" Current User With Id:{currentUserId} Not Found");

            var comment = await _unitOfWork.GetRepository<Comment, int>().GetByIdAsync(commentId);
            if (comment is null)
                throw new Exception($" Comment With Id:{commentId} Not Found");

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            var isAuthor = comment.AuthorId == currentUserId;
         
            // // Only Admin or Comment Author can Delete
            if (!isAdmin && !isAuthor)
                throw new UnauthorizedAccessException($" Only Admin or Comment Author can delete");

            _unitOfWork.GetRepository<Comment, int>().Remove(comment);

            await _unitOfWork.SaveChangesAsync();
        }

        // GET COMMENT BY TASK ID
        public async Task<IEnumerable<CommentResultDTO>> GetCommentsByTaskId(int taskId)
        {
            // اللي باعت الطلب بتجيبو من التوكن User بتاع ال Id بتجيب ال
            var currentUserId = _currentUserService.UserId;

            // نفسو وتعمل فحص عليه User بتجيب ال
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new Exception($" Current User Is Null");

            var task = await _unitOfWork.GetRepository<Tasks, int>().GetByIdAsync(taskId);
            if (task is null)
                throw new Exception($" Task With Id:{taskId} Is Null");

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");

            // Non-admin must be Project Owner or Project Member
            if (!isAdmin)
            {
                var project = await _unitOfWork.GetRepository<Project, int>().GetByIdAsync(task.ProjectId);

                if (project is null)
                    throw new Exception($"Project With Id:{task.ProjectId} Is Null");


                var isMember = await _projectMemberRepository.GetProjectMemberAsync
                                                             (task.ProjectId,currentUserId);


                if (isMember is null && project.OwnerId != currentUserId)
                    throw new UnauthorizedAccessException("You are Not a Member Of This Project");
                
            }


            var spec = isAdmin
                 // comments هيخش هنا و يجيب كل ال Admin لو هو
                 // userAdmin , Memeber , Owner علي ال check جوا بيعمل
                 ? new CommentsByTaskSpecifications(taskId)
                 // member ولا owner هيخش يشوف بقا هل هو Admin لو مش
                 : new CommentsByTaskSpecifications(taskId, currentUserId);

            var allComments = await _unitOfWork.GetRepository<Comment, int>().GetAllAsync(spec);

            return _mapper.Map<IEnumerable<CommentResultDTO>>(allComments);
        }

        //  UPDATE COMMENT WITH COMMENT ID                                            
        public async Task<CommentResultDTO> UpdateComment(int commentId, UpdateCommentDTO updateCommentDTO)
        {
            var currentUserId = _currentUserService.UserId;
        
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            if (currentUser is null)
                throw new Exception($" Cureent User With:{currentUserId} Not Found");


            var comment = await _unitOfWork.GetRepository<Comment, int>().GetByIdAsync(commentId);
            if (comment is null)
                throw new Exception($" Comment With Id:{commentId} Not Found");
         

            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "Admin");
         
            var isAuthor = comment.AuthorId == currentUserId;

            // // Only Admin or Comment Author can Update
            if (!isAdmin && !isAuthor)
                throw new UnauthorizedAccessException($" Only Admin or Comment Author can update");

            // update نعمل بقا ال
            comment.Content = updateCommentDTO.Content;
            comment.UpdatedAt = DateTime.UtcNow;

            // save
            await _unitOfWork.SaveChangesAsync();

            // Get Comment again with Author and Task
            // Include في ال Author and Task تاني عشان نرجع معاه ال Comment بنجيب ال
            var spec = new CommentWithTaskAndAuthorSpecifications(comment.Id);

            var updateComment = await _unitOfWork.GetRepository<Comment, int>().GetByIdAsync(spec);
            if (updateComment is null)
                throw new Exception($" Update Comment With Id:{comment.Id} Not Found");

            return _mapper.Map<CommentResultDTO>(updateComment);
        }


    }
}
