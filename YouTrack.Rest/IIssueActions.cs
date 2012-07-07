using System.Collections.Generic;
using System.Threading.Tasks;

namespace YouTrack.Rest
{
    public interface IIssueActions
    {
        string Id { get; }
        Task AttachFile(string filePath);
        Task<IEnumerable<IAttachment>> GetAttachments();
        Task AddComment(string comment);
        Task RemoveComment(string commentId);
        IEnumerable<IComment> Comments { get; }
        Task SetSubsystem(string subsystem);
        Task SetType(string type);
        Task AttachFile(string fileName, byte[] bytes);
        Task ApplyCommand(string command);
        Task ApplyCommands(params string[] commands);
    }
}