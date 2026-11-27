using System.ComponentModel.DataAnnotations;

namespace library_app.Models.Requests;

public class UpdateTitleRequest : TitleRequestBase
{
    [Range(1, int.MaxValue, ErrorMessage = "Id cannot be negative")]
    public int Id { get; set; }
}