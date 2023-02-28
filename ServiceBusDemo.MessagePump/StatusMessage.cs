

using System.ComponentModel.DataAnnotations;

public record StatusMessage(
    Guid Id,
    StatusEnum Status,
    string Message,
    DateTime? TimestampUtc
);


public enum StatusEnum {
    [Display(Name = "Happy")] HAPPY = 1,
    [Display(Name = "Sad")] SAD = 2,
    [Display(Name = "Enraged")] ENRAGED = 3,
    [Display(Name = "A Teapot")] A_TEAPOT = 4,
    [Display(Name = "Not A Teapot")] NOT_A_TEAPOT = 5
}