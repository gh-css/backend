using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Data;

public class Report
{
    [StringLength(256)]
    public string? ReportReason { get; set; }
    
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public uint GitHubUserId { get; set; }

    [ForeignKey(nameof(GitHubUserId))]
    public GitHubUser GitHubUser { get; set; }
}

public class ReportRequest
{
    public string ReportReason { get; set; }
}