using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Data;

public class GitHubUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public uint Id { get; set; }

    public bool IsBanned { get; set; }
    public uint ReportScore { get; set; }
}