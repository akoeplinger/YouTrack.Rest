using System.Collections.Generic;
using System.Linq;

namespace YouTrack.Rest.Deserialization
{
    internal class CommentsCollection
    {
        public List<Comment> Comments { get; set; }

        public IEnumerable<IComment> GetComments(IConnection connection)
        {
            return Comments.Select(c => c.GetComment(connection));
        }
    }
}
