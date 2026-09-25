using System;
using System.Collections.Generic;
using System.Text;

namespace EducationSystem.Models
{
    public sealed class Group
    {
        public int Id { get; set; }
        public string Prefix { get; set; } = string.Empty;
        public byte Number { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string name => $"{Prefix}-{Number}";
    }
}
