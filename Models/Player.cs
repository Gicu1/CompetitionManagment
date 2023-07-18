using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CompetitionManagment.Models;

[Table("PLAYER")]
public partial class Player
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is byte[] file)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"File size must be less than {_maxFileSize} bytes.");
                }
            }

            return ValidationResult.Success;
        }
    }

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;

        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult($"File extension must be one of: {string.Join(", ", _extensions)}");
                }
            }

            return ValidationResult.Success;
        }
    }


    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? FirstName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? LastName { get; set; }

    public int? Age { get; set; }

    [Column("TeamID")]
    public int? TeamId { get; set; }

    [NotMapped]
    public IFormFile? PictureFile { get; set; }

    [MaxFileSize(1 * 1024 * 1024)]
    [AllowedExtensions(new string[] { ".jpg", ".png" })]
    public byte[]? Picture { get; set; }

    [ForeignKey("TeamId")]
    [InverseProperty("Players")]
    public virtual Team? Team { get; set; }
}
