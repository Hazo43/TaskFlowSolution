using Domain.Entites;
using Shared.DTOs;

namespace Services.Specifications
{
    // Get All 
    public class TaskWithCategoryAndProjectAndAssignedSpecifications:BaseSpecification<Tasks , int>
    {
        public TaskWithCategoryAndProjectAndAssignedSpecifications(TaskSpecificationParameter parameter,
                                       int? currentUserId = null) // request بتاع الشخص اللي عامل Id دا ال

            : base(t =>
                    (!parameter.AssignedToId.HasValue ||
                        t.AssignedToId == parameter.AssignedToId) &&

                    (!parameter.ProjectId.HasValue ||
                        t.ProjectId == parameter.ProjectId) &&

                    (!parameter.Status.HasValue ||
                        t.Status == (Domain.Entites.Enums.TaskStatus)parameter.Status) &&

                    (!parameter.Priority.HasValue ||
                        t.Priority == (Domain.Entites.Enums.TaskPriority)parameter.Priority) &&

                    (string.IsNullOrEmpty(parameter.Search) ||
                        t.Title.ToLower().Contains(parameter.Search.ToLower())) &&

            // اللي احنا بعتناه currentUserId  يساوي ال UserId بتشوف هل ف المشروع اللي فيه التاسك دا في
            // علي الاقل Member فيها او Owner اللي هو tasks بتخش تجبلو ال Admin مش  request دي لو الل باعت ال          
                       
                            (!currentUserId.HasValue ||
                            t.Project.OwnerId == currentUserId ||
                            t.Project.ProjectMembers.Any(pm =>
                                pm.UserId == currentUserId)))
        {
            // Include
            AddInclude(t => t.AssignedTo!);
            AddInclude(t => t.Project);
            AddInclude(t => t.Category!);

            //  Pagination 
            ApplyPagination(parameter.PageIndex, parameter.PageSize);
        }

        // Get By Id
        public TaskWithCategoryAndProjectAndAssignedSpecifications( int id) :base( t => t.Id == id)
        {
            // Include
            AddInclude(t => t.AssignedTo!);
            AddInclude(t => t.Project);
            AddInclude(t => t.Category!);
        }
    }
}
