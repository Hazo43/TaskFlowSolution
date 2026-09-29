using Domain.Entites;

namespace Services.Specifications
{
    public class CommentsByTaskSpecifications : BaseSpecification<Comment , int>
    {
        // Get Comments By TaskId
        // taskId ب ال Task بتاع ال Comments بتجيب كل ال
        public CommentsByTaskSpecifications(int taskId , int? currentUserId = null)
               : base( c => c.TaskId == taskId &&  // هيخش هنا و لازم الشرطين يتحققو admin لو هو مش 
               (
                currentUserId == null ||                   // admin دا user دا لو اتحقق معناها ان اللي ال
                c.Task.Project.OwnerId == currentUserId || // اللي هو فيه ولا لا task بتاع ال Owner هل هو
                c.Task.Project.ProjectMembers.Any( pm => pm.UserId == currentUserId)   // عشان هو عضو task بتاع ال comments هنرجعلو ال Project لو هو عضو ف ال
               ))
        {
            AddInclude(c => c.Author);
            AddInclude(c => c.Task);
        }
    }
}
