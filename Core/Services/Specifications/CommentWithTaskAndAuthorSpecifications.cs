using Domain.Entites;

namespace Services.Specifications
{
    public class CommentWithTaskAndAuthorSpecifications : BaseSpecification<Comment , int>
    {
        // Get Comment By Id
        // Comment بتاع ال Id واحد ب ال Comment بتجيب 
        // Update , Create بتستخدم ف
        public CommentWithTaskAndAuthorSpecifications(int id) :base(c => c.Id == id)
        {
            AddInclude(c => c.Author);
            AddInclude(c => c.Task);
        }
    }
}
